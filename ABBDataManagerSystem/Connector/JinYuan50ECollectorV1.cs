using System.Collections.Concurrent;
using System.Text;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYuan50ECollectorV1
    {
        public static readonly int Interval = 400; // 每400ms发送一次寻机指令，从机返回数据

        private ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        private readonly ResistanceCurrentInfoCollector Collector;

        private TestCurrentType _CurrentMode;

        public TestCurrentType CurrentMode
        {
            get { return _CurrentMode; }
            set { _CurrentMode = value; }
        }

        private TestType50E _Mode;

        public TestType50E Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        public TestPattern PatternMode
        {
            set; get;
        }


        public enum InstrumentStatus : byte
        {
            Reset = 0x42,           // 42H 复位状态
            GeneralParameter = 0x44,// 44H 常规参数状态
            GeneralTest = 0x46,     // 46H 常规测试状态
            TemperatureRiseParameter = 0x45, // 45H 温升参数状态
            TemperatureRiseTiming = 0x47,    // 47H 温升定时状态
            TemperatureRiseTest = 0x48,      // 48H 温升测试状态
            Other = 0x50           // 50H 其他状态
        }

        public static string GetInstrumentStatusDescription(InstrumentStatus status)
        {
            switch (status)
            {
                case InstrumentStatus.Reset:
                    return ("仪器处于复位状态");
                case InstrumentStatus.GeneralParameter:
                    return ("仪器处于常规参数状态");
                case InstrumentStatus.GeneralTest:
                    return ("仪器处于常规测试状态");
                case InstrumentStatus.TemperatureRiseParameter:
                    return ("仪器处于温升参数状态");
                case InstrumentStatus.TemperatureRiseTiming:
                    return ("仪器处于温升定时状态");
                case InstrumentStatus.TemperatureRiseTest:
                    return ("仪器处于温升测试状态");
                case InstrumentStatus.Other:
                    return ("仪器处于其他状态");
                default:
                    return ("未知状态");
            }
        }

        public enum TestStatus
        {
            Charging = 0x31,        // 31H — 正在充电
            Testing = 0x32,         // 32H — 正在测试
            Stopping = 0x33,        // 33H — 正在停止
            Stopped = 0x34,         // 34H — 测试停止
            OverRange = 0x35,       // 35H — 超出量程
            Discharging = 0x36      // 36H — 正在放电
        }


        // 电流枚举
        public enum TestCurrentType : byte
        {
            _002A = 0x38,
            _01A = 0x37,
            _03A = 0x36,
            _1A = 0x35,
            _3A = 0x34,
            _10A = 0x33,
            _20A = 0x32,
            _40A = 0x31,
            _50A = 0x30
        }

        // 方式枚举
        public enum TestType50E : byte
        {
            Normal = 0x30,
            TemperatureRise10Sec,
            TemperatureRise30Sec,
            TemperatureRise60Sec,
        }

        // 模式枚举
        public enum TestPattern : byte
        {
            SingleChannel = 0x30,
            DoubleChannel = 0x31,
        }

        // 绕组枚举
        public enum TestWindingType : byte
        {
            Rx = 0x30,
            Rx1_Rx2 = 0x31,
            Rab_RBO = 0x32,
            Rbc_RCO = 0x33,
            Rca_RAO = 0x34,
            Rab = 0x35,
            Rbc = 0x36,
            Rca = 0x37,
            RAO = 0x38,
            RBO = 0x39,
            RCO = 0x3A
        }

        #region 枚举值转换
        public static class EnumHelper
        {
            // 电流枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> currentMap = new Dictionary<byte, string>
            {
                { 0x38, "0.02A" },
                { 0x37, "0.1A" },
                { 0x36, "0.3A" },
                { 0x35, "1A" },
                { 0x34, "3A" },
                { 0x33, "10A" },
                { 0x32, "20A" },
                { 0x31, "40A" },
                { 0x30, "50A" },
            };

            // 方式枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> modeMap = new Dictionary<byte, string>
            {
                { 0x30, "常规" },
                { 0x31, "温升" },
            };

            // 模式枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> patternMap = new Dictionary<byte, string>
            {
                { 0x30, "单通道" },
                { 0x31, "双通道" },
            };


            // 获取电流枚举值对应的字符串
            public static string GetCurrentString(TestCurrentType current)
            {
                return GetValueFromDictionary(currentMap, (byte)current);
            }

            // 获取方式枚举值对应的字符串
            public static string GetModeString(TestType50E mode)
            {
                return GetValueFromDictionary(modeMap, (byte)mode);
            }

            // 获取模式枚举值对应的字符串
            public static string GetPatternString(TestPattern pattern)
            {
                return GetValueFromDictionary(patternMap, (byte)pattern);
            }

            // 从字典中获取值
            private static string GetValueFromDictionary(Dictionary<byte, string> map, byte key)
            {
                if (map.TryGetValue(key, out string value))
                {
                    return value;
                }
                throw new ArgumentException("Invalid enum value.");
            }

            // 获取电流字符串对应的枚举值
            public static TestCurrentType GetCurrentEnum(string currentStr)
            {
                return GetEnumFromDictionary<TestCurrentType>(currentMap, currentStr);
            }

            // 获取方式字符串对应的枚举值
            public static TestType50E GetModeEnum(string modeStr)
            {
                return GetEnumFromDictionary<TestType50E>(modeMap, modeStr);
            }

            // 获取模式字符串对应的枚举值
            public static TestPattern GetPatternEnum(string patternStr)
            {
                return GetEnumFromDictionary<TestPattern>(patternMap, patternStr);
            }


            // 从字典中获取枚举值
            private static TEnum GetEnumFromDictionary<TEnum>(Dictionary<byte, string> map, string value)
            {
                foreach (var pair in map)
                {
                    if (pair.Value == value)
                    {
                        return (TEnum)Enum.ToObject(typeof(TEnum), pair.Key);
                    }
                }
                throw new ArgumentException("Invalid string value.");
            }

            private static readonly Dictionary<TestStatus, string> TestStatusDescriptions = new Dictionary<TestStatus, string>
            {
                { TestStatus.Charging, "正在充电" },
                { TestStatus.Testing, "正在测试" },
                { TestStatus.Stopping, "正在停止" },
                { TestStatus.Stopped, "测试停止" },
                { TestStatus.OverRange, "超出量程" },
                { TestStatus.Discharging, "正在放电" }
            };

            public static string GetTestStatusDescription(TestStatus status)
            {
                if (TestStatusDescriptions.TryGetValue(status, out var description))
                {
                    return description;
                }
                return "未知状态";
            }
        }
        #endregion

        public JinYuan50ECollectorV1(string portName, int baudRate = 9600)
        {
            Collector = new ResistanceCurrentInfoCollector(portName, baudRate)
            {
                StartByte = 0x7E,
                StopByte = 0x0D,
                DeviceAddress = new byte[] { 0x51, 0x51 },
                SendLenByteCount = 1,
            };
        }

        public bool Connect()
        {
            return Collector.Open();
        }

        public void Disconnect()
        {
            Collector.Close();
        }


        //常规测试命令，复位状态下可用
        public void SendCommonTest()
        {
            actions.Enqueue(() =>
            {
                byte[] command = new byte[] { 0x51, 0x30 };
                Collector.SendCommand(command);
            });
        }

        //温升测试命令，复位状态下可用
        public void SendTempRiseTest()
        {
            actions.Enqueue(() =>
            {
                byte[] command = new byte[] { 0x51, 0x31 };
                Collector.SendCommand(command);
            });
        }


        //参数设置命令 常规参数状态/温升参数状态 下可用
        public void SendParameterSetCommand()
        {
            actions.Enqueue(() =>
            {
                byte[] tempRiseIntervalBytes = new byte[] { 0x20, 0x20 };
                byte mode = 0x30;
                if (Mode != TestType50E.Normal)
                {
                    mode = 0x31;
                    int TempRiseInterval = 10;
                    switch (Mode)
                    {
                        case TestType50E.TemperatureRise10Sec:
                            TempRiseInterval = 10;
                            break;
                        case TestType50E.TemperatureRise30Sec:
                            TempRiseInterval = 30;
                            break;
                        case TestType50E.TemperatureRise60Sec:
                            TempRiseInterval = 60;
                            break;
                    }
                    tempRiseIntervalBytes = Encoding.UTF8.GetBytes(TempRiseInterval.ToString());
                }
                byte[] command = new byte[] { 0x52, mode, (byte)CurrentMode, (byte)PatternMode, 0x20, tempRiseIntervalBytes[0], tempRiseIntervalBytes[1] };
                Collector.SendCommand(command);
            });
        }

        //复位命令 测试过程中不可用
        public void SendResetCommand()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x6B, 0x20, 0x20, 0x20, 0x20 });
            });
        }

        #region 常规测试命令
        //保存命令
        public void SendSaveCommandAtNormal()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x54, 0x35 });
            });
        }

        //打印命令
        public void SendPrintCommandAtNormal()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x54, 0x34 });
            });
        }

        //退出命令 (需要检测仪器的测试状态为测试停止时，才能发送退出命令。)
        public void SendExitCommandAtNormal()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x64, 0x32 });
            });
        }

        //停止命令
        public void SendStopCommandAtNormal()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x54, 0x31 });
            });
        }

        //测试命令
        public void SendStartCommonTest()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x54, 0x30, 0x20 }); // 测试的相
            });
        }
        #endregion

        #region 温升测试命令
        // (需要检测仪器的测试状态为测试停止时，才能发送退出命令。)
        // 注：温升测试时，需要先发送定时命令，然后在发送测试命令。


        //定时命令
        public void SendTempRiseTestStartTiming()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x53, 0x2F });
            });
        }

        //测试命令
        public void SendTempRiseStartTest()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x53, 0x30, 0x20, 0x20 }); // 联结方式 测试的相
            });
        }

        //停止命令
        public void SendTempRiseStopCommand()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x53, 0x31 });
            });
        }

        //退出命令
        public void SendTempRiseExitCommand()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x53, 0x32 });
            });
        }
        #endregion


        //请求数据命令
        public void SendRequestDataCommand()
        {
            Collector.SendCommand(new byte[] { 0x50 });
        }

        public CommonPacket? ReadPacket()
        {
            if (Collector == null)
            {
                return null;
            }
            if (actions.TryDequeue(out Action? action))
            {
                if (action != null)
                {
                    action();
                    return null;
                }
                else
                {
                    SendRequestDataCommand();
                }
            }
            else
            {

                SendRequestDataCommand();
            }
            byte[]? packet = Collector.ReadData();
            if (packet == null)
            {
                return null;
            }
            return ParseResponse(packet);
        }

        public CommonPacket? ParseResponse(byte[] packet)
        {
            if (packet == null || packet.Length < 1)
                return null;
            if (packet[0] < 0x42 || packet[0] > 0x50)
            {
                return null;
            }
            InstrumentStatus Status = (InstrumentStatus)packet[0];
            var commonPacket = new CommonPacket()
            {
                Status = GetInstrumentStatusDescription(Status)
            };
            if (Status == InstrumentStatus.Reset || Status == InstrumentStatus.GeneralParameter
                || Status == InstrumentStatus.TemperatureRiseParameter || Status == InstrumentStatus.Other)
            {
                return commonPacket;
            }

            if (Status == InstrumentStatus.GeneralTest)
            {
                if (packet.Length < 33)
                {
                    Log.Info("Fail to parse packet step2");
                    return null;
                }
                commonPacket.current = (TestCurrentType)packet[1];
                var testMode = (TestPattern)packet[2];
                var testStatus = (TestStatus)packet[4];
                commonPacket.pattern = testMode;
                commonPacket.Status = EnumHelper.GetTestStatusDescription(testStatus);
                int offset = 5;
                commonPacket.strRealTimeCurrent = Encoding.ASCII.GetString(packet, offset, 6);
                offset += 6;
                if (testMode == TestPattern.SingleChannel
                    || testMode == TestPattern.DoubleChannel)
                {
                    commonPacket.strRealTimeResistance1 = Encoding.ASCII.GetString(packet, offset + 2, 9);
                    offset += (2 + 9);
                }
                if (testMode == TestPattern.DoubleChannel)
                {
                    commonPacket.strRealTimeResistance2 = Encoding.ASCII.GetString(packet, offset + 2, 9);
                    offset += (2 + 9);
                }
            }
            else if (Status == InstrumentStatus.TemperatureRiseTest)
            {
                // 7E 51 51 36 34 48 + 电流(1byte) + 测试方式(1byte) + 联结方式(1byte) + 测试的相(1byte) + 温升间隔(2byte)+ 秒表时间(5byte)
                // + 测试状态(1byte) + 测试电流(6byte) + 测试电阻1名称(2byte) + 测试电阻1(9byte) + 测试电阻2名称(2byte) + 测试电阻2(9byte)
                // + 温升时间(5byte) + 温升电阻1(9byte) + 温升电阻2(9byte) + XOR 0D
                if (packet.Length < 64)
                {
                    Log.Info("Fail to parse packet step3");
                    return null;
                }
                commonPacket.current = (TestCurrentType)packet[1];
                var testMode = (TestPattern)packet[2];
                commonPacket.pattern = testMode;

                int offset = 5;
                string tempRiseInterval = Encoding.ASCII.GetString(packet, offset, 2);
                offset += 2;
                commonPacket.strRealTime = Encoding.ASCII.GetString(packet, offset, 5);
                offset += 5;
                var testStatus = (TestStatus)packet[offset];
                commonPacket.Status = EnumHelper.GetTestStatusDescription(testStatus);
                offset += 1;
                commonPacket.strRealTimeCurrent = Encoding.ASCII.GetString(packet, offset, 6);
                offset += 6;
                offset += 2;
                commonPacket.strRealTimeResistance1 = Encoding.ASCII.GetString(packet, offset, 9);
                offset += 9;
                offset += 2;
                commonPacket.strRealTimeResistance2 = Encoding.ASCII.GetString(packet, offset, 9);
                offset += 9;
                commonPacket.strSecTime = Encoding.ASCII.GetString(packet, offset, 5);
                offset += 5;
                commonPacket.strSecResistance1 = Encoding.ASCII.GetString(packet, offset, 9);
                offset += 9;
                commonPacket.strSecResistance2 = Encoding.ASCII.GetString(packet, offset, 9);
                offset += 9;
            }

            return commonPacket;
        }

        public class CommonPacket
        {
            public TestCurrentType current;
            public TestType50E mode;
            public TestPattern pattern;
            public string? Status = string.Empty;

            public string? strRealTime = string.Empty;
            public string? strRealTimeCurrent = string.Empty;
            public string? strRealTimeResistance1 = string.Empty;
            public string? strRealTimeResistance2 = string.Empty;
            public string? strSecTime = string.Empty;
            public string? strSecResistance1 = string.Empty;
            public string? strSecResistance2 = string.Empty;
            public string? strResistanceTemp1 = string.Empty;
            public string? strResistanceTemp2 = string.Empty;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"current: {current}");
                sb.AppendLine($"mode: {mode}");
                sb.AppendLine($"pattern: {pattern}");
                sb.AppendLine($"Status: {Status}");
                sb.AppendLine($"strRealTime: {strRealTime}");
                sb.AppendLine($"strRealTimeCurrent: {strRealTimeCurrent}");
                sb.AppendLine($"strRealTimeResistance1: {strRealTimeResistance1}");
                sb.AppendLine($"strRealTimeResistance2: {strRealTimeResistance2}");
                sb.AppendLine($"strSecTime: {strSecTime}");
                sb.AppendLine($"strSecResistance1: {strSecResistance1}");
                sb.AppendLine($"strSecResistance2: {strSecResistance2}");
                sb.AppendLine($"strResistanceTemp1: {strResistanceTemp1}");
                sb.AppendLine($"strResistanceTemp2: {strResistanceTemp2}");

                return sb.ToString();
            }
        }
    }
}

using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg;
using System.Collections.Concurrent;
using System.Text;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYuan20ECollector
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

        private TestType20E _Mode;

        public TestType20E Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        private TestPattern _Pattern;

        public TestPattern PatternMode
        {
            get { return _Pattern; }
            set { _Pattern = value; }
        }

        private TestWindingType _WindingMode;

        public TestWindingType WindingMode
        {
            get { return _WindingMode; }
            set { _WindingMode = value; }
        }

        public enum InstrumentStatus : byte
        {
            ParameterSetting = 0x01,        // 01H 参数设置状态
            NormalTesting = 0x02,           // 02H 正常测试状态
            Demagnetization = 0x03,         // 03H 去磁状态
            DemagnetizationComplete = 0x14, // 14H 去磁完成状态
            Discharging = 0x13,             // 13H 正在放电状态
            MeasurementOutOfRange = 0x04,   // 04H 测量数据超范围
            TemperatureRiseTiming = 0x05,   // 05H 温升定时状态
            Charging = 0x06,                // 06H 正在充电状态
            TemperatureConversionInput = 0x07, // 07H 温度换算输入状态
            TimeModification = 0x08,        // 08H 时间修改状态
            ViewHistoricalRecords = 0x09    // 09H 调阅历史记录状态
        }

        public static string GetInstrumentStatusDescription(InstrumentStatus status)
        {
            switch (status)
            {
                case InstrumentStatus.ParameterSetting:
                    return "参数设置状态";
                case InstrumentStatus.NormalTesting:
                    return "正常测试状态";
                case InstrumentStatus.Demagnetization:
                    return "去磁状态";
                case InstrumentStatus.DemagnetizationComplete:
                    return "去磁完成状态";
                case InstrumentStatus.Discharging:
                    return "正在放电状态";
                case InstrumentStatus.MeasurementOutOfRange:
                    return "测量数据超范围";
                case InstrumentStatus.TemperatureRiseTiming:
                    return "温升定时状态";
                case InstrumentStatus.Charging:
                    return "正在充电状态";
                case InstrumentStatus.TemperatureConversionInput:
                    return "温度换算输入状态";
                case InstrumentStatus.TimeModification:
                    return "时间修改状态";
                case InstrumentStatus.ViewHistoricalRecords:
                    return "调阅历史记录状态";
                default:
                    return "未知状态";
            }
        }


        // 电流枚举
        public enum TestCurrentType : byte
        {
            _03A = 0x36,
            _25mA = 0x35,
            _1A = 0x34,
            _3A = 0x33,
            _10A = 0x32,
            _20A = 0x31
        }

        // 方式枚举
        public enum TestType20E : byte
        {
            Normal = 0x30,
            TemperatureRise10Sec = 0x31,
            TemperatureRise30Sec = 0x32,
            TemperatureRise60Sec = 0x33
        }

        // 模式枚举
        public enum TestPattern : byte
        {
            SingleChannel = 0x30,
            DoubleChannel = 0x31,
            YnD11 = 0x32,
            PhaseSelection = 0x33
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
                { 0x36, "0.3A" },
                { 0x35, "25mA" },
                { 0x34, "1A" },
                { 0x33, "3A" },
                { 0x32, "10A" },
                { 0x31, "20A" }
            };

            // 方式枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> modeMap = new Dictionary<byte, string>
            {
                { 0x30, "常规" },
                { 0x31, "温升10s" },
                { 0x32, "温升30s" },
                { 0x33, "温升60s" }
            };

            // 模式枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> patternMap = new Dictionary<byte, string>
            {
                { 0x30, "单通道" },
                { 0x31, "双通道" },
                { 0x32, "YnD11助磁" },
                { 0x33, "选相" }
            };

            // 绕组枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> windingMap = new Dictionary<byte, string>
            {
                { 0x30, "Rx" },
                { 0x31, "Rx1-Rx2" },
                { 0x32, "Rab-RBO" },
                { 0x33, "Rbc-RCO" },
                { 0x34, "Rca-RAO" },
                { 0x35, "Rab" },
                { 0x36, "Rbc" },
                { 0x37, "Rca" },
                { 0x38, "RAO" },
                { 0x39, "RBO" },
                { 0x3A, "RCO" }
            };

            // 获取电流枚举值对应的字符串
            public static string GetCurrentString(TestCurrentType current)
            {
                return GetValueFromDictionary(currentMap, (byte)current);
            }

            // 获取方式枚举值对应的字符串
            public static string GetModeString(TestType20E mode)
            {
                return GetValueFromDictionary(modeMap, (byte)mode);
            }

            // 获取模式枚举值对应的字符串
            public static string GetPatternString(TestPattern pattern)
            {
                return GetValueFromDictionary(patternMap, (byte)pattern);
            }

            // 获取绕组枚举值对应的字符串
            public static string GetWindingString(TestWindingType winding)
            {
                return GetValueFromDictionary(windingMap, (byte)winding);
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
            public static TestType20E GetModeEnum(string modeStr)
            {
                return GetEnumFromDictionary<TestType20E>(modeMap, modeStr);
            }

            // 获取模式字符串对应的枚举值
            public static TestPattern GetPatternEnum(string patternStr)
            {
                return GetEnumFromDictionary<TestPattern>(patternMap, patternStr);
            }

            // 获取绕组字符串对应的枚举值
            public static TestWindingType GetWindingEnum(string windingStr)
            {
                return GetEnumFromDictionary<TestWindingType>(windingMap, windingStr);
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
        }
        #endregion

        public JinYuan20ECollector(string portName, int baudRate = 9600)
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


        //测试命令，温升模式下为定时
        public void SendTestCommand()
        {
            actions.Enqueue(() =>
            {
                byte[] command = new byte[] { 0x62, 0x20, 0x20, 0x20, 0x20 };
                Collector.SendCommand(command);
            });
        }

        //参数设置命令
        public void SendParameterSetCommand()
        {
            actions.Enqueue(() =>
            {
                byte[] command = new byte[] { 0x60, (byte)Mode, (byte)PatternMode, (byte)WindingMode, (byte)CurrentMode };
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

        #region 常规测试界面下可用的命令
        //保存命令
        public void SendSaveCommandAtNormal()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x63, 0x20, 0x20, 0x20, 0x20 });
            });
        }

        //打印命令
        public void SendPrintCommandAtNormal()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x64, 0x20, 0x20, 0x20, 0x20 });
            });
        }

        //退出命令
        public void SendExitCommandAtNormal()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x65, 0x20, 0x20, 0x20, 0x20 });
            });
        }
        #endregion

        #region 温升定时界面下可用的命令
        //测试命令
        public void SendTempRiseTestCommandAtTiming()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x67, 0x20, 0x20, 0x20, 0x20 });
            });
        }

        //退出命令
        public void SendTempRiseExitCommandAtTiming()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x66, 0x20, 0x20, 0x20, 0x20 });
            });
        }
        #endregion

        #region 温升测试界面下可用的命令
        //退出命令
        public void SendTempRiseExitCommandAtTest()
        {
            actions.Enqueue(() =>
            {
                Collector.SendCommand(new byte[] { 0x68, 0x20, 0x20, 0x20, 0x20 });
            });
        }
        #endregion

        #region 去磁测试界面下可用的命令
        //退出命令
        public void SendExitDemagnetizeCommand()
        {
            Collector.SendCommand(new byte[] { 0x69, 0x20, 0x20, 0x20, 0x20 });
        }
        #endregion

        //请求数据命令
        public void SendRequestDataCommand()
        {
            Collector.SendCommand(new byte[] { 0x70, 0x20, 0x20, 0x20, 0x20 });
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
            if (packet[0] == 0x00 || packet[0] >= 0xA)
            {
                return null;
            }
            InstrumentStatus Status = (InstrumentStatus)packet[0];
            CommonPacket? commonPacket = ParseCommonPacket(packet);
            if (commonPacket == null)
            {
                Log.Info("Fail to parse packet step1");
                return null;
            }
            commonPacket.Status = GetInstrumentStatusDescription(Status);
            if (Status != InstrumentStatus.NormalTesting && Status != InstrumentStatus.MeasurementOutOfRange)
            {
                return commonPacket;
            }
            int offset = 5;
            if (commonPacket.mode == TestType20E.Normal)
            {
                if (packet.Length < 46)
                {
                    Log.Info("Fail to parse packet step2");
                    return null;
                }
                // 实时时间（4位）+实时电流（5位）+实时电阻1（7位）+实时电阻2（7位）
                // +秒时间（4位）<可忽略>
                // +第1电阻的温度转换值（7位）<可忽略>
                // +第2电阻的转换值（7位）<可忽略>，
                // 其中每组数据最后一位为单位，即k、m或者μ(电阻Ω，电流A为默认，不传送)。
                string strRealTime = Encoding.ASCII.GetString(packet, offset, 4);
                offset += 4;
                string strRealTimeCurrent = Encoding.ASCII.GetString(packet, offset, 5);
                offset += 5;
                string strRealTimeResistance1 = Encoding.ASCII.GetString(packet, offset, 7);
                offset += 7;
                string strRealTimeResistance2 = Encoding.ASCII.GetString(packet, offset, 7);
                offset += 7;
                string strSecTime = Encoding.ASCII.GetString(packet, offset, 4);
                offset += 4;
                string strResistanceTemp1 = Encoding.ASCII.GetString(packet, offset, 7);
                offset += 7;
                string strResistanceTemp2 = Encoding.ASCII.GetString(packet, offset, 7);

                commonPacket.strRealTime = strRealTime;
                commonPacket.strRealTimeCurrent = strRealTimeCurrent;
                commonPacket.strRealTimeResistance1 = strRealTimeResistance1;
                commonPacket.strRealTimeResistance2 = strRealTimeResistance2;
                commonPacket.strSecTime = strSecTime;
                commonPacket.strResistanceTemp1 = strResistanceTemp1;
                commonPacket.strResistanceTemp2 = strResistanceTemp2;
            }
            else
            {
                // 实时时间（4位）+实时电流（5位）+实时电阻1（7位）+实时电阻2（7位）
                // +秒时间（4位）+秒电阻1（7位）+秒电阻2（7位），
                // 其中每组数据最后一位为单位，即k、m或者μ(电阻Ω，电流A为默认，不传送)。
                string strRealTime = Encoding.ASCII.GetString(packet, offset, 4);
                offset += 4;
                string strRealTimeCurrent = Encoding.ASCII.GetString(packet, offset, 5);
                offset += 5;
                string strRealTimeResistance1 = Encoding.ASCII.GetString(packet, offset, 7);
                offset += 7;
                string strRealTimeResistance2 = Encoding.ASCII.GetString(packet, offset, 7);
                offset += 7;
                string strSecTime = Encoding.ASCII.GetString(packet, offset, 4);
                offset += 4;
                string strSecResistance1 = Encoding.ASCII.GetString(packet, offset, 7);
                offset += 7;
                string strSecResistance2 = Encoding.ASCII.GetString(packet, offset, 7);

                commonPacket.strRealTime = strRealTime;
                commonPacket.strRealTimeCurrent = strRealTimeCurrent;
                commonPacket.strRealTimeResistance1 = strRealTimeResistance1;
                commonPacket.strRealTimeResistance2 = strRealTimeResistance2;
                commonPacket.strSecTime = strSecTime;
                commonPacket.strSecResistance1 = strSecResistance1;
                commonPacket.strSecResistance2 = strSecResistance2;
            }

            return commonPacket;
        }

        private CommonPacket? ParseCommonPacket(byte[] packet)
        {
            // 仪器状态+模式+方式+绕组+电流（量程）
            if (packet.Length < 5) return null;
            TestPattern pattern = (TestPattern)packet[1];
            TestType20E mode = (TestType20E)packet[2];
            TestWindingType winding = (TestWindingType)packet[3];
            TestCurrentType current = (TestCurrentType)packet[4];

            return new CommonPacket()
            {
                current = current,
                mode = mode,
                pattern = pattern,
                winding = winding
            };
        }

        public class CommonPacket
        {
            public TestCurrentType current;
            public TestType20E mode;
            public TestPattern pattern;
            public TestWindingType winding;
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
                sb.AppendLine($"winding: {winding}");
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

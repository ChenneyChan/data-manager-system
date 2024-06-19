using System.Text;
using static ABBDataManagerSystem.Connector.JinYuan20WCollector;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYuan20ECollector
    {
        public static readonly int Interval = 400; // 每400ms发送一次寻机指令，从机返回数据

        private readonly ResistanceCurrentInfoCollector Collector;

        private CurrentEnum _CurrentMode;

        public CurrentEnum CurrentMode
        {
            get { return _CurrentMode; }
            set { _CurrentMode = value; }
        }

        private ModeEnum _Mode;

        public ModeEnum Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        private PatternEnum _Pattern;

        public PatternEnum PatternMode
        {
            get { return _Pattern; }
            set { _Pattern = value; }
        }

        private WindingEnum _WindingMode;

        public WindingEnum WindingMode
        {
            get { return _WindingMode; }
            set { _WindingMode = value; }
        }

        public enum InstrumentStatus
        {
            Unknown = 0,
            Reset = 0x41,
            Charging = 0x42,
            PreparingForDemagnetization = 0x43,
            RangeHigh = 0x44,
            RangeLow = 0x45,
            Discharging = 0x46,
            Testing = 0x47,
            TestComplete = 0x48,
            Demagnetizing = 0x49,
            DemagnetizationComplete = 0x4A,
            TimingSuccess = 0x4B,
            PreparingForTiming = 0x4C
        }

        public string GetStatusDescription(InstrumentStatus status)
        {
            switch (status)
            {
                case InstrumentStatus.Reset:
                    return "复位状态";
                case InstrumentStatus.Charging:
                    return "正在充电状态";
                case InstrumentStatus.PreparingForDemagnetization:
                    return "准备开始去磁状态";
                case InstrumentStatus.RangeHigh:
                    return "量程大状态";
                case InstrumentStatus.RangeLow:
                    return "量程小状态";
                case InstrumentStatus.Discharging:
                    return "正在放电状态";
                case InstrumentStatus.Testing:
                    return "正在测试状态";
                case InstrumentStatus.TestComplete:
                    return "测试完成循环测试状态";
                case InstrumentStatus.Demagnetizing:
                    return "正在去磁状态";
                case InstrumentStatus.DemagnetizationComplete:
                    return "去磁完成状态";
                case InstrumentStatus.TimingSuccess:
                    return "定时成功状态";
                case InstrumentStatus.PreparingForTiming:
                    return "准备定时状态";
                default:
                    return "未知状态";
            }
        }

        // 电流枚举
        public enum CurrentEnum : byte
        {
            _15mA = 0x35,
            _1A = 0x34,
            _3A = 0x33,
            _10A = 0x32,
            _20A = 0x31
        }

        // 方式枚举
        public enum ModeEnum : byte
        {
            Normal = 0x31,
            TemperatureRise = 0x30
        }

        // 模式枚举
        public enum PatternEnum : byte
        {
            DoubleChannel = 0x31,
            SingleChannel = 0x30,
            YnD11 = 0x32,
            PhaseSelection = 0x33
        }

        // 绕组枚举
        public enum WindingEnum : byte
        {
            Rx = 0x30,
            Rx1_Rx2 = 0x31,
            Rca_RAO = 0x32,
            Rab_RBO = 0x33,
            Rbc_RCO = 0x34,
            Rca = 0x35,
            Rab = 0x36,
            Rbc = 0x37,
            RAO = 0x38,
            RBO = 0x39,
            RCO = 0x3A
        }

        public static class EnumHelper
        {
            // 电流枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> currentMap = new Dictionary<byte, string>
            {
                { 0x35, "15mA" },
                { 0x34, "1A" },
                { 0x33, "3A" },
                { 0x32, "10A" },
                { 0x31, "20A" }
            };

            // 方式枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> modeMap = new Dictionary<byte, string>
            {
                { 0x31, "正常" },
                { 0x30, "温升" }
            };

            // 模式枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> patternMap = new Dictionary<byte, string>
            {
                { 0x31, "双通道" },
                { 0x30, "单通道" },
                { 0x32, "YnD11助磁" },
                { 0x33, "选相" }
            };

            // 绕组枚举值与字符串的映射表
            public static readonly Dictionary<byte, string> windingMap = new Dictionary<byte, string>
            {
                { 0x30, "Rx" },
                { 0x31, "Rx1-Rx2" },
                { 0x32, "Rca-RAO" },
                { 0x33, "Rab-RBO" },
                { 0x34, "Rbc-RCO" },
                { 0x35, "Rca" },
                { 0x36, "Rab" },
                { 0x37, "Rbc" },
                { 0x38, "RAO" },
                { 0x39, "RBO" },
                { 0x3A, "RCO" }
            };

            // 获取电流枚举值对应的字符串
            public static string GetCurrentString(CurrentEnum current)
            {
                return GetValueFromDictionary(currentMap, (byte)current);
            }

            // 获取方式枚举值对应的字符串
            public static string GetModeString(ModeEnum mode)
            {
                return GetValueFromDictionary(modeMap, (byte)mode);
            }

            // 获取模式枚举值对应的字符串
            public static string GetPatternString(PatternEnum pattern)
            {
                return GetValueFromDictionary(patternMap, (byte)pattern);
            }

            // 获取绕组枚举值对应的字符串
            public static string GetWindingString(WindingEnum winding)
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
            public static CurrentEnum GetCurrentEnum(string currentStr)
            {
                return GetEnumFromDictionary<CurrentEnum>(currentMap, currentStr);
            }

            // 获取方式字符串对应的枚举值
            public static ModeEnum GetModeEnum(string modeStr)
            {
                return GetEnumFromDictionary<ModeEnum>(modeMap, modeStr);
            }

            // 获取模式字符串对应的枚举值
            public static PatternEnum GetPatternEnum(string patternStr)
            {
                return GetEnumFromDictionary<PatternEnum>(patternMap, patternStr);
            }

            // 获取绕组字符串对应的枚举值
            public static WindingEnum GetWindingEnum(string windingStr)
            {
                return GetEnumFromDictionary<WindingEnum>(windingMap, windingStr);
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

        public JinYuan20ECollector(string portName, int baudRate = 9600)
        {
            Collector = new ResistanceCurrentInfoCollector(portName, baudRate)
            {
                StartByte = 0x7E,
                StopByte = 0x0D,
                DeviceAddress = new byte[] { 0x32, 0x32 }
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


        //测试/复测命令
        public void SendTestCommand()
        {
            byte[] command = new byte[] { 0x41, (byte)CurrentMode, (byte)Mode, (byte)PatternMode, (byte)WindingMode };
            Collector.SendCommand(command);
        }

        //参数设置命令
        public void SendParameterSetCommand()
        {
            byte[] command = new byte[] { 0x42, (byte)CurrentMode, (byte)Mode, (byte)PatternMode, (byte)WindingMode };
            Collector.SendCommand(command);
        }

        //复位命令
        public void SendResetCommand()
        {
            Collector.SendCommand(new byte[] { 0x43 });
        }

        //打印命令
        public void SendPrintCommand()
        {
            Collector.SendCommand(new byte[] { 0x44 });
        }

        //定时命令
        public void SendTimingCommand(string time)
        {
            byte[] command = new byte[] { 0x45 }; // todo
            Collector.SendCommand(command);
        }

        //去磁命令
        public void SendDemagnetizeCommand()
        {
            Collector.SendCommand(new byte[] { 0x46 });
        }

        //请求数据命令
        public void SendRequestDataCommand()
        {
            Collector.SendCommand(new byte[] { 0x47 });
        }


        public CommonPacket? ReadPacket()
        {
            if (Collector == null)
            {
                return null;
            }
            SendRequestDataCommand();
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

            InstrumentStatus Status = (InstrumentStatus)packet[0];
            if (Status == InstrumentStatus.Unknown)
            {
                return null;
            }
            CommonPacket? commonPacket = ParseCommonPacket(packet);
            if (commonPacket == null)
            {
                return null;
            }
            commonPacket.Status = GetStatusDescription(Status);
            if (Status != InstrumentStatus.TestComplete)
            {
                return commonPacket;
            }
            // 每个电阻7字节
            if (commonPacket.mode == ModeEnum.Normal || commonPacket.mode == ModeEnum.TemperatureRise)
            {
                if (commonPacket.pattern == PatternEnum.SingleChannel)
                {
                    if (packet.Length < 5 + 7)
                    {
                        return null;
                    }
                    string strResistance = Encoding.ASCII.GetString(packet, 5, 7);
                    commonPacket.CH1ResistanceIsMill = strResistance.IndexOf("m") >= 0;
                    commonPacket.CH1Resistance = Utils.ParseFloatNull(strResistance.Replace("m", ""));
                }
                else if (commonPacket.pattern == PatternEnum.DoubleChannel)
                {
                    string strResistance1 = Encoding.ASCII.GetString(packet, 5, 7);
                    string strResistance2 = Encoding.ASCII.GetString(packet, 5 + 7, 7);
                    commonPacket.CH1ResistanceIsMill = strResistance1.IndexOf("m") >= 0;
                    commonPacket.CH1Resistance = Utils.ParseFloatNull(strResistance1.Replace("m", ""));
                    commonPacket.CH2ResistanceIsMill = strResistance1.IndexOf("m") >= 0;
                    commonPacket.CH2Resistance = Utils.ParseFloatNull(strResistance1.Replace("m", ""));
                }

                if (commonPacket.mode == ModeEnum.TemperatureRise && (commonPacket.pattern == PatternEnum.SingleChannel || commonPacket.pattern == PatternEnum.DoubleChannel))
                {
                    int offset = commonPacket.pattern == PatternEnum.SingleChannel ? (5 + 7) : (5 + 7 + 7);
                    if (packet.Length < offset + 6)
                    {
                        return null;
                    }
                    string strTime = Encoding.ASCII.GetString(packet, offset, 6).Trim();
                    if (strTime.Length == 6)
                    {
                        commonPacket.Time = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4);
                    }
                }
            }

            return commonPacket;
        }

        private CommonPacket? ParseCommonPacket(byte[] packet)
        {
            // 7E 32 32 30 35 41 （电流）（方式）（模式）（绕组）(XOR) 0D
            if (packet.Length < 5) return null;
            CurrentEnum current = (CurrentEnum)packet[1];
            ModeEnum mode = (ModeEnum)packet[2];
            PatternEnum pattern = (PatternEnum)packet[3];
            WindingEnum winding = (WindingEnum)packet[4];

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
            public CurrentEnum current;
            public ModeEnum mode;
            public PatternEnum pattern;
            public WindingEnum winding;

            public float? CH1Resistance = null;
            public float? CH2Resistance = null;
            public bool CH1ResistanceIsMill = false;
            public bool CH2ResistanceIsMill = false;
            public string? Time = null;
            public string? Status;
        }


    }
}

using System.Text;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYuanJYTACollector
    {
        public enum TestTypeJYTA
        {
            SinglePhaseTest,
            ThreePhaseTest,
            ThreePhaseAutoTest,
        }

        // 分接位置
        public enum TappingPointType
        {
            HighVoltage,
            LowVoltage,
        }

        public enum HomopolarityDisplayType
        {
            Positive,
            Negtive,
        }

        public enum TestVoltageType
        {
            Voltage160V,
            Voltage20V,
        }

        public enum HighVoltageConnectionType
        {
            Type_Y,
            Type_YN,
            Type_D,
            Type_Z,
            Type_ZN,
            Type_V,
            Type_SCOTT,
            Type_IN_SCOTT,
        }

        public enum LowVoltageConnectionType
        {
            //y - 30H  yn – 31H  d – 32H  z - 33H   zn – 34H  
            // v-35H  Scott-36H  In-Scott-37H  空-38H

            Type_Y,
            Type_YN,
            Type_D,
            Type_Z,
            Type_ZN,
            Type_V,
            Type_SCOTT,
            Type_IN_SCOTT,
            Type_NULL,
        }

        public enum DeviceState
        {
            Initial,
            SelfeCheck,
            MainMenu,
            SinglePhaseSetting,
            TreePhaseSetting,
            TreePhaseAutoSetting,
            SinglePhaseTest,
            TreePhaseTest,
            TreePhaseAutoTest,
            VoltageProtection,
            CurrentProtection,
            OverheatProtection,
            OtherState,
        }

        public enum TipInfoType
        {
            Null,
            Testing,
            TestDone,
            TestError,
        }

        public enum PowerCodeType
        {
            Full,
            Percent75,
            Percent50,
            Percent25,
            Percent0,
            ExternalPower,
        }

        public static int Interval = 400; // 每400ms发送一次寻机指令，从机返回数据

        private readonly ResistanceCurrentInfoCollector Collector;

        private TestTypeJYTA _TestType = TestTypeJYTA.SinglePhaseTest;

        public DeviceState deviceState = DeviceState.MainMenu;

        public TipInfoType tipInfo = TipInfoType.Null;

        public PowerCodeType powerCode = PowerCodeType.Full;

        public TestTypeJYTA TestType { get { return _TestType; } set { _TestType = value; IsConfigChanged = true; } }

        private TappingPointType _TappingPoint;

        public TappingPointType TappingPoint
        {
            get { return _TappingPoint; }
            set { _TappingPoint = value; }
        }

        private HomopolarityDisplayType _HomopolarityDisplayType;

        public HomopolarityDisplayType HomopolarityDisplay
        {
            get { return _HomopolarityDisplayType; }
            set { _HomopolarityDisplayType = value; }
        }

        private TestVoltageType _TestVoltageType;

        public TestVoltageType TestVoltage
        {
            get { return _TestVoltageType; }
            set { _TestVoltageType = value; }
        }

        private HighVoltageConnectionType _HighVoltageConnection;

        public HighVoltageConnectionType HighVoltageConnection
        {
            get { return _HighVoltageConnection; }
            set { _HighVoltageConnection = value; }
        }

        private LowVoltageConnectionType _LowVoltageConnection;

        public LowVoltageConnectionType LowVoltageConnection
        {
            get { return _LowVoltageConnection; }
            set { _LowVoltageConnection = value; }
        }

        private bool IsConfigChanged = false;

        public JinYuanJYTACollector(string portName, int baudRate = 9600)
        {
            Collector = new ResistanceCurrentInfoCollector(portName, baudRate)
            {
                StartByte = 0x7E,
                StopByte = 0x0D,
                RspLenByteCount = 3,
                DeviceAddress = new byte[] { 0x54, 0x55 }
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

        public bool CheckCanSendCommand()
        {
            return deviceState != DeviceState.Initial && deviceState != DeviceState.SelfeCheck;
        }

        public void SetResetCommand()
        {
            Collector.SendCommand(new byte[] { 0x40 });
        }

        public float RatedHighVoltage = 0;
        public float RatedLowVoltage = 0;
        public int RatedTapping = 0;
        public int PositiveTappingCount = 0;
        public float TappingSpacing = 0;
        public string ProductSequence = "";
        public int Group = 0; // 0 ~ 11

        private static int SinglePhaseCmdLen = 1 + 8 + 8 + 2 + 1 + 2 + 6 + 14 + 1 + 1;
        private byte[] SinglePhaseCmdPacket = new byte[SinglePhaseCmdLen];

        private static int ThreePhaseCmdLen = 1 + 8 + 8 + 2 + 1 + 2 + 6 + 14 + 1 + 1 + 1 + 2 + 1;
        private byte[] ThreePhaseCmdPacket = new byte[ThreePhaseCmdLen];

        #region 仪器参数设置命令

        public void SetSinglePhaseTest()
        {
            // <额定高压:8字节><额定低压:8字节><额定分接:2字节><分接位置:1字节><正分接数:2字节><分接间距:6字节>
            // <试品编号:14字节> <同极性显示:1字节> <试验电压:1字节>XOR 0D  

            byte[] bRatedHV = new byte[8];
            byte[] bRatedLV = new byte[8];
            byte[] bRatedTapping = new byte[2];
            byte[] bPositiveTappingCount = new byte[2];
            byte[] bTappingSpacing = new byte[6];
            byte[] bProductSequence = new byte[14];
            Utils.FloatToBytes(RatedHighVoltage, bRatedHV);
            Utils.FloatToBytes(RatedLowVoltage, bRatedLV);
            Utils.IntToBytes(RatedTapping, bRatedTapping);
            Utils.IntToBytes(PositiveTappingCount, bPositiveTappingCount);
            Utils.FloatToBytes(TappingSpacing, bTappingSpacing);
            Utils.ByteCopy(Encoding.UTF8.GetBytes(ProductSequence), bProductSequence);

            SinglePhaseCmdPacket[0] = 0x61;
            int offset = 1;
            Array.Copy(SinglePhaseCmdPacket, offset, bRatedHV, 0, bRatedHV.Length);
            offset += bRatedHV.Length;
            Array.Copy(SinglePhaseCmdPacket, offset, bRatedLV, 0, bRatedLV.Length);
            offset += bRatedLV.Length;
            Array.Copy(SinglePhaseCmdPacket, offset, bRatedTapping, 0, bRatedTapping.Length);
            offset += bRatedTapping.Length;
            SinglePhaseCmdPacket[offset] = TappingPointTypeCmdMap[TappingPoint];
            offset += 1;
            Array.Copy(SinglePhaseCmdPacket, offset, bPositiveTappingCount, 0, bPositiveTappingCount.Length);
            offset += bPositiveTappingCount.Length;
            Array.Copy(SinglePhaseCmdPacket, offset, bTappingSpacing, 0, bTappingSpacing.Length);
            offset += bTappingSpacing.Length;
            Array.Copy(SinglePhaseCmdPacket, offset, bProductSequence, 0, bProductSequence.Length);
            offset += bProductSequence.Length;
            SinglePhaseCmdPacket[offset] = HomopolarityDisplayTypeCmdMap[HomopolarityDisplay];
            offset += 1;
            SinglePhaseCmdPacket[offset] = TestVoltageTypeCmdMap[TestVoltage];
            offset += 1;
            if (offset != SinglePhaseCmdPacket.Length)
            {
                Log.Error($"SetSinglePhaseTest Packet Fill Offset Error {offset} {SinglePhaseCmdPacket.Length}");
            }

            Collector.SendCommand(SinglePhaseCmdPacket);
        }

        public void SetThreePhaseTest()
        {
            //  <额定高压:8字节><额定低压:8字节><额定分接:2字节><分接位置:1字节><正分接数:2字节><分接间距:6字节>
            //  <试品编号:14字节><高压联结:1字节><低压联结:1字节><组别:2字节> <试验电压:1字节> XOR 0D  

            byte[] bRatedHV = new byte[8];
            byte[] bRatedLV = new byte[8];
            byte[] bRatedTapping = new byte[2];
            byte[] bPositiveTappingCount = new byte[2];
            byte[] bTappingSpacing = new byte[6];
            byte[] bProductSequence = new byte[14];
            byte[] bGroup = new byte[2];
            Utils.FloatToBytes(RatedHighVoltage, bRatedHV);
            Utils.FloatToBytes(RatedLowVoltage, bRatedLV);
            Utils.IntToBytes(RatedTapping, bRatedTapping);
            Utils.IntToBytes(PositiveTappingCount, bPositiveTappingCount);
            Utils.FloatToBytes(TappingSpacing, bTappingSpacing);
            Utils.ByteCopy(Encoding.UTF8.GetBytes(ProductSequence), bProductSequence);
            Utils.IntToBytes(Group, bGroup);

            ThreePhaseCmdPacket[0] = 0x62;
            int offset = 1;
            Array.Copy(ThreePhaseCmdPacket, offset, bRatedHV, 0, bRatedHV.Length);
            offset += bRatedHV.Length;
            Array.Copy(ThreePhaseCmdPacket, offset, bRatedLV, 0, bRatedLV.Length);
            offset += bRatedLV.Length;
            Array.Copy(ThreePhaseCmdPacket, offset, bRatedTapping, 0, bRatedTapping.Length);
            offset += bRatedTapping.Length;
            ThreePhaseCmdPacket[offset] = TappingPointTypeCmdMap[TappingPoint];
            offset += 1;
            Array.Copy(ThreePhaseCmdPacket, offset, bPositiveTappingCount, 0, bPositiveTappingCount.Length);
            offset += bPositiveTappingCount.Length;
            Array.Copy(ThreePhaseCmdPacket, offset, bTappingSpacing, 0, bTappingSpacing.Length);
            offset += bTappingSpacing.Length;
            Array.Copy(ThreePhaseCmdPacket, offset, bProductSequence, 0, bProductSequence.Length);
            offset += bProductSequence.Length;
            ThreePhaseCmdPacket[offset] = HighVoltageConnectionTypeCmdMap[HighVoltageConnection];
            offset += 1;
            ThreePhaseCmdPacket[offset] = LowVoltageConnectionTypeCmdMap[LowVoltageConnection];
            offset += 1;
            Array.Copy(ThreePhaseCmdPacket, offset, bGroup, 0, bGroup.Length);
            offset += bGroup.Length;
            ThreePhaseCmdPacket[offset] = TestVoltageTypeCmdMap[TestVoltage];
            offset += 1;
            if (offset != ThreePhaseCmdPacket.Length)
            {
                Log.Error($"SetThreePhaseTest Packet Fill Offset Error {offset} {ThreePhaseCmdPacket.Length}");
            }

            Collector.SendCommand(ThreePhaseCmdPacket);
        }


        // 注：此命令在主菜单状态(63H)、单相参数设置(65H)、三相参数设置(66H)、三相自动参数设置(67H)下状态有效。
        // 当测试方式为三相自动测试时，测试仪默认<换相方式>为“自动换相”.
        public void SendStartTest()
        {
            byte typeByte = TestTypeCommandMap[TestType];
            Collector.SendCommand(new byte[] { 0x64, typeByte, 0x31 });
        }

        // 注：测试一次完成后，需要再次测试时，发送此命令。
        // 此命令在 单相测试(6AH)、三相测试(6BH)、三相自动测试(6CH)下，
        // 并且提示信息为<测试完成(32H)>时可用。
        public void SendRestartTest()
        {
            Collector.SendCommand(new byte[] { 0x65 });
        }

        // 注：此命令只有在手动换相时，且当前正在测试的为A相或者B相时，才起作用。
        // 此命令在 单相测试(6AH)、三相测试(6BH)、三相自动测试(6CH)下，并且提示信息为<正在测试(31H)>时可用。
        public void SendChangePhaseCommand()
        {
            Collector.SendCommand(new byte[] { 0x66 });
        }

        //注：此命令只有在手动换相时，且当前正在测试的为单相或者C相时，才起作用。
        //此命令在 单相测试(6AH)、三相测试(6BH)、三相自动测试(6CH)下，并且提示信息为<正在测试(31H)>时可用。
        public void SendStopRecuesTestCommand()
        {
            Collector.SendCommand(new byte[] { 0x67 });
        }

        public void SendPrintCommand()
        {
            Collector.SendCommand(new byte[] { 0x68 });
        }

        public void SendStoreCommand()
        {
            Collector.SendCommand(new byte[] { 0x69 });
        }

        public void SendRequest()
        {
            Collector.SendCommand(new byte[] { 0x6A });
        }

        public static byte GetBoundRateByte(int boundRate)
        {
            //9600 bps：（30H）、4800 bps：（31H）、19200 bps：（32H）
            switch (boundRate)
            {
                case 9600: return 0x30;
                case 4800: return 0x31;
                case 19200: return 0x32;
                default:
                    return 0x30;
            }
        }
        #endregion

        public static Dictionary<byte, string> CHStatusMap = new Dictionary<byte, string>()
        {
            { 0x51, "参数配置" },
            { 0x52, "正在充电" },
            { 0x53, "正在测试" },
            { 0x54, "正在放电" },
            { 0x55, "温升正在定时" },
            { 0x56, "过热保护" },
            { 0x57, "记录查看" },
            { 0x58, "时间调整" },
            { 0x59, "通道故障" },
            { 0x5A, "换大电流" },
            { 0x5B, "换小电流" },
        };

        public static Dictionary<string, TestTypeJYTA> TestTypeMap = new Dictionary<string, TestTypeJYTA>()
        {
            { "单相测试",  TestTypeJYTA.SinglePhaseTest},
            { "三相测试",  TestTypeJYTA.ThreePhaseTest},
            { "三相自动测试",  TestTypeJYTA.ThreePhaseAutoTest},
        };

        public static Dictionary<TestTypeJYTA, byte> TestTypeCommandMap = new Dictionary<TestTypeJYTA, byte>()
        {
            { TestTypeJYTA.SinglePhaseTest, 0x30},
            { TestTypeJYTA.ThreePhaseTest, 0x31},
            { TestTypeJYTA.ThreePhaseAutoTest, 0x32},
        };

        public static Dictionary<string, TappingPointType> TappingPointTypeMap = new Dictionary<string, TappingPointType>()
        {
            { "高压侧",  TappingPointType.HighVoltage},
            { "低压侧",  TappingPointType.LowVoltage},
        };

        public static Dictionary<TappingPointType, byte> TappingPointTypeCmdMap = new Dictionary<TappingPointType, byte>()
        {
            { TappingPointType.HighVoltage, 0x30},
            { TappingPointType.LowVoltage, 0x31},
        };

        public static Dictionary<string, HomopolarityDisplayType> HomopolarityDisplayTypeMap = new Dictionary<string, HomopolarityDisplayType>()
        {
            { "负（0）",  HomopolarityDisplayType.Negtive},
            { "正（+）",  HomopolarityDisplayType.Positive},
        };

        public static Dictionary<HomopolarityDisplayType, byte> HomopolarityDisplayTypeCmdMap = new Dictionary<HomopolarityDisplayType, byte>()
        {
            { HomopolarityDisplayType.Negtive, 0x30},
            { HomopolarityDisplayType.Positive, 0x31},
        };

        public static Dictionary<string, TestVoltageType> TestVoltageTypeMap = new Dictionary<string, TestVoltageType>()
        {
            { "160V",  TestVoltageType.Voltage160V},
            { "20V",  TestVoltageType.Voltage20V},
        };

        public static Dictionary<TestVoltageType, byte> TestVoltageTypeCmdMap = new Dictionary<TestVoltageType, byte>()
        {
            { TestVoltageType.Voltage160V, 0x30},
            { TestVoltageType.Voltage20V, 0x31},
        };

        public static Dictionary<string, HighVoltageConnectionType> HighVoltageConnectionTypeMap = new Dictionary<string, HighVoltageConnectionType>()
        {
            { "Y",  HighVoltageConnectionType.Type_Y},
            { "YN",  HighVoltageConnectionType.Type_YN},
            { "D",  HighVoltageConnectionType.Type_D},
            { "Z",  HighVoltageConnectionType.Type_Z},
            { "ZN",  HighVoltageConnectionType.Type_ZN},
            { "V",  HighVoltageConnectionType.Type_V},
            { "Scott",  HighVoltageConnectionType.Type_SCOTT},
            { "In-Scott",  HighVoltageConnectionType.Type_IN_SCOTT},
        };

        public static Dictionary<HighVoltageConnectionType, byte> HighVoltageConnectionTypeCmdMap = new Dictionary<HighVoltageConnectionType, byte>()
        {
            { HighVoltageConnectionType.Type_Y,  0x30 },
            { HighVoltageConnectionType.Type_YN, 0x31 },
            { HighVoltageConnectionType.Type_D,  0x32 },
            { HighVoltageConnectionType.Type_Z,  0x33 },
            { HighVoltageConnectionType.Type_ZN, 0x34 },
            { HighVoltageConnectionType.Type_V,  0x35 },
            { HighVoltageConnectionType.Type_SCOTT, 0x36 },
            { HighVoltageConnectionType.Type_IN_SCOTT, 0x37 },
        };

        public static Dictionary<string, LowVoltageConnectionType> LowVoltageConnectionTypeMap = new Dictionary<string, LowVoltageConnectionType>()
        {
            { "y",  LowVoltageConnectionType.Type_Y},
            { "yn",  LowVoltageConnectionType.Type_YN},
            { "d",  LowVoltageConnectionType.Type_D},
            { "z",  LowVoltageConnectionType.Type_Z},
            { "zn",  LowVoltageConnectionType.Type_ZN},
            { "v",  LowVoltageConnectionType.Type_V},
            { "Scott",  LowVoltageConnectionType.Type_SCOTT},
            { "In-Scott",  LowVoltageConnectionType.Type_IN_SCOTT},
            { "-",  LowVoltageConnectionType.Type_NULL},
        };

        public static Dictionary<LowVoltageConnectionType, byte> LowVoltageConnectionTypeCmdMap = new Dictionary<LowVoltageConnectionType, byte>()
        {
            { LowVoltageConnectionType.Type_Y,  0x30 },
            { LowVoltageConnectionType.Type_YN, 0x31 },
            { LowVoltageConnectionType.Type_D,  0x32 },
            { LowVoltageConnectionType.Type_Z,  0x33 },
            { LowVoltageConnectionType.Type_ZN, 0x34 },
            { LowVoltageConnectionType.Type_V,  0x35 },
            { LowVoltageConnectionType.Type_SCOTT, 0x36 },
            { LowVoltageConnectionType.Type_IN_SCOTT, 0x37 },
            { LowVoltageConnectionType.Type_NULL, 0x38 },
        };

        public static Dictionary<DeviceState, string> DeviceStateMap = new Dictionary<DeviceState, string>()
        {
            { DeviceState.Initial             , "初始" },
            { DeviceState.SelfeCheck          , "自检" },
            { DeviceState.MainMenu            , "主菜单" },
            { DeviceState.SinglePhaseSetting  , "单相参数设置" },
            { DeviceState.TreePhaseSetting    , "三相参数设置" },
            { DeviceState.TreePhaseAutoSetting, "三相自动参数设置" },
            { DeviceState.SinglePhaseTest     , "单相测试" },
            { DeviceState.TreePhaseTest       , "三相测试" },
            { DeviceState.TreePhaseAutoTest   , "三相自动测试" },
            { DeviceState.VoltageProtection   , "电压保护" },
            { DeviceState.CurrentProtection   , "电流保护" },
            { DeviceState.OverheatProtection  , "过热保护" },
            { DeviceState.OtherState          , "其他状态" },
        };

        public static Dictionary<byte, DeviceState> DeviceStateCmdMap = new Dictionary<byte, DeviceState>()
        {
            { 0x60, DeviceState.Initial },
            { 0x61, DeviceState.SelfeCheck },
            { 0x63, DeviceState.MainMenu },
            { 0x65, DeviceState.SinglePhaseSetting  },
            { 0x66, DeviceState.TreePhaseSetting },
            { 0x67, DeviceState.TreePhaseAutoSetting },
            { 0x6A, DeviceState.SinglePhaseTest },
            { 0x6B, DeviceState.TreePhaseTest },
            { 0x6C, DeviceState.TreePhaseAutoTest },
            { 0x70, DeviceState.VoltageProtection },
            { 0x71, DeviceState.CurrentProtection },
            { 0x72, DeviceState.OverheatProtection },
            { 0x7A, DeviceState.OtherState },
        };

        public static Dictionary<TipInfoType, string> TipInfoTypeMap = new Dictionary<TipInfoType, string>()
        {
            { TipInfoType.Null,      "" },
            { TipInfoType.Testing,   "测试中" },
            { TipInfoType.TestDone,  "测试结束" },
            { TipInfoType.TestError, "测试错误" },
        };

        public static Dictionary<byte, TipInfoType> TipInfoTypeByteMap = new Dictionary<byte, TipInfoType>()
        {
            { 0x30, TipInfoType.Null },
            { 0x31, TipInfoType.Testing },
            { 0x32, TipInfoType.TestDone },
            { 0x33, TipInfoType.TestError },
        };

        public static Dictionary<PowerCodeType, string> PowerCodeTypeMap = new Dictionary<PowerCodeType, string>()
        {
            { PowerCodeType.Full,            "100"          },
            { PowerCodeType.Percent75,       "75"           },
            { PowerCodeType.Percent50,       "50"           },
            { PowerCodeType.Percent25,       "25"           },
            { PowerCodeType.Percent0,        "0"            },
            { PowerCodeType.ExternalPower,   "外部供电"     },
        };

        public static Dictionary<byte, PowerCodeType> PowerCodeTypeByteMap = new Dictionary<byte, PowerCodeType>()
        {
            { 0x30, PowerCodeType.Full },
            { 0x31, PowerCodeType.Percent75 },
            { 0x32, PowerCodeType.Percent50 },
            { 0x33, PowerCodeType.Percent25 },
            { 0x34, PowerCodeType.Percent0 },
            { 0x35, PowerCodeType.ExternalPower },
        };

        public static HighVoltageConnectionType? GetHighConnectionType(string connection)
        {
            foreach (var item in HighVoltageConnectionTypeMap)
            {
                if (item.Key == connection)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static LowVoltageConnectionType? GetLowConnectionType(string connection)
        {
            foreach (var item in LowVoltageConnectionTypeMap)
            {
                if (item.Key == connection)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static HomopolarityDisplayType? GetHomopolarityDisplayType(string type)
        {
            foreach (var item in HomopolarityDisplayTypeMap)
            {
                if (item.Key == type)
                {
                    return item.Value;
                }
            }
            return null;
        }


        public JinYunJYTATestResult? ReadPacket(ref bool needReset)
        {
            needReset = false;
            if (Collector == null)
            {
                return null;
            }
            if (IsConfigChanged)
            {
                IsConfigChanged = false;
                //SetParameters();
            }
            SendRequest();
            byte[]? packet = Collector.ReadData();
            if (packet == null)
            {
                return null;
            }
            if (packet.Length < 1)
            {
                Log.Error("Read Packet Len err" + packet.Length);
                return null;
            }

            byte _deviceState = packet[0];
            deviceState = DeviceStateCmdMap[_deviceState];
            if (deviceState == DeviceState.Initial || deviceState == DeviceState.SelfeCheck)
            {
                return null;
            }

            if (deviceState == DeviceState.VoltageProtection || deviceState == DeviceState.CurrentProtection
                 || deviceState == DeviceState.OtherState)
            {
                needReset = true;
                return null;
            }

            if (packet.Length < 3)
            {
                Log.Error("Read Packet Len err" + packet.Length);
                return null;
            }
            byte _TipInfo = packet[1];
            tipInfo = TipInfoTypeByteMap[_TipInfo];
            byte _PowerCode = packet[2];
            powerCode = PowerCodeTypeByteMap[_PowerCode];
            if (deviceState == DeviceState.MainMenu)
            {
                return null;
            }

            if (deviceState == DeviceState.SinglePhaseTest)
            {
                return ProcessSinglePhaseTestResult(packet);
            }

            if (deviceState == DeviceState.TreePhaseTest)
            {
                return ProcessThreePhaseTestResult(packet);
            }

            return null;
        }

        private JinYunJYTATestResult? ProcessThreePhaseTestResult(byte[] packet)
        {
            // 7E 54 55 31 32 36 6B <提示信息:1字节> <电量码:1字节> 
            // < 变比A:6字节 > < 变比B:6字节 > < 变比C:6字节 >
            // < 匝比A:6字节 > < 匝比B:6字节 > < 匝比C:6字节 >
            // < 误差A:7字节 > < 误差B:7字节 > < 误差C:7字节 >
            // < 试验电压A:7字节 > < 试验电压B:7字节 > < 试验电压C:7字节 >
            // < 励磁电流A:7字节 > < 励磁电流B:7字节 > < 励磁电流C:7字节 >
            // < 联结方式:8字节 > < 分接位:3字节 > < 频率:7字节 > < 计算变比:6字节 > XOR 0D

            if (packet.Length < 126 || packet[0] != 0x6B)
            {
                return null;
            }
            int offset = 3; // 跳过设备状态、提示信息、电量码 三个字节
            string strRatioA = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;
            string strRatioB = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;
            string strRatioC = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;

            string strTurnRatioA = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;
            string strTurnRatioB = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;
            string strTurnRatioC = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;

            string strErrorA = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strErrorB = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strErrorC = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;

            string strVoltageA = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strVoltageB = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strVoltageC = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;

            string strCurrentA = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strCurrentB = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strCurrentC = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;

            string strConnectionType = Encoding.ASCII.GetString(packet, offset, 8);
            offset += 8;

            string tappingPosition = Encoding.ASCII.GetString(packet, offset, 3);
            offset += 3;

            string strFrequence = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;

            string strCalculatedRatio = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;

            float ratioA = Utils.ParseFloat(strRatioA);
            float ratioB = Utils.ParseFloat(strRatioB);
            float ratioC = Utils.ParseFloat(strRatioC);
            float turnRatioA = Utils.ParseFloat(strTurnRatioA);
            float turnRatioB = Utils.ParseFloat(strTurnRatioB);
            float turnRatioC = Utils.ParseFloat(strTurnRatioC);
            float errorA = Utils.ParseFloat(strErrorA);
            float errorB = Utils.ParseFloat(strErrorB);
            float errorC = Utils.ParseFloat(strErrorC);
            float voltageA = Utils.ParseFloat(strVoltageA);
            float voltageB = Utils.ParseFloat(strVoltageB);
            float voltageC = Utils.ParseFloat(strVoltageC);
            float currentA = Utils.ParseFloat(strCurrentA);
            float currentB = Utils.ParseFloat(strCurrentB);
            float currentC = Utils.ParseFloat(strCurrentC);
            float frequence = Utils.ParseFloat(strFrequence);
            float calculatedRatio = Utils.ParseFloat(strCalculatedRatio);

            return new JinYunJYTATestResult()
            {
                IsSinglePhase = false,
                Ratio = new float[] { ratioA, ratioB, ratioC },
                Error = new float[] { errorA, errorB, errorC },
                Voltage = new float[] { voltageA, voltageB, voltageC },
                Current = new float[] { currentA, currentB, currentC },
                TurnRatio = new float[] { turnRatioA, turnRatioB, turnRatioC },
                Frequence = frequence,
                CalculatedRatio = calculatedRatio,
                TappingPosition = tappingPosition,
                ConnectionType = strConnectionType,
            };
        }

        private JinYunJYTATestResult? ProcessSinglePhaseTestResult(byte[] packet)
        {
            // 7E 54 55 30 34 37 6A <提示信息:1字节> <电量码:1字节>
            // <变比:6字节> <误差:7字节> <电压:7字节> <电流:7字节> <频率:7字节>
            // <极性:1字节> <分接位:3字节>  <计算变比:6字节> XOR 0D
            if (packet.Length < 47 || packet[0] != 0x6A)
            {
                return null;
            }
            int offset = 3; // 跳过设备状态、提示信息、电量码 三个字节
            string strRatio = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;
            string strError = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strVoltage = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strCurrent = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            string strFrequence = Encoding.ASCII.GetString(packet, offset, 7);
            offset += 7;
            byte polarity = packet[offset];
            string tappingPosition = Encoding.ASCII.GetString(packet, offset, 3);
            offset += 3;
            string strCalculatedRatio = Encoding.ASCII.GetString(packet, offset, 6);
            offset += 6;
            float ratio = Utils.ParseFloat(strRatio);
            float error = Utils.ParseFloat(strError);
            float voltage = Utils.ParseFloat(strVoltage);
            float current = Utils.ParseFloat(strCurrent);
            float frequence = Utils.ParseFloat(strFrequence);
            float calculatedRatio = Utils.ParseFloat(strCalculatedRatio);

            return new JinYunJYTATestResult()
            {
                IsSinglePhase = true,
                Ratio = new float[] { ratio, 0, 0 },
                Error = new float[] { error, 0, 0 },
                Voltage = new float[] { voltage, 0, 0 },
                Current = new float[] { current, 0, 0 },
                Frequence = frequence,
                CalculatedRatio = calculatedRatio,
                Polarity = polarity,
                TappingPosition = tappingPosition,
            };
        }

        public class JinYunJYTATestResult
        {
            public bool IsSinglePhase = true;
            public float[] Ratio;
            public float[] Error;
            public float[] Voltage;
            public float[] Current;
            public float[] TurnRatio;
            public float Frequence;
            public float CalculatedRatio;
            public byte Polarity;
            public string TappingPosition = string.Empty;
            public string ConnectionType = string.Empty;
        }


        // 主机打开，每400豪秒访问一次从机（寻机命令）或是发送命令，只有发送寻机命令
    }
}

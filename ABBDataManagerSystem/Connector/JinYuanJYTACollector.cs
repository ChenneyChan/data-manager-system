using ABBDataManagerSystem.Configs;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        }

        public enum TipInfoType
        {
            Null,
            Testing,
            TestDone,
            TestError,
        }

        public enum PowerInfoType
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
            Utils.ByteCopy(ProductSequence.ToCharArray(), bProductSequence);

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
            Utils.ByteCopy(ProductSequence.ToCharArray(), bProductSequence);
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
            Collector.SendCommand(new byte[] { 0x64, typeByte });
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

        public class JinYuanJYTAPacketInfo
        {
            public byte deviceState;

            public JinYuanJYTAPacketInfo()
            {

            }

            public override string ToString()
            {
                return "";
            }
        }

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

        public static Dictionary<string, DeviceState> DeviceStateMap = new Dictionary<string, DeviceState>()
        {
            { "初始",  DeviceState.Initial},
            { "自检",  DeviceState.SelfeCheck},
            { "主菜单",  DeviceState.MainMenu},
        };

        public static Dictionary<byte, DeviceState> DeviceStateCmdMap = new Dictionary<byte, DeviceState>()
        {
            {  0x60, DeviceState.Initial },
            {  0x61, DeviceState.SelfeCheck },
            {  0x63, DeviceState.MainMenu },
        };

        public static Dictionary<string, TipInfoType> TipInfoTypeMap = new Dictionary<string, TipInfoType>()
        {
            { "",  TipInfoType.Null},
            { "测试中",  TipInfoType.Testing},
            { "测试结束",  TipInfoType.TestDone},
            { "测试错误",  TipInfoType.TestError},
        };

        public static Dictionary<byte, TipInfoType> TipInfoTypeByteMap = new Dictionary<byte, TipInfoType>()
        {
            { 0x30, TipInfoType.Null },
            { 0x31, TipInfoType.Testing },
            { 0x32, TipInfoType.TestDone },
            { 0x33, TipInfoType.TestError },
        };

        public static Dictionary<string, PowerInfoType> PowerInfoTypeMap = new Dictionary<string, PowerInfoType>()
        {
            { "100",  PowerInfoType.Full},
            { "75",  PowerInfoType.Percent75},
            { "50",  PowerInfoType.Percent50},
            { "25",  PowerInfoType.Percent25},
            { "0",  PowerInfoType.Percent0},
            { "外部供电",  PowerInfoType.ExternalPower},
        };

        public static Dictionary<byte, PowerInfoType> PowerInfoTypeByteMap = new Dictionary<byte, PowerInfoType>()
        {
            { 0x30, PowerInfoType.Full },
            { 0x31, PowerInfoType.Percent75 },
            { 0x32, PowerInfoType.Percent50 },
            { 0x33, PowerInfoType.Percent25 },
            { 0x34, PowerInfoType.Percent0 },
            { 0x35, PowerInfoType.ExternalPower },
        };

        public JinYuanJYTAPacketInfo? ReadPacket()
        {
            /**
             *  从机返回给主机的状态分11类：CH1、CH2状态分别返回
                参数配置状态(51H)、正在充电状态(52H)、正在测试状态（53H）正在放电状态(54H) 、
                温升正在定时状态（55H）、过热保护状态（56H）、记录查看状态(57H)、
                时间调整状态(58H)、通道故障状态(59H)、换大电流 (5AH) 、换小电流 (5BH)
             */

            // （CH1状态）（CH2状态）（波特率）（模式）（CH1通道）（CH1选定电流）（CH2通道）（CH2选定电流）
            // （5字节CH1实时测试电流）（7字节CH1实时测试电阻） 
            // （5字节CH2实时测试电流）（7字节CH2实时测试电阻）
            // （4字节温升定时采集时间）
            // （7字节CH1定时采集电阻）（7字节CH2定时采集电阻） 

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

            byte _deviceState = packet[0];
            DeviceState deviceState = DeviceStateCmdMap[_deviceState];
            byte _TipInfo = packet[1];
            TipInfoType tipInfo = TipInfoTypeByteMap[_TipInfo];
            byte _PowerInfo = packet[2];
            PowerInfoType powerInfo = PowerInfoTypeByteMap[_PowerInfo];

            if (packet.Length < 50)
            {
                Log.Error("Read Packet Len err" + packet.Length);
                return null;
            }


            return null;

        }

        // 主机打开，每400豪秒访问一次从机（寻机命令）或是发送命令，只有发送寻机命令
    }
}

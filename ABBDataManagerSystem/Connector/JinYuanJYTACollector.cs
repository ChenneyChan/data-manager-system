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



        public static int Interval = 400; // 每400ms发送一次寻机指令，从机返回数据

        private readonly ResistanceCurrentInfoCollector Collector;

        private TestTypeJYTA _TestType = TestTypeJYTA.SinglePhaseTest;

        public TestTypeJYTA TestType { get { return _TestType; } set { _TestType = value; IsConfigChanged = true; } }

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

        public void SetResetCommand()
        {
            Collector.SendCommand(new byte[] { 0x40 });
        }

        public float RatedHighVoltage = 0;
        public float RatedLowVoltage = 0;
        public float RatedConnection = 0;
        public float ConnectionPosition = 0;
        public float PositiveConnectionCount = 0;
        public float ConnectionGap = 0;
        public string ProductSequence = "";
        public float HomopolarityDisplay = 0;
        public float TestVoltage = 0;

        public void SetSinglePhaseTest()
        {
            // <额定高压:8字节><额定低压:8字节><额定分接:2字节><分接位置:1字节><正分接数:2字节><分接间距:6字节>
            // <试品编号:14字节> <同极性显示:1字节> <试验电压:1字节>XOR 0D  

            byte[] bRatedHV = new byte[8];
            byte[] bRatedLV = new byte[8];
            Utils.FloatToBytes(RatedHighVoltage, bRatedHV);
            Utils.FloatToBytes(RatedLowVoltage, bRatedLV);

            Collector.SendCommand(new byte[] { 0x61 });
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

        #region 仪器参数设置命令
        public void SetParameters()
        {
            // 0x45（波特率）（模式）（CH1）（CH1电流）（CH2）（CH2电流）
            Collector.SendCommand(new byte[] {
                0x45,
                GetBoundRateByte(9600),
                GetTestType(TestType),
                (byte)(CH1Enabled ? 0x31 : 0x30),
                (byte)(CH2Enabled ? 0x31 : 0x30),
                GetChanelOneCurrentSet(CH1CurrentConfig),
                GetChanelTwoCurrentSet(CH2CurrentConfig),
            });
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

        public static byte GetTestType(TestType20W type)
        {
            // 常规模式：（30H）、10″温升：（31H）、30″温升：（32H）、60″温升：（33H）
            switch (type)
            {
                case TestType20W.CommonTest: return 0x30;
                case TestType20W.TemperatureRaise10Minute: return 0x31;
                case TestType20W.TemperatureRaise30Minute: return 0x32;
                case TestType20W.TemperatureRaise60Minute: return 0x33;
                default:
                    return 0x30;
            }
        }

        public static TestType20W GetTestTypeByByte(byte type)
        {
            switch (type)
            {
                case 0x31:
                    return TestType20W.TemperatureRaise10Minute;
                case 0x32:
                    return TestType20W.TemperatureRaise30Minute;
                case 0x33:
                    return TestType20W.TemperatureRaise60Minute;
                case 0x30:
                default:
                    return TestType20W.CommonTest;
            }
        }

        public static byte GetChanelOneCurrentSet(CH1Currents current)
        {
            // 5A：(30H) 、1A：(31H) 、0.1A：(32H) 、0.01A：(33H)
            switch (current)
            {
                case CH1Currents.Current5A: return 0x30;
                case CH1Currents.Current1A: return 0x31;
                case CH1Currents.Current01A: return 0x32;
                case CH1Currents.Current001A: return 0x33;
                default:
                    return 0x30;
            }
        }

        public static byte GetChanelTwoCurrentSet(CH2Currents current)
        {
            // 20A：(30H) 、10A：(31H) 、5A：(32H) 、2A：(33H)
            switch (current)
            {
                case CH2Currents.Current20A: return 0x30;
                case CH2Currents.Current10A: return 0x31;
                case CH2Currents.Current5A: return 0x32;
                case CH2Currents.Current2A: return 0x33;
                default:
                    return 0x30;
            }
        }

        public static CH1Currents GetCH1CurrentConfig(string currentConfig)
        {
            return CH1CurrentsMap[currentConfig];
        }

        public static CH2Currents GetCH2CurrentConfig(string currentConfig)
        {
            return CH2CurrentsMap[currentConfig];
        }

        #endregion

        public class JinYuan20WPacketInfo
        {
            public byte ch1Status;
            public byte ch2Status;
            public bool ch1Enabled;
            public bool ch2Enabled;
            public TestType20W type;
            public byte ch1SelectedCurrent;
            public byte ch2SelectedCurrent;
            public float ch1RealTimeCurrent;
            public float ch2RealTimeCurrent;
            public float ch1RealTimeResistance;
            public float ch2RealTimeResistance;
            public float ch1TimedResistance;
            public float ch2TimedResistance;
            public float tempRaiseTimeInterval;
            public JinYuan20WPacketInfo()
            {

            }

            public override string ToString()
            {
                return $"Channel 1 Status: {ch1Status}, " +
                       $"Channel 2 Status: {ch2Status}, " +
                       $"Channel 1 Enabled: {ch1Enabled}, " +
                       $"Channel 2 Enabled: {ch2Enabled}, " +
                       $"Type: {type}, " +
                       $"Channel 1 Selected Current: {ch1SelectedCurrent}, " +
                       $"Channel 2 Selected Current: {ch2SelectedCurrent}, " +
                       $"Channel 1 Real Time Current: {ch1RealTimeCurrent}, " +
                       $"Channel 2 Real Time Current: {ch2RealTimeCurrent}, " +
                       $"Channel 1 Real Time Resistance: {ch1RealTimeResistance}, " +
                       $"Channel 2 Real Time Resistance: {ch2RealTimeResistance}, " +
                       $"Channel 1 Timed Resistance: {ch1TimedResistance}, " +
                       $"Channel 2 Timed Resistance: {ch2TimedResistance}, " +
                       $"Temperature Raise Time Interval: {tempRaiseTimeInterval}";
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

        public static Dictionary<string, CH1Currents> CH1CurrentsMap = new Dictionary<string, CH1Currents>()
        {
            { "5A", CH1Currents.Current5A },
            { "1A", CH1Currents.Current1A },
            { "0.1A", CH1Currents.Current01A },
            { "0.01A", CH1Currents.Current001A },
        };

        public static Dictionary<string, CH2Currents> CH2CurrentsMap = new Dictionary<string, CH2Currents>()
        {
            { "20A", CH2Currents.Current20A },
            { "10A", CH2Currents.Current10A },
            { "5A", CH2Currents.Current5A },
            { "2A", CH2Currents.Current2A },
        };

        public JinYuan20WPacketInfo? ReadPacket()
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
                SetParameters();
            }
            SendRequest();
            byte[]? packet = Collector.ReadData();
            if (packet == null)
            {
                return null;
            }
            if (packet.Length < 50)
            {
                Log.Error("Read Packet Len err" + packet.Length);
                return null;
            }
            byte ch1Status = packet[0];
            byte ch2Status = packet[1];
            byte boudRate = packet[2];
            byte testMode = packet[3];
            byte ch1 = packet[4];
            byte ch1Current = packet[5];
            byte ch2 = packet[6];
            byte ch2Current = packet[7];
            int startIndex = 7;
            byte[] ch1CurrentValue = { packet[startIndex + 1], packet[startIndex + 2], packet[startIndex + 3], packet[startIndex + 4], packet[startIndex + 5] };
            byte[] ch1ResistanceValue = { packet[startIndex + 6], packet[startIndex + 7], packet[startIndex + 8], packet[startIndex + 9], packet[startIndex + 10], packet[startIndex + 11], packet[startIndex + 12] };
            startIndex = startIndex + 12;
            byte[] ch2CurrentValue = { packet[startIndex + 1], packet[startIndex + 2], packet[startIndex + 3], packet[startIndex + 4], packet[startIndex + 5] };
            byte[] ch2ResistanceValue = { packet[startIndex + 6], packet[startIndex + 7], packet[startIndex + 8], packet[startIndex + 9], packet[startIndex + 10], packet[startIndex + 11], packet[startIndex + 12] };
            startIndex = startIndex + 12;
            byte[] tempSetTimeInterval = { packet[startIndex + 1], packet[startIndex + 2], packet[startIndex + 3], packet[startIndex + 4] };
            startIndex = startIndex + 4;
            byte[] ch1TimedResistanceValue = { packet[startIndex + 1], packet[startIndex + 2], packet[startIndex + 3], packet[startIndex + 4], packet[startIndex + 5], packet[startIndex + 6], packet[startIndex + 7] };
            startIndex = startIndex + 7;
            byte[] ch2TimedResistanceValue = { packet[startIndex + 1], packet[startIndex + 2], packet[startIndex + 3], packet[startIndex + 4], packet[startIndex + 5], packet[startIndex + 6], packet[startIndex + 7] };
            startIndex = startIndex + 7;

            string strCh1Current = Encoding.ASCII.GetString(ch1CurrentValue);
            string strCh2Current = Encoding.ASCII.GetString(ch2CurrentValue);
            string strCh1Resistance = Encoding.ASCII.GetString(ch1ResistanceValue);
            string strCh2Resistance = Encoding.ASCII.GetString(ch2ResistanceValue);
            string strCh1TimedResistant = Encoding.ASCII.GetString(ch1TimedResistanceValue);
            string strCh2TimedResistant = Encoding.ASCII.GetString(ch2TimedResistanceValue);
            string strTempSetTimeInterval = Encoding.ASCII.GetString(tempSetTimeInterval);

            return new JinYuan20WPacketInfo()
            {
                ch1Enabled = ch1 == 0x31,
                ch2Enabled = ch2 == 0x31,
                ch1Status = ch1Status,
                ch2Status = ch2Status,
                type = GetTestTypeByByte(testMode),
                ch1RealTimeCurrent = Utils.ParseFloat(strCh1Current),
                ch2RealTimeCurrent = Utils.ParseFloat(strCh2Current),
                ch1RealTimeResistance = Utils.ParseFloat(strCh1Resistance),
                ch2RealTimeResistance = Utils.ParseFloat(strCh2Resistance),
                ch1SelectedCurrent = ch1Current,
                ch2SelectedCurrent = ch2Current,
                ch1TimedResistance = Utils.ParseFloat(strCh1TimedResistant),
                ch2TimedResistance = Utils.ParseFloat(strCh2TimedResistant),
                tempRaiseTimeInterval = Utils.ParseFloat(strTempSetTimeInterval),
            };
        }

        // 主机打开，每400豪秒访问一次从机（寻机命令）或是发送命令，只有发送寻机命令
    }
}

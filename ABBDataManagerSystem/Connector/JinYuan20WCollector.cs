using System.Text;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYuan20WCollector
    {
        /**
         * 常规测试命令 (41H)、温升测试命令（42H）、温升定时命令（43H）、
            复位命令 (44H)、参数设置命令 (45H)、常规保存命令 (46H)、
            常规打印命令 (47H)、寻机命令(48H)。
        */
        public enum TestType20W
        {
            CommonTest,
            TemperatureRise10Sec,
            TemperatureRise30Sec,
            TemperatureRise60Sec,
        }

        // CH1高压侧 CH2低压侧

        public enum CH1Currents
        {
            Current5A = 0,
            Current1A,
            Current01A,
            Current001A,
        }

        public enum CH2Currents
        {
            Current20A = 0,
            Current10A,
            Current5A,
            Current2A,
        }

        public static int Interval = 400; // 每400ms发送一次寻机指令，从机返回数据

        private readonly ResistanceCurrentInfoCollector Collector;

        private TestType20W _TestType = TestType20W.CommonTest;
        private bool _CH1Enabled = false;
        private bool _CH2Enabled = false;
        private CH1Currents _CH1CurrentConfig = CH1Currents.Current5A;
        private CH2Currents _CH2CurrentConfig = CH2Currents.Current20A;

        public TestType20W TestType { get { return _TestType; } set { _TestType = value; IsConfigChanged = true; } }

        public bool CH1Enabled { get { return _CH1Enabled; } set { _CH1Enabled = value; IsConfigChanged = true; } }

        public bool CH2Enabled { get { return _CH2Enabled; } set { _CH2Enabled = value; IsConfigChanged = true; } }

        public CH1Currents CH1CurrentConfig { get { return _CH1CurrentConfig; } set { _CH1CurrentConfig = value; IsConfigChanged = true; } }

        public CH2Currents CH2CurrentConfig { get { return _CH2CurrentConfig; } set { _CH2CurrentConfig = value; IsConfigChanged = true; } }

        private bool IsConfigChanged = false;

        private bool NeedDelay = false;


        public JinYuan20WCollector(string portName, int baudRate = 9600)
        {
            Collector = new ResistanceCurrentInfoCollector(portName, baudRate)
            {
                StartByte = 0x7E,
                StopByte = 0x0D,
                DeviceAddress = new byte[] { 0x3E, 0x3E }
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

        public void SetCommonTest()
        {
            Collector.SendCommand(new byte[] { 0x41 });
            NeedDelay = true;
        }

        public void SetTemperatureRaiseTest()
        {
            Collector.SendCommand(new byte[] { 0x42 });
            NeedDelay = true;
        }

        public void SetTemperatureRaiseTimerCommand()
        {
            Collector.SendCommand(new byte[] { 0x43 });
            NeedDelay = true;
        }

        public void SetRestCommand()
        {
            Collector.SendCommand(new byte[] { 0x44 });
            NeedDelay = true;
        }

        public void SetPrintCommand()
        {
            Collector.SendCommand(new byte[] { 0x47 });
            NeedDelay = true;
        }

        public void SendRequest()
        {
            Collector.SendCommand(new byte[] { 0x48 });
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
            NeedDelay = true;
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
                case TestType20W.TemperatureRise10Sec: return 0x31;
                case TestType20W.TemperatureRise30Sec: return 0x32;
                case TestType20W.TemperatureRise60Sec: return 0x33;
                default:
                    return 0x30;
            }
        }

        public static TestType20W GetTestTypeByByte(byte type)
        {
            switch (type)
            {
                case 0x31:
                    return TestType20W.TemperatureRise10Sec;
                case 0x32:
                    return TestType20W.TemperatureRise30Sec;
                case 0x33:
                    return TestType20W.TemperatureRise60Sec;
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

        public static Dictionary<string, TestType20W> TestTypeMap = new Dictionary<string, TestType20W>()
        {
            { "常规测试",  TestType20W.CommonTest},
            { "温升测试-10秒",  TestType20W.TemperatureRise10Sec},
            { "温升测试-30秒",  TestType20W.TemperatureRise30Sec},
            { "温升测试-60秒",  TestType20W.TemperatureRise60Sec},
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
                return null;
            }
            if (NeedDelay)
            {
                NeedDelay = false;
                return null;
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

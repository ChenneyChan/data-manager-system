using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYuan20WCollector
    {
        /**
         * 常规测试命令 (41H)、温升测试命令（42H）、温升定时命令（43H）、
            复位命令 (44H)、参数设置命令 (45H)、常规保存命令 (46H)、
            常规打印命令 (47H)、寻机命令(48H)。
        */
        public enum TestType
        {
            CommonTest,
            TemperatureRaise10Minute,
            TemperatureRaise30Minute,
            TemperatureRaise60Minute,
        }

        public enum ChanelOneCurrents
        {
            Current5A,
            Current1A,
            Current01A,
            Current001A,
        }

        public enum ChanelTwoCurrents
        {
            Current20A,
            Current10A,
            Current5A,
            Current2A,
        }

        private readonly ResistanceCurrentInfoCollector collector;

        public TestType testType { get; set; } = TestType.CommonTest;

        public bool chanelOneEnable { get; set; } = true;

        public bool chanelTwoEnable { get; set; } = true;

        public ChanelOneCurrents chanelOneCurrent { get; set; } = ChanelOneCurrents.Current5A;

        public ChanelTwoCurrents chanelTwoCurrent { get; set; } = ChanelTwoCurrents.Current20A;


        public JinYuan20WCollector(string portName, int baudRate = 9600)
        {
            collector = new ResistanceCurrentInfoCollector(portName, baudRate)
            {
                StartByte = 0x7E,
                StopByte = 0x0D,
                DeviceAddress = new byte[] { 0x3E, 0x3E }
            };
        }

        public bool StartCollect()
        {
            return collector.Open();
        }

        public void StopCollect()
        {
            collector.Close();
        }

        public void SetCommonTest()
        {
            collector.SendCommand(new byte[] { 0x41 });
        }

        public void SetTemperatureRaiseTest()
        {
            collector.SendCommand(new byte[] { 0x42 });
        }

        public void SetTemperatureRaiseTimerCommand()
        {
            collector.SendCommand(new byte[] { 0x43 });
        }

        public void SetRestCommand()
        {
            collector.SendCommand(new byte[] { 0x44 });
        }

        public void SendRequest()
        {
            collector.SendCommand(new byte[] { 0x48 });
        }

        #region 仪器参数设置命令
        public void SetParameters()
        {
            collector.SendCommand(new byte[] {
                0x45,
                GetBoundRateByte(9600),
                GetTestType(testType),
                (byte)(chanelOneEnable ? 0x31 : 0x30),
                (byte)(chanelTwoEnable ? 0x31 : 0x30),
                GetChanelOneCurrentSet(chanelOneCurrent),
                GetChanelTwoCurrentSet(chanelTwoCurrent),
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

        public static byte GetTestType(TestType type)
        {
            // 常规模式：（30H）、10″温升：（31H）、30″温升：（32H）、60″温升：（33H）
            switch (type)
            {
                case TestType.CommonTest: return 0x30;
                case TestType.TemperatureRaise10Minute: return 0x31;
                case TestType.TemperatureRaise30Minute: return 0x32;
                case TestType.TemperatureRaise60Minute: return 0x33;
                default:
                    return 0x30;
            }
        }

        public static byte GetChanelOneCurrentSet(ChanelOneCurrents current)
        {
            // 5A：(30H) 、1A：(31H) 、0.1A：(32H) 、0.01A：(33H)
            switch (current)
            {
                case ChanelOneCurrents.Current5A: return 0x30;
                case ChanelOneCurrents.Current1A: return 0x31;
                case ChanelOneCurrents.Current01A: return 0x32;
                case ChanelOneCurrents.Current001A: return 0x33;
                default:
                    return 0x30;
            }
        }

        public static byte GetChanelTwoCurrentSet(ChanelTwoCurrents current)
        {
            // 20A：(30H) 、10A：(31H) 、5A：(32H) 、2A：(33H)
            switch (current)
            {
                case ChanelTwoCurrents.Current20A: return 0x30;
                case ChanelTwoCurrents.Current10A: return 0x31;
                case ChanelTwoCurrents.Current5A: return 0x32;
                case ChanelTwoCurrents.Current2A: return 0x33;
                default:
                    return 0x30;
            }
        }
        #endregion

        public void ReadPacket()
        {
            if (collector == null)
            {
                return;
            }
            SendRequest();
            byte[]? packet = collector.ReadData();
            if (packet == null)
            {
                return;
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
        }

        // 主机打开，每400豪秒访问一次从机（寻机命令）或是发送命令，只有发送寻机命令
    }
}

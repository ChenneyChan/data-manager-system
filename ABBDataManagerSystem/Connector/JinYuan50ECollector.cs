using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYuan50ECollector
    {
        public enum TestType50E
        {
            CommonTest,
            TemperatureRaise,
        }   

        public enum TestMode
        {
            SinglePhase,
            DoublePhase,
            HighVoltagePhaseSelection,
            LowVoltagePhaseSelection,
        }

        public static int Interval = 400; // 每400ms发送一次寻机指令，从机返回数据

        private readonly ResistanceCurrentInfoCollector Collector;
        
        public TestType50E TestType;

        public TestMode TestModeValue;

        public string ConnectionMode = string.Empty;

        public string CurrentSelected = string.Empty;

        public string PhaseSelected = string.Empty;

        public int TempRiseInterval = 10;

        public JinYuan50ECollector(string portName, int baudRate = 9600)
        {
            Collector = new ResistanceCurrentInfoCollector(portName, baudRate)
            {
                StartByte = 0x7E,
                StopByte = 0x0D,
                DeviceAddress = new byte[] { 0x51, 0x51 },
                SendLenByteCount = 1,
                RspLenByteCount = 2,
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

        public static Dictionary<TestMode, object[]> TestModeByteMap = new Dictionary<TestMode, object[]>()
        {
            {TestMode.SinglePhase, new object[]{"单通道", (byte)0x30} },
            {TestMode.DoublePhase, new object[]{ "双通道", (byte)0x31 } },
            {TestMode.HighVoltagePhaseSelection, new object[]{ "高压选相", (byte)0x32 } },
            {TestMode.LowVoltagePhaseSelection, new object[]{ "低压选相", (byte)0x33 } },
        };

        public static Dictionary<string, object[]> TestModeMap = new Dictionary<string, object[]>
        {
            { "单通道", new object[]{ (byte)0x30, new Dictionary<byte, string>() { { 0x20, ""} } } },
            { "双通道", new object[]{ (byte)0x31, new Dictionary<byte, string>() { { 0x20, ""} } } },
            { "高压选相", new object[]{ (byte)0x32, new Dictionary<byte, string>() { { 0x30, "Yn"}, { 0x31, "Dy"} } } },
            { "低压选相", new object[]{ (byte)0x33, new Dictionary<byte, string>() { { 0x30, "Yn"}, { 0x31, "Dy"} } } },
            //{ "助磁", new object[]{ 0x34, 
            //    new Dictionary<byte, string>() 
            //    { 
            //        { 0x30, "YnD 1"}, 
            //        { 0x31, "YnD 3"} 
            //    } 
            //} },
        };

        public Dictionary<string, byte> CurrentStringMap = new Dictionary<string, byte>{
            { "50A", 0x30},
            { "40A", 0x31},
            { "20A", 0x32},
            { "10A", 0x33},
            { "3A", 0x34},
            { "1A", 0x35},
            { "0.3A", 0x36},
            { "0.1A", 0x37},
            { "0.02A", 0x38}, // (常规单通道模式下才有)
        };

        private byte GetConnectionModeByte()
        {
            string testModeStrig = (string)TestModeByteMap[TestModeValue][0];
            Dictionary<byte, string> dict1 = (Dictionary<byte, string>)TestModeMap[testModeStrig][1];

            foreach (var item in dict1)
            {
                if (ConnectionMode == item.Value)
                {
                    return item.Key;
                }
            }
            return 0x20;
        }

        // 查询命令/请求数据命令（唯一有返回的命令 300ms查询一次）
        public void SendQueryCommand()
        {
            Collector.SendCommand(new byte[] { 0x50 });
        }

        public byte[]? QueryMsg()
        {
            SendQueryCommand();
            var packet = Collector.ReadData();
            return packet;
        }

        // 测试项目选择命令(复位状态下可用)
        public void SetTestType(TestType50E type)
        {
            byte typeByte = type == TestType50E.CommonTest ? (byte)0x30 : (byte)0x31;
            Collector.SendCommand(new byte[] { 0x51, typeByte });
        }

        // 参数设置/开始命令(常规参数状态/温升参数状态 下可用) 
        public void SetParameters()
        {
            string tempRiseIntervalStr = TempRiseInterval < 10 ? $" {TempRiseInterval}" : TempRiseInterval.ToString();
            byte[] tempRiseIntervalBytes = Encoding.UTF8.GetBytes(tempRiseIntervalStr);

            byte testMode = (byte)TestModeByteMap[TestModeValue][1];
            // 52 +测试项目(1byte)+电流(1byte) + 测试方式(1byte)  + 联结方式(1byte) +温升间隔(2byte)
            Collector.SendCommand(new byte[] {
                0x52,
                TestType == TestType50E.CommonTest ? (byte)0x30 : (byte)0x31,
                CurrentStringMap[CurrentSelected],
                testMode,
                GetConnectionModeByte(),
                tempRiseIntervalBytes[0],
                tempRiseIntervalBytes[1],
            });
        }

        #region 温升测试
        // 温升测试命令：定时
        public void StartTempRiseStartClocking()
        {
            Collector.SendCommand(new byte[] { 0x53, 0x2F });
        }

        // 温升测试命令：测试
        public void StartTempRiseStartTest()
        {
            Collector.SendCommand(new byte[] { 0x53, 0x30 });
        }

        // 温升测试命令：停止
        public void StartTempRiseStopTest()
        {
            Collector.SendCommand(new byte[] { 0x53, 0x31 });
        }

        // 温升测试命令：退出
        public void StartTempRiseStopQuit()
        {
            Collector.SendCommand(new byte[] { 0x53, 0x32 });
        }
        #endregion
    }
}

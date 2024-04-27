using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Connector
{
    internal class JinYun50ECollector
    {
        public enum TestType50E
        {
            CommonTest,
            TemperatureRaise,
        }

        public static int Interval = 400; // 每400ms发送一次寻机指令，从机返回数据

        private readonly ResistanceCurrentInfoCollectorSingleLen Collector;

        public JinYun50ECollector(string portName, int baudRate = 9600)
        {
            Collector = new ResistanceCurrentInfoCollectorSingleLen(portName, baudRate)
            {
                StartByte = 0x7E,
                StopByte = 0x0D,
                DeviceAddress = new byte[] { 0x51, 0x51 }
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

        // （唯一有返回的命令 300ms查询一次）
        public void SendQueryCommand()
        {
            Collector.SendCommand(new byte[] { 0x50 });
        }

        public void SetTestType(TestType50E type)
        {
            byte typeByte;
            if (type == TestType50E.CommonTest)
            {
                typeByte = 0x30;
            }
            else
            {
                typeByte = 0x31;
            }
            Collector.SendCommand(new byte[] { 0x51, typeByte });
        }

        TestType50E TestType;

        private static byte GetTestType(TestType50E testType)
        {
            if (testType == TestType50E.CommonTest)
            {
                return 0x30;
            }
            else
            {
                return 0x31;
            }
        }

        public static Dictionary<string, object[]> TestModeMap = new Dictionary<string, object[]>
        {
            { "单通道", new object[]{ 0x30, new Dictionary<byte, string>() { { 0x20, ""} } } },
            { "双通道", new object[]{ 0x31, new Dictionary<byte, string>() { { 0x20, ""} } } },
            { "高压选相", new object[]{ 0x32, new Dictionary<byte, string>() { { 0x30, "Yn"}, { 0x31, "Dy"} } } },
            { "低压选相", new object[]{ 0x33, new Dictionary<byte, string>() { { 0x30, "Yn"}, { 0x31, "Dy"} } } },
            //{ "助磁", new object[]{ 0x34, 
            //    new Dictionary<byte, string>() 
            //    { 
            //        { 0x30, "YnD 1"}, 
            //        { 0x31, "YnD 3"} 
            //    } 
            //} },
        };

        public byte[]? QueryMsg()
        {
            SendQueryCommand();
            var packet = Collector.ReadData();
            return packet;
        }

        //public void SetParameters()
        //{
        //    // 52 +测试项目(1byte)+电流(1byte) + 测试方式(1byte)  + 联结方式(1byte) +温升间隔(2byte)
        //    Collector.SendCommand(new byte[] {
        //        0x52,
        //        GetTestType(TestType),
        //        (byte)(CH1Enabled ? 0x31 : 0x30),
        //        (byte)(CH2Enabled ? 0x31 : 0x30),
        //        GetChanelOneCurrentSet(CH1CurrentConfig),
        //        GetChanelTwoCurrentSet(CH2CurrentConfig),
        //    });
        //}
    }
}

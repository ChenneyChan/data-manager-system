using System;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using ABBDataManagerSystem.Tools;
using Modbus.Device;
using Modbus.Utility; // NModbus命名空间  

namespace ABBDataManagerSystem.Connector
{
    public class TempModbusCollector
    {
        private string _serialName;
        private int _serialBoundRate;
        private int _slaveId;

        private SerialPort? _serialPort;
        private ModbusMaster? _modbusMaster;

        private ushort startIndex = 0;
        private ushort count = 0;
        private int recordLog = 0;
        private bool isConfigRead = false;

        public TempModbusCollector(string serialName, int serialBoundRate = 9600, int slaveId = 1)
        {
            _serialName = serialName;
            _serialBoundRate = serialBoundRate;
            _slaveId = slaveId;
            ReadConfig();
        }

        private void ReadConfig()
        {
            string filePath = "./temp_config.txt"; // 文件路径  
            string? firstLine = null;

            try
            {
                // 使用StreamReader读取文件  
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // 读取第一行  
                    firstLine = sr.ReadLine();
                }
                if (firstLine == null)
                {
                    return;
                }

                // 输出第一行内容  
                Log.Info("第一行内容: " + firstLine);

                var lines = firstLine.Split(" ");
                if (lines.Length == 3)
                {
                    startIndex = (ushort)Utils.ParseInt(lines[0]);
                    count = (ushort)Utils.ParseInt(lines[1]);
                    recordLog = Utils.ParseInt(lines[2]);
                    isConfigRead = true;
                }
            }
            catch (Exception)
            {
            }
        }

        public bool Connect()
        {
            return ConnectRtu();
        }

        private bool ConnectRtu()
        {
            lock (this)
            {
                _serialPort = new SerialPort(_serialName, _serialBoundRate, Parity.None)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000,
                };
                try
                {
                    _serialPort.Open();
                    if (!_serialPort.IsOpen)
                    {
                        Log.Error("fail to open serial port no exception");
                        _serialPort = null;
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("fail to open serial port " + e.Message);
                    return false;
                }
                //_modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);
                return true;
            }
        }

        public void Disconnect()
        {
            lock (this)
            {
                _modbusMaster?.Dispose();
                _modbusMaster = null;
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    try
                    {
                        _serialPort.Close();
                    }
                    catch (IOException e)
                    {
                        Log.Error("fail to close serial port " + e.Message);
                    }
                }
                _serialPort = null;
            }
        }

        public List<float>? ReadPacket2(int slotCount)
        {
            if (_serialPort == null)
            {
                return null;
            }
            byte[] request = {
                1, // 从站地址
                0x03, // 功能码（读取多个寄存器）
                0x00, (byte)startIndex, // 起始寄存器地址（通道1的地址30001转换为16进制）
                0x00, (byte)slotCount, // 寄存器个数（读取4个寄存器，即通道1至4）
                0x00, 0x00  // CRC校验，需根据Modbus协议计算得出
            };
            var crcs = ModbusCRC.Calculate_CRC16_C(request, 0, request.Length - 2);
            request[request.Length - 2] = crcs[0];
            request[request.Length - 1] = crcs[1];
            int bufferLen = slotCount * 2 + 10;
            byte[] buffer = new byte[bufferLen];
            try
            {
                _serialPort.Write(request, 0, request.Length);
                int byteRead = _serialPort.Read(buffer, 0, bufferLen);
                Log.Info("ReadPacket Len is " + byteRead);
                Utils.DumpBuffer(buffer, 0, byteRead);
            }
            catch { }
            //// 假设10个连续通道的float型温度寄存器地址起始于30101（通道1）至301A（通道10）
            //for (int channel = 1; channel <= 10; channel++)
            //{
            //    // 计算当前通道的寄存器地址（假设浮点寄存器地址每通道递增2）
            //    int regAddress = 3000 + (channel - 1) * 2;

            //    // 构建读取浮点数的Modbus请求（示例中未含实际构建过程，实际需根据Modbus协议构造请求帧）
            //    // 注意：实际应用中需要实现发送Modbus请求和接收响应的逻辑

            //    // 这里简化处理，直接模拟数据处理逻辑，实际应替换为串口读取并解析响应
            //    byte[] tempBytes = new byte[4]; // 一个float占4字节
            //                                    // 假设从串口读取到了正确响应并转换为tempBytes，实际应用需用mySerialPort.Read等方法读取并处理CRC校验等
            //    float temperature = BitConverter.ToSingle(tempBytes);

            //    Console.WriteLine($"Channel {channel}: {temperature} °C");

            //    // 模拟延迟10ms，实际应用中可能需要更复杂的同步机制来确保准确周期
            //    Thread.Sleep(10);
            //}
            return null;
        }

        public List<float> ReadData(int slotCount, out string msg)
        {
            lock (this)
            {
                List<float> list = new List<float>();
                if (_modbusMaster == null)
                {
                    msg = "";
                    return list;
                }
                try
                {
                    ushort startAddress = 1000;
                    if (isConfigRead)
                    {
                        startAddress = this.startIndex;
                    }
                    // 读取输入寄存器的浮点数值
                    string outValue = "Bytes:\r\n\t";
                    ushort[] registers = _modbusMaster.ReadInputRegisters((byte)_slaveId, startAddress, (ushort)(slotCount * 2));
                    foreach (var item in registers)
                    {
                        outValue += item.ToString("X2") + ", ";
                        if (item != 0 && this.recordLog > 0)
                        {
                            Log.Info("GetValidValue " + item);
                        }
                    }
                    outValue += "\r\nFormated:\r\n\t";
                    for (int i = 0; i < slotCount; i++)
                    {
                        if (i * 2 + 1 < registers.Length)
                        {
                            float value = ModbusUtility.GetSingle(registers[i * 2 + 1], registers[i * 2]); // 转换为浮点数值
                            list.Add(value);
                            outValue += Utils.FloatFormat(value) + " ";
                        }
                    }
                    msg = outValue;
                }
                catch (Exception e)
                {
                    Log.Error("fail to read register:" + e.Message);
                    msg = "Fail to read register:";
                }
                if (this.recordLog > 0)
                {
                    Log.Info("msg: " + msg);
                }
                return list;
            }
        }
    }
}
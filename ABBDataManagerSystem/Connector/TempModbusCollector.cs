using System;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
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

        private TcpClient? _tcpClient;

        private bool UsingTcp = false;

        private ushort startIndex = 0;
        private ushort count = 0;
        private int recordLog = 0;

        public TempModbusCollector(string serialName, int serialBoundRate = 9600, bool useTcp = false, int slaveId = 1)
        {
            _serialName = serialName;
            _serialBoundRate = serialBoundRate;
            _slaveId = slaveId;
            UsingTcp = useTcp;
        }

        private void ReadConfig()
        {
            string filePath = "./temp_config.txt"; // 文件路径  
            string firstLine = string.Empty;

            try
            {
                // 使用StreamReader读取文件  
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // 读取第一行  
                    firstLine = sr.ReadLine();
                }

                // 输出第一行内容  
                Log.Info("第一行内容: " + firstLine);

                var lines = firstLine.Split(" ");
                if (lines.Length == 3)
                {
                    startIndex = (ushort)Utils.ParseInt(lines[0]);
                    count = (ushort)Utils.ParseInt(lines[1]);
                    recordLog = Utils.ParseInt(lines[2]);
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception ex)
            {
            }
        }

        public bool Connect()
        {
            if (UsingTcp)
            {
                //ReadConfig();
                return ConnectTcp();
            }
            else
            {
                return ConnectRtu();
            }
        }

        private bool ConnectRtu()
        {
            lock (this)
            {
                _serialPort = new SerialPort(_serialName, _serialBoundRate, Parity.None)
                {
                    ReadTimeout = 2000,
                    WriteTimeout = 2000,
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
                _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);
                return true;
            }
        }

        private bool ConnectTcp()
        {
            lock (this)
            {
                try
                {
                    _tcpClient = new TcpClient(_serialName, _serialBoundRate)
                    {
                        SendTimeout = 2000,
                        ReceiveTimeout = 2000,
                    };
                }
                catch (Exception e)
                {
                    Log.Error("fail to open tcp client " + e.Message);
                    return false;
                }
                _modbusMaster = ModbusIpMaster.CreateIp(_tcpClient);
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
                if (_tcpClient != null)
                {
                    try
                    {
                        _tcpClient.Close();
                    }
                    catch (IOException e)
                    {
                        Log.Error("fail to close tcp client " + e.Message);
                    }
                }
                _tcpClient = null;
            }
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
                    ushort startAddress = 0;
                    if (UsingTcp)
                    {
                        //startAddress = this.startIndex;
                        startAddress = 1000;
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
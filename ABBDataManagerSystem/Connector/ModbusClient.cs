using System;
using System.Net.Sockets;
using Modbus.Device; // NModbus命名空间  

namespace ABBDataManagerSystem.Connector
{
    public class ModbusClient
    {
        // private ModbusIpMaster master;
        // // private ModbusIpSlave slave;
        // private TcpClient tcpClient;
        // private NetworkStream stream;

        // public ModbusClient(string ipAddress, int port)
        // {
        //     tcpClient = new TcpClient(ipAddress, port);
        //     master = ModbusIpMaster.CreateIp(tcpClient); // 创建主站（客户端）对象  
        // }

        // public void Connect()
        // {
        //     if (tcpClient.Connected) return; // 如果已连接，则直接返回  
        //     stream = tcpClient.GetStream(); // 获取网络流  
        //     master.Connect(stream); // 连接主站到从站（服务器）  
        // }

        // public void Disconnect()
        // {
        //     if (!tcpClient.Connected) return; // 如果未连接，则直接返回  
        //     stream.Close(); // 关闭网络流  
        //     master.Disconnect(); // 断开主站连接  
        // }

        // public void ReadCoilStatus(int address) // 读取线圈状态（单个位）  
        // {
        //     ushort[] registers = master.ReadCoils(address, 1); // 读取多个线圈状态，返回一个数组的位状态  
        //     if (registers[0] == 0) Console.WriteLine("Coil {0} is OFF", address); // 如果线圈状态为0，则输出"Coil {0} is OFF"  
        //     else Console.WriteLine("Coil {0} is ON", address); // 如果线圈状态为1，则输出"Coil {0} is ON"  
        // }

        // public void WriteCoil(int address, bool value) // 写入线圈状态（单个位）  
        // {
        //     ushort register = (ushort)(value ? 0x0001 : 0x0000); // 如果是true则写入1，否则写入0  
        //     master.WriteSingleRegister(address, register); // 写入单个寄存器，线圈地址为寄存器地址，线圈状态为寄存器值（1或0）  
        //     Console.WriteLine($"Coil {address} has been set to {value}"); // 输出"Coil {address} has been set to {value}"，表示线圈已经被设置到指定状态  
        // }

        public void ReadData()
        {
            string ipAddress = "192.168.1.1"; // Modbus 设备的 IP 地址
            int port = 502;  // Modbus TCP 端口号

            try
            {
                using (TcpClient tcpClient = new TcpClient(ipAddress, port))
                {
                    ModbusIpMaster modbusMaster = ModbusIpMaster.CreateIp(tcpClient);

                    // 向 Modbus 设备发送读取保持寄存器的请求
                    ushort startAddress = 0; // 读取起始地址
                    ushort numOfPoints = 10; // 读取的寄存器数量
                    ushort[] registers = modbusMaster.ReadHoldingRegisters(1, startAddress, numOfPoints);

                    // 处理读取到的寄存器值
                    for (int i = 0; i < registers.Length; i++)
                    {
                        Console.WriteLine("Register {0}: {1}", startAddress + i, registers[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Modbus communication error: " + ex.Message);
            }
        }
    }
}

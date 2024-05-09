using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Connector
{
    // 50E与PC机485/232串口通讯协议
    internal class ResistanceCurrentInfoCollectorSingleLen
    {
        private SerialPort serialPort;

        public byte StartByte { set; get; } = 0x7E;

        public byte StopByte { set; get; } = 0x0D;

        public byte[] DeviceAddress { set; get; } = { 0x51, 0x51 };

        public ResistanceCurrentInfoCollectorSingleLen(string portName, int baudRate = 9600)
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 2000,
                WriteTimeout = 2000,
            };
        }

        public bool Open()
        {
            if (serialPort.IsOpen)
            {
                return true;
            }
            try
            {
                serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"fail to open serial port {ex.Message}");
                return false;
            }
        }

        // 主机发送命令
        public void SendCommand(byte[] command)
        {
            byte[] packet = ConstructPacket(command);
            try
            {
                serialPort.Write(packet, 0, packet.Length);
            }
            catch (Exception ex)
            {
                Log.Error("Fail to write resistance command SinglePhaseCmdPacket: " + ex.Message);
            }
        }

        private byte[] ConstructPacket(byte[] command)
        {
            if (command.Length > 9)
            {
                Log.Error("Command Length Error");
            }
            int length = command.Length + 6; // 6 bytes for header, address * 2, length, checksum, and footer
            byte[] packet = new byte[length];
            string lenStr = command.Length.ToString();
            char[] lenArray = lenStr.ToCharArray();

            packet[0] = 0x7E; // 报文头
            packet[1] = 0x3E; // 从机地址高字节
            packet[2] = 0x3E; // 从机地址低字节
            packet[3] = (byte)lenArray[0];  // 数据和命令长度

            Array.Copy(command, 0, packet, 4, command.Length); // 命令数据

            byte xorChecksum = CalculateChecksum(packet, 0, length - 2); // 从地址到数据部分的校验
            packet[length - 2] = xorChecksum; // 异或校验
            packet[length - 1] = 0x0D; // 报文尾

            return packet;
        }

        // 计算异或校验
        private byte CalculateChecksum(byte[] data, int offset, int length)
        {
            byte checksum = 0x00;
            for (int i = offset; i < length; i++)
            {
                checksum ^= data[i];
            }
            return checksum;
        }

        // 主机接收数据
        public byte[]? ReadData()
        {
            byte[] buffer = new byte[1024]; // 假设数据长度不超过1024字节
            int bytesRead;
            try
            {
                bytesRead = serialPort.Read(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Log.Error("Fail to read resistance SinglePhaseCmdPacket: " + ex.Message);
                return null;
            }

            // 处理接收到的数据
            if (bytesRead > 0)
            {
                // 检查报文头和尾
                if (buffer[0] == 0x7E && buffer[bytesRead - 1] == 0x0D)
                {
                    // 获取数据部分
                    int dataLength = bytesRead - 6; // 6 bytes for header, address, length, checksum, and footer
                    byte[] data = new byte[dataLength];
                    Array.Copy(buffer, 4, data, 0, dataLength); // 从第6个字节开始是数据部分
                                                                // 返回数据部分，不包括报文头和尾
                    return data;
                }
                else
                {
                    Log.Error("ResistanceCurrentInfoCollector: Invalid SinglePhaseCmdPacket format.");
                }
            }
            return null;
        }

        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}

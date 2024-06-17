using ABBDataManagerSystem.PowerAnalyzer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Connector
{
    // JYR-20W与PC机485/232串口通讯协议
    internal class ResistanceCurrentInfoCollector
    {
        private SerialPort serialPort;

        public byte StartByte { set; get; } = 0x7E;

        public byte StopByte { set; get; } = 0x0D;

        public byte[] DeviceAddress { set; get; } = { 0x3E, 0x3E };

        public short SendLenByteCount { set; get; } = 2;
        
        public short RspLenByteCount {  set; get; } = 2;

        public ResistanceCurrentInfoCollector(string portName, int baudRate = 9600)
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 200,
                WriteTimeout = 200,
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
                Log.Info("SendMsg Is " + Utils.DumpBuffer(packet, 0, packet.Length));
                serialPort.Write(packet, 0, packet.Length);
            }
            catch (Exception ex)
            {
                Log.Error("Fail to write resistance command SinglePhaseCmdPacket: " + ex.Message);
            }
        }

        private byte[] ConstructPacket(byte[] command)
        {
            if ((SendLenByteCount == 1 && command.Length > 9) || command.Length > 99)
            {
                Log.Error($"Command Length Error, SendLenByteCount is {SendLenByteCount}, CommandLen is {command.Length}");
            }
            int length = command.Length + 5 + SendLenByteCount; // 7 bytes for header, address * 2, length * SendLenByteCount, checksum, and footer
            byte[] packet = new byte[length];
            string lenStr;
            if (SendLenByteCount == 1)
            {
                lenStr = command.Length.ToString();
            }
            else
            {
                lenStr = command.Length < 10 ? $"0{command.Length}" : command.Length.ToString();
            }
            byte[] lenArray = Encoding.UTF8.GetBytes(lenStr);

            packet[0] = 0x7E; // 报文头
            packet[1] = DeviceAddress[0]; // 从机地址高字节
            packet[2] = DeviceAddress[1]; // 从机地址低字节
            if (SendLenByteCount == 1)
            {
                packet[3] = (byte)lenArray[0];  // 数据和命令长度
            } 
            else
            {
                packet[3] = (byte)lenArray[0];  // 数据和命令长度高字节
                packet[4] = (byte)lenArray[1];  // 数据和命令长度低字节
            }

            int offset = SendLenByteCount == 1 ? 4 : 5;
            Array.Copy(command, 0, packet, offset, command.Length); // 命令数据

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
                Log.Info("Response is " + Utils.DumpBuffer(buffer,0, bytesRead));
                // 检查报文头和尾
                if (buffer[0] == 0x7E && buffer[bytesRead - 1] == 0x0D)
                {
                    int startOffset = 1 + 2 + RspLenByteCount;
                    int footerOffset = 2;

                    // 获取数据部分
                    int dataLength = bytesRead - startOffset - footerOffset; // for header, address, length, checksum, and footer
                    byte[] data = new byte[dataLength];
                    Array.Copy(buffer, startOffset, data, 0, dataLength); // 从第6个字节开始是数据部分
                                                                // 返回数据部分，不包括报文头和尾
                    return data;
                }
                else
                {
                    Log.Error("ReadData Fail, No Bytes Readed");
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

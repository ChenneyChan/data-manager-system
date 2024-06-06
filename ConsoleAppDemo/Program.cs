// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;

class Program
{
    // 将float转换为大端字节序的byte数组  
    static byte[] FloatToBigEndianBytes(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);

        // 如果当前系统是小端字节序，则反转字节顺序  
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }

    // 将大端字节序的byte数组转换回float
    static float BigEndianBytesToFloat(byte[] bigEndianBytes)
    {
        // 如果当前系统是小端字节序，则先反转字节顺序  
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bigEndianBytes);
        }

        return BitConverter.ToSingle(bigEndianBytes, 0);
    }

    static void ConvertTestMain()
    {
        // 假设我们有一个float值  
        float value = 123.456f;

        // 将float转换为大端字节序的byte数组  
        byte[] bigEndianBytes = FloatToBigEndianBytes(value);

        // 打印转换后的byte数组（仅用于验证）  
        Console.WriteLine("Big-Endian Bytes: ");
        foreach (byte b in bigEndianBytes)
        {
            Console.Write("{0:X2} ", b);
        }
        Console.WriteLine();

        // 将大端字节序的byte数组转换回float  
        float convertedValue = BigEndianBytesToFloat(bigEndianBytes);

        // 打印转换后的float值（应该与原始值相同）  
        Console.WriteLine("Converted Float Value: " + convertedValue);
    }

    static void Main()
    {

        ConvertTestMain();

        // 定义你的10个float字段  
        float[] floatsToSend = new float[] { 1.23f, 2.34f, 3.45f, 0.0f, 4.56f, 5.67f, 6.78f, 0.0f, 7.89f, 8.90f, 9.01f, 0.0f, 10.12f };
        string msg = "";
        // 将float数组转换为byte数组  
        byte[] data = new byte[floatsToSend.Length * 4];
        for (int  i = 0;  i < floatsToSend.Length;  i++)
        {
            msg += floatsToSend[i].ToString() + "\t";
            byte[] bs = FloatToBigEndianBytes(floatsToSend[0]);
            Buffer.BlockCopy(bs, 0, data, i * 4, 4);
        }

        // 创建UdpClient实例  
        using (UdpClient udpClient = new UdpClient())
        {
            // 设置广播模式（如果需要）  
            // 注意：在某些系统上，可能需要管理员权限来启用广播  
            udpClient.EnableBroadcast = true;

            // 构造目标EndPoint（广播地址和端口）  
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 8899);

            // 发送数据  
            while (true)
            {
                udpClient.Send(data, data.Length, endPoint);

                Console.WriteLine("\r\n\r\n数据已在UDP 8899 端口广播发送");
                Console.WriteLine($"数据排序：ua ub uc uabc ia ib ic iabc pa pb pc p3 frequence");
                Console.WriteLine($"数据值为：\r\n{msg}");
                Thread.Sleep(1000);
            }

            // （可选）接收响应（如果你需要的话）  
            // byte[] receivedBytes = udpClient.Receive(ref endPoint);  
            // ...处理接收到的数据...  

            // 关闭UdpClient（如果使用using语句，则不需要显式调用Close方法）  
        }
    }
}
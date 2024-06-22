// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json.Linq;
using System.IO.Ports;

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

    public static float ParseFloat(string? valule, float defaultValue = 0)
    {
        if (valule == null) { return defaultValue; }
        try
        {
            return float.Parse(valule);
        }
        catch
        {
            return defaultValue;
        }
    }

    public class JinYunJYTATestResult
    {
        public bool IsSinglePhase = true;
        public float[] Ratio;
        public float[] Error;
        public float[] Voltage;
        public float[] Current;
        public float[] TurnRatio;
        public float Frequence;
        public float CalculatedRatio;
        public byte Polarity;
        public string TappingPosition = string.Empty;
        public string ConnectionType = string.Empty;
    }

    static void PacketByteParse()
    {
        // header: 7e 54 55 31 32 36 
        string packetString = "6b 32 30 32 35 2e 30 30 32 32 35 2e 30 31 31 32 35 2e 30 31 31 34 33 2e 33 30 33 34 33 2e 33 31 39 34 33 2e 33 31 39 20 31 2e 30 32 20 25 20 31 2e 30 35 20 25 20 31 2e 30 35 20 25 31 35 38 2e 38 39 56 31 35 39 2e 32 31 56 31 35 39 2e 34 34 56 20 36 2e 34 30 6d 41 20 34 2e 37 30 6d 41 20 36 2e 34 32 6d 41 44 20 2d 79 20 2d 31 31 20 30 37 35 30 2e 30 30 48 7a 32 34 2e 37 35 30 55 0d ";
        packetString = "6b 99 35 2e 30 31 30 32 35 2e 30 31 30 34 33 2e 33 30 32 34 33 2e 33 31 37 34 33 2e 33 31 37 20 31 2e 30 31 20 25 20 31 2e 30 35 20 25 20 31 2e 30 35 20 25 31 36 32 2e 35 37 56 31 36 32 2e 37 34 56 31 36 32 2e 35 33 56 20 36 2e 35 33 6d 41 20 34 2e 38 30 6d 41 20 36 2e 35 32 6d 41 44 20 2d 79 20 2d 31 31 2d 30 31 35 30 2e 30 31 48 7a 32 34 2e 37 35 30 5e 0d";
        //var byteSKtrings = packet.Split(" ");
        var packet = Convert.FromHexString(packetString.Replace(" ", ""));
        //Console.WriteLine("Byte array:");
        //foreach (byte b in bytes)
        //{
        //    Console.WriteLine(b);
        //}

        if (packet.Length < 126 || packet[0] != 0x6B)
        {
            Console.WriteLine("Packet Invalid");
            return;
        }
        int offset = 3; // 跳过设备状态、提示信息、电量码 三个字节
        string strRatioA = Encoding.ASCII.GetString(packet, offset, 6);
        offset += 6;
        string strRatioB = Encoding.ASCII.GetString(packet, offset, 6);
        offset += 6;
        string strRatioC = Encoding.ASCII.GetString(packet, offset, 6);
        offset += 6;

        string strTurnRatioA = Encoding.ASCII.GetString(packet, offset, 6);
        offset += 6;
        string strTurnRatioB = Encoding.ASCII.GetString(packet, offset, 6);
        offset += 6;
        string strTurnRatioC = Encoding.ASCII.GetString(packet, offset, 6);
        offset += 6;

        string strErrorA = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;
        string strErrorB = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;
        string strErrorC = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;

        string strVoltageA = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;
        string strVoltageB = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;
        string strVoltageC = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;

        string strCurrentA = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;
        string strCurrentB = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;
        string strCurrentC = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;

        string strConnectionType = Encoding.ASCII.GetString(packet, offset, 8);
        offset += 8;

        string tappingPosition = Encoding.ASCII.GetString(packet, offset, 3);
        offset += 3;

        string strFrequence = Encoding.ASCII.GetString(packet, offset, 7);
        offset += 7;

        string strCalculatedRatio = Encoding.ASCII.GetString(packet, offset, 6);
        offset += 6;

        float ratioA = ParseFloat(strRatioA);
        float ratioB = ParseFloat(strRatioB);
        float ratioC = ParseFloat(strRatioC);
        float turnRatioA = ParseFloat(strTurnRatioA);
        float turnRatioB = ParseFloat(strTurnRatioB);
        float turnRatioC = ParseFloat(strTurnRatioC);
        float errorA = ParseFloat(strErrorA.Replace("%", ""));
        float errorB = ParseFloat(strErrorB.Replace("%", ""));
        float errorC = ParseFloat(strErrorC.Replace("%", ""));
        float voltageA = ParseFloat(strVoltageA.Replace("V", ""));
        float voltageB = ParseFloat(strVoltageB.Replace("V", ""));
        float voltageC = ParseFloat(strVoltageC.Replace("V", ""));
        float currentA = ParseFloat(strCurrentA.Replace("mA", ""));
        float currentB = ParseFloat(strCurrentB.Replace("mA", ""));
        float currentC = ParseFloat(strCurrentC.Replace("mA", ""));
        float frequence = ParseFloat(strFrequence.Replace("Hz", ""));
        float calculatedRatio = ParseFloat(strCalculatedRatio);

        var result = new JinYunJYTATestResult()
        {
            IsSinglePhase = false,
            Ratio = new float[] { ratioA, ratioB, ratioC },
            Error = new float[] { errorA, errorB, errorC },
            Voltage = new float[] { voltageA, voltageB, voltageC },
            Current = new float[] { currentA, currentB, currentC },
            TurnRatio = new float[] { turnRatioA, turnRatioB, turnRatioC },
            Frequence = frequence,
            CalculatedRatio = calculatedRatio,
            TappingPosition = tappingPosition,
            ConnectionType = strConnectionType,
        };
        return;
    }

    static void PacketParse20W()
    {
        // header: 7e 54 55 31 32 36 
        string packetString = "6b 32 30 32 35 2e 30 30 32 32 35 2e 30 31 31 32 35 2e 30 31 31 34 33 2e 33 30 33 34 33 2e 33 31 39 34 33 2e 33 31 39 20 31 2e 30 32 20 25 20 31 2e 30 35 20 25 20 31 2e 30 35 20 25 31 35 38 2e 38 39 56 31 35 39 2e 32 31 56 31 35 39 2e 34 34 56 20 36 2e 34 30 6d 41 20 34 2e 37 30 6d 41 20 36 2e 34 32 6d 41 44 20 2d 79 20 2d 31 31 20 30 37 35 30 2e 30 30 48 7a 32 34 2e 37 35 30 55 0d ";
        packetString = "54 54 30 33 31 30 30 30 35 2e 30 34 20 30 2e 32 34 37 38 6d 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 3c 0d ";
        //var byteSKtrings = packet.Split(" ");
        var packet = Convert.FromHexString(packetString.Replace(" ", ""));
        //Console.WriteLine("Byte array:");
        //foreach (byte b in bytes)
        //{
        //    Console.WriteLine(b);
        //}

        //if (packet.Length < 126 || packet[0] != 0x6B)
        //{
        //    Console.WriteLine("Packet Invalid");
        //    return;
        //}

        if (packet == null)
        {
            return;
        }
        if (packet.Length < 50)
        {
            //Log.Error("Read Packet Len err" + packet.Length);
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
        startIndex = startIndex + 7;

        string strCh1Current = Encoding.ASCII.GetString(ch1CurrentValue);
        string strCh2Current = Encoding.ASCII.GetString(ch2CurrentValue);
        string strCh1Resistance = Encoding.ASCII.GetString(ch1ResistanceValue);
        string strCh2Resistance = Encoding.ASCII.GetString(ch2ResistanceValue);
        string strCh1TimedResistant = Encoding.ASCII.GetString(ch1TimedResistanceValue);
        string strCh2TimedResistant = Encoding.ASCII.GetString(ch2TimedResistanceValue);
        string strTempSetTimeInterval = Encoding.ASCII.GetString(tempSetTimeInterval);

        //return new JinYuan20WPacketInfo()
        //{
        //    ch1Enabled = ch1 == 0x31,
        //    ch2Enabled = ch2 == 0x31,
        //    ch1Status = ch1Status,
        //    ch2Status = ch2Status,
        //    type = GetTestTypeByByte(testMode),
        //    ch1RealTimeCurrent = Utils.ParseFloat(strCh1Current),
        //    ch2RealTimeCurrent = Utils.ParseFloat(strCh2Current),
        //    ch1RealTimeResistance = Utils.ParseFloat(strCh1Resistance),
        //    ch2RealTimeResistance = Utils.ParseFloat(strCh2Resistance),
        //    ch1SelectedCurrent = ch1Current,
        //    ch2SelectedCurrent = ch2Current,
        //    ch1TimedResistance = Utils.ParseFloat(strCh1TimedResistant),
        //    ch2TimedResistance = Utils.ParseFloat(strCh2TimedResistant),
        //    tempRaiseTimeInterval = Utils.ParseFloat(strTempSetTimeInterval),
        //};
    }

    static void UdpListen()
    {
        int port = 8844;
        UdpClient udpClient = new UdpClient(port);

        try
        {
            Console.WriteLine("Waiting for a message on port " + port + "...");

            // 创建一个IPEndPoint来接收任何IP地址发送的数据  
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Loopback, 0);

            while (true)
            {
                // 开始异步接收数据  
                // 注意：为了简单起见，这里使用同步的Receive方法  
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

                // 将字节转换为字符串  
                string receivedData = Encoding.UTF8.GetString(receivedBytes);

                Console.WriteLine($"Received message({receivedBytes.Length}): " + receivedData + " from " + remoteEndPoint.ToString());

                byte[] buf = new byte[4];
                // 如果需要，可以持续监听，只需将上面的接收代码放在一个循环中  
                for (int i = 0; i < (receivedBytes.Length - 16 * 2) / 4; i++)
                {
                    Array.Copy(receivedBytes, i * 4, buf, 0, 4);
                    // 将大端字节序的byte数组转换回float  
                    float convertedValue = BigEndianBytesToFloat(buf);
                    Console.Write(convertedValue.ToString() + "\t");
                }
                buf = new byte[16];
                Array.Copy(receivedBytes, receivedBytes.Length - 16 * 2, buf, 0, 16);
                Console.WriteLine("WorkflowId = " + Encoding.ASCII.GetString(buf));
                Array.Copy(receivedBytes, receivedBytes.Length - 16, buf, 0, 16);
                Console.WriteLine("Tapping = " + Encoding.ASCII.GetString(buf));
                Console.WriteLine();
            }

            // 当你完成后，关闭UDP客户端  
            udpClient.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void Main()
    {
        PacketParse20W();
        //PacketByteParse();
        Console.WriteLine();
        ConvertTestMain();

        // 定义你的10个float字段  
        float[] floatsToSend = new float[] { 1.23f, 2.34f, 3.45f, 0.0f, 4.56f, 5.67f, 6.78f, 0.0f, 7.89f, 8.90f, 9.01f, 0.0f, 10.12f };
        string msg = "";
        // 将float数组转换为byte数组  
        byte[] data = new byte[floatsToSend.Length * 4];
        for (int i = 0; i < floatsToSend.Length; i++)
        {
            msg += floatsToSend[i].ToString() + "\t";
            byte[] bs = FloatToBigEndianBytes(floatsToSend[i]);
            Buffer.BlockCopy(bs, 0, data, i * 4, 4);
        }
        // 打印转换后的byte数组（仅用于验证）  
        Console.WriteLine("Buffer Bytes: ");
        foreach (byte b in data)
        {
            Console.Write("{0:X2} ", b);
        }
        Console.WriteLine();

        new Thread(() => { UdpListen(); }).Start();

        return;
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


class ProgramB
{
    static void MainB()
    {
        // 替换为你的 JSON 数据
        string jsonData = File.ReadAllText("C:\\Users\\cheneychan\\AppData\\Local\\Temp\\__temp__workflow__.json");

        // 替换为你的 Excel 模板文件路径
        string templatePath = "E:\\01_Code\\vite-electron\\extraResources\\templates\\source_data_template.xlsx";

        // 替换为你的输出 Excel 文件路径
        string outputPath = "demo_output.xlsx";

        WriteJsonToExcel(jsonData, templatePath, outputPath);
    }

    static void WriteJsonToExcel(string jsonData, string templatePath, string outputPath)
    {
        // 读取 JSON 数据
        var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);
        if (jsonObject == null)
        {
            return;
        }

        // 加载 Excel 模板文件
        using (var templateFile = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            var workbook = new XSSFWorkbook(templateFile);
            var sheet = workbook.GetSheetAt(0);

            IEnumerator rowEnumerator = sheet.GetRowEnumerator();
            while (rowEnumerator.MoveNext())
            {
                XSSFRow row = (XSSFRow)rowEnumerator.Current;

                // 获取单元格迭代器
                IEnumerator cellEnumerator = row.GetEnumerator();

                while (cellEnumerator.MoveNext())
                {
                    XSSFCell cell = (XSSFCell)cellEnumerator.Current;

                    // 获取单元格的内容
                    string cellValue = cell.ToString().Trim();
                    //Console.WriteLine("Cell Value: " + cellValue);

                    // 在模板中填写 JSON 数据
                    if (cellValue.StartsWith("${") && cellValue.EndsWith("}"))
                    {
                        Console.WriteLine("Cell Value: " + cellValue);

                        var placeholder = cellValue.Substring(2, cellValue.Length - 3);
                        Console.WriteLine("JsonDesc " + placeholder);

                        // 获取路径1的值
                        JToken token1 = jsonObject.SelectToken(placeholder);
                        if (token1 != null)
                        {
                            string value1 = token1.ToString();
                            Console.WriteLine($"{placeholder} 的值为: {value1}");
                            cell.SetCellValue(value1);
                            if (value1.GetType() == typeof(float) || value1.GetType() == typeof(int) || value1.GetType() == typeof(double))
                            {
                                cell.SetCellType(CellType.Numeric);
                                cell.SetCellValue(value1);
                            }
                            else
                            {
                                cell.SetCellType(CellType.String);
                            }
                            cell.SetCellValue(value1);
                        }
                        else
                        {
                            Console.WriteLine($"{placeholder} 未找到对应的值.");
                            cell.SetCellValue("");
                        }
                    }
                }
            }

            // 保存输出 Excel 文件
            using (var outputFile = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(outputFile);
            }
        }
    }
}

#region 打印串口读取到的数据
class ProgramC
{
    static void MainC()
    {
        string portName = "COM7"; // 串口名称，根据实际情况修改
        int baudRate = 9600; // 波特率，根据实际情况修改

        SerialPort serialPort = new SerialPort(portName, baudRate);
        serialPort.DataReceived += SerialPort_DataReceived;

        try
        {
            serialPort.Open();
            Console.WriteLine($"串口 {portName} 已打开，等待接收数据...");

            Console.ReadLine(); // 等待用户输入任意键退出

            serialPort.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"发生异常：{ex.Message}");
        }
    }

    private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort serialPort = (SerialPort)sender;
        int bytesToRead = serialPort.BytesToRead;
        byte[] buffer = new byte[bytesToRead];

        serialPort.Read(buffer, 0, bytesToRead);

        DateTime timestamp = DateTime.Now;
        string formattedTimestamp = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

        StringBuilder hexBuilder = new StringBuilder(buffer.Length * 3);
        foreach (byte b in buffer)
        {
            hexBuilder.Append(Convert.ToString(b, 16).PadLeft(2, '0').ToUpper() + " ");
        }
        string hexString = hexBuilder.ToString().Trim();

        Console.WriteLine($"[{formattedTimestamp}] 接收到数据：{hexString}");
    }
}
#endregion
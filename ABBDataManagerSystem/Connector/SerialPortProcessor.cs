using ABBDataManagerSystem;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

class SerialPortProcessor
{
    private SerialPort? _serialPort;
    private const int ResponseSize = 44; // 响应的大小，根据实际情况调整  
    private const int RequestSize = 4; // 请求的大小，根据实际情况调整  
    static object lockObj = new object();
    static byte[] responseData = new byte[ResponseSize];

    public SerialPortProcessor(string Port, int BoudRate)
    {
        _serialPort = new SerialPort(Port, BoudRate, Parity.None, 8, StopBits.One); // 根据实际情况修改串口配置  
    }

    public bool Start()
    {
        if (_serialPort == null)
        {
            return false;
        }
        try
        {
            _serialPort.Open(); // 打开串口
            _serialPort.ReadTimeout = 2000;
            _serialPort.WriteTimeout = 2000;
            return _serialPort.IsOpen;
        }
        catch (Exception ex)
        {
            Log.Error("fai to open serial port " + ex.Message);
            return false;
        }
    }

    public byte[]? SendRequestAndReceiveResponseAsync(byte[] requestBytes)
    {
        byte[] request = requestBytes; // 假设Request是4字节固定内容，根据实际情况修改  

        // 发送请求  
        try
        {
            _serialPort.Write(request, 0, request.Length);
        }
        catch (Exception ex)
        {
            Log.Error("fail to write request " + ex.Message);
            return null;
        }

        int bytesRead = 0;
        try
        {
            while (bytesRead < ResponseSize)
            {
                // 异步读取响应。假设我们知道响应的大小是44字节
                bytesRead += _serialPort.Read(responseData, bytesRead, ResponseSize - bytesRead);
            }
        }
        catch (Exception e)
        {
            Log.Error("Canceled !! " + e.Message);
            return null;
        }

        // 处理响应数据。这里的处理方式取决于你的实际需求
        // ...  s
        return responseData;
    }

    public void Close()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            try
            {
                _serialPort.Close(); // 关闭串口连接  
            }
            catch (Exception e)
            {
                Log.Error("fail to close serial port " + e.Message);
            }
            _serialPort = null;
        }
    }
}
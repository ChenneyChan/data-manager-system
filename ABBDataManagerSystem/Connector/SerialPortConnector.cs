using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ABBDataManagerSystem.Connector
{
    delegate void OnUpdate(byte[] data, int len);
    internal class SerialPortConnector
    {
        int BoudRate { set; get; }
        string Port { set; get; }

        private Thread? thread = null;

        private SerialPort? serialPort;

        private OnUpdate? callback = null;

        private bool running = false;

        public SerialPortConnector(string port, int boudRate, OnUpdate callback)
        {
            BoudRate = boudRate;
            Port = port;
            this.callback = callback;
        }

        public bool StartConnect()
        {
            Log.Info("Start Connect ThreadID = " + Thread.CurrentThread.ManagedThreadId);
            serialPort = new SerialPort(Port, BoudRate);
            try
            {
                serialPort.Open();
                return serialPort.IsOpen;
            }
            catch (Exception ex)
            {
                Log.Error("Fail to open serial " + ex.Message);
                serialPort.Close();
                serialPort = null;
                return false;
            }
        }

        private void ThreadRun()
        {
            running = true;

            while (running && serialPort != null)
            {
                if (serialPort?.BytesToRead > 0) // 如果串口中有可读数据  
                {
                    byte[] buffer = new byte[serialPort.BytesToRead];
                    int readLen = serialPort.Read(buffer, 0, serialPort.BytesToRead); // 读取所有可读数据  
                    Console.WriteLine("Received data: " + readLen); // 打印接收到的数据  
                    if (callback != null)
                    {
                        callback(buffer, readLen);
                    }
                }
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort = null;
                }
                Thread.Sleep(100);
            }
        }

        public void WriteData(byte[] data)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Write(data, 0, data.Length);
            }
        }

        // public byte[] ReadData(int size, byte[] startWith)
        // {

        //     byte[] buffer = new byte[size];

        //     serialPort.
        //     while (serialPort?.BytesToRead > 0) // 如果串口中有可读数据  
        //     {
        //         byte[] buffer = new byte[serialPort.BytesToRead];
        //         int readLen = serialPort.Read(buffer, 0, serialPort.BytesToRead); // 读取所有可读数据  
        //         Console.WriteLine("Received data: " + readLen); // 打印接收到的数据  
        //         if (callback != null)
        //         {
        //             callback(buffer, readLen);
        //         }
        //     }
        // }

        public void StopTask()
        {
            Log.Info("Stop Connect ThreadID = " + Thread.CurrentThread.ManagedThreadId);
            running = false;
            CloseSerialPort();
            Log.Info("--Done Stop Connect");
        }

        public void Dispose()
        {
            StopTask();
        }

        private void CloseSerialPort() // 在此方法中关闭并释放SerialPort资源  
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen) serialPort.Close(); // 关闭串口连接  
            }
            catch (Exception ex) // 捕获并记录异常（如果有必要）  
            {
                Log.Error("Fail to close serial port: " + ex.Message);
            }
            finally // 确保释放资源，无论是否发生异常  
            {
                serialPort?.Dispose(); // 释放SerialPort资源（如果需要）  
                thread?.Join(); // 停止读取数据的线程（如果需要）  
                serialPort = null;
                thread = null;
            }
        } // CloseSerialPort方法结束处添加调用这个方法来关闭串口资源的代码（例如在Form的Closing事件中调用CloseSerialPort()方法） } // SerialPortExample类结束处添加调用这个方法来关闭串口资源的代码（例如在Form的Closing事件中调用CloseSerialPort()方法） } // SerialPortExample类结束处添加调用这个方法来关闭串口资源的代码（例如在Form


        public bool IsRunning() { return running; }

        public static bool TestConnect(string port, string boudRate)
        {
            try
            {
                var serial = new SerialPort(port, int.Parse(boudRate));
                serial.Open();
                if (serial.IsOpen)
                {
                    serial.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}

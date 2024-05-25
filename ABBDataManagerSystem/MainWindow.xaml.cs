using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages;
using ABBDataManagerSystem.PowerAnalyzer;
using HandyControl.Controls;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using TabItem = HandyControl.Controls.TabItem;
using Window = System.Windows.Window;

namespace ABBDataManagerSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Configs.Configs.LoadFromFile();
        }

        private void MenuItemSetting_Click(object sender, RoutedEventArgs e)
        {
            new WindowSettings().ShowDialog();
        }

        private void PowerAnalyzeTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new PageDeviceSearch()
            {
                Title = "功率分析仪",
                Width = 500,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            window.ShowDialog();
        }

        private void TemperatureTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window()
            {
                Title = "温度检测仪",
                Width = 1200,
                Height = 900,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            var uc = new TempTestPage();
            window.Content = uc;
            window.ShowDialog();
        }

        private void JnYuan20WTest_Click(object sender, RoutedEventArgs e)
        {
            var item = new TabItem()
            {
                Content = new JinYuan20W(),
                Header = "金源20W测试仪",
                IsSelected = true,
            };
            tabControl.Items.Add(item);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var item in tabControl.Items)
            {
                var tabItem = item as TabItem;
                if  (tabItem == null)
                {
                    continue;
                }
                var closeable = tabItem.Content as ICloseable;
                if (closeable != null)
                {
                    closeable.Close();
                }
            }
            Configs.Configs.SaveToFile();
        }

        private void btStartSerialTest_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var ports = SerialPort.GetPortNames();
                Array.Sort(ports);

                foreach (var port in ports)
                {
                    AppendMsg("Port " + port + " Start...");
                    var collector = new JinYun50ECollector(port, 9600);
                    if (collector.Connect())
                    {
                        AppendMsg("Connect Success!");
                        collector.SetTestType(JinYun50ECollector.TestType50E.CommonTest);
                        var packet = collector.QueryMsg();
                        if (packet == null)
                        {
                            AppendMsg("Fail to Get Response, Done!");
                        }
                        else
                        {
                            AppendMsg($"Get Response, Size is {packet.Length}, Done!");
                        }
                        collector.Disconnect();
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        AppendMsg($"Connect fail, skip {port}!");
                    }
                }
                AppendMsg("All Port Test Done!");

            }).Start();
        }

        private void AppendMsg(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                tbMsg.Text = tbMsg.Text + "\r\n" + msg;
            }));
        }

        bool IsTemp = false;

        private const ushort Polynomial = 0x1021;
        private const ushort InitialValue = 0xFFFF;
        public ushort ComputeChecksum(byte[] bytes, int offset, int len)
        {
            ushort crc = InitialValue;
            for (int i = offset; i < bytes.Length && i < len; ++i)
            {
                crc ^= (ushort)(bytes[i] << 8);
                for (int j = 0; j < 8; ++j)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ Polynomial);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }
            return crc;
        }


        #region 计算CRC校验码
        /// <summary>
        /// 计算CRC校验码，并转换为十六进制字符串
        /// Cyclic Redundancy Check 循环冗余校验码
        /// 是数据通信领域中最常用的一种差错校验码
        /// 特征是信息字段和校验字段的长度可以任意选定
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static new byte[] get_CRC16_C(byte[] data, int offset, int len)
        {
            byte num = 0xff;
            byte num2 = 0xff;

            byte num3 = 1;
            byte num4 = 160;
            byte[] buffer = data;

            for (int i = offset; i < buffer.Length && i < len; i++)
            {
                //位异或运算
                num = (byte)(num ^ buffer[i]);

                for (int j = 0; j <= 7; j++)
                {
                    byte num5 = num2;
                    byte num6 = num;

                    //位右移运算
                    num2 = (byte)(num2 >> 1);
                    num = (byte)(num >> 1);

                    //位与运算
                    if ((num5 & 1) == 1)
                    {
                        //位或运算
                        num = (byte)(num | 0x80);
                    }
                    if ((num6 & 1) == 1)
                    {
                        num2 = (byte)(num2 ^ num4);
                        num = (byte)(num ^ num3);
                    }
                }
            }
            return new byte[] { num, num2 };
        }
        #endregion


        private void btStartTempTest_Click(object sender, RoutedEventArgs e)
        {
            if (IsTemp)
            {
                IsTemp = false;
                AppendMsg("Stop!!!");
                return;
            }
            new Thread(() =>
            {
                var serialPort = new SerialPort("COM3", 9600)
                {
                    WriteTimeout = 2000,
                    ReadTimeout = 2000,
                };
                try
                {
                    AppendMsg("Start Test:");
                    ushort startAddress = 30101;
                    byte[] start = { (byte)(startAddress >> 8 & 0xFF), (byte)(startAddress & 0x00FF) };
                    ushort count = 15;
                    byte[] c = { (byte)(count >> 8 & 0xFF), (byte)(count & 0x00FF) };
                    var packet = new byte[] { 0x01, 0x04, start[0], start[1], c[0], c[1], 0x31, 0xCA };
                    var crc = get_CRC16_C(packet, 0, packet.Length - 2);
                    packet[packet.Length - 2] = crc[0];
                    packet[packet.Length - 1] = crc[1];
                    AppendMsg(crc[0].ToString("X2") + " " + crc[1].ToString("X2"));
                    string p = "";
                    for (int i = 0; i < packet.Length; i++)
                    {
                        p += packet[i].ToString("x2") + " ";
                    }
                    AppendMsg($"Requeset Packet is {p}");

                    serialPort.Open();
                    serialPort.Write(packet, 0, packet.Length);
                    AppendMsg("Send Done");
                    byte[] buffer = new byte[1024];
                    int len = serialPort.Read(buffer, 0, buffer.Length);
                    AppendMsg($"Read Packet SinglePhaseCmdLen {len}");
                    p = "";
                    for (int i = 0; i < len; i++)
                    {
                        p += buffer[i].ToString("x2") + " ";
                    }
                    AppendMsg("Packet is: " + p);
                    serialPort.Close();
                    AppendMsg("Stop Test!");
                }
                catch (Exception ex)
                {
                    AppendMsg("Fail to process, Error: " + ex.Message);
                }
            }).Start();
        }

        private void btHarmonicTest_Click(object sender, RoutedEventArgs e)
        {
            new HarmonicInfo()
            {
                Width = 1200,
                Height = 800,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            }.ShowDialog();
        }

        private void PowerAnalyze2Test_Click(object sender, RoutedEventArgs e)
        {
            var item = new TabItem()
            {
                Content = new UCPowerAanlyzer(),
                Header = "功率分析仪",
                IsSelected = true,
            };
            tabControl.Items.Add(item);
        }

        private void JinYuanJYT_A_Click(object sender, RoutedEventArgs e)
        {
            var item = new TabItem()
            {
                Content = new JinYuanJYT_A(),
                Header = "金源JYT-A",
                IsSelected = true
            };
            tabControl.Items.Add(item);
        }

        private void btThreadTest_Click(object sender, RoutedEventArgs e)
        {
            tbMsg.Text += "\r\nStart Thread Test!";
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Dispatcher.Invoke(new Action(() => { tbMsg.Text += "\r\nThreadPool Start!" + DateTime.Now.ToString("hh-mm-ss"); }));
                Thread.Sleep(2000);
                Dispatcher.Invoke(new Action(() => { tbMsg.Text += "\r\nThreadPool Done!" + DateTime.Now.ToString("hh-mm-ss"); }));
            });
            Task.Run(() =>
            {
                Thread.Sleep(2000);
                Dispatcher.Invoke(new Action(() => { tbMsg.Text += "\r\nTask Start!" + DateTime.Now.ToString("hh-mm-ss"); }));
                Thread.Sleep(1000);
                Dispatcher.Invoke(new Action(() => { tbMsg.Text += "\r\nTask Done!" + DateTime.Now.ToString("hh-mm-ss"); }));
            });
            tbMsg.Text += "\r\nStart Thread UITread Done!";
        }
    }
}
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages;
using ABBDataManagerSystem.PowerAnalyzer;
using ABBDataManagerSystem.Tools;
using DevZest.Windows.Docking;
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
        private bool UsingDocing = true;

        public MainWindow()
        {
            InitializeComponent();
            Configs.Configs.LoadFromFile();
        }

        private void MenuItemSetting_Click(object sender, RoutedEventArgs e)
        {
            new WindowSettings().ShowDialog();
        }

        #region 试验页面加载
        private void PowerAnalyzeTest_Click(object sender, RoutedEventArgs e)
        {
            string title = "功率分析仪";
            if (GetTabItemAndActive(title) != null || GetDockItemAndActive(title) != null)
            {
                return;
            }
            var window = new PageDeviceSearch(() =>
            {
                StartTabItem<UCPowerAanlyzer>("功率分析仪");
            })
            {
                Title = "功率分析仪",
                Width = 500,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            window.ShowDialog();
        }

        private void PowerAnalyze2Test_Click(object sender, RoutedEventArgs e)
        {
            StartTabItem<UCPowerAanlyzer>("功率分析仪2");
        }

        private void JinYuanJYT_A_Click(object sender, RoutedEventArgs e)
        {
            string title = "金源JYT-A变比测试仪";
            StartTabItem<JinYuanJYT_A>(title);
        }

        private void TemperatureTest_Click(object sender, RoutedEventArgs e)
        {
            string title = "盘古温度测试仪";
            StartTabItem<TempTestPage>(title);
        }

        private bool IsFirst = true;
        private void JnYuan20WTest_Click(object sender, RoutedEventArgs e)
        {
            string title = IsFirst ? "金源20W测试仪" : "金源50e测试仪";
            StartTabItem<JinYuan20W>(title);
            IsFirst = false;
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
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var item in tabControl.Items)
            {
                var tabItem = item as TabItem;
                if (tabItem == null)
                {
                    continue;
                }
                var closeable = tabItem.Content as ICloseable;
                if (closeable != null)
                {
                    closeable.Close();
                }
            }

            foreach (var item in dockControl.DockItems)
            {
                var scrollViewer = item.Content as ScrollViewer;
                if (scrollViewer != null)
                {
                    var closeable = scrollViewer.Content as ICloseable;
                    if (closeable != null)
                    {
                        closeable.Close();
                    }
                }
            }
            Configs.Configs.SaveToFile();
        }

        #region 独立测试
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

        bool IsTemp = false;

        private void btStartTempTest_Click(object sender, RoutedEventArgs e)
        {
            AppendMsg(UCPowerAanlyzer.RearrangeWireString("P3C3"));
            AppendMsg(UCPowerAanlyzer.RearrangeWireString("P3W4"));
            //byte[] request = {
            //    1, // 从站地址
            //    0x04, // 功能码（读取多个寄存器）
            //    0x00, 0x00, // 起始寄存器地址（通道1的地址30001转换为16进制）
            //    0x00, 0x28, // 寄存器个数（读取4个寄存器，即通道1至4）
            //    0x00, 0x00 // CRC校验，需根据Modbus协议计算得出
            //};
            //int startIndex = 000;
            //int slotCount = 10;
            byte[] request = {
                1, // 从站地址
                0x04, // 功能码（读取多个寄存器）
                0x00, 0x64, // 起始寄存器地址（通道1的地址30001转换为16进制）
                0x00, 0x0A, // 寄存器个数（读取4个寄存器，即通道1至4）
                0x31, 0xD2  // CRC校验，需根据Modbus协议计算得出
            };
            var crcs = ModbusCRC.Calculate_CRC16_C(request, 0, request.Length - 2);
            AppendMsg($"CRC1 IS = {crcs[0].ToString("X2")} {crcs[1].ToString("X2")}");

            byte[] response = {
                1, // 从站地址
                0x04, // 功能码（读取多个寄存器）
                0x02, // 字节数 
                0x00, 0x64, // 寄存器数据
                0x00, 0x00 // CRC校验，需根据Modbus协议计算得出
            };
            crcs = ModbusCRC.Calculate_CRC16_C(response, 0, response.Length - 2);
            AppendMsg($"CRC2 IS = {crcs[0].ToString("X2")} {crcs[1].ToString("X2")}");

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
                    var crc = ModbusCRC.Calculate_CRC16_C(packet, 0, packet.Length - 2);
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
        #endregion

        private TabItem? GetTabItemAndActive(string title)
        {
            foreach (var item in tabControl.Items)
            {
                var tabItem = item as TabItem;
                if (tabItem == null)
                {
                    continue;
                }
                if ((string)tabItem.Header == title)
                {
                    tabItem.IsSelected = true;
                    return tabItem;
                }
            }
            return null;
        }

        private DockItem? GetDockItemAndActive(string title)
        {
            foreach (var item in dockControl.DockItems)
            {
                if (item.Title == title)
                {
                    item.Activate();
                    return item;
                }
            }
            return null;
        }

        private void StartTabItem<T>(string title) where T : UserControl, new()
        {
            if (!UsingDocing)
            {
                if (GetTabItemAndActive(title) != null)
                {
                    return;
                }
                var tabItem = new TabItem()
                {
                    Content = new T(),
                    Header = title,
                    IsSelected = true,
                };
                tabControl.Items.Add(tabItem);
                return;
            }

            if (GetDockItemAndActive(title) != null)
            {
                return;
            }

            var scrollViewer = new HandyControl.Controls.ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                Content = new T()
            };
            var item = new DockItem()
            {
                Content = scrollViewer,
                Title = title,
                TabText = title
            };
            IsFirst = false;

            item.Show(dockControl, DockPosition.Document);
        }

        private void AppendMsg(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                tbMsg.Text = tbMsg.Text + "\r\n" + msg;
            }));
        }
    }
}
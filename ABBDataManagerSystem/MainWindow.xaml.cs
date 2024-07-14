using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages;
using ABBDataManagerSystem.PowerAnalyzer;
using ABBDataManagerSystem.Tools;
using DevZest.Windows.Docking;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using EventManager = ABBDataManagerSystem.Tools.EventManager;
using TabItem = HandyControl.Controls.TabItem;
using Window = System.Windows.Window;
using System.Text;

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
            EventManager.Instance.Subscribe("WorkflowSelected", EventHandler);
            tbCurrentWorkflow.Text = "当前工作令：" + Configs.Configs.WorkflowID;
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            if (Configs.Configs.IsEnableVerboseDebug)
            {
                btPowerAnalyzerDebug.Visibility = Visibility.Visible;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dockBottom.Close();
            Task.Run(() => { UpdateWorkflow(); });
            StartBroadCastWorkflowInfo();
            StartTabItem<WorkflowDetail>("工作令信息");
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            IsBroadCasting = false;
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

        private void JnYuan20WTest_Click(object sender, RoutedEventArgs e)
        {
            string title = "金源20W测试仪";
            StartTabItem<JinYuan20W>(title);
        }

        private void JnYuan20ETest_Click(object sender, RoutedEventArgs e)
        {
            string title = "金源20E测试仪";
            StartTabItem<JinYuan20E>(title);
        }

        private void JnYuan50ETest_Click(object sender, RoutedEventArgs e)
        {
            string title = "金源50E测试仪";
            StartTabItem<JinYuan50E>(title);
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
                var scrollViewer = item.Content as HandyControl.Controls.ScrollViewer;
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
            EventManager.Instance.Unsubscribe("WorkflowSelected", EventHandler);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            EventManager.Instance.Stop();
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
                    var collector = new JinYuan50ECollector(port, 9600)
                    {
                        TestType = JinYuan50ECollector.TestType50E.TemperatureRaise,
                        CurrentSelected = "0.1A",
                        TestModeValue = JinYuan50ECollector.TestMode.HighVoltagePhaseSelection,
                        ConnectionMode = "Dy",
                        TempRiseInterval = 55
                    };
                    if (collector.Connect())
                    {
                        AppendMsg("Connect Success!");
                        collector.SetParameters();
                        Thread.Sleep(100);
                        collector.SetTestType(JinYuan50ECollector.TestType50E.CommonTest);
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
            var item = new DockItemImpl()
            {
                Content = scrollViewer,
                Title = title,
                TabText = title,
                HideOnPerformClose = false,
            };

            item.Show(dockControl, DockPosition.Document);
        }

        private void AppendMsg(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                tbMsg.Text = tbMsg.Text + "\r\n" + msg;
            }));
        }

        #region DockItem缩放处理
        private void dockControl_ActiveItemChanged(object sender, EventArgs e)
        {
            var dockItem = dockControl.ActiveItem as DockItemImpl;
            if (dockItem != null)
            {
                PreviewSliderHorizontal.Value = dockItem.Scale * 100;
            }
        }

        private void PreviewSliderHorizontal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var dockItem = dockControl.ActiveItem as DockItemImpl;
            if (dockItem != null)
            {
                dockItem.Scale = (float)(PreviewSliderHorizontal.Value / 100);
            }
        }

        private void btResumeScale_Click(object sender, RoutedEventArgs e)
        {
            PreviewSliderHorizontal.Value = 100;
        }
        #endregion

        private void btInsertLoad_Click(object sender, RoutedEventArgs e)
        {
            var data = new VoltageCurrentLossDataInfo()
            {
                WorkflowId = "SHUILENG_002",
                LoadType = "空载",
                TappingPosition = "",
                ua = 100,
                ub = 200,
                uc = 300,
                ia = 101,
                ib = 102,
                ic = 103,
                pa = 110,
                pb = 120,
                pc = 130,
            };
            if (data.WriteToDB())
            {
                AppendMsg("Write Success");
            }
            else
            {
                AppendMsg("Write Fail");
            }
            var datas = VoltageCurrentLossDataInfo.ReadFromDB();
            if (datas != null)
            {
                foreach (var item in datas)
                {
                    AppendMsg("--" + item.WorkflowId + " " + item.LoadType + " " + item.TappingPosition);
                }
            }
        }

        private void btShowWorkflowView_Click(object sender, RoutedEventArgs e)
        {
            new WorkflowInfoView().ShowDialog();
        }

        private void EventHandler(object sender, TestEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                tbCurrentWorkflow.Text = "当前工作令：" + Configs.Configs.WorkflowID;
           });
        }

        public static void UpdateWorkflow()
        {
            var infos = WorkflowInfo.ReadFromDB(Configs.Configs.WorkflowID);
            if (infos != null && infos.Count > 0)
            {
                Configs.Configs.WorkflowInfo = infos[0];
            }
            else
            {
                Configs.Configs.WorkflowInfo = null;
            }
        }

        #region 数据广播
        private bool IsBroadCasting = false;
        private void StartBroadCastWorkflowInfo()
        {
            IsBroadCasting = true;
            new Thread(() =>
            {
                Log.Info("Start BroadCast Workflow");
                // 创建UdpClient实例  
                using (UdpClient udpClient = new UdpClient())
                {
                    // 设置广播模式（如果需要）  
                    // 注意：在某些系统上，可能需要管理员权限来启用广播  
                    udpClient.EnableBroadcast = true;

                    // 构造目标EndPoint（广播地址和端口）  
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8844);

                    // 发送数据  
                    while (IsBroadCasting)
                    {
                        var workflow = Configs.Configs.WorkflowInfo;
                        if (workflow != null)
                        {
                            int workflowLen = 16;
                            int tappingLen = 16;
                            // xxxx
                            float?[] floatsToSend = new float?[] {
                                (float)workflow.RatedPower,
                                workflow.RatedVoltageHv,
                                workflow.RatedVoltageLv, workflow.RatedVoltageYv,
                                workflow.RatedCurrentHv, workflow.Frequency, Utils.GetFloat(workflow.ImpedanceS)
                            };

                            string msg = "";
                            // 将float数组转换为byte数组  
                            byte[] data = new byte[floatsToSend.Length * 4 + workflowLen + tappingLen];
                            for (int i = 0; i < floatsToSend.Length; i++)
                            {
                                msg += floatsToSend[i].ToString() + "\t";
                                byte[] bs = Utils.FloatToBigEndianBytes(floatsToSend[i] ?? 0);
                                Buffer.BlockCopy(bs, 0, data, i * 4, 4);
                            }
                            var Id = workflow.ID;
                            if (Id.Length > workflowLen)
                            {
                                Id = Id.Substring(0, workflowLen);
                            }
                            else
                            {
                                Id = Id.PadLeft(workflowLen, ' ');
                            }

                            var tapping = workflow.CONNSymbol;
                            if (tapping.Length > tappingLen)
                            {
                                tapping = tapping.Substring(0, tappingLen);
                            }
                            else
                            {
                                tapping = tapping.PadLeft(tappingLen, ' ');
                            }
                            byte[] workflowIdStr = Encoding.ASCII.GetBytes(Id);
                            byte[] tappingStr = Encoding.ASCII.GetBytes(tapping);
                            Buffer.BlockCopy(workflowIdStr, 0, data, floatsToSend.Length * 4, Math.Min(workflowIdStr.Length, workflowLen));
                            Buffer.BlockCopy(tappingStr, 0, data, floatsToSend.Length * 4 + workflowLen, Math.Min(tappingStr.Length, tappingLen));

                            udpClient.Send(data, data.Length, endPoint);
                        }
                        Thread.Sleep(2000);
                    }
                }
            }).Start();
        }
        #endregion

        private void Button50ETestDemo_Click(object sender, RoutedEventArgs e)
        {
            new Window50ETestDemo().Show();
        }
    }
}
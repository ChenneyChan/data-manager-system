using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static ABBDataManagerSystem.Connector.JinYuan20WCollector;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// JinYuan20W.xaml 的交互逻辑
    /// </summary>
    public partial class JinYuan20W : UserControl, ICloseable
    {
        private JinYuan20WCollector? Collector = null;
        private List<CurrentResistanceInfo> dataItems = new List<CurrentResistanceInfo>();
        private ObservableCollection<MyItem> items2;
        private bool IsConneted = false;
        private bool IsCollecting = false;
        private ManualResetEvent? ResetEvent = null;
        private bool CommandChange = false;

        private Dictionary<string, TappingResistanceFields> tappingResistanceFields = new Dictionary<string, TappingResistanceFields>();
        private Dictionary<string, ToggleButton> highVoltageToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<string, ToggleButton> lowVoltageToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<string, ToggleButton> tappingToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<TestType20W, ToggleButton> testTypeToggleButtons = new Dictionary<TestType20W, ToggleButton>();

        private string SelectedHighVoltageTapping = "";
        private string SelectedLowVoltageTapping = "";
        private string SelectedTapping = "";
        private TestType20W? SelectedTesting = TestType20W.CommonTest;

        private DispatcherTimer TimerSecond;
        private int SecondElapsed = 0;
        private bool IsCommonTesting = false;
        private int SelectedLowVoltageLevel = 0;

        private JinYuan20WCollector.JinYuan20WPacketInfo? lastPacket = null;


        public JinYuan20W()
        {
            InitializeComponent();
            InitTimer();
            InitToggleButtons();
            InitTappings();
            btSetInterval.Visibility = Visibility.Collapsed;
            panelTappingChoice.Visibility = Visibility.Collapsed;

            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 1, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 2, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 3, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 4, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 5, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 6, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 9, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 10, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 11, Resistance = 222.2f });
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 12, Resistance = 222.2f });

            // 数据模型
            items2 = new ObservableCollection<MyItem>
            {
                new MyItem { Name = "项目1", Description = "这是项目1的描述" },
                new MyItem { Name = "项目2", Description = "这是项目2的描述" },
                new MyItem { Name = "项目3", Description = "这是项目3的描述" }
            };

            // 数据绑定
            this.DataContext = new { Items = dataItems };
        }

        private void InitToggleButtons()
        {
            highVoltageToggleButtons.Add("AB", tbAB);
            highVoltageToggleButtons.Add("AO", tbAO);
            highVoltageToggleButtons.Add("BC", tbBC);
            highVoltageToggleButtons.Add("BO", tbBO);
            highVoltageToggleButtons.Add("CA", tbCA);
            highVoltageToggleButtons.Add("CO", tbCO);

            lowVoltageToggleButtons.Add("ab", tbab);
            lowVoltageToggleButtons.Add("ao", tbao);
            lowVoltageToggleButtons.Add("bc", tbbc);
            lowVoltageToggleButtons.Add("bo", tbbo);
            lowVoltageToggleButtons.Add("ca", tbca);
            lowVoltageToggleButtons.Add("co", tbco);

            tappingToggleButtons.Add("1", tbTapping1);
            tappingToggleButtons.Add("2", tbTapping2);
            tappingToggleButtons.Add("3", tbTapping3);
            tappingToggleButtons.Add("4", tbTapping4);
            tappingToggleButtons.Add("5", tbTapping5);
            tappingToggleButtons.Add("6", tbTapping6);
            tappingToggleButtons.Add("7", tbTapping7);
            tappingToggleButtons.Add("8", tbTapping8);
            tappingToggleButtons.Add("9", tbTapping9);
            tappingToggleButtons.Add("21", tbTapping21);

            testTypeToggleButtons.Add(TestType20W.CommonTest, tbTestTypeCommon);
            testTypeToggleButtons.Add(TestType20W.TemperatureRise10Sec, tbTestTypeTempRise10s);
            testTypeToggleButtons.Add(TestType20W.TemperatureRise30Sec, tbTestTypeTempRise30s);
            testTypeToggleButtons.Add(TestType20W.TemperatureRise60Sec, tbTestTypeTempRise60s);
        }

        private void InitTappings()
        {
            tappingResistanceFields.Add("1", trTap1);
            tappingResistanceFields.Add("2", trTap2);
            tappingResistanceFields.Add("3", trTap3);
            tappingResistanceFields.Add("4", trTap4);
            tappingResistanceFields.Add("5", trTap5);
            tappingResistanceFields.Add("6", trTap6);
            tappingResistanceFields.Add("7", trTap7);
            tappingResistanceFields.Add("8", trTap8);
            tappingResistanceFields.Add("9", trTap9);
            tappingResistanceFields.Add("10", trTap10);

            tappingResistanceFields.Add("11", trTap11);
            tappingResistanceFields.Add("12", trTap12);

            tappingResistanceFields.Add("21", trTap21);
            tappingResistanceFields.Add("22", trTap22);
        }

        #region 计数器的Timer
        private void InitTimer()
        {
            TimerSecond = new DispatcherTimer();
            TimerSecond.Interval = TimeSpan.FromSeconds(1);
            TimerSecond.Tick += TimerSecond_Tick;
        }

        private void TimerSecond_Tick(object? sender, EventArgs e)
        {
            SecondElapsed += 1;
            if (SecondElapsed > 60 * 60) 
            {
                SecondElapsed = 0;
            }
            UpdateClockDisplay();
        }

        private void UpdateClockDisplay()
        {
            int minutes = SecondElapsed / 60;
            int seconds = SecondElapsed % 60;

            string mm = minutes < 10 ? $"0{minutes}" : minutes.ToString();
            string ss = seconds < 10 ? $"0{seconds}" : seconds.ToString();
            TestingTimer.Text = $"{mm}:{ss}";
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (var item in ports)
            {
                cbSerialPort.Items.Add(item);
            }
            cbSerialPort.SelectedIndex = ports.Length > 0 ? 0 : -1;

            foreach (var item in JinYuan20WCollector.CH1CurrentsMap.Keys)
            {
                cbHVCurrents.Items.Add(item);
            }
            cbHVCurrents.SelectedIndex = 0;

            foreach (var item in JinYuan20WCollector.CH2CurrentsMap.Keys)
            {
                cbLVCurrents.Items.Add(item);
            }
            cbLVCurrents.SelectedIndex = 0;

            foreach (var item in JinYuan20WCollector.TestTypeMap.Keys)
            {
                cbTestType.Items.Add(item);
            }
            cbTestType.SelectedIndex = 0;
            UpdateControlEnableState();
        }


        public class CurrentResistanceInfo
        {
            public float Current { get; set; }
            public int SortIndex { get; set; }
            public float Resistance { get; set; }
        }

        private void btAddRecord_Click(object sender, RoutedEventArgs e)
        {
            dataItems.Add(new CurrentResistanceInfo() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            lvUsers.Items.Refresh();

            items2.Add(new MyItem() { Name = "新增", Description = "这是新增的" });
        }

        public class MyItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private void btDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (dataItems.Count > 0)
            {
                dataItems.RemoveAt(dataItems.Count - 1);
                lvUsers.Items.Refresh();
            }
            if (items2.Count > 0)
            {
                items2.RemoveAt(items2.Count - 1);
            }
        }

        private void swConnect_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (swConnect.IsChecked == true)
            {
                IsConneted = true;
                Collector = new JinYuan20WCollector(cbSerialPort.SelectedItem.ToString(), Utils.ParseInt(cbBoundRate.Text))
                {
                    CH1Enabled = cbCH1.IsChecked == true,
                    CH2Enabled = cbCH2.IsChecked == true,
                    CH1CurrentConfig = JinYuan20WCollector.GetCH1CurrentConfig(cbHVCurrents.SelectedItem.ToString()),
                    CH2CurrentConfig = JinYuan20WCollector.GetCH2CurrentConfig(cbLVCurrents.SelectedItem.ToString()),
                    TestType = JinYuan20WCollector.TestTypeMap[cbTestType.SelectedItem.ToString()],
                };
                new Thread(() =>
                {
                    if (!Collector.Connect())
                    {
                        Collector = null;
                        IsConneted = false;
                        Dispatcher.Invoke(() =>
                        {
                            swConnect.IsChecked = false;
                        });
                    }
                    else
                    {
                        StartSyncState();
                    }
                }).Start();
            }
            else
            {
                StopSyncState();
                if (Collector != null)
                {
                    Collector.Disconnect();
                    Collector = null;
                }
                IsConneted = false;

            }
            UpdateControlEnableState();
        }

        private void UpdateControlEnableState()
        {
            cbBoundRate.IsEnabled = !IsConneted;
            cbSerialPort.IsEnabled = !IsConneted;

            //btStart.IsEnabled = IsConneted && !IsCollecting;
            //btStop.IsEnabled = IsConneted && IsCollecting;

            cbHVCurrents.IsEnabled = cbCH1.IsChecked == true;
            cbLVCurrents.IsEnabled = cbCH2.IsChecked == true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Close()
        {
            IsCollecting = false;
            if (Collector != null)
            {
                Collector.Disconnect();
                Collector = null;
            }
            if (TimerSecond.IsEnabled == true)
            {
                TimerSecond.Stop();
            }
        }

        #region 采集数据
        private void CollectDataOnce()
        {
            if (Collector != null)
            {
                if (CommandChange)
                {
                    CommandChange = false;

                }
                try
                {
                    var packet = Collector.ReadPacket();
                    if (packet != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UpdateDisplayByPacket(packet);
                            UpdateRealTimePanelDisplay(packet);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Process SinglePhaseCmdPacket Error " + ex.Message);
                }
            }
        }

        private void StartSyncState()
        {
            if (!IsConneted || IsCollecting)
            {
                return;
            }
            // 创建一个ManualResetEvent，初始状态为未设置（false）  
            ResetEvent = new ManualResetEvent(false);
            IsCollecting = true;
            Dispatcher.Invoke(new Action(() => { UpdateControlEnableState(); }));
            Log.Info("Start 20W Collect...");

            // 创建一个新线程并执行任务  
            new Thread(() =>
            {
                while (IsCollecting)
                {
                    CollectDataOnce();
                    if (ResetEvent.WaitOne(JinYuan20WCollector.Interval))
                    {
                        // 线程没有超时被唤醒，说明要停止循环了
                    }
                }
                IsCollecting = false;
                ResetEvent = null;
                Log.Info("Temp Collector DONE");
            }).Start();
        }

        private void StopSyncState()
        {
            if (!IsCollecting)
            {
                return;
            }
            IsCollecting = false;
            if (ResetEvent != null)
            {
                ResetEvent.Set();
            }
            UpdateControlEnableState();
        }
        #endregion

        #region 修改配置，下发给Collector
        private void cbCH1_CheckedChange(object sender, RoutedEventArgs e)
        {
            cbHVCurrents.IsEnabled = cbCH1.IsChecked == true;
            if (Collector != null)
            {
                Collector.CH1Enabled = cbCH1.IsChecked == true;
            }
        }

        private void cbCH2_CheckedChange(object sender, RoutedEventArgs e)
        {
            cbLVCurrents.IsEnabled = cbCH2.IsChecked == true;
            if (Collector != null)
            {
                Collector.CH2Enabled = cbCH2.IsChecked == true;
            }
        }

        private void cbHVCurrents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Collector != null)
            {
                Collector.CH1CurrentConfig = JinYuan20WCollector.GetCH1CurrentConfig(cbHVCurrents.SelectedItem.ToString());
            }
        }

        private void cbLVCurrents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Collector != null)
            {
                Collector.CH2CurrentConfig = JinYuan20WCollector.GetCH2CurrentConfig(cbLVCurrents.SelectedItem.ToString());
            }
        }
        #endregion

        private void btTime_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btRecord_Click(object sender, RoutedEventArgs e)
        {

        }


        #region 开始常规测试
        private void btCommonTest_Click(object sender, RoutedEventArgs e)
        {
            if (!IsConneted)
            {
                MessageBox.Show("请先连接设备！");
                return;
            }
            if (cbCH1.IsChecked != true && cbCH2.IsChecked != true)
            {
                MessageBox.Show("必须选择一个通道才可以开始测试！");
                return;
            }
            lastPacket = null;
            panelConfig.IsEnabled = false;
            panelTestChoice.IsEnabled = false;

            SecondElapsed = 0;
            UpdateClockDisplay();
            TimerSecond.Start();

            Collector?.SetParameters();
            Collector?.SetCommonTest();
            IsCommonTesting = true;

            UpdatePanelTestConfigDisplay();
            btCommonTest.IsEnabled = false;
        }
        #endregion

        #region 页面实时刷新
        private void UpdatePanelTestConfigDisplay()
        {
            if (cbCH1.IsChecked == true)
            {
                tbCH1TestConfig.Text = "常规-高压CH1-" + cbHVCurrents.Text;
            }
            else
            {
                tbCH1TestConfig.Text = "";
            }
            if (cbCH2.IsChecked == true)
            {
                tbCH2TestConfig.Text = "常规-低压CH2-" + cbLVCurrents.Text;
            }
            else
            {
                tbCH2TestConfig.Text = "";
            }
        }

        private void UpdateRealTimePanelDisplay(JinYuan20WCollector.JinYuan20WPacketInfo packet)
        {
            tbCH1Current.Text = Utils.FloatFormat(packet.ch1RealTimeCurrent, 2) + " A";
            tbCH2Current.Text = Utils.FloatFormat(packet.ch2RealTimeCurrent, 2) + " A";
            tbCH1Resistance.Text = Utils.FloatFormat(packet.ch1RealTimeResistance, 3) + " mΩ";
            tbCH2Resistance.Text = Utils.FloatFormat(packet.ch2RealTimeResistance, 3) + " mΩ";
            tbCH1State.Text = "【" + JinYuan20WCollector.CHStatusMap[packet.ch1Status] + "】";
            tbCH2State.Text = "【" + JinYuan20WCollector.CHStatusMap[packet.ch2Status] + "】";
            lastPacket = packet;
        }

        private void UpdateDisplayByPacket(JinYuan20WCollector.JinYuan20WPacketInfo packet)
        {
            tbCH1Enabled.Text = packet.ch1Enabled ? "是" : "否";
            tbCH2Enabled.Text = packet.ch2Enabled ? "是" : "否";

            tbCH1Status.Text = JinYuan20WCollector.CHStatusMap[packet.ch1Status];
            tbCH2Status.Text = JinYuan20WCollector.CHStatusMap[packet.ch2Status];

            tbCH1RealTimeCurrent.Text = Utils.FloatFormat(packet.ch1RealTimeCurrent);
            tbCH2RealTimeCurrent.Text = Utils.FloatFormat(packet.ch2RealTimeCurrent);

            tbCH1RealTimeResistance.Text = Utils.FloatFormat(packet.ch1RealTimeResistance);
            tbCH2RealTimeResistance.Text = Utils.FloatFormat(packet.ch2RealTimeResistance);

            tbCH1TimedResistance.Text = Utils.FloatFormat(packet.ch1TimedResistance);
            tbCH2TimedResistance.Text = Utils.FloatFormat(packet.ch2TimedResistance);

            tbDebugMsg.Text = "Debug Packet: " + packet.ToString();
        }
        #endregion

        private void btSetInterval_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbTestType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string? selectedType = cbTestType.SelectedItem != null ? cbTestType.SelectedItem.ToString() : null;
            if (selectedType == null)
            {
                return;
            }
            if (selectedType.Contains("常规"))
            {
            }
            else if (selectedType.Contains("温升"))
            {
            }
        }

        #region 测试结束，填写数据
        private void btQuitTest_Click(object sender, RoutedEventArgs e)
        {
            Collector?.SetRestCommand();
            panelConfig.IsEnabled = true;
            panelTestChoice.IsEnabled = true;
            TimerSecond.Stop();
            IsCommonTesting = false;
            btCommonTest.IsEnabled = true;
            if (tappingResistanceFields.Keys.Contains(SelectedTapping))
            {
                float value = 0;
                if (lastPacket != null)
                {
                    if (lastPacket.ch1Enabled)
                    {
                        value = lastPacket.ch1RealTimeResistance;
                    } 
                    else if (lastPacket.ch2Enabled)
                    {
                        value = lastPacket.ch2RealTimeResistance;
                    }
                }
                tappingResistanceFields[SelectedTapping].UpdateValueForActiveItem(value);
            }
        }
        #endregion

        #region 测试仪打印数据
        private void btPrint_Click(object sender, RoutedEventArgs e)
        {
            Collector?.SetPrintCommand();
        }
        #endregion

        #region 试验和分接选择
        private void TestTypeSelectedTappingChange(object sender, RoutedEventArgs e)
        {
            ToggleButton? b = sender as ToggleButton;
            if (b == null)
            {
                return;
            }
            if (b.IsChecked)
            {
                foreach (var item in testTypeToggleButtons)
                {
                    if (item.Value == b)
                    {
                        SelectedTesting = item.Key;
                    }
                    else
                    {
                        item.Value.IsChecked = false;
                    }
                }
            }
            else
            {
                SelectedTesting = null;
            }
            DumpSelectedTapping();

            if (SelectedTesting == null)
            {
                btCommonTest.Visibility = Visibility.Collapsed;
                btSetInterval.Visibility = Visibility.Collapsed;
            }
            else if (SelectedTesting == TestType20W.CommonTest)
            {
                btCommonTest.Visibility = Visibility.Visible;
                btSetInterval.Visibility = Visibility.Collapsed;

                //panelTappingChoice.Visibility = Visibility.Visible;
                panelCommonTestResult.Visibility = Visibility.Visible;
                lvUsers.Visibility = Visibility.Collapsed;
            } 
            else
            {
                btCommonTest.Visibility = Visibility.Collapsed;
                btSetInterval.Visibility = Visibility.Visible;

                //panelTappingChoice.Visibility = Visibility.Collapsed;
                panelCommonTestResult.Visibility = Visibility.Collapsed;
                lvUsers.Visibility = Visibility.Visible;
            }
        }

        private void HighVoltageSelectedTappingChange(object sender, RoutedEventArgs e)
        {
            var b = sender as ToggleButton;
            if (b == null)
            {
                return;
            }
            if (b.IsChecked)
            {
                foreach (var item in highVoltageToggleButtons)
                {
                    if (item.Value == b)
                    {
                        SelectedHighVoltageTapping = item.Key;
                    }
                    else
                    {
                        item.Value.IsChecked = false;
                    }
                }
            } 
            else
            {
                SelectedHighVoltageTapping = "";
            }
            DumpSelectedTapping();
        }

        private void LowVoltageSelectedTappingChange(object sender, RoutedEventArgs e)
        {
            var b = sender as ToggleButton;
            if (b == null)
            {
                return;
            }
            if (b.IsChecked)
            {
                foreach (var item in lowVoltageToggleButtons)
                {
                    if (item.Value == b)
                    {
                        SelectedLowVoltageTapping = item.Key;
                    }
                    else
                    {
                        item.Value.IsChecked = false;
                    }
                }
            }
            else
            {
                SelectedLowVoltageTapping = "";
            }
            DumpSelectedTapping();
        }

        private void TappingSelectedTappingChange(object sender, RoutedEventArgs e)
        {
            var b = sender as ToggleButton;
            if (b == null)
            {
                return;
            }
            if (b.IsChecked)
            {
                foreach (var item in tappingToggleButtons)
                {
                    if (item.Value == b)
                    {
                        SelectedTapping = item.Key;
                    }
                    else
                    {
                        item.Value.IsChecked = false;
                    }
                }
            }
            else
            {
                SelectedTapping = "";
            }
            DumpSelectedTapping();
        }

        // 低压1、低压2 选择
        private void LowVoltageLevelSelectedChange(object sender, RoutedEventArgs e)
        {
            var b = sender as ToggleButton;
            if (b == null)
            {
                return;
            }
            if (b == tbLowVoltageLevel1)
            {
                tbLowVoltageLevel2.IsChecked = false;
                SelectedLowVoltageLevel = 1;
            } 
            else if(b == tbLowVoltageLevel2)
            {
                tbLowVoltageLevel1.IsChecked = false;
                SelectedLowVoltageLevel = 2;
            } else
            {
                SelectedLowVoltageLevel = 0;
            }
        }

        private void DumpSelectedTapping()
        {
            string testing = "";
            foreach(var item in TestTypeMap)
            {
                if (item.Value == SelectedTesting)
                {
                    testing = item.Key;
                    break;
                }
            }
            tbDebugMsg.Text = $"{testing} - {SelectedTapping} - {SelectedHighVoltageTapping} - {SelectedLowVoltageTapping}";
        }
        #endregion

        #region 结果数据表格的激活状态切换
        private void Trf_ActiveEvent(TappingResistanceFields obj, int index)
        {
            foreach(var item in tappingResistanceFields.Values)
            {
                if (item == obj)
                {
                    continue;
                }
                item.ResetSelection();
            }
            SelectedTapping = obj.TappingIndex;
            DumpSelectedTapping();
        }
        #endregion
    }

}

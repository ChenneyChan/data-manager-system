using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using System.IO.Ports;
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
        private bool IsSimulate = false;
        private Random random = new Random();
        private bool IsFirstLoaded = true;
        private JinYuan20WCollector? Collector = null;
        private List<CommonTempRiseTestResistanceInfo> dataItems = new List<CommonTempRiseTestResistanceInfo>();
        private bool IsConneted = false;
        private bool IsCollecting = false;
        private ManualResetEvent? ResetEvent = null;
        private bool CommandChange = false;

        private Dictionary<string, TappingResistanceFields> tappingResistanceFields = new Dictionary<string, TappingResistanceFields>();
        private Dictionary<string, ToggleButton> highVoltageToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<string, ToggleButton> lowVoltageToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<string, ToggleButton> tappingToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<TestType20W, ToggleButton> testTypeToggleButtons = new Dictionary<TestType20W, ToggleButton>();
        private Dictionary<string, TextBox> tempRiseCoolTbs = new Dictionary<string, TextBox>();

        private string SelectedHighVoltageTapping = "";
        private string SelectedLowVoltageTapping = "";
        private string SelectedTapping = "";
        private TestType20W? SelectedTesting = TestType20W.CommonTest;
        private bool IsTempRiseCool = false;
        private int CurrentIndex = 0;

        private DispatcherTimer TimerSecond;
        private int SecondElapsed = 0;
        private bool IsCommonTesting = false;
        private bool IsTempRiseTesting = false;
        private int SelectedLowVoltageLevel = 0;
        private string SelectedTempRiseCoolItem = "";
        private string? TempRiseCoolSelectedCh1 = string.Empty;
        private string? TempRiseCoolSelectedCh2 = string.Empty;
        private string? TempRiseCoolSelectedLevel = string.Empty;

        private JinYuan20WPacketInfo? lastPacket = null;


        public JinYuan20W()
        {
            InitializeComponent();
            InitTimer();
            InitToggleButtons();
            InitTappings();
            InitTempRiseCoolTbs();
            btStartTiming.Visibility = Visibility.Collapsed;
            panelTappingChoice.Visibility = Visibility.Collapsed;
            dataGridPanel.Visibility = Visibility.Collapsed;

            // 数据绑定
            this.DataContext = new { Items = dataItems };

            // 开关调试信息
            panelDebug.Visibility = Configs.Configs.IsEnableVerboseDebug ? Visibility.Visible : Visibility.Collapsed;
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
            //testTypeToggleButtons.Add(TestType20W.TemperatureRise10Sec, tbTestTypeTempRiseCool);
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

        private void InitTempRiseCoolTbs()
        {
            tempRiseCoolTbs.Add("HV1", tbTempCoolHV1);
            tempRiseCoolTbs.Add("LV11", tbTempCoolLV11);
            tempRiseCoolTbs.Add("LV12", tbTempCoolLV12);

            tempRiseCoolTbs.Add("HV2", tbTempCoolHV2);
            tempRiseCoolTbs.Add("LV21", tbTempCoolLV21);
            tempRiseCoolTbs.Add("LV22", tbTempCoolLV22);
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
            if (!IsFirstLoaded)
            {
                return;
            }
            IsFirstLoaded = false;
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            int selectedIndex = ports.Length > 0 ? 0 : -1;
            for (int i = 0; i < ports.Length; i++)
            {
                var item = ports[i];
                cbSerialPort.Items.Add(item);
                if (item == Configs.Configs.SerialPort20W)
                {
                    selectedIndex = i;
                }
            }
            cbSerialPort.SelectedIndex = selectedIndex;

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

            UpdateControlEnableState();
        }

        private void swConnect_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (swConnect.IsChecked == true)
            {
                IsConneted = true;
                Configs.Configs.SerialPort20W = cbSerialPort.Text;
                Collector = new JinYuan20WCollector(cbSerialPort.SelectedItem.ToString(), Utils.ParseInt(cbBoundRate.Text))
                {
                    CH1Enabled = cbCH1.IsChecked == true,
                    CH2Enabled = cbCH2.IsChecked == true,
                    CH1CurrentConfig = JinYuan20WCollector.GetCH1CurrentConfig(cbHVCurrents.SelectedItem.ToString()),
                    CH2CurrentConfig = JinYuan20WCollector.GetCH2CurrentConfig(cbLVCurrents.SelectedItem.ToString()),
                    TestType = SelectedTesting ?? TestType20W.CommonTest,
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
            //Close();
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

        #region 开始常规测试
        private bool TestConfigCheck()
        {
            if (!IsConneted)
            {
                MessageBox.Show("请先连接设备！");
                return false;
            }
            if (cbCH1.IsChecked != true && cbCH2.IsChecked != true)
            {
                MessageBox.Show("必须选择一个通道才可以开始测试！");
                return false;
            }
            return true;
        }

        private void btCommonTest_Click(object sender, RoutedEventArgs e)
        {
            if (!TestConfigCheck())
            {
                return;
            }
            lastPacket = null;
            panelConfig.IsEnabled = false;
            panelTestChoice.IsEnabled = false;

            SecondElapsed = 0;
            UpdateClockDisplay();
            TimerSecond.Start();

            Collector?.SetCommonTest();
            IsCommonTesting = true;

            UpdatePanelTestConfigDisplay();
            btCommonTest.IsEnabled = false;
        }
        #endregion

        #region 常规测试结束，填写数据
        private void btQuitTest_Click(object sender, RoutedEventArgs e)
        {
            Collector?.SetRestCommand();
            panelConfig.IsEnabled = true;
            panelTestChoice.IsEnabled = true;
            TimerSecond.Stop();
            IsCommonTesting = false;
            btCommonTest.IsEnabled = true;

            float value = 0;
            if (lastPacket != null)
            {
                if (lastPacket.ch1Enabled)
                {
                    value = lastPacket.ch1RealTimeResistance * (lastPacket.ch1RealTimeResistanceIsMill ? 0.001f : 1f);
                }
                else if (lastPacket.ch2Enabled)
                {
                    value = lastPacket.ch2RealTimeResistance * (lastPacket.ch2RealTimeResistanceIsMill ? 0.001f : 1f);
                }
            }
            if (value == 0 && IsSimulate)
            {
                value = (float)random.Next() % 1000 + (float)random.NextDouble();
            }

            if (!IsTempRiseCool && tappingResistanceFields.Keys.Contains(SelectedTapping))
            {
                tappingResistanceFields[SelectedTapping].UpdateValueForActiveItem(value);
                UpdateMaxUnBalance();
            }
            else if (IsTempRiseCool && (TempRiseCoolSelectedCh1 != "空" || TempRiseCoolSelectedCh2 != "空"))
            {
                UpdateTempRiseCoolValue();
            }
        }

        #region 计算最大不平衡差
        private float[]? GetMaxMin(TappingResistanceFields tpr)
        {
            if (tpr.ValueAB == null || tpr.ValueBC == null || tpr.ValueCA == null)
            {
                return null;
            }
            float max = Math.Max((float)tpr.ValueAB, Math.Max((float)tpr.ValueBC, (float)tpr.ValueCA));
            float min = Math.Min((float)tpr.ValueAB, Math.Min((float)tpr.ValueBC, (float)tpr.ValueCA));

            return new float[] { max, min };
        }

        private float? CalculateMaxUnbalanceDiff(TappingResistanceFields tpr)
        {
            var maxMin = GetMaxMin(tpr);
            if (maxMin == null || maxMin.Length != 2)
            {
                return null;
            }
            float sum = ((float)tpr.ValueAB + (float)tpr.ValueBC + (float)tpr.ValueCA);
            if (sum == 0)
            {
                return null;
            }
            return ((maxMin[0] - maxMin[1]) / sum) * 100;
        }
        #endregion

        private void UpdateMaxUnBalance()
        {
            // 高压
            List<float> vs = new List<float>();
            for (int i = 1; i <= 9; i++)
            {
                var tpr = tappingResistanceFields[i.ToString()];
                if (tpr == null)
                {
                    continue;
                }
                var v = CalculateMaxUnbalanceDiff(tpr);
                if (v == null)
                {
                    continue;
                }
                vs.Add((float)v);
            }
            vs.Sort();
            if (vs.Count > 0)
            {
                tbHVMaxUnbalanceDiff.Text = Utils.FloatFormat(vs[vs.Count - 1]);
            }
            else
            {
                tbHVMaxUnbalanceDiff.Text = "";
            }

            // 低压
            tbLVMaxUnbalanceDiff11.Text = Utils.FloatFormatZeroIsNull(CalculateMaxUnbalanceDiff(tappingResistanceFields["11"]));
            tbLVMaxUnbalanceDiff12.Text = Utils.FloatFormatZeroIsNull(CalculateMaxUnbalanceDiff(tappingResistanceFields["12"]));
            tbLVMaxUnbalanceDiff21.Text = Utils.FloatFormatZeroIsNull(CalculateMaxUnbalanceDiff(tappingResistanceFields["21"]));
            tbLVMaxUnbalanceDiff22.Text = Utils.FloatFormatZeroIsNull(CalculateMaxUnbalanceDiff(tappingResistanceFields["22"]));
        }

        private void UpdateTempRiseCoolValue()
        {
            float? valueCh1 = null;
            float? valueCh2 = null;
            if (lastPacket != null)
            {
                if (lastPacket.ch1Enabled)
                {
                    valueCh1 = lastPacket.ch1RealTimeResistance * (lastPacket.ch1RealTimeResistanceIsMill ? 0.001f : 1f);
                }
                if (lastPacket.ch2Enabled)
                {
                    valueCh2 = lastPacket.ch2RealTimeResistance * (lastPacket.ch2RealTimeResistanceIsMill ? 0.001f : 1f);
                }
            }
            else if (IsSimulate)
            {
                valueCh1 = (float)random.Next() % 1000 + (float)random.NextDouble();
                valueCh2 = (float)random.Next() % 1000 + (float)random.NextDouble();
            }

            TextBox hv = TempRiseCoolSelectedLevel == "第一次" ? tbTempCoolHV1 : tbTempCoolHV2;
            TextBox lv1 = TempRiseCoolSelectedLevel == "第一次" ? tbTempCoolLV11 : tbTempCoolLV21;
            TextBox lv2 = TempRiseCoolSelectedLevel == "第一次" ? tbTempCoolLV12 : tbTempCoolLV22;

            if (valueCh1 != null)
            {
                switch (TempRiseCoolSelectedCh1)
                {
                    case "高压":
                        hv.Text = Utils.FormatFloat(valueCh1 ?? 0, 4);
                        break;
                    case "低压1":
                        lv1.Text = Utils.FormatFloat(valueCh1 ?? 0, 4);
                        break;
                    case "低压2":
                        lv2.Text = Utils.FormatFloat(valueCh1 ?? 0, 4);
                        break;
                }
            }

            if (valueCh2 != null)
            {
                switch (TempRiseCoolSelectedCh2)
                {
                    case "高压":
                        hv.Text = Utils.FormatFloat(valueCh2 ?? 0, 4);
                        break;
                    case "低压1":
                        lv1.Text = Utils.FormatFloat(valueCh2 ?? 0, 4);
                        break;
                    case "低压2":
                        lv2.Text = Utils.FormatFloat(valueCh2 ?? 0, 4);
                        break;
                }
            }
        }
        #endregion

        #region 开始温升计时
        private void btStartTiming_Click(object sender, RoutedEventArgs e)
        {
            if (!TestConfigCheck())
            {
                return;
            }
            lastPacket = null;
            panelConfig.IsEnabled = false;
            panelTestChoice.IsEnabled = false;

            SecondElapsed = 0;
            UpdateClockDisplay();
            TimerSecond.Start();

            Collector?.SetTemperatureRaiseTimerCommand();

            UpdatePanelTestConfigDisplay();
            btStartTiming.IsEnabled = false;
            btTempRiseTest.IsEnabled = true;
        }
        #endregion

        #region 开始温升测试
        private void btTempRiseTest_Click(object sender, RoutedEventArgs e)
        {
            btTempRiseTest.IsEnabled = false;
            Collector?.SetTemperatureRaiseTest();
            IsTempRiseTesting = true;

            cbTestPhase.IsEnabled = false;
            cbTestStatus.IsEnabled = false;
            cbTestCount.IsEnabled = false;
            cbCoolingMode.IsEnabled = false;
            dataItems.Clear();
            lvUsers.Items.Refresh();
            CurrentIndex = 0;
        }
        #endregion

        #region 退出温升测试
        private void btQuitTestTempRise_Click(object sender, RoutedEventArgs e)
        {
            Collector?.SetRestCommand();
            panelConfig.IsEnabled = true;
            panelTestChoice.IsEnabled = true;
            TimerSecond.Stop();
            IsTempRiseTesting = false;
            btStartTiming.IsEnabled = true;
            btTempRiseTest.IsEnabled = false;

            cbTestPhase.IsEnabled = true;
            cbTestStatus.IsEnabled = true;
            cbTestCount.IsEnabled = true;
            cbCoolingMode.IsEnabled = true;
        }
        #endregion

        #region 处理温升定时数据
        private void TempRiseTestRecord()
        {
            if (lastPacket == null)
            {
                return;
            }
            CurrentIndex += 1;
            // 定时记录数据
            float rhv = lastPacket.ch1TimedResistance * (lastPacket.ch1TimedResistanceIsMill ? 0.001f : 1f);
            float rlv = lastPacket.ch2TimedResistance * (lastPacket.ch2TimedResistanceIsMill ? 0.001f : 1f);
            float chv = lastPacket.ch1RealTimeCurrent * (lastPacket.ch1RealTimeCurrentIsMill ? 0.001f : 1f);
            float clv = lastPacket.ch2RealTimeCurrent * (lastPacket.ch2RealTimeCurrentIsMill ? 0.001f : 1f);
            dataItems.Add(new CommonTempRiseTestResistanceInfo()
            {
                SortIndex = CurrentIndex,
                CurrentHV = chv,
                ResistanceHV = rhv,
                CurrentLV = clv,
                ResistanceLV = rlv,
                CurrentTime = lastPacket.tempRaiseTime,
            });
            lvUsers.Items.Refresh();
        }
        #endregion

        #region 页面实时刷新
        private void UpdatePanelTestConfigDisplay()
        {
            string testType = SelectedTesting == TestType20W.CommonTest ? "常规" : "温升";
            if (cbCH1.IsChecked == true)
            {
                tbCH1TestConfig.Text = testType + "-高压CH1-" + cbHVCurrents.Text;
            }
            else
            {
                tbCH1TestConfig.Text = "";
            }
            if (cbCH2.IsChecked == true)
            {
                tbCH2TestConfig.Text = testType + "-低压CH2-" + cbLVCurrents.Text;
            }
            else
            {
                tbCH2TestConfig.Text = "";
            }
        }

        private void UpdateRealTimePanelDisplay(JinYuan20WCollector.JinYuan20WPacketInfo packet)
        {
            tbCH1Current.Text = packet.ch1RealTimeCurrent + (packet.ch1RealTimeCurrentIsMill ? "mA" : " A");
            tbCH2Current.Text = packet.ch2RealTimeCurrent + (packet.ch2RealTimeCurrentIsMill ? "mA" : " A");
            tbCH1Resistance.Text = packet.ch1RealTimeResistance + (packet.ch1RealTimeResistanceIsMill ? " mΩ" : "Ω");
            tbCH2Resistance.Text = packet.ch2RealTimeResistance + (packet.ch2RealTimeResistanceIsMill ? " mΩ" : "Ω");
            tbCH1State.Text = "【" + JinYuan20WCollector.CHStatusMap[packet.ch1Status] + "】";
            tbCH2State.Text = "【" + JinYuan20WCollector.CHStatusMap[packet.ch2Status] + "】";
            bool needRecordTempRise = false;
            if (SelectedTesting != TestType20W.CommonTest && lastPacket != null && packet.tempRaiseTime.Length > 0)
            {
                needRecordTempRise = packet.tempRaiseTime != lastPacket.tempRaiseTime;
            }
            lastPacket = packet;
            if (needRecordTempRise)
            {
                TempRiseTestRecord();
            }
            DumpPacekt();
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
            IsTempRiseCool = false;
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
                if (b == tbTestTypeTempRiseCool)
                {
                    SelectedTesting = TestType20W.CommonTest;
                    IsTempRiseCool = true;
                }
                else
                {
                    tbTestTypeTempRiseCool.IsChecked = false;
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
                btQuitTest.Visibility = Visibility.Collapsed;
                btPrint.Visibility = Visibility.Collapsed;

                btStartTiming.Visibility = Visibility.Collapsed;
                btTempRiseTest.Visibility = Visibility.Collapsed;
                btQuitTestTempRise.Visibility = Visibility.Collapsed;
            }
            else if (SelectedTesting == TestType20W.CommonTest)
            {
                btCommonTest.Visibility = Visibility.Visible;
                btQuitTest.Visibility = Visibility.Visible;
                btPrint.Visibility = Visibility.Visible;

                btStartTiming.Visibility = Visibility.Collapsed;
                btTempRiseTest.Visibility = Visibility.Collapsed;
                btQuitTestTempRise.Visibility = Visibility.Collapsed;

                //panelTappingChoice.Visibility = Visibility.Visible;
                if (b == tbTestTypeTempRiseCool)
                {
                    panelTempRiseCoolTestResult.Visibility = Visibility.Visible;
                    panelCommonTestResult.Visibility = Visibility.Collapsed;
                    dataGridPanel.Visibility = Visibility.Visible;
                    lvUsers.Visibility = Visibility.Collapsed;
                    panelStatus.Visibility = Visibility.Collapsed;
                    btUpdateTempRiseRecords.Visibility = Visibility.Collapsed;
                }
                else
                {
                    panelTempRiseCoolTestResult.Visibility = Visibility.Collapsed;
                    panelCommonTestResult.Visibility = Visibility.Visible;
                    dataGridPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                btCommonTest.Visibility = Visibility.Collapsed;
                btQuitTest.Visibility = Visibility.Collapsed;
                btPrint.Visibility = Visibility.Collapsed;

                btStartTiming.Visibility = Visibility.Visible;
                btTempRiseTest.Visibility = Visibility.Visible;
                btQuitTestTempRise.Visibility = Visibility.Visible;

                //panelTappingChoice.Visibility = Visibility.Collapsed;
                panelCommonTestResult.Visibility = Visibility.Collapsed;
                panelTempRiseCoolTestResult.Visibility = Visibility.Collapsed;
                dataGridPanel.Visibility = Visibility.Visible;
                lvUsers.Visibility = Visibility.Visible;
                panelStatus.Visibility = Visibility.Visible;
                btUpdateTempRiseRecords.Visibility = Visibility.Visible;
            }

            if (Collector != null)
            {
                Collector.TestType = SelectedTesting ?? TestType20W.CommonTest;
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

        private void DumpSelectedTapping()
        {
            string testing = "";
            foreach (var item in TestTypeMap)
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
            foreach (var item in tappingResistanceFields.Values)
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

        #region 温升冷电阻选相
        private bool IsEnableTempRiseCoolSingleSelect = false;
        private void tbTempCool_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsEnableTempRiseCoolSingleSelect)
            {
                return;
            }
            foreach (var item in tempRiseCoolTbs)
            {
                if (sender == item.Value)
                {
                    SelectedTempRiseCoolItem = item.Key;
                    item.Value.BorderThickness = new Thickness(2);
                }
                else
                {
                    item.Value.BorderThickness = new Thickness(0);
                }
            }
        }

        private void TempRiseCoolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTempRiseCoolCH1 == null || cbTempRiseCoolCH2 == null || cbTempRiseCoolLevel == null)
            {
                return;
            }
            var ch1 = (cbTempRiseCoolCH1.SelectedItem as ComboBoxItem).Content as string;
            var ch2 = (cbTempRiseCoolCH2.SelectedItem as ComboBoxItem).Content as string;
            var selectedLevel = (cbTempRiseCoolLevel.SelectedItem as ComboBoxItem).Content as string;
            var thicknessSelected = new Thickness(2);
            var thicknessNone = new Thickness(0);
            tbTempCoolHV1.BorderThickness = ((ch1 == "高压" || ch2 == "高压") && selectedLevel == "第一次") ? thicknessSelected : thicknessNone;
            tbTempCoolHV2.BorderThickness = ((ch1 == "高压" || ch2 == "高压") && selectedLevel == "第二次") ? thicknessSelected : thicknessNone;
            tbTempCoolLV11.BorderThickness = ((ch1 == "低压1" || ch2 == "低压1") && selectedLevel == "第一次") ? thicknessSelected : thicknessNone;
            tbTempCoolLV12.BorderThickness = ((ch1 == "低压2" || ch2 == "低压2") && selectedLevel == "第一次") ? thicknessSelected : thicknessNone;
            tbTempCoolLV21.BorderThickness = ((ch1 == "低压1" || ch2 == "低压1") && selectedLevel == "第二次") ? thicknessSelected : thicknessNone;
            tbTempCoolLV22.BorderThickness = ((ch1 == "低压2" || ch2 == "低压2") && selectedLevel == "第二次") ? thicknessSelected : thicknessNone;
            if (ch1 == ch2 && ch1 != "空")
            {
                HandyControl.Controls.Growl.Error("通道一、通道二选择相同冲突");
            }
            TempRiseCoolSelectedCh1 = ch1;
            TempRiseCoolSelectedCh2 = ch2;
            TempRiseCoolSelectedLevel = selectedLevel;
        }
        #endregion

        #region 数据上传
        private void btUpdateCoolingResistance_Click(object sender, RoutedEventArgs e)
        {
            if (!ControlUtils.CheckWorkflowBeforeUpload())
            {
                return;
            }
            var value = new CommonTempRiseCoolResistanceInfo()
            {
                WorkflowID = Configs.Configs.WorkflowID,
                Temperature = Utils.ParseFloatNull(tbTemperature2.Text),
                HighVoltageResistance1 = Utils.ParseFloatNull(tbTempCoolHV1.Text),
                HighVoltageResistance2 = Utils.ParseFloatNull(tbTempCoolHV2.Text),
                LowVoltageResistance11 = Utils.ParseFloatNull(tbTempCoolLV11.Text),
                LowVoltageResistance12 = Utils.ParseFloatNull(tbTempCoolLV12.Text),
                LowVoltageResistance21 = Utils.ParseFloatNull(tbTempCoolLV21.Text),
                LowVoltageResistance22 = Utils.ParseFloatNull(tbTempCoolLV22.Text),
                TestingIndex = Utils.ParseInt(cbTestCount.Text),
                TestingStatus = cbTestStatus.Text,
                CoolingMode = cbCoolingMode.Text
            };
            var slot1Current = Utils.ParseFloat(cbHVCurrents.Text.Replace("A", ""));
            var slot2Current = Utils.ParseFloat(cbLVCurrents.Text.Replace("A", ""));
            if (cbTempRiseCoolCH1.Text == "高压")
            {
                value.HighVoltageCurrent = slot1Current;
            }
            else if (cbTempRiseCoolCH1.Text == "低压1")
            {
                value.LowVoltageCurrent1 = slot1Current;
            }
            else if (cbTempRiseCoolCH1.Text == "低压2")
            {
                value.LowVoltageCurrent2 = slot1Current;
            }

            if (cbTempRiseCoolCH2.Text == "高压")
            {
                value.HighVoltageCurrent = slot2Current;
            }
            else if (cbTempRiseCoolCH2.Text == "低压1")
            {
                value.LowVoltageCurrent1 = slot2Current;
            }
            else if (cbTempRiseCoolCH2.Text == "低压2")
            {
                value.LowVoltageCurrent2 = slot2Current;
            }

            bool ret = value.UpdateWithNotNullFieldsOrInsert();
            ControlUtils.ShowUploadTips(ret);
        }

        // 温升电阻持续采集数据上传
        private void btUpdateTempRiseRecords_Click(object sender, RoutedEventArgs e)
        {
            int? slot1Type = null, slot2Type = null;
            if (!ControlUtils.CheckWorkflowBeforeUpload())
            {
                return;
            }
            // 三绕组场景，需要获取判断具体通道选择
            bool isSanRaoZu = (Configs.Configs.WorkflowInfo != null && Configs.Configs.WorkflowInfo.WorkflowType == "三绕组");
            var dialog = new TempRiseResistanceDialog(isSanRaoZu);
            var result = dialog.ShowDialog();
            slot1Type = dialog.Slot1VoltageType;
            slot2Type = dialog.Slot2VoltageType;
            if (result != true || ((slot1Type == null || slot1Type < 0) && (slot2Type == null || slot2Type < 0)))
            {
                return;
            }
            if (dataItems.Count == 0)
            {
                MessageBox.Show("暂无数据可以上传，请先采集数据!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            CommonTempRiseTestInfo configItem;
            int testIndex = Utils.ParseInt(cbTestCount.Text);
            var items = CommonTempRiseTestInfo.ReadFromDB(Configs.Configs.WorkflowID, cbTestPhase.Text, cbTestStatus.Text, cbCoolingMode.Text, testIndex, 2);
            if (items == null || items.Count == 0)
            {
                configItem = new CommonTempRiseTestInfo()
                {
                    TestingPhase = cbTestPhase.Text,
                    TestingStatus = cbTestStatus.Text,
                    WorkflowId = Configs.Configs.WorkflowID,
                    TestingIndex = testIndex,
                    CoolingMode = cbCoolingMode.Text,
                    DateTime = DateTime.Now,
                    TestingMode = 2,
                    TestingUser = dialog.TestUserName,
                };
                if (!configItem.WriteToDB())
                {
                    MessageBox.Show("数据上传失败!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                configItem = items[0];
            }

            bool ret = false;
            if (!isSanRaoZu)
            {
                // 删除之前的时间数据
                CommonTempRiseTestResistanceInfo.DeleteData(configItem.ID);

                // 将DataTable中的数据转成试验数据格式并且一条条上传
                List<CommonTempRiseTestResistanceInfo> itemsToUpload = new List<CommonTempRiseTestResistanceInfo>();
                foreach (var item in dataItems)
                {
                    var data = new CommonTempRiseTestResistanceInfo()
                    {
                        ID = configItem.ID,
                        VoltageType = 0,
                        SortIndex = item.SortIndex,
                        CurrentTime = item.CurrentTime
                    };
                    if (slot1Type != null)
                    {
                        if (slot1Type == 0)
                        {
                            data.CurrentHV = item.CurrentHV;
                            data.ResistanceHV = item.ResistanceHV;
                        } 
                        else if (slot1Type == 1)
                        {
                            data.CurrentLV = item.CurrentHV;
                            data.ResistanceLV = item.ResistanceHV;
                        }
                    }
                    if (slot2Type != null)
                    {
                        if (slot2Type == 0)
                        {
                            data.CurrentHV = item.CurrentLV;
                            data.ResistanceHV = item.ResistanceLV;
                        }
                        else if (slot2Type == 1)
                        {
                            data.CurrentLV = item.CurrentLV;
                            data.ResistanceLV = item.ResistanceLV;
                        }
                    }
                    itemsToUpload.Add(data);
                }
                ret = CommonTempRiseTestResistanceInfo.BatchInsertData(itemsToUpload);
            }
            else
            {
                List<CommonTempRiseTestResistanceInfo> dataList = new List<CommonTempRiseTestResistanceInfo>();
                // 三绕组场景，本次要上传哪个电压的数据，就先删除这个电压的已有数据
                if (slot1Type != null && slot1Type >= 0)
                {
                    CommonTempRiseTestResistanceInfo.DeleteData(configItem.ID, slot1Type + 1);
                    foreach (var item in dataItems)
                    {
                        dataList.Add(new CommonTempRiseTestResistanceInfo()
                        {
                            ID = configItem.ID,
                            SortIndex = item.SortIndex,
                            CurrentTime = item.CurrentTime,
                            VoltageType = (slot1Type ?? 0) + 1,
                            CurrentHV = item.CurrentHV,
                            ResistanceHV = item.ResistanceHV
                        });
                    }
                }
                if (slot2Type != null && slot2Type >= 0)
                {
                    CommonTempRiseTestResistanceInfo.DeleteData(configItem.ID, slot2Type + 1);
                    foreach (var item in dataItems)
                    {
                        dataList.Add(new CommonTempRiseTestResistanceInfo()
                        {
                            ID = configItem.ID,
                            SortIndex = item.SortIndex,
                            CurrentTime = item.CurrentTime,
                            VoltageType = (slot2Type ?? 0) + 1,
                            CurrentHV = item.CurrentLV,
                            ResistanceHV = item.ResistanceLV
                        });
                    }
                }
                ret = CommonTempRiseTestResistanceInfo.BatchInsertData(dataList);
            }

            if (ret)
            {
                MessageBox.Show($"数据上传成功，共{dataItems.Count}条数据!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
                MessageBox.Show($"数据上传出错，请检查或者重新尝试!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btUpdateResistanceRecords_Click(object sender, RoutedEventArgs e)
        {
            UpdateMaxUnBalance();
            if (!ControlUtils.CheckWorkflowBeforeUpload())
            {
                return;
            }
            DCResistanceInfo.DeleteData(Configs.Configs.WorkflowID);
            List<DCResistanceInfo> list = new List<DCResistanceInfo>();
            for (int i = 1; i <= 9; i++)
            {
                var tpr = tappingResistanceFields[i.ToString()];
                var v = new DCResistanceInfo()
                {
                    ProductSequence = Configs.Configs.WorkflowID,
                    Winding = "高压",
                    Tapping = i.ToString(),
                    AB = tpr.ValueAB,
                    BC = tpr.ValueBC,
                    CA = tpr.ValueCA,
                };
                if (i == 1)
                {
                    v.Temperature = Utils.ParseFloatNull(tbTemperature.Text);
                    v.DateTime = DateTime.Now;
                    v.MaxError = Utils.ParseFloatNull(tbHVMaxUnbalanceDiff.Text);
                }
                list.Add(v);
            }

            var tpr1 = tappingResistanceFields["10"];
            var item = new DCResistanceInfo()
            {
                ProductSequence = Configs.Configs.WorkflowID,
                Winding = "高压",
                Tapping = "额定分接",
                AB = tpr1.ValueAB,
                BC = tpr1.ValueBC,
                CA = tpr1.ValueCA,
            };
            list.Add(item);

            tpr1 = tappingResistanceFields["11"];
            item = new DCResistanceInfo()
            {
                ProductSequence = Configs.Configs.WorkflowID,
                Winding = "低压一",
                Tapping = "线电压",
                AB = tpr1.ValueAB,
                BC = tpr1.ValueBC,
                CA = tpr1.ValueCA,
            };
            list.Add(item);

            tpr1 = tappingResistanceFields["12"];
            item = new DCResistanceInfo()
            {
                ProductSequence = Configs.Configs.WorkflowID,
                Winding = "低压一",
                Tapping = "相电压",
                AB = tpr1.ValueAB,
                BC = tpr1.ValueBC,
                CA = tpr1.ValueCA,
            };
            list.Add(item);

            tpr1 = tappingResistanceFields["21"];
            item = new DCResistanceInfo()
            {
                ProductSequence = Configs.Configs.WorkflowID,
                Winding = "低压二",
                Tapping = "线电压",
                AB = tpr1.ValueAB,
                BC = tpr1.ValueBC,
                CA = tpr1.ValueCA,
            };
            list.Add(item);

            tpr1 = tappingResistanceFields["22"];
            item = new DCResistanceInfo()
            {
                ProductSequence = Configs.Configs.WorkflowID,
                Winding = "低压二",
                Tapping = "相电压",
                AB = tpr1.ValueAB,
                BC = tpr1.ValueBC,
                CA = tpr1.ValueCA,
            };
            list.Add(item);

            var ret = DCResistanceInfo.BatchInsertData(list);
            ControlUtils.ShowUploadTips(ret);
        }
        #endregion

        private void DumpPacekt()
        {
            Dispatcher.Invoke(() =>
            {
                if (lastPacket != null)
                {
                    tbDebug.Text = $"{DateTime.Now.ToString()}: {lastPacket.ToString()}";
                }
            });
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            if (ControlUtils.ShowClearMessage())
            {
                dataItems.Clear();
                lvUsers.Items.Refresh();

                foreach (var item in tappingResistanceFields)
                {
                    item.Value.ClearValue();
                }

                foreach (var item in tempRiseCoolTbs)
                {
                    item.Value.Text = "";
                }

                tbHVMaxUnbalanceDiff.Text = "";
                tbLVMaxUnbalanceDiff11.Text = "";
                tbLVMaxUnbalanceDiff12.Text = "";
                tbLVMaxUnbalanceDiff21.Text = "";
                tbLVMaxUnbalanceDiff22.Text = "";
            }
        }
    }

}

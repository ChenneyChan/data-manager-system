using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using ABBDataManagerSystem.Tools;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static ABBDataManagerSystem.Connector.JinYuan20ECollector;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// JinYuan20E.xaml 的交互逻辑
    /// </summary>
    public partial class JinYuan20E : UserControl, ICloseable
    {
        private bool IsSimulate = false;
        private Random random = new Random();
        private bool IsFirstLoaded = true;
        private JinYuan20ECollector? Collector = null;
        private List<CommonTempRiseTestResistanceInfo> dataItems = new List<CommonTempRiseTestResistanceInfo>();
        private bool IsConneted = false;
        private bool IsCollecting = false;
        private ManualResetEvent? ResetEvent = null;
        private bool CommandChange = false;

        private Dictionary<string, TappingResistanceFields> tappingResistanceFields = new Dictionary<string, TappingResistanceFields>();
        private Dictionary<string, ToggleButton> highVoltageToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<string, ToggleButton> lowVoltageToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<string, ToggleButton> tappingToggleButtons = new Dictionary<string, ToggleButton>();
        private Dictionary<TestType20E, ToggleButton> testTypeToggleButtons = new Dictionary<TestType20E, ToggleButton>();
        private Dictionary<string, TextBox> tempRiseCoolTbs = new Dictionary<string, TextBox>();

        private string SelectedHighVoltageTapping = "";
        private string SelectedLowVoltageTapping = "";
        private string SelectedTapping = "";
        private TestType20E? SelectedTesting = TestType20E.Normal;
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

        private JinYuan20ECollector.CommonPacket? lastPacket = null;


        public JinYuan20E()
        {
            InitializeComponent();
            InitTimer();
            InitToggleButtons();
            InitTappings();
            InitTempRiseCoolTbs();
            btStartTiming.Visibility = Visibility.Collapsed;
            panelTappingChoice.Visibility = Visibility.Collapsed;
            dataGridPanel.Visibility = Visibility.Collapsed;

            var currents = JinYuan20ECollector.EnumHelper.currentMap.Values;
            foreach (var current in currents)
            {
                cb20ECurrents.Items.Add(current);
            }
            cb20ECurrents.SelectedIndex = 0;

            var patterns = JinYuan20ECollector.EnumHelper.patternMap.Values;
            //foreach (var pattern in patterns)
            {
                cb20EPatterns.Items.Add("单通道");
                cb20EPatterns.Items.Add("双通道");
            }
            cb20EPatterns.SelectedIndex = 0;

            // 数据绑定
            this.DataContext = new { Items = dataItems };

            // 开关调试信息
            panelDebug.Visibility = Configs.Configs.IsEnableVerboseDebug ? Visibility.Visible: Visibility.Collapsed;

            Tools.EventManager.Instance.Subscribe("TestingEnableStatusChange", TestingEnableStatusEventHandler);
            ToolgeTestingStatus();
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

            testTypeToggleButtons.Add(TestType20E.Normal, tbTestTypeCommon);
            testTypeToggleButtons.Add(TestType20E.TemperatureRise10Sec, tbTestTypeTempRise10s);
            testTypeToggleButtons.Add(TestType20E.TemperatureRise30Sec, tbTestTypeTempRise30s);
            testTypeToggleButtons.Add(TestType20E.TemperatureRise60Sec, tbTestTypeTempRise60s);
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
                if (item == Configs.Configs.SerialPort20E)
                {
                    selectedIndex = i;
                }
            }
            cbSerialPort.SelectedIndex = selectedIndex;

            UpdateControlEnableState();
        }

        private void swConnect_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (swConnect.IsChecked == true)
            {
                IsConneted = true;
                Configs.Configs.SerialPort20E = cbSerialPort.Text;
                Collector = new JinYuan20ECollector(cbSerialPort.SelectedItem.ToString(), Utils.ParseInt(cbBoundRate.Text));
                Collector.CurrentMode = JinYuan20ECollector.EnumHelper.GetCurrentEnum(cb20ECurrents.SelectedItem.ToString());
                Collector.Mode = SelectedTesting ?? TestType20E.Normal;
                Collector.PatternMode = JinYuan20ECollector.EnumHelper.GetPatternEnum(cb20EPatterns.SelectedItem.ToString());
                Collector.WindingMode = Collector.PatternMode == TestPattern.SingleChannel ? TestWindingType.Rx : TestWindingType.Rx1_Rx2;
                Collector.SendParameterSetCommand();
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

            Tools.EventManager.Instance.Unsubscribe("TestingEnableStatusChange", TestingEnableStatusEventHandler);
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
            Log.Info("Start 20E/40E Collect...");

            // 创建一个新线程并执行任务  
            new Thread(() =>
            {
                while (IsCollecting)
                {
                    CollectDataOnce();
                    if (ResetEvent.WaitOne(JinYuan20ECollector.Interval))
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
        private void cb20E_ComboBox_ConfigChanges(object sender, RoutedEventArgs e)
        {
            if (cb20EPatterns.SelectedItem == null || cb20ECurrents.SelectedItem == null)
            {
                return;
            }
            if (cb20EPatterns.SelectedItem.ToString() != "单通道" || SelectedTesting != TestType20E.Normal)
            {
                if (cb20ECurrents.SelectedItem.ToString() == "25mA")
                {
                    MessageBox.Show("只有常规单通道测试模式下，才可以使用25mA电压！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            if (Collector != null)
            {
                Collector.CurrentMode = JinYuan20ECollector.EnumHelper.GetCurrentEnum(cb20ECurrents.SelectedItem.ToString());
                Collector.Mode = SelectedTesting ?? TestType20E.Normal;
                Collector.PatternMode = JinYuan20ECollector.EnumHelper.GetPatternEnum(cb20EPatterns.SelectedItem.ToString());
                Collector.WindingMode = Collector.PatternMode == TestPattern.SingleChannel ? TestWindingType.Rx : TestWindingType.Rx1_Rx2;
                Collector.SendParameterSetCommand();
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

            Collector?.SendTestCommand();
            IsCommonTesting = true;

            UpdatePanelTestConfigDisplay();
            btCommonTest.IsEnabled = false;
        }
        #endregion

        #region 常规测试结束，填写数据
        private void btQuitTest_Click(object sender, RoutedEventArgs e)
        {
            Collector?.SendExitCommandAtNormal();
            panelConfig.IsEnabled = true;
            panelTestChoice.IsEnabled = true;
            TimerSecond.Stop();
            IsCommonTesting = false;
            btCommonTest.IsEnabled = true;

            float value = 0;
            if (lastPacket != null)
            {
                value = Utils.GetValueWithMill(lastPacket.strRealTimeResistance1, false) ?? 0;
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
                var v = JinYuanUtils.CalculateMaxUnbalanceDiff(tpr);
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
            tbLVMaxUnbalanceDiff11.Text = Utils.FloatFormatZeroIsNull(JinYuanUtils.CalculateMaxUnbalanceDiff(tappingResistanceFields["11"]));
            tbLVMaxUnbalanceDiff12.Text = Utils.FloatFormatZeroIsNull(JinYuanUtils.CalculateMaxUnbalanceDiff(tappingResistanceFields["12"]));
            tbLVMaxUnbalanceDiff21.Text = Utils.FloatFormatZeroIsNull(JinYuanUtils.CalculateMaxUnbalanceDiff(tappingResistanceFields["21"]));
            tbLVMaxUnbalanceDiff22.Text = Utils.FloatFormatZeroIsNull(JinYuanUtils.CalculateMaxUnbalanceDiff(tappingResistanceFields["22"]));
        }

        private void UpdateTempRiseCoolValue()
        {
            float? valueCh1 = null;
            float? valueCh2 = null;
            if (lastPacket != null)
            {
                valueCh1 = Utils.GetValueWithMill(lastPacket.strRealTimeResistance1, false);
                valueCh2 = Utils.GetValueWithMill(lastPacket.strRealTimeResistance2, false);
            }
            else if (IsSimulate)
            {
                valueCh1 = (float)random.Next() % 1000 + (float)random.NextDouble();
                valueCh2 = (float)random.Next() % 1000 + (float)random.NextDouble();
            }
            string? ch1 = valueCh1 == null ? "" : Utils.FormatFloat((float)valueCh1, 4);
            string? ch2 = valueCh2 == null ? "" : Utils.FormatFloat((float)valueCh2, 4);

            TextBox hv = TempRiseCoolSelectedLevel == "第一次" ? tbTempCoolHV1 : tbTempCoolHV2;
            TextBox lv1 = TempRiseCoolSelectedLevel == "第一次" ? tbTempCoolLV11 : tbTempCoolLV21;
            TextBox lv2 = TempRiseCoolSelectedLevel == "第一次" ? tbTempCoolLV12 : tbTempCoolLV22;

            if (ch1 != null)
            {
                switch (TempRiseCoolSelectedCh1)
                {
                    case "高压":
                        hv.Text = ch1;
                        break;
                    case "低压1":
                        lv1.Text = ch1;
                        break;
                    case "低压2":
                        lv2.Text = ch1;
                        break;
                }
            }

            if (ch2 != null)
            {
                switch (TempRiseCoolSelectedCh2)
                {
                    case "高压":
                        hv.Text = ch2;
                        break;
                    case "低压1":
                        lv1.Text = ch2;
                        break;
                    case "低压2":
                        lv2.Text = ch2;
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

            Collector?.SendTestCommand();

            UpdatePanelTestConfigDisplay();
            btStartTiming.IsEnabled = false;
            btTempRiseTest.IsEnabled = true;
        }
        #endregion

        #region 开始温升测试
        private void btTempRiseTest_Click(object sender, RoutedEventArgs e)
        {
            btTempRiseTest.IsEnabled = false;
            Collector?.SendTempRiseTestCommandAtTiming();
            IsTempRiseTesting = true;

            cbTestPhase.IsEnabled = false;
            cbTestStatus.IsEnabled = false;
            cbTestCount.IsEnabled = false;
            tbTestCount.IsEnabled = false;
            cbCoolingMode.IsEnabled = false;
            dataItems.Clear();
            lvUsers.Items.Refresh();
            CurrentIndex = 0;
        }
        #endregion

        #region 退出温升测试
        private void btQuitTestTempRise_Click(object sender, RoutedEventArgs e)
        {
            Collector?.SendTempRiseExitCommandAtTest();
            panelConfig.IsEnabled = true;
            panelTestChoice.IsEnabled = true;
            TimerSecond.Stop();
            IsTempRiseTesting = false;
            btStartTiming.IsEnabled = true;
            btTempRiseTest.IsEnabled = false;

            cbTestPhase.IsEnabled = true;
            cbTestStatus.IsEnabled = true;
            cbTestCount.IsEnabled = true;
            tbTestCount.IsEnabled = true;
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
            float current = Utils.GetValueWithMill(lastPacket.strRealTimeCurrent, false) ?? 0;
            float resistance1 = Utils.GetValueWithMill(lastPacket.strSecResistance1, false) ?? 0;
            float resistance2 = Utils.GetValueWithMill(lastPacket.strSecResistance2, false) ?? 0;
            string time = (lastPacket.strSecTime != null && lastPacket.strSecTime.Length == 4) ? lastPacket.strSecTime.Substring(0, 2) + ":" + lastPacket.strSecTime.Substring(2, 2) : "";

            CurrentIndex += 1;
            // 定时记录数据
            dataItems.Add(new CommonTempRiseTestResistanceInfo()
            {
                SortIndex = CurrentIndex,
                CurrentHV = current,
                ResistanceHV = resistance1,
                CurrentLV = cb20EPatterns.Text == "双通道" ? current : 0,
                ResistanceLV = cb20EPatterns.Text == "双通道" ? resistance2 : 0,
                CurrentTime = time
            });
            lvUsers.Items.Refresh();
        }
        #endregion

        #region 页面实时刷新
        private void UpdatePanelTestConfigDisplay()
        {
            string testType = SelectedTesting == TestType20E.Normal ? "常规" : "温升";
            tbCH1TestConfig.Text = testType + "-高压CH1-" + cb20ECurrents.Text;
            if (cb20EPatterns.Text == "双通道")
            {
                tbCH2TestConfig.Text = testType + "-低压CH2-" + cb20ECurrents.Text;
            }
            else
            {
                tbCH2TestConfig.Text = "";
            }
        }

        private void UpdateRealTimePanelDisplay(JinYuan20ECollector.CommonPacket packet)
        {
            //Log.Info("UpdateRealTimePanelDisplay: " + packet.ToString());
            tbCH1Resistance.Text = packet.strRealTimeResistance1 + "Ω";
            tbCH2Resistance.Text = packet.strRealTimeResistance2 + "Ω";
            tbCH1State.Text = "【" + packet.Status + "】";
            tbCH2State.Text = "【" + packet.Status + "】";
            tbCH1Current.Text = packet.strRealTimeCurrent + "A";
            tbCH2Current.Text = packet.strRealTimeCurrent + "A";
            bool needRecordTempRise = false;

            if (SelectedTesting != TestType20E.Normal && lastPacket != null && packet.strSecTime.Length > 0)
            {
                // 记录一次温升数据
                needRecordTempRise = packet.strSecTime != lastPacket.strSecTime && packet.strSecTime != "0000";
                Log.Info($"lastTime = {lastPacket.strSecTime} currentTime = {packet.strSecTime} needRecord = {needRecordTempRise}");
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
            Collector?.SendPrintCommandAtNormal();
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
                    SelectedTesting = TestType20E.Normal;
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
            if (Collector != null)
            {
                Collector.Mode = SelectedTesting ?? TestType20E.Normal;
                Collector.SendParameterSetCommand();
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
            else if (SelectedTesting == TestType20E.Normal)
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
            //foreach (var item in TestTypeMap)
            //{
            //    if (item.Value == SelectedTesting)
            //    {
            //        testing = item.Key;
            //        break;
            //    }
            //}
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
            bool isMill = cb20ECurrents.Text.IndexOf("m") >= 0;
            float current = Utils.GetFloat(cb20ECurrents.Text) ?? 0 * (isMill ? 0.001f : 1f);
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
                TestingIndex = (int)tbTestCount.Value,
                TestingStatus = cbTestStatus.Text,
                CoolingMode = cbCoolingMode.Text
            };
            if (value.HighVoltageResistance1 != null || value.HighVoltageResistance2 != null)
            {
                value.HighVoltageCurrent = current;
            }
            if (value.LowVoltageResistance11 != null || value.LowVoltageResistance12 != null)
            {
                value.LowVoltageCurrent1 = current;
            }
            if (value.LowVoltageResistance21 != null || value.LowVoltageResistance22 != null)
            {
                value.LowVoltageCurrent2 = current;
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
            int testIndex = (int)tbTestCount.Value;
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
                Tapping = "额定分接相电阻",
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
                Tapping = "线电阻",
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
                Tapping = "相电阻",
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
                Tapping = "线电阻",
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
                Tapping = "相电阻",
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

        private void TestingEnableStatusEventHandler(object sender, TestEventArgs e)
        {
            ToolgeTestingStatus();
        }

        private void ToolgeTestingStatus()
        {
            Dispatcher.Invoke(() =>
            {
                panelCommonTestControl.IsEnabled = Configs.Configs.IsEnableTesting;
            });
            if (Collector != null && !Configs.Configs.IsEnableTesting)
            {
                Collector.SendResetCommand();

                Dispatcher.Invoke(() =>
                {
                    if (SelectedTesting == TestType20E.Normal) //常规测试
                    {
                        panelConfig.IsEnabled = true;
                        panelTestChoice.IsEnabled = true;
                        TimerSecond.Stop();
                        IsCommonTesting = false;
                        btCommonTest.IsEnabled = true;
                    }
                    else // 温升测试
                    {
                        btQuitTestTempRise_Click(new object(), new RoutedEventArgs());
                    }
                });
            }
        }
    }

}

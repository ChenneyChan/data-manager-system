using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using ABBDataManagerSystem.Tools;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using static ABBDataManagerSystem.Connector.JinYuanJYTACollector;

namespace ABBDataManagerSystem.Pages
{

    /// <summary>
    /// JinYuanJYT_A.xaml 的交互逻辑
    /// </summary>
    public partial class JinYuanJYT_A : UserControl, ICloseable
    {
        private Dictionary<string, TappingVoltageRatioFields> RatioValueFields = new Dictionary<string, TappingVoltageRatioFields>();
        private Dictionary<string, JinYunJYTATestResult> SavedResults = new Dictionary<string, JinYunJYTATestResult>();
        private JinYuanJYTACollector? Collector = null;
        private bool IsTesting = false;
        private int Interval = 200;
        private JinYunJYTATestResult? CurrentResult = null;
        private static readonly int MAX_HV_COUNT = 9;

        public JinYuanJYT_A()
        {
            InitializeComponent();
            InitRatioValues();
            InitTestTappingChoice();
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (var item in ports)
            {
                cbSerialPort.Items.Add(item);
            }
            if (ports.Length > 0) { cbSerialPort.SelectedIndex = 0; }
            InitDeviceConfigs();
            Tools.EventManager.Instance.Subscribe("WorkflowSelected", WorkflowEventHandler);
            HandleWorkflowChange();
        }

        #region 初始化一些配置和值
        private void InitDeviceConfigs()
        {
            foreach (var item in JinYuanJYTACollector.TestVoltageTypeMap.Keys)
            {
                cbVoltageConfig.Items.Add(item);
            }
            cbVoltageConfig.SelectedIndex = 0;

            foreach (var item in JinYuanJYTACollector.TappingPointTypeMap.Keys)
            {
                cbTappingPoint.Items.Add(item);
            }
            cbTappingPoint.SelectedIndex = 0;

            foreach (var item in JinYuanJYTACollector.HomopolarityDisplayTypeMap.Keys)
            {
                cbHomopolarityDisplay.Items.Add(item);
            }
            cbHomopolarityDisplay.SelectedIndex = 0;

            foreach (var item in JinYuanJYTACollector.HighVoltageConnectionTypeMap.Keys)
            {
                cbHighVoltageConnection.Items.Add(item);
            }
            cbHighVoltageConnection.SelectedIndex = 0;

            foreach (var item in JinYuanJYTACollector.LowVoltageConnectionTypeMap.Keys)
            {
                cbLowVoltageConnection.Items.Add(item);
            }
            cbLowVoltageConnection.SelectedIndex = 0;
        }

        private void InitRatioValues()
        {
            RatioValueFields.Add("1", tvrTappingPoint1);
            RatioValueFields.Add("2", tvrTappingPoint2);
            RatioValueFields.Add("3", tvrTappingPoint3);
            RatioValueFields.Add("4", tvrTappingPoint4);
            RatioValueFields.Add("5", tvrTappingPoint5);
            RatioValueFields.Add("6", tvrTappingPoint6);
            RatioValueFields.Add("7", tvrTappingPoint7);
            RatioValueFields.Add("8", tvrTappingPoint8);
            RatioValueFields.Add("9", tvrTappingPoint9);
            RatioValueFields.Add("21", tvrTappingPoint21);
        }

        private void InitTestTappingChoice()
        {
            for (int i = 1; i <= 9; i++)
            {
                cbTestTapping.Items.Add($"{i}分接");
            }
            cbTestTapping.Items.Add("2-1分接");
            cbTestTapping.SelectedIndex = 0;
        }

        private TappingVoltageRatioFields? GetSelectedTVR(ref string fieldKey)
        {
            if (cbTestTapping.SelectedIndex < 0)
            {
                return null;
            }
            string selectedTapping = (string)cbTestTapping.SelectedValue;
            string index = selectedTapping.Remove(selectedTapping.IndexOf("分接"));
            if (index == "2-1")
            {
                index = "21";
            }
            fieldKey = index;
            return RatioValueFields[index];
        }
        #endregion

        private void swConnect_CheckedChange(object sender, RoutedEventArgs e)
        {
            cbSerialPort.IsEnabled = swConnect.IsChecked != true;
            cbBoundRate.IsEnabled = swConnect.IsChecked != true;
            if (swConnect.IsChecked != true && Collector != null)
            {
                IsTesting = false;
                Collector.Disconnect();
                Collector = null;
            }
            else if (swConnect.IsChecked == true)
            {
                Collector = new JinYuanJYTACollector(cbSerialPort.Text, Utils.ParseInt(cbBoundRate.Text));
                new Thread(() =>
                {
                    if (!Collector.Connect())
                    {
                        swConnect.IsChecked = false;
                    }
                    else
                    {
                        IsTesting = true;
                        new Thread(() =>
                        {
                            while (IsTesting && Collector != null)
                            {
                                bool needReset = false;
                                var result = Collector.ReadPacket(ref needReset);
                                if (needReset)
                                {
                                }
                                Dispatcher.Invoke(new Action(() => { HandleResult(result); }));
                                Thread.Sleep(this.Interval);
                            }
                        }).Start();
                    }

                }).Start();
            }
        }

        private void HandleResult(JinYuanJYTACollector.JinYunJYTATestResult? result)
        {
            if (Collector == null)
            {
                tbDeviceState.Text = "设备未连接";
                return;
            }

            #region 状态信息
            if (JinYuanJYTACollector.DeviceStateMap.Keys.Contains(Collector.deviceState))
            {
                tbDeviceState.Text = JinYuanJYTACollector.DeviceStateMap[Collector.deviceState];
            }
            if (JinYuanJYTACollector.PowerCodeTypeMap.Keys.Contains(Collector.powerCode))
            {
                tbDevicePowerInfo.Text = JinYuanJYTACollector.PowerCodeTypeMap[Collector.powerCode];
            }
            if (JinYuanJYTACollector.TipInfoTypeMap.Keys.Contains(Collector.tipInfo))
            {
                tbTipInfo.Text = JinYuanJYTACollector.TipInfoTypeMap[Collector.tipInfo];
            }
            #endregion

            if (result == null)
            {
                return;
            }
            CurrentResult = result;
            if (result.IsSinglePhase)
            {
                tbSingleRatio.Text = Utils.FloatFormat(result.Ratio[0]);
                tbSingleVoltage.Text = Utils.FloatFormat(result.Voltage[0]);
                tbSingleCurrent.Text = Utils.FloatFormat(result.Current[0]);
                tbSingleCalculatedRatio.Text = Utils.FloatFormat(result.CalculatedRatio);
                tbSingleError.Text = Utils.FloatFormat(result.Error[0]);
                tbSingleTappingPosition.Text = result.TappingPosition;
                tbPolarity.Text = result.Polarity.ToString();
                tbSingleFrequence.Text = Utils.FloatFormat(result.Frequence);
            }
            else
            {
                tbRatioA.Text = Utils.FloatFormat(result.Ratio[0]);
                tbRatioB.Text = Utils.FloatFormat(result.Ratio[1]);
                tbRatioC.Text = Utils.FloatFormat(result.Ratio[2]);
                tbTurnRatioA.Text = Utils.FloatFormat(result.TurnRatio[0]);
                tbTurnRatioB.Text = Utils.FloatFormat(result.TurnRatio[1]);
                tbTurnRatioC.Text = Utils.FloatFormat(result.TurnRatio[2]);
                tbVoltageA.Text = Utils.FloatFormat(result.Voltage[0]);
                tbVoltageB.Text = Utils.FloatFormat(result.Voltage[1]);
                tbVoltageC.Text = Utils.FloatFormat(result.Voltage[2]);
                tbCurrentA.Text = Utils.FloatFormat(result.Current[0]);
                tbCurrentB.Text = Utils.FloatFormat(result.Current[1]);
                tbCurrentC.Text = Utils.FloatFormat(result.Current[2]);
                tbCalculatedRatio.Text = Utils.FloatFormat(result.CalculatedRatio);
                tbErrorA.Text = Utils.FloatFormat(result.Error[0]);
                tbErrorB.Text = Utils.FloatFormat(result.Error[1]);
                tbErrorC.Text = Utils.FloatFormat(result.Error[2]);
                tbTappingPosition.Text = result.TappingPosition;
                tbFrequence.Text = Utils.FloatFormat(result.Frequence);
                tbConnectionType.Text = result.ConnectionType;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ActiveSelectedTest();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Close();
        }

        public void Close()
        {
            if (Collector != null)
            {
                Collector.Disconnect();
                Collector = null;
            }
            IsTesting = false;
            Tools.EventManager.Instance.Unsubscribe("WorkflowSelected", WorkflowEventHandler);
        }

        private void cbTestMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActiveSelectedTest();
        }

        private void ActiveSelectedTest()
        {
            if (cbTestMode.SelectedIndex != 0 && tiSinglePhase != null && tiSinglePhase.IsSelected != true)
            {
                tiSinglePhase.IsSelected = true;
            }
            else if (cbTestMode.SelectedIndex != 1 && tiThreePhase != null && tiThreePhase.IsSelected != true)
            {
                tiThreePhase.IsSelected = true;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tiSinglePhase.IsSelected == true)
            {
                cbTestMode.SelectedIndex = 1;
            }
            else if (tiThreePhase.IsSelected == true)
            {
                cbTestMode.SelectedIndex = 0;
            }
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            if (Collector != null)
                Collector.SetResetCommand();
        }

        private void btSetParameters_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null)
            {
                return;
            }
            Collector.RatedHighVoltage = (float)tbHighVoltage.Value;
            Collector.RatedLowVoltage = (float)tbLowVoltage.Value;
            Collector.RatedTapping = (int)tbRatedTapping.Value;
            Collector.TappingPoint = cbTappingPoint.Text == "高压侧" ? TappingPointType.HighVoltage : TappingPointType.LowVoltage;
            Collector.PositiveTappingCount = (int)tbPositiveTappingCount.Value;
            Collector.TappingSpacing = (float)tbTappingSpacing.Value;
            Collector.TestVoltage = cbVoltageConfig.Text == "160V" ? TestVoltageType.Voltage160V : TestVoltageType.Voltage20V;
            Collector.ProductSequence = Configs.Configs.WorkflowID;
            if (cbTestMode.Text == "三相测试")
            {
                //  <额定高压:8字节><额定低压:8字节><额定分接:2字节><分接位置:1字节><正分接数:2字节><分接间距:6字节>
                //  <试品编号:14字节><高压联结:1字节><低压联结:1字节><组别:2字节> <试验电压:1字节> XOR 0D
                Collector.HighVoltageConnection = GetHighConnectionType(cbHighVoltageConnection.Text) ?? HighVoltageConnectionType.Type_Y;
                Collector.LowVoltageConnection = GetLowConnectionType(cbLowVoltageConnection.Text) ?? LowVoltageConnectionType.Type_Y;
                Collector.Group = (int)tbGroup.Value;
                Collector.TestType = TestTypeJYTA.ThreePhaseTest;

                Collector.SetThreePhaseTest();
            }
            else
            {
                // <额定高压:8字节><额定低压:8字节><额定分接:2字节><分接位置:1字节><正分接数:2字节><分接间距:6字节>
                // <试品编号:14字节> <同极性显示:1字节> <试验电压:1字节>XOR 0D  
                Collector.HomopolarityDisplay = GetHomopolarityDisplayType(cbHomopolarityDisplay.Text) ?? HomopolarityDisplayType.Negtive;
                Collector.TestType = TestTypeJYTA.SinglePhaseTest;
                Collector.SetSinglePhaseTest();
            }
        }

        private void btStartTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null)
            {
                return;
            }
            Collector.SendStartTest();
        }

        private void btReTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null)
            {
                return;
            }
            Collector.SendRestartTest();
        }

        private void cbTestTapping_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string key = "";
            var tvr = GetSelectedTVR(ref key);
            if (tvr == null)
            {
                return;
            }
            foreach (var item in RatioValueFields.Values)
            {
                if (item != tvr)
                {
                    item.IsSelected = false;
                }
                else
                {
                    item.IsSelected = true;
                }
            }
        }

        #region 测试数据写入右侧表格
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentResult == null)
            {
                return;
            }
            string key = "";
            var tvr = GetSelectedTVR(ref key);
            if (tvr == null)
            {
                return;
            }
            if (tvr != tvrTappingPoint21)
            {
                tbConnectionGroup1.Text = CurrentResult.ConnectionType;
            }
            else
            {
                tbConnectionGroup2.Text = CurrentResult.ConnectionType;
            }
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentResult == null)
            {
                return;
            }
            string key = "";
            var tvr = GetSelectedTVR(ref key);
            if (tvr == null)
            {
                return;
            }
            SavedResults[key] = CurrentResult;
            tvr.ValueAB = CurrentResult.Ratio[0];
            tvr.ValueBC = CurrentResult.Ratio[1];
            tvr.ValueCA = CurrentResult.Ratio[2];
        }
        #endregion

        #region 根据工作令计算显示分接
        private void WorkflowEventHandler(object sender, TestEventArgs e)
        {
            HandleWorkflowChange();
        }

        private void HandleWorkflowChange()
        {
            Dispatcher.Invoke(() =>
            {
                // 先清空已有的数据
                tbProductSequence.Text = Configs.Configs.WorkflowID;
                for (int i = 1; i <= MAX_HV_COUNT; i++)
                {
                    RatioValueFields[i.ToString()].TappingVoltage = 0;
                    RatioValueFields[i.ToString()].CalculatedRatio = 0;
                }
                RatioValueFields["21"].TappingVoltage = 0;
                RatioValueFields["21"].CalculatedRatio = 0;
                tbTapping.Text = "";
                tbRatedHighVoltage.Text = "";
                tbRatedLowVoltage.Text = "";
                tbRatedLowVoltage2.Text = "";
                tbCONNSymbol.Text = "";
            });
            Task.Run(() =>
            {
                var workflows = WorkflowInfo.ReadFromDB(Configs.Configs.WorkflowID);
                if (workflows == null || workflows.Count == 0)
                {
                    return;
                }
                var workflow = workflows[0];
                List<float> tappings = new List<float>();
                var voltages = workflow.TappingVoltages.Split(" ");
                foreach (var v in voltages)
                {
                    if (v == "NULL")
                    {
                        break;
                    }
                    tappings.Add(Utils.ParseFloat(v));
                }
                Dispatcher.Invoke(() =>
                {
                    for (int i = 1; i <= tappings.Count && i <= MAX_HV_COUNT; i++)
                    {
                        RatioValueFields[i.ToString()].TappingVoltage = tappings[tappings.Count - i];
                        RatioValueFields[i.ToString()].CalculatedRatio = tappings[tappings.Count - i] / workflow.RatedVoltageLv;
                    }
                    if (workflow.WorkflowType == "三绕组")
                    {
                        RatioValueFields["21"].TappingVoltage = tappings[tappings.Count - 1];
                        RatioValueFields["21"].CalculatedRatio = tappings[tappings.Count - 1] / workflow.RatedVoltageYv;
                    }
                    tbTapping.Text = workflow.RatedVoltageInterval;
                    tbRatedHighVoltage.Text = Utils.FloatFormat(workflow.RatedVoltageHv);
                    tbRatedLowVoltage.Text = Utils.FloatFormat(workflow.RatedVoltageLv);
                    tbRatedLowVoltage2.Text = Utils.FloatFormat(workflow.RatedVoltageYv);
                    tbCONNSymbol.Text = workflow.CONNSymbol;

                    tbTappingSpacing.Value = 2.5;
                    tbPositiveTappingCount.Value = tappings.Count;
                    tbRatedTapping.Value = (int)Math.Ceiling(tappings.Count / 2f);
                    tbHighVoltage.Value = workflow.RatedVoltageHv / 1000f;
                    tbLowVoltage.Value = workflow.RatedVoltageLv / 1000f;
                    cbHighVoltageConnection.Text = "D";
                    cbLowVoltageConnection.Text = "y";
                    tbGroup.Value = 11;
                });
            });
        }

        #region 计算分接电压
        private void CalculateTappingVoltages()
        {
            string tapping = tbTapping.Text.Trim();
            int? hv = Utils.ParseIntNull(tbRatedHighVoltage.Text);
            int? lv = Utils.ParseIntNull(tbRatedLowVoltage.Text);
            if (tapping.Length == 0 || hv == null)
            {
                return;
            }
            int id = 0;
            if ((id = tapping.IndexOf("+-")) < 0)
            {
                return;
            }
            var strs = tapping.Split("*");
            if (strs.Length != 2)
            {
                return;
            }
            var count = int.Parse(strs[0].Replace("+-", ""));
            var gap = float.Parse(strs[1].Replace("%", "")) * 0.01f;

            for (int i = count; i >= -count; i--)
            {
                float v = (float)hv * (1.0f + gap * i);
                if (i + count + 1 > MAX_HV_COUNT)
                {
                    return;
                }
                var maxKey = i + count + 1;
                string key = $"{maxKey}";
                RatioValueFields[key].TappingVoltage = v;
            }
            for (int i = (count * 2 + 1) + 1; i <= MAX_HV_COUNT; i++)
            {
                RatioValueFields[i.ToString()].TappingVoltage = 0;
            }
        }

        private void tbProductSequence_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }
            if (tbProductSequence.Text.Length == 0)
            {
                return;
            }
            var workflows = WorkflowInfo.ReadFromDB(tbProductSequence.Text);
            if (workflows != null && workflows.Count > 0)
            {
                Log.Info($"Workflow {workflows[0].TappingVoltages}");
                tbTapping.Text = workflows[0].RatedVoltageInterval;
                tbRatedHighVoltage.Text = workflows[0].RatedVoltageHv.ToString();
                tbRatedLowVoltage.Text = workflows[0].RatedVoltageLv.ToString();
            }
            else
            {
                tbTapping.Text = "";
                tbRatedHighVoltage.Text = "";
                tbRatedLowVoltage.Text = "";
            }
        }
        #endregion

        #endregion

        #region 数据上传
        private void btUpload_Click(object sender, System.EventArgs e)
        {
            if (!Utils.CheckWorkflowBeforeUpload())
            {
                return;
            }
            VoltageRatioInfo.DeleteData(Configs.Configs.WorkflowID);
            List<VoltageRatioInfo> list = new List<VoltageRatioInfo>();
            foreach (var key in RatioValueFields.Keys)
            {
                var item = RatioValueFields[key];
                var value = new VoltageRatioInfo()
                {
                    WorkflowId = Configs.Configs.WorkflowID,
                    TappingPosition = Utils.ParseInt(key),
                    TappingVoltage = item.TappingVoltage,
                    CalRatio = item.CalculatedRatio,
                    ErrorAB = item.ValueAB,
                    ErrorBC = item.ValueBC,
                    ErrorCA = item.ValueCA,
                };
                if (key == "1")
                {
                    value.ConnectionGroup1 = tbConnectionGroup1.Text;
                    value.ConnectionGroup2 = tbConnectionGroup2.Text;
                }
                list.Add(value);
            }
            var ret = VoltageRatioInfo.BatchInsertData(list);
            Utils.ShowUploadTips(ret);
        }
        #endregion
    }
}

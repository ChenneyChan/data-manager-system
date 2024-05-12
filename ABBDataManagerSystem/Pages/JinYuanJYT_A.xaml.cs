using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// JinYuanJYT_A.xaml 的交互逻辑
    /// </summary>
    public partial class JinYuanJYT_A : UserControl
    {
        private Dictionary<string, TappingVoltageRatioFields> RatioValueFields = new Dictionary<string, TappingVoltageRatioFields>();
        private JinYuanJYTACollector? Collector = null;
        private bool IsTesting = false;
        private int Interval = 200;

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
        }

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

        private TappingVoltageRatioFields? GetSelectedTVR()
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
            return RatioValueFields[index];
        }

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
                            while (IsTesting)
                            {
                                Thread.Sleep(this.Interval);
                                bool needReset = false;
                                var result = Collector.ReadPacket(ref needReset);
                                if (needReset)
                                {
                                }
                                Dispatcher.Invoke(new Action(() => { HandleResult(result); }));
                            }
                        }).Start();
                    }

                }).Start();
            }
        }

        private void HandleResult(JinYuanJYTACollector.JinYunJYTATestResult? result)
        {
            if (result == null)
            {
                return;
            }
            if (Collector == null)
            {
                tbDeviceState.Text = "设备未连接";
                return;
            }

            #region 状态信息
            foreach (var item in JinYuanJYTACollector.DeviceStateMap)
            {
                if (item.Value == Collector.deviceState)
                {
                    tbDeviceState.Text = item.Key;
                    break;
                }
            }

            foreach (var item in JinYuanJYTACollector.PowerCodeTypeMap)
            {
                if (item.Value == Collector.powerCode)
                {
                    tbDevicePowerInfo.Text = item.Key;
                    break;
                }
            }

            foreach (var item in JinYuanJYTACollector.TipInfoTypeMap)
            {
                if (item.Value == Collector.tipInfo)
                {
                    tbTipInfo.Text = item.Key;
                    break;
                }
            }
            #endregion

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
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ActiveSelectedTest();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Collector != null)
            {
                Collector.Disconnect();
                Collector = null;
            }
            IsTesting = false;
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
            if (cbTestMode.Text == "三相测试")
            {
                Collector.SetThreePhaseTest();
            }
            else
            {
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
            var tvr = GetSelectedTVR();
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
    }
}

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

        public JinYuanJYT_A()
        {
            InitializeComponent();
            InitRatioValues();
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (var item in ports)
            {
                cbSerialPort.Items.Add(item);
            }
            if (ports.Length > 0) { cbSerialPort.SelectedIndex = 0; }

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

        private void swConnect_CheckedChange(object sender, RoutedEventArgs e)
        {
            cbSerialPort.IsEnabled = swConnect.IsChecked != true;
            cbBoundRate.IsEnabled = swConnect.IsChecked != true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ActiveSelectedTest();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

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
    }
}

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
        public JinYuanJYT_A()
        {
            InitializeComponent();
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (var item in ports)
            {
                cbSerialPort.Items.Add(item);
            }
            if (ports.Length > 0) { cbSerialPort.SelectedIndex = 0; }
        }

        private void swConnect_CheckedChange(object sender, RoutedEventArgs e)
        {
            cbSerialPort.IsEnabled = swConnect.IsChecked != true;
            cbBoundRate.IsEnabled = swConnect.IsChecked != true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

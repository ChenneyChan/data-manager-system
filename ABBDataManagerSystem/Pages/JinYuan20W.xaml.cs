using ABBDataManagerSystem.Connector;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// JinYuan20W.xaml 的交互逻辑
    /// </summary>
    public partial class JinYuan20W : UserControl
    {
        private JinYuan20WCollector? collector = null;

        public JinYuan20W()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            foreach (var item in ports)
            {
                cbSerialPort.Items.Add(item);
            }
            cbSerialPort.SelectedIndex = ports.Length > 0 ? 0 : -1;

            foreach (var item in JinYuan20WCollector.Ch1CurrentValue)
            {
                cbHVCurrents.Items.Add($"{item}A");
            }
            cbHVCurrents.SelectedIndex = 0;

            foreach (var item in JinYuan20WCollector.Ch2CurrentValue)
            {
                cbLVCurrents.Items.Add($"{item}A");
            }
            cbLVCurrents.SelectedIndex = 0;

        }
    }
}

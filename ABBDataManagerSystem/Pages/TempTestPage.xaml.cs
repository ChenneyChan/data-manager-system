using ABBDataManagerSystem.Connector;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// TempTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TempTestPage : UserControl
    {
        private static bool Simulate = false;

        private bool UsingSerial = true;
        private List<TemperatureSlotView> Slots = new List<TemperatureSlotView>();

        private TempChartsNew tempCharts;

        private bool IsCollecting = false;

        private Random random = new Random();

        private string csvFilePath = string.Empty;

        private readonly int MaxSlotCount = 0;
        private StreamWriter? csvWriter = null;

        private TempModbusCollector? tempModbusCollector;
        private Thread? CollectThread = null;
        private int Interval = 200;
        private int SlotCount = 20;

        public TempTestPage()
        {
            InitializeComponent();
            InitView();
        }

        private void InitView()
        {
            var ports = SerialPort.GetPortNames();
            cbSerialPort.Items.Add(ports);
            cbSerialPort.SelectedIndex = 0;

            var baundRates = new List<string>()
            {
                "9600",
                "12800"
            };
            baundRates.ForEach(port => { cbSerialBaudRate.Items.Add(ports); });
            rbEthernet.IsChecked = false;
            rbSerialPort.IsChecked = true;
            rbEthernet.Checked += RbEthernet_Checked;
            rbSerialPort.Checked += RbSerialPort_Checked;
        }

        private void RbSerialPort_Checked(object sender, RoutedEventArgs e)
        {
            rbEthernet.IsChecked = false;
            rbSerialPort.IsChecked = true;
        }

        private void RbEthernet_Checked(object sender, RoutedEventArgs e)
        {
            rbEthernet.IsChecked = true;
            rbSerialPort.IsChecked = false;
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (rbSerialPort.IsChecked == true)
            {
                new TempModbusCollector
            }
        }
    }
}

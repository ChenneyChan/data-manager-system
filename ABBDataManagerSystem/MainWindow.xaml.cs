using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages;
using ABBDataManagerSystem.PowerAnalyzer;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ABBDataManagerSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Configs.Configs.LoadFromFile();
        }

        private void MenuItemSetting_Click(object sender, RoutedEventArgs e)
        {
            new WindowSettings().ShowDialog();
        }

        private void PowerAnalyzeTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window()
            {
                Title = "功率分析仪",
                Width = 500,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            var uc = new UCDeviceSearch();
            window.Content = uc;
            window.ShowDialog();
        }

        private void TemperatureTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window()
            {
                Title = "温度检测仪",
                Width = 1200,
                Height = 900,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            var uc = new TempTestPage();
            window.Content = uc;
            window.ShowDialog();
        }

        private void JnYuan20WTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window()
            {
                Title = "金源20W测试仪",
                Width = 1200,
                Height = 900,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            var uc = new JinYuan20W();
            window.Content = uc;
            window.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Configs.Configs.SaveToFile();
        }

        private void btStartSerialTest_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                var ports = SerialPort.GetPortNames();
                Array.Sort(ports);

                foreach (var port in ports)
                {
                    AppendMsg("Port " + port + " Start...");
                    var collector = new JinYun50ECollector(port, 9600);
                    if (collector.Connect())
                    {
                        AppendMsg("Connect Success!");
                        collector.SetTestType(JinYun50ECollector.TestType50E.CommonTest);
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

        private void AppendMsg(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                tbMsg.Text = tbMsg.Text + "\r\n" + msg;
            }));
        }
    }
}
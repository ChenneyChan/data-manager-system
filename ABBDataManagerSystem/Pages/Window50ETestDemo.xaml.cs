using ABBDataManagerSystem.Connector;
using System.Globalization;
using System.IO.Ports;
using System.Windows;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// Window50ETestDemo.xaml 的交互逻辑
    /// </summary>
    public partial class Window50ETestDemo : Window
    {
        JinYuan50ECollectorV1? Collector = null;
        bool IsCollecting = false;

        public Window50ETestDemo()
        {
            InitializeComponent();
            var ports = SerialPort.GetPortNames();
            foreach (var item in ports)
            {
                cbPorts.Items.Add(item);
            }
            cbPorts.SelectedIndex = 0;

            var currents = JinYuan50ECollectorV1.EnumHelper.currentMap.Values;
            foreach (var current in currents)
            {
                cbCurrents.Items.Add(current);
            }
            cbCurrents.SelectedIndex = 0;

            cbTestMode.Items.Add("单通道");
            cbTestMode.Items.Add("双通道");
            cbTestMode.SelectedIndex = 0;
        }

        private void btStartConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCollecting)
            {
                Collector = new JinYuan50ECollectorV1(cbPorts.Text);
                if (!Collector.Connect())
                {
                    Collector = null;
                }
                else
                {
                    btStartConnect.Content = "断开";
                    IsCollecting = true;
                    StartRequestTask();
                }
            }
            else
            {
                IsCollecting = false;
                Collector?.Disconnect();
                Collector = null;
                btStartConnect.Content = "连接";
            }
        }

        private void StartRequestTask()
        {
            Task.Run(() =>
            {
                while (IsCollecting && Collector != null)
                {
                    var packet = Collector.ReadPacket();
                    if (packet != null)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            btStatus.Text = $"Device Status = {Collector.DeviceStatus}\r\nTestStatus = {Collector.DeviceTestStatus}";
                            btPacketDebug.Text = DateTime.Now.ToString("HH:mm:ss") + "\r\n" + packet.ToString();
                        }));
                    }
                    Thread.Sleep(400);
                }
            });
        }

        private void btGotoCommon_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendCommonTest();
        }

        private void btSetCommonParams_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.CurrentMode = JinYuan50ECollectorV1.EnumHelper.GetCurrentEnum(cbCurrents.Text);
            Collector.PatternMode = JinYuan50ECollectorV1.EnumHelper.GetPatternEnum(cbTestMode.Text);
            Collector.Mode = JinYuan50ECollectorV1.TestType50E.Normal;
            Collector.SendParameterSetCommand();
        }

        private void btStartCommonTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendStartCommonTest();
        }

        private void btStopCommonTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendStopCommandAtNormal();
        }

        private void btExitCommonTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendExitCommandAtNormal();
        }

        private void btResetAtCommon_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.ResetDevice();
        }

        private void btGotoTempRise_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendTempRiseTest();
        }

        private void btSetTempRiseParams10s_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.CurrentMode = JinYuan50ECollectorV1.EnumHelper.GetCurrentEnum(cbCurrents.Text);
            Collector.PatternMode = JinYuan50ECollectorV1.EnumHelper.GetPatternEnum(cbTestMode.Text);
            Collector.Mode = JinYuan50ECollectorV1.TestType50E.TemperatureRise10Sec;
            Collector.SendParameterSetCommand();
        }

        private void btSetTempRiseParams30s_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.CurrentMode = JinYuan50ECollectorV1.EnumHelper.GetCurrentEnum(cbCurrents.Text);
            Collector.PatternMode = JinYuan50ECollectorV1.EnumHelper.GetPatternEnum(cbTestMode.Text);
            Collector.Mode = JinYuan50ECollectorV1.TestType50E.TemperatureRise30Sec;
            Collector.SendParameterSetCommand();
        }

        private void btSetTempRiseParams60s_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.CurrentMode = JinYuan50ECollectorV1.EnumHelper.GetCurrentEnum(cbCurrents.Text);
            Collector.PatternMode = JinYuan50ECollectorV1.EnumHelper.GetPatternEnum(cbTestMode.Text);
            Collector.Mode = JinYuan50ECollectorV1.TestType50E.TemperatureRise60Sec;
            Collector.SendParameterSetCommand();
        }

        private void btStartTempRiseTiming_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendTempRiseTestStartTiming();
        }

        private void btStartTempRiseTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendTempRiseStartTest();
        }

        private void btStopTempRiseTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendTempRiseStopCommand();
        }

        private void btExitTempRiseTest_Click(object sender, RoutedEventArgs e)
        {
            if (Collector == null) { return; }
            Collector.SendTempRiseExitCommand();
        }

        public static byte[] ParseHexString(string hexString)
        {
            // 分割字符串以提取每个十六进制数字
            string[] hexValues = hexString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // 将每个十六进制字符串转换为 byte 并返回数组
            return hexValues.Select(hex => byte.Parse(hex.Substring(2), NumberStyles.HexNumber)).ToArray();
        }

        private void btSendCommand_Click(object sender, RoutedEventArgs e)
        {
            if (tbCommandBytes.Text.Trim().Length > 0)
            {
                var msg = tbCommandBytes.Text.Trim();
                var command = ParseHexString(msg);
                if (command.Length > 0)
                {
                    Collector?.SendCommond(command);
                }
            }
        }
    }
}

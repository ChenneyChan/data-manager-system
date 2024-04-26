using ABBDataManagerSystem.Charts;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using Microsoft.Win32;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// TempTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TempTestPage : UserControl
    {
        private static bool Simulate = true;

        private bool UsingSerial = true;
        private List<TemperatureSlotView> Slots = new List<TemperatureSlotView>();

        private TempChartsNew tempCharts;

        private bool IsCollecting = false;

        private Random random = new Random();

        private string csvFilePath = string.Empty;

        private StreamWriter? csvWriter = null;

        private TempModbusCollector? tempModbusCollector;
        private ManualResetEvent? ResetEvent = null;
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
            Array.Sort(ports);
            ports.ToList().ForEach(port => { cbSerialPort.Items.Add(port); });
            cbSerialPort.SelectedIndex = cbSerialPort.Items.Count > 0 ? 0 : -1;

            var baundRates = new List<string>()
            {
                "9600",
                "12800"
            };
            baundRates.ForEach(rate => { cbSerialBoudRate.Items.Add(rate); });
            for (var i = 1; i < 36; i++)
            {
                cbSlotNums.Items.Add(i.ToString());
            }
            cbSlotNums.SelectedIndex = 10;

            var intervals = new List<string>()
            {
                "1s",
                "5s",
                "30s",
                "1min",
                "5min",
                "30min",
            };
            intervals.ForEach(port => { cbInterval.Items.Add(port); });
            cbInterval.SelectedIndex = 0;

            rbEthernet.IsChecked = false;
            rbSerialPort.IsChecked = true;
            rbEthernet.Checked += RbEthernet_Checked;
            rbSerialPort.Checked += RbSerialPort_Checked;

            cbInterval.SelectedIndex = 1;
            cbInterval.SelectionChanged += CbInterval_SelectedIndexChanged;
            cbSlotNums.SelectionChanged += CbSlotNum_SelectedIndexChanged;
            UpdateByConfig();
            UpdateInterval();
        }

        private void RbSerialPort_Checked(object sender, RoutedEventArgs e)
        {
            UsingSerial = true;
            HandleConectionTypeChange();
        }

        private void RbEthernet_Checked(object sender, RoutedEventArgs e)
        {
            UsingSerial = false;
            HandleConectionTypeChange();
        }

        private void HandleConectionTypeChange()
        {
            rbEthernet.IsChecked = !UsingSerial;
            rbSerialPort.IsChecked = UsingSerial;

            cbSerialPort.IsEnabled = UsingSerial;
            cbSerialBoudRate.IsEnabled = UsingSerial;
            tbEthernetIP.IsEnabled = !UsingSerial;
            tbEthernetPort.IsEnabled = !UsingSerial;
        }

        private void ToogleAllStatus()
        {
            rbEthernet.IsEnabled = !IsCollecting;
            rbSerialPort.IsEnabled = !IsCollecting;
            cbSlotNums.IsEnabled = !IsCollecting;
            cbInterval.IsEnabled = !IsCollecting;

            if (IsCollecting)
            {
                cbSerialPort.IsEnabled = false;
                cbSerialBoudRate.IsEnabled = false;
                tbEthernetIP.IsEnabled = false;
                tbEthernetPort.IsEnabled = false;
            } 
            else
            {
                HandleConectionTypeChange();
            }
        }

        #region 从Winform拷贝的代码


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateByConfig();
            InitSlot();
            InitChartRange();
            tempCharts = new TempChartsNew(plotView, SlotCount, SlotCount);
            tempCharts.InitChart();
        }

        private void CbSlotNum_SelectedIndexChanged(object? sender, RoutedEventArgs e)
        {
            InitSlot();
            if (tempCharts != null)
            {
                tempCharts.SetEnableSlotCount(SlotCount);
            }
        }

        private void InitSlot()
        {
            SlotWrapPanel.Children.Clear();
            Slots.Clear();

            if (cbSlotNums.SelectedIndex < 0 || cbSlotNums.SelectedItem == null)
            {
                SlotCount = 0;
                return;
            }
            SlotCount = Utils.ParseInt(cbSlotNums.SelectedItem.ToString());
            if (SlotCount >= Slots.Count)
            {
                for (int i = Slots.Count; i < SlotCount; i++)
                {
                    TemperatureSlotView uc = new TemperatureSlotView()
                    {
                        Slot = i + 1,
                        Temperature = -200f,
                        Margin = new Thickness(5),
                        Width = 80,
                        Height = 80,
                    };
                    SlotWrapPanel.Children.Add(uc);
                    Slots.Add(uc);
                }
            }
            else
            {
                Slots.RemoveRange(SlotCount, Slots.Count - SlotCount);
            }
            ResizeChartView();
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (IsCollecting)
            {
                var ret = MessageBox.Show("停止采集？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (ret == MessageBoxResult.No)
                {
                    return;
                }
                IsCollecting = false;
                if (ResetEvent != null)
                {
                    ResetEvent.Set();
                }
                btStart.Content = "启动";
                btStart.Background = Brushes.DodgerBlue;
                btStart.Foreground = Brushes.White;
                ToogleAllStatus();
                return;
            }
            if (ResetEvent != null)
            {
                return;
            }
            SaveConfig();
            bool needSaveCsv = false;
            if (tbSaveFilePath.Text.Length == 0)
            {
                needSaveCsv = SelectOpenFile();
            }
            if (!Simulate)
            {
                if (rbEthernet.IsChecked == true)
                {
                    tempModbusCollector = new TempModbusCollector(tbEthernetIP.Text, Utils.ParseInt(tbEthernetPort.Text), true);
                }
                else
                {
                    tempModbusCollector = new TempModbusCollector(cbSerialPort.Text, Utils.ParseInt(cbSerialBoudRate.Text));
                }
                bool ret = tempModbusCollector.Connect();
                if (!ret)
                {
                    MessageBox.Show("设备连接失败，请检查配置！", "连接失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (tempCharts == null || tempCharts.GetMaxSlotCount() != SlotCount)
            {
                tempCharts = new TempChartsNew(plotView, SlotCount, SlotCount);
                tempCharts.InitChart();
            }
            if (needSaveCsv)
            {
                StartCSVFile();
            }
            IsCollecting = true;
            btStart.Content = "停止";
            btStart.Background = Brushes.Red;
            btStart.Foreground = Brushes.White;
            ToogleAllStatus();

            // 创建一个ManualResetEvent，初始状态为未设置（false）  
            ResetEvent = new ManualResetEvent(false);

            // 创建一个新线程并执行任务  
            Task.Run(() =>
            {
                IsCollecting = true;
                while (IsCollecting)
                {
                    CollectDataOnce();
                    if (ResetEvent.WaitOne(Interval))
                    {
                        // 线程没有超时被唤醒，说明要停止循环了
                    }
                }
                tempModbusCollector?.Disconnect();
                tempModbusCollector = null;
                StopCSVFile();
                ResetEvent = null;
                Log.Info("Temp collector DONE");
            });
        }

        private void Destroy()
        {
            if (IsCollecting)
            {
                IsCollecting = false;
            }
        }

        private bool SelectOpenFile()
        {
            // 获取当前时间  
            DateTime now = DateTime.Now;
            // 格式化时间，您可以选择任何喜欢的格式  
            string fileName = $"temperature_records_{now:yyyydMMddHHmmss}.csv";
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = fileName,
                Title = "选择文件存储路径",
                Filter = "CSV files (*.csv)|*.csv"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                tbSaveFilePath.Text = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        private void tableLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            ResizeChartView();
        }

        private void ResizeChartView()
        {
            // 假设你的TableLayoutPanel的名字是tableLayoutPanel1  
            //int lastRowIndex = tableLayoutPanel1.RowCount - 1;
            //var lastRowStyle = tableLayoutPanel1.RowStyles[lastRowIndex];
            ////var lastRowHeight = lastRowStyle.Height;
            //panelFormChart.Height = tableLayoutPanel1.Height - panelFormChart.Location.Y - panelFormChart.Margin.Bottom;
            //Log.Info($"plotView1 location {panelFormChart.Location.ToString()} size {panelFormChart.Size}");
            //Log.Info($"tableLayout location {tableLayoutPanel1.Location.ToString()} size {tableLayoutPanel1.Size}");
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            var ret = MessageBox.Show("确定清空数据？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (ret == MessageBoxResult.No)
            {
                return;
            }
            tempCharts.ClearRecords();
        }

        private void HandleRecords(float[] values)
        {
            plotView.Dispatcher.InvokeAsync(new Action(() =>
            {
                tempCharts.AddRecords(values);
                for (int i = 0; i < Slots.Count; i++)
                {
                    if (i < values.Length)
                    {
                        Slots[i].Temperature = values[i];
                    }
                    else
                    {
                        Slots[i].Temperature = -200f;
                    }
                }
            }));
        }

        private void UpdateInterval()
        {
            if (cbInterval.SelectedItem == null)
            {
                return;
            }
            string value = cbInterval.SelectedItem.ToString();
            if (value == null)
            {
                return;
            }
            if (value.IndexOf("ms") >= 0)
            {
                int interval = int.Parse(value.Split("ms")[0]);
                Interval = interval;
            }
            else if (value.IndexOf("s") >= 0)
            {
                int interval = int.Parse(value.Split("s")[0]) * 1000;
                Interval = interval;
            }
            else if (value.IndexOf("min") >= 0)
            {
                int interval = int.Parse(value.Split("min")[0]) * 1000 * 60;
                Interval = interval;
            }
        }

        private void CollectDataOnce() // todo: 改成子线程读取，避免UI线程阻塞
        {
            float[] values;
            if (Simulate || tempModbusCollector == null)
            {
                values = new float[SlotCount];
                for (int i = 0; i < SlotCount; i++)
                {
                    int value = random.Next(0, 10) + 10 * i;
                    values[i] = value;
                }
            }
            else
            {
                string msg;
                var listValues = tempModbusCollector.ReadData(SlotCount, out msg);
                values = listValues.ToArray();
            }
            if (values.Length == 0)
            {
                return;
            }
            HandleRecords(values);
            WriteCSVFile(values);
        }

        private void CbInterval_SelectedIndexChanged(object? sender, RoutedEventArgs e)
        {
            UpdateInterval();
        }

        private void btSaveToPng_Click(object sender, RoutedEventArgs e)
        {

            // 获取当前时间  
            DateTime now = DateTime.Now;
            // 格式化时间，您可以选择任何喜欢的格式  
            string fileName = $"temperature_charts_{now:yyyydMMddHHmmss}.png";
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = fileName,
                Filter = "PNG files (*.png)|*.png",
                Title = "导出当前图表",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                tempCharts.ExportToPng(saveFileDialog.FileName);
            }
        }

        private void btResteAxis_Click(object sender, RoutedEventArgs e)
        {
            plotView.Model.ResetAllAxes();
            plotView.Model.InvalidatePlot(false);
        }

        private void btToogleLegends_Click(object sender, RoutedEventArgs e)
        {
            tempCharts.ToogleLegends();
        }

        private void StartCSVFile()
        {
            // 定义要写入CSV文件的数据  
            string[] titles = new string[SlotCount + 1];
            for (int i = 0; i < titles.Length; i++)
            {
                titles[i] = $"Slot{i + 1}";
            }
            titles[SlotCount] = "Time";

            // 定义CSV文件路径和文件名  
            csvFilePath = tbSaveFilePath.Text.Trim();

            // 创建CSV文件并写入数据  
            csvWriter = new StreamWriter(csvFilePath, true, Encoding.UTF8);
            csvWriter.WriteLine(string.Join(",", titles)); // 使用逗号分隔每个字段并写入行  
        }

        private static int MAX_FLUSH_COUNT = 20;
        private int flushDelay = MAX_FLUSH_COUNT;

        private void WriteCSVFile(float[] data)
        {
            if (data.Length == 0 || csvWriter == null) return;
            string[] values = new string[SlotCount + 1];
            for (int i = 0; i < SlotCount && i < data.Length; i++)
            {
                values[i] = data[i].ToString("0.0");
            }
            values[SlotCount] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            csvWriter.WriteLine(string.Join(",", values)); // 使用逗号分隔每个字段并写入行
            flushDelay--;
            if (flushDelay == 0)
            {
                flushDelay = MAX_FLUSH_COUNT;
                csvWriter.Flush();
            }
        }

        private void StopCSVFile()
        {
            csvWriter?.Flush();
            csvWriter?.Close();
            csvWriter = null;
        }

        #region 表格X轴时间范围设置
        private void InitChartRange()
        {
            //cbChartRange.Items.Clear();
            //cbChartRange.Items.AddRange(new string[]
            //{
            //    "显示全部",
            //    "显示最近一小时",
            //    "显示最近30分钟",
            //    "显示最近5分钟",
            //});
            //cbChartRange.SelectedIndex = 0;
            //cbChartRange.SelectedIndexChanged += CbChartRange_SelectedIndexChanged;
        }

        private void CbChartRange_SelectedIndexChanged(object? sender, RoutedEventArgs e)
        {
            //switch (cbChartRange.SelectedIndex)
            //{
            //    case 0:
            //        tempCharts.SetDateTimeAxisRange(-1);
            //        break;
            //    case 1:
            //        tempCharts.SetDateTimeAxisRange(60);
            //        break;
            //    case 2:
            //        tempCharts.SetDateTimeAxisRange(30);
            //        break;
            //    case 3:
            //        tempCharts.SetDateTimeAxisRange(5);
            //        break;
            //}
        }
        #endregion

        private void tbHideAllLine_Click(object sender, RoutedEventArgs e)
        {
            tempCharts.HideAllLines();
        }

        private void btResumeLines_Click(object sender, RoutedEventArgs e)
        {
            tempCharts.ResuneAllLines();
        }

        private void UpdateByConfig()
        {
            if (UsingSerial)
            {
                if (Configs.Configs.TPSerialPort.Length > 0)
                {
                    int index = cbSerialPort.Items.IndexOf(Configs.Configs.TPSerialPort);
                    if (index != -1)
                    {
                        cbSerialPort.SelectedIndex = index;
                    }
                }
                if (Configs.Configs.TPSerialBoundRate.Length > 0)
                {
                    int index = cbSerialBoudRate.Items.IndexOf(Configs.Configs.TPSerialBoundRate);
                    if (index != -1)
                    {
                        cbSerialBoudRate.SelectedIndex = index;
                    }
                }
            }
            else
            {
                if (Configs.Configs.TPIPAddress.Length > 0)
                {
                    tbEthernetIP.Text = Configs.Configs.TPIPAddress;
                }
                if (Configs.Configs.TPPort != 0)
                {
                    tbEthernetPort.Text = Configs.Configs.TPPort.ToString();
                }
            }

            if (Configs.Configs.TPInterval.Length > 0)
            {
                int index = cbInterval.Items.IndexOf(Configs.Configs.TPInterval);
                if (index != -1)
                {
                    cbInterval.SelectedIndex = index;
                }
            }
            if (Configs.Configs.TPSlotNum > 0)
            {
                int index = cbSlotNums.Items.IndexOf(Configs.Configs.TPSlotNum.ToString());
                if (index != -1)
                {
                    cbSlotNums.SelectedIndex = index;
                }
            } 
        }

        private void SaveConfig()
        {
            Configs.Configs.TPUsingSerialPort = UsingSerial ? 1 : 0;
            if (UsingSerial)
            {
                Configs.Configs.TPSerialPort = cbSerialPort.Text;
                Configs.Configs.TPSerialBoundRate = cbSerialBoudRate.Text;
            }
            else
            {
                Configs.Configs.TPIPAddress = tbEthernetIP.Text;
                Configs.Configs.TPPort = Utils.ParseInt(tbEthernetPort.Text);
            }
            Configs.Configs.TPSlotNum = Utils.ParseInt(cbSlotNums.SelectedItem.ToString());
            Configs.Configs.TPInterval = cbInterval.Text;
        }


        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Destroy();
        }
    }
}

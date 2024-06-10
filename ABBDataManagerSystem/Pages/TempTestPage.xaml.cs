using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Charts;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using ABBDataManagerSystem.Tools;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ABBDataManagerSystem.Pages
{
    class VoltageInfo
    {
        public float ua;
        public float ub;
        public float uc;
        public float u3;

        public float ia;
        public float ib;
        public float ic;
        public float i3;

        public float p3;

        public bool IsUpdated = false;
    }
    /// <summary>
    /// TempTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TempTestPage : UserControl, ICloseable
    {
        private static bool Simulate = false;
        private bool IsFirstLoad = true;

        private bool UsingSerial = true;
        private List<TemperatureSlotView> Slots = new List<TemperatureSlotView>();

        private TempChartsNew tempCharts;

        private bool IsCollecting = false;

        private Random random = new Random();

        private string csvFilePath = string.Empty;

        private StreamWriter? csvWriter = null;
        private List<int> SelectedSlots = new List<int>();
        private bool SelectedSlotChange = false;

        private TempModbusCollector? tempModbusCollector;
        private ManualResetEvent? ResetEvent = null;
        private int Interval = 200;
        private int SlotCount = 20;
        private static readonly int MaxSlotCount = 36;
        private DataTable Table = new DataTable();
        private VoltageInfo CurrentVoltageInfo = new();
        private Object objLock = new object();

        public TempTestPage()
        {
            InitializeComponent();
            InitView();
            List<string> SlotChoices = new List<string>();
            for (int i = 1; i <= MaxSlotCount; i++)
            {
                SlotChoices.Add("通道-" + i.ToString());
            }
            this.DataContext = new { DataList = SlotChoices, Table };
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

            var intervals = new List<string>()
            {
                "1s",
                "5s",
                "30s",
                "1min",
                "5min",
                "30min",
                "60min",
            };
            intervals.ForEach(port => { cbInterval.Items.Add(port); });
            cbInterval.SelectedIndex = 0;

            rbEthernet.IsChecked = false;
            rbSerialPort.IsChecked = true;
            rbEthernet.Checked += RbEthernet_Checked;
            rbSerialPort.Checked += RbSerialPort_Checked;

            cbInterval.SelectedIndex = 1;
            cbInterval.SelectionChanged += CbInterval_SelectedIndexChanged;
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
            cbInterval.IsEnabled = !IsCollecting;
            cbTestType.IsEnabled = !IsCollecting;
            cbTestStatus.IsEnabled = !IsCollecting;
            btSelectSlots.IsEnabled = !IsCollecting;

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
            if (!IsFirstLoad)
            {
                return;
            }
            IsFirstLoad = false;
            UpdateByConfig();
            InitSlot();
            InitChartRange();
            tempCharts = new TempChartsNew(plotView, SelectedSlots);
            tempCharts.InitChart();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //Close();
        }

        private void InitSlot()
        {
            SlotWrapPanel.Children.Clear();
            Slots.Clear();
            SlotCount = ProcessSelectedSlots();
            if (SlotCount == 0) return;
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
            for (int i = 0; i < Slots.Count && i < SlotCount; i++)
            {
                var uc = Slots[i];
                uc.Slot = SelectedSlots[i];
            }
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
            if (SelectedSlots.Count == 0)
            {
                MessageBox.Show("请至少选择一个通道", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            SaveConfig();
            bool needSaveCsv = false;
            if (tbSaveFilePath.Text.Length == 0)
            {
                needSaveCsv = SelectOpenFile();
            }
            InitDataGrid();
            if (!Simulate)
            {
                if (rbEthernet.IsChecked == true)
                {
                    //tempModbusCollector = new TempModbusCollector(tbEthernetIP.Text, Utils.ParseInt(tbEthernetPort.Text), true);
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

            if (tempCharts == null || SelectedSlotChange)
            {
                SelectedSlotChange = false;
                tempCharts = new TempChartsNew(plotView, SelectedSlots);
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
                Log.Info("Temp Collector DONE");
            });
        }

        public void Close()
        {
            if (IsCollecting)
            {
                IsCollecting = false;
            }
            if (ResetEvent != null)
            {
                ResetEvent.Set();
            }
            Tools.EventManager.Instance.Unsubscribe<TestEventArgs>("PowerAnalyzer", EventHandler);
        }

        #region 图表相关操作
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

        private void tbHideAllLine_Click(object sender, RoutedEventArgs e)
        {
            tempCharts.HideAllLines();
        }

        private void btResumeLines_Click(object sender, RoutedEventArgs e)
        {
            tempCharts.ResuneAllLines();
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
        #endregion

        #region 记录表格相关操作
        private void InitDataGrid()
        {
            dgTempRecord.ItemsSource = null;
            dgTempRecord.Columns.Clear();
            Table.Rows.Clear();
            Table.Columns.Clear();

            dgTempRecord.Columns.Add(new DataGridTextColumn
            {
                Header = "时间",
                Binding = new Binding("时间")
                {
                    StringFormat = "yyyy-MM-dd HH:mm:ss"
                }
            });

            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "Ua",
                Binding = new Binding("Ua"),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "Ub",
                Binding = new Binding("Ub"),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "Uc",
                Binding = new Binding("Uc"),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "U3",
                Binding = new Binding("U3"),
                MinWidth = 40
            });

            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "Ia",
                Binding = new Binding("Ia"),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "Ib",
                Binding = new Binding("Ib"),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "Ic",
                Binding = new Binding("Ic"),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "I3",
                Binding = new Binding("I3"),
                MinWidth = 40
            });

            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = "P3",
                Binding = new Binding("P3"),
                MinWidth = 40
            });

            Table.Columns.Add("时间", typeof(DateTime));
            Table.Columns.Add("Ua", typeof(float));
            Table.Columns.Add("Ub", typeof(float));
            Table.Columns.Add("Uc", typeof(float));
            Table.Columns.Add("U3", typeof(float));
            Table.Columns.Add("Ia", typeof(float));
            Table.Columns.Add("Ib", typeof(float));
            Table.Columns.Add("Ic", typeof(float));
            Table.Columns.Add("I3", typeof(float));
            Table.Columns.Add("P3", typeof(float));

            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"绕组A",
                Binding = new Binding(Configs.Configs.WindingA)
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"绕组B",
                Binding = new Binding(Configs.Configs.WindingB)
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"绕组C",
                Binding = new Binding(Configs.Configs.WindingC)
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"铁心",
                Binding = new Binding(Configs.Configs.Core)
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"环境A",
                Binding = new Binding(Configs.Configs.EnvA)
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"环境B",
                Binding = new Binding(Configs.Configs.EnvB)
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"环境C",
                Binding = new Binding(Configs.Configs.EnvC)
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"环境D",
                Binding = new Binding(Configs.Configs.EnvD)
            });
            for (int i = 0; i < SelectedSlots.Count; i++)
            {
                var slot = SelectedSlots[i];
                Table.Columns.Add($"Slot-{slot}", typeof(float));
            }

            dgTempRecord.AutoGenerateColumns = false;
            dgTempRecord.ItemsSource = Table.DefaultView;

            Tools.EventManager.Instance.Subscribe<TestEventArgs>("PowerAnalyzer", EventHandler);
        }

        private void UpdateDataGrid(float[] values)
        {
            DataRow newRow = Table.NewRow();
            newRow["时间"] = DateTime.Now;
            for (int i = 0; i < SelectedSlots.Count && i < values.Length; i++)
            {
                var slot = SelectedSlots[i];
                newRow[$"Slot-{slot}"] = values[i];
            }
            lock (objLock)
            {
                if (CurrentVoltageInfo.IsUpdated)
                {
                    CurrentVoltageInfo.IsUpdated = false;
                    newRow["ua"] = CurrentVoltageInfo.ua;
                    newRow["ub"] = CurrentVoltageInfo.ub;
                    newRow["uc"] = CurrentVoltageInfo.uc;
                    newRow["u3"] = CurrentVoltageInfo.u3;
                    newRow["ia"] = CurrentVoltageInfo.ua;
                    newRow["ib"] = CurrentVoltageInfo.ub;
                    newRow["ic"] = CurrentVoltageInfo.uc;
                    newRow["i3"] = CurrentVoltageInfo.u3;
                    newRow["p3"] = CurrentVoltageInfo.p3;
                }
            }
            Dispatcher.Invoke(() =>
            {
                Table.Rows.Add(newRow);
            });
        }

        private void EventHandler(object sender, TestEventArgs e)
        {
            if (e.obj == null) { return; }
            var info = e.obj as VoltageCurrentLossDataInfo;
            if (info != null)
            {
                lock (objLock)
                {
                    CurrentVoltageInfo.ua = info.ua ?? 0;
                    CurrentVoltageInfo.ub = info.ub ?? 0;
                    CurrentVoltageInfo.uc = info.uc ?? 0;
                    CurrentVoltageInfo.u3 = info.u3 ?? 0;

                    CurrentVoltageInfo.ia = info.ia ?? 0;
                    CurrentVoltageInfo.ib = info.ib ?? 0;
                    CurrentVoltageInfo.ic = info.ic ?? 0;
                    CurrentVoltageInfo.i3 = info.i3 ?? 0;

                    CurrentVoltageInfo.p3 = info.p3 ?? 0;
                    CurrentVoltageInfo.IsUpdated = true;
                }
            }
        }
        #endregion

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
            float[] values = new float[SlotCount];
            if (Simulate || tempModbusCollector == null)
            {
                for (int i = 0; i < SlotCount; i++)
                {
                    int value = random.Next(0, 10) + 10 * i;
                    values[i] = value;
                }
            }
            else
            {
                string msg;
                int maxSelectedSlotIndex = SelectedSlots[SelectedSlots.Count - 1];
                var listValues = tempModbusCollector.ReadData(maxSelectedSlotIndex + 1, out msg);
                for (int i = 0; i < SlotCount; i++)
                {
                    if (SelectedSlots[i] >= listValues.Count)
                    {
                        break;
                    }
                    values[i] = listValues[SelectedSlots[i]];
                }
            }
            if (values.Length == 0)
            {
                return;
            }
            HandleRecords(values);
            WriteCSVFile(values);
            UpdateDataGrid(values);
        }

        private void CbInterval_SelectedIndexChanged(object? sender, RoutedEventArgs e)
        {
            UpdateInterval();
        }

        #region 数据写入CSV
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

        private void StartCSVFile()
        {
            // 定义要写入CSV文件的数据  
            string[] titles = new string[SlotCount + 1];
            for (int i = 0; i < SelectedSlots.Count; i++)
            {
                titles[i] = $"Slot{SelectedSlots[i]}";
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
        #endregion

        #region 配置读写
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
            if (Configs.Configs.TPSlots.Length > 0)
            {
                UpdateSlotsMappingDisplay();
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
            string slots = "";
            foreach (var slot in SelectedSlots)
            {
                slots += $"{slot},";
            }
            Configs.Configs.TPSlots = slots;
            Configs.Configs.TPInterval = cbInterval.Text;
        }
        #endregion

        #endregion

        #region 温度槽位选择
        private void SelectedSlots_SelectionChanged()
        {
            InitSlot();
            SelectedSlotChange = true;
            UpdateSlotsMappingDisplay();
        }

        private int ProcessSelectedSlots()
        {
            SelectedSlots.Clear();
            List<string> slots = new List<string>();
            slots.Add(Configs.Configs.WindingA);
            slots.Add(Configs.Configs.WindingB);
            slots.Add(Configs.Configs.WindingC);
            slots.Add(Configs.Configs.Core);
            slots.Add(Configs.Configs.EnvA);
            slots.Add(Configs.Configs.EnvB);
            slots.Add(Configs.Configs.EnvC);
            slots.Add(Configs.Configs.EnvD);
            foreach (var item in slots)
            {
                try
                {
                    if (item.Length > 0 && item.IndexOf("-") >= 0)
                    {
                        var slotIndex = item.Split("-")[1];
                        SelectedSlots.Add(Utils.ParseInt(slotIndex));
                    }
                }
                catch { }
            }
            SelectedSlots.Sort();
            SelectedSlots = SelectedSlots.Distinct().ToList();
            return SelectedSlots.Count;
        }

        private void btSelectSlots_Click(object sender, RoutedEventArgs e)
        {
            var selectDialog = new TempSlotSelectView(MaxSlotCount) { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            if (selectDialog.ShowDialog() == true)
            {
                SelectedSlots_SelectionChanged();
            }
        }

        private void UpdateSlotsMappingDisplay()
        {
            string msg = "";
            msg += $"绕组A - {Configs.Configs.WindingA}, "; 
            msg += $"绕组B - {Configs.Configs.WindingB}, "; 
            msg += $"绕组C - {Configs.Configs.WindingC}, "; 
            msg += $"铁心 - {Configs.Configs.Core}, "; 
            msg += $"环境1 - {Configs.Configs.EnvA}, "; 
            msg += $"环境2 - {Configs.Configs.EnvB}, "; 
            msg += $"环境3 - {Configs.Configs.EnvC}, "; 
            msg += $"环境4 - {Configs.Configs.EnvD}, "; 
            tbSlotMappingShow.Text = msg;
            shWindingA.Status = Configs.Configs.WindingA;
            shWindingB.Status = Configs.Configs.WindingB;
            shWindingC.Status = Configs.Configs.WindingC;
            shCore.Status = Configs.Configs.Core;
            shEnvA.Status = Configs.Configs.EnvA;
            shEnvB.Status = Configs.Configs.EnvB;
            shEnvC.Status = Configs.Configs.EnvC;
            shEnvD.Status = Configs.Configs.EnvD;
        }
        #endregion
    }
}

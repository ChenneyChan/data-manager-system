using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Charts;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using ABBDataManagerSystem.Tools;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;
using System.Net;
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
        public float fu;

        public bool IsUpdated = false;
    }

    public class CoolDeviceInfo
    {
        /// <summary>
        /// 出水口温度
        /// </summary>
        public float OutletWaterTemperature { get; set; }

        /// <summary>
        /// 回水口温度
        /// </summary>
        public float InletWaterTemperature { get; set; }

        /// <summary>
        /// 环境温度1
        /// </summary>
        public float AmbientTemperature1 { get; set; }

        /// <summary>
        /// 环境温度2
        /// </summary>
        public float AmbientTemperature2 { get; set; }

        /// <summary>
        /// 出风口温度1
        /// </summary>
        public float OutletAirTemperature1 { get; set; }

        /// <summary>
        /// 出风口温度2
        /// </summary>
        public float OutletAirTemperature2 { get; set; }

        /// <summary>
        /// 出风口温度3
        /// </summary>
        public float OutletAirTemperature3 { get; set; }

        /// <summary>
        /// 出风口温度4
        /// </summary>
        public float OutletAirTemperature4 { get; set; }

        /// <summary>
        /// 水流量
        /// </summary>
        public float WaterFlowRate { get; set; }

        public bool IsUpdated { get; set; } = false;

        public override string ToString()
        {
            return $"OutletWaterTemperature: {OutletWaterTemperature}, InletWaterTemperature: {InletWaterTemperature}, " +
                   $"AmbientTemperature1: {AmbientTemperature1}, AmbientTemperature2: {AmbientTemperature2}, " +
                   $"OutletAirTemperature1: {OutletAirTemperature1}, OutletAirTemperature2: {OutletAirTemperature2}, " +
                   $"OutletAirTemperature3: {OutletAirTemperature3}, OutletAirTemperature4: {OutletAirTemperature4}, " +
                   $"WaterFlowRate: {WaterFlowRate}";
        }
    }



    /// <summary>
    /// TempTestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TempTestPage : UserControl, ICloseable
    {
        private static bool Simulate = Configs.Configs.TPIsSimulate;
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
        private int Interval = 500;
        private int SlotCount = 20;
        private int RecordInterval = 1;
        private DateTime LastRecordTime = DateTime.Now;
        private static readonly int MaxSlotCount = 36;
        private DataTable Table = new DataTable();
        private VoltageInfo CurrentVoltageInfo = new();
        private CoolDeviceInfo CurrentCoolDeviceInfo = new();
        private Object objLock = new object();
        private int Index = 0;
        private bool IsAFWF = false; // 是否水冷

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

            cbInterval.SelectedIndex = 1;
            cbInterval.SelectionChanged += CbInterval_SelectedIndexChanged;
            UpdateByConfig();
            UpdateInterval();
            UpdateSlotShieldState();
            cbCoolingMode.SelectionChanged += cbCoolingMode_SelectionChanged;
            panelCoolDevice.Visibility = IsAFWF ? Visibility.Visible : Visibility.Collapsed;
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
            cbSerialPort.IsEnabled = UsingSerial;
            cbSerialBoudRate.IsEnabled = UsingSerial;
        }

        private void ToogleAllStatus()
        {
            cbInterval.IsEnabled = !IsCollecting;
            cbTestPhase.IsEnabled = !IsCollecting;
            cbTestStatus.IsEnabled = !IsCollecting;
            btSelectSlots.IsEnabled = !IsCollecting;
            cbCoolingMode.IsEnabled = !IsCollecting;

            if (IsCollecting)
            {
                cbSerialPort.IsEnabled = false;
                cbSerialBoudRate.IsEnabled = false;
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
            if (IsAFWF)
            {
                AddCoolTempToCharts();
            }
            tempCharts.ToogleLegends();
            GetWorkflowBaseInfo();
            Tools.EventManager.Instance.Subscribe("WorkflowSelected", WorkflowUpdateEvent);
            Tools.EventManager.Instance.Subscribe("PowerAnalyzer", EventHandler);
            StartListening();
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
                        Width = 120,
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
            LastRecordTime = DateTime.Parse("1990-10-10");
            SaveConfig();
            bool needSaveCsv = false;
            if (tbSaveFilePath.Text.Length == 0)
            {
                needSaveCsv = SelectOpenFile();
            }
            InitDataGrid();
            if (!Simulate)
            {
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
                if (IsAFWF) 
                {
                    AddCoolTempToCharts();
                }
                tempCharts.ToogleLegends();
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
            Tools.EventManager.Instance.Unsubscribe("PowerAnalyzer", EventHandler);
            Tools.EventManager.Instance.Unsubscribe("WorkflowSelected", WorkflowUpdateEvent);
            StopListening();
        }

        #region 图表相关操作
        private void AddCoolTempToCharts()
        {
            tempCharts.AddSeries(new List<string> {
                "出水口温度",
                "回水口温度",
                "外循环出风口1",
                "外循环出风口2",
                "外循环出风口3",
                "外循环出风口4",
                "外循环环境温度1",
                "外循环环境温度2",
            });
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
            Dispatcher.InvokeAsync(new Action(() =>
            {
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
            Index = 0;
            dgTempRecord.ItemsSource = null;
            dgTempRecord.Columns.Clear();
            Table.Rows.Clear();
            Table.Columns.Clear();

            dgTempRecord.Columns.Add(new DataGridTextColumn
            {
                Header = "序号",
                Binding = new Binding("序号"),
                MinWidth = 60
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn
            {
                Header = "时间",
                Binding = new Binding("时间")
                {
                    StringFormat = "HH:mm:ss"
                },
                MinWidth = 60
            });

            if (!IsAFWF)
            {
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Ua",
                    Binding = new Binding("Ua") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Ub",
                    Binding = new Binding("Ub") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Uc",
                    Binding = new Binding("Uc") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "U3",
                    Binding = new Binding("U3") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });

                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Ia",
                    Binding = new Binding("Ia") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Ib",
                    Binding = new Binding("Ib") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Ic",
                    Binding = new Binding("Ic") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "I3",
                    Binding = new Binding("I3") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });

                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "P3",
                    Binding = new Binding("P3") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
            }
            else
            {
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "U3",
                    Binding = new Binding("U3") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "I3",
                    Binding = new Binding("I3") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = "P3",
                    Binding = new Binding("P3") { StringFormat = "{0:N2}" },
                    MinWidth = 40
                });
            }

            Table.Columns.Add("序号", typeof(int));
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
                Binding = new Binding(Configs.Configs.WindingA),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"绕组B",
                Binding = new Binding(Configs.Configs.WindingB),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"绕组C",
                Binding = new Binding(Configs.Configs.WindingC),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"铁心",
                Binding = new Binding(Configs.Configs.Core),
                MinWidth = 40
            });

            if (!IsAFWF)
            {
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境A",
                    Binding = new Binding(Configs.Configs.EnvA),
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境B",
                    Binding = new Binding(Configs.Configs.EnvB),
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境C",
                    Binding = new Binding(Configs.Configs.EnvC),
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境D",
                    Binding = new Binding(Configs.Configs.EnvD),
                    MinWidth = 40
                });
            }
            else
            {
                var outs = Configs.Configs.OutletTemperature.Split(",");
                var ins = Configs.Configs.InletTemperature.Split(",");
                for (int i = 1; i <= 6; i++)
                {
                    string binding = outs.Length >= i ? outs[i - 1] : "None";
                    dgTempRecord.Columns.Add(new DataGridTextColumn()
                    {
                        Header = $"出风口温度{i}",
                        Binding = new Binding(binding),
                        Width = 40
                    });
                }
                for (int i = 1; i <= 3; i++)
                {
                    string binding = ins.Length >= i ? ins[i - 1] : "None";
                    dgTempRecord.Columns.Add(new DataGridTextColumn()
                    {
                        Header = $"进风口温度{i}",
                        Binding = new Binding(binding),
                        Width = 40
                    });
                }
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"顶部温度",
                    Binding = new Binding(Configs.Configs.TopTemperature),
                    Width = 40
                });
                Dictionary<string, string> maps = new Dictionary<string, string>()
                {
                    { "出水口", "Outletwater"},
                    { "回水口", "Inletwater"},
                    { "外循环出风口1", "OutletAir1"},
                    { "外循环出风口2", "OutletAir2"},
                    { "外循环出风口3", "OutletAir3"},
                    { "外循环出风口4", "OutletAir4"},
                    { "外循环环境温度1", "Ambient1"},
                    { "外循环环境温度2", "Ambient2"},
                    { "流量", "Flow"},
                };
                foreach (var item in maps)
                {
                    dgTempRecord.Columns.Add(new DataGridTextColumn()
                    {
                        Header = item.Key,
                        Binding = new Binding(item.Value),
                        Width = 40
                    });
                    Table.Columns.Add(item.Value, typeof(float));
                }
            }

            for (int i = 0; i < SelectedSlots.Count; i++)
            {
                var slot = SelectedSlots[i];
                Table.Columns.Add($"Slot-{slot}", typeof(float));
            }

            dgTempRecord.AutoGenerateColumns = false;
            dgTempRecord.ItemsSource = Table.DefaultView;
        }

        private void UpdateDataGrid(float[] values)
        {
            DataRow newRow = Table.NewRow();
            newRow["序号"] = ++Index;
            newRow["时间"] = DateTime.Now;
            for (int i = 0; i < SelectedSlots.Count && i < values.Length; i++)
            {
                var slot = SelectedSlots[i];
                newRow[$"Slot-{slot}"] = values[i];
            }
            if (IsAFWF)
            {
                newRow["Ambient1"] = CurrentCoolDeviceInfo.AmbientTemperature1;
                newRow["Ambient2"] = CurrentCoolDeviceInfo.AmbientTemperature2;
                newRow["Inletwater"] = CurrentCoolDeviceInfo.InletWaterTemperature;
                newRow["Outletwater"] = CurrentCoolDeviceInfo.OutletWaterTemperature;
                newRow["OutletAir1"] = CurrentCoolDeviceInfo.OutletAirTemperature1;
                newRow["OutletAir2"] = CurrentCoolDeviceInfo.OutletAirTemperature2;
                newRow["OutletAir3"] = CurrentCoolDeviceInfo.OutletAirTemperature3;
                newRow["OutletAir4"] = CurrentCoolDeviceInfo.OutletAirTemperature4;
                newRow["Flow"] = CurrentCoolDeviceInfo.WaterFlowRate;
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
                    newRow["ia"] = CurrentVoltageInfo.ia;
                    newRow["ib"] = CurrentVoltageInfo.ib;
                    newRow["ic"] = CurrentVoltageInfo.ic;
                    newRow["i3"] = CurrentVoltageInfo.i3;
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
                    CurrentVoltageInfo.fu = info.fU ?? 0;
                    CurrentVoltageInfo.IsUpdated = true;
                }
                UpdatePowerAnalyzerInfo();
            }
        }

        private void UpdatePowerAnalyzerInfo()
        {
            Dispatcher.Invoke(() =>
            {
                string msg = "";
                msg += $"Ua: {Utils.FloatFormat(CurrentVoltageInfo.ua)},  ";
                msg += $"Ub: {Utils.FloatFormat(CurrentVoltageInfo.ub)},  ";
                msg += $"Uc: {Utils.FloatFormat(CurrentVoltageInfo.uc)},  ";
                msg += $"U3: {Utils.FloatFormat(CurrentVoltageInfo.u3)},  ";
                msg += $"Ia: {Utils.FloatFormat(CurrentVoltageInfo.ia)},  ";
                msg += $"Ib: {Utils.FloatFormat(CurrentVoltageInfo.ib)},  ";
                msg += $"Ic: {Utils.FloatFormat(CurrentVoltageInfo.ic)},  ";
                msg += $"I3: {Utils.FloatFormat(CurrentVoltageInfo.i3)},  ";
                msg += $"P3: {Utils.FloatFormat(CurrentVoltageInfo.p3)},  ";
                msg += $"Fu: {Utils.FloatFormat(CurrentVoltageInfo.fu)},  ";
                tbPorwerAnalyzerInfo.Text = msg;
            });
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
            if (value.IndexOf("s") >= 0)
            {
                int interval = int.Parse(value.Split("s")[0]);
                RecordInterval = interval;
            }
            else if (value.IndexOf("min") >= 0)
            {
                int interval = int.Parse(value.Split("min")[0]) * 60;
                RecordInterval = interval;
            }
        }

        private bool NeedRecord()
        {
            DateTime endTime = DateTime.Now;

            TimeSpan elapsedTime = endTime - LastRecordTime;
            double seconds = elapsedTime.TotalSeconds;
            if (seconds > RecordInterval)
            {
                LastRecordTime = endTime;
                return true;
            }
            return false;
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
                var listValues = tempModbusCollector.ReadData(maxSelectedSlotIndex, out msg);
                for (int i = 0; i < SlotCount; i++)
                {
                    if (SelectedSlots[i] - 1 >= listValues.Count)
                    {
                        break;
                    }
                    values[i] = listValues[SelectedSlots[i] - 1];
                    //Log.Info($"TempValue {i},Slot {SelectedSlots[i]},value {values[i]}");
                }
            }
            if (values.Length == 0)
            {
                return;
            }
            HandleRecords(values);
            if (NeedRecord())
            {
                WriteCSVFile(values);
                UpdateDataGrid(values);
                Dispatcher.Invoke(() =>
                {
                    float[]? coolTemp = null;
                    if (IsAFWF && CurrentCoolDeviceInfo != null)
                    {
                        coolTemp = new float[]
                        {
                            CurrentCoolDeviceInfo.OutletWaterTemperature,
                            CurrentCoolDeviceInfo.InletWaterTemperature,
                            CurrentCoolDeviceInfo.OutletAirTemperature1,
                            CurrentCoolDeviceInfo.OutletAirTemperature2,
                            CurrentCoolDeviceInfo.OutletAirTemperature3,
                            CurrentCoolDeviceInfo.OutletAirTemperature4,
                            CurrentCoolDeviceInfo.AmbientTemperature1,
                            CurrentCoolDeviceInfo.AmbientTemperature2
                        };
                    }
                    tempCharts.AddRecords(values, coolTemp);
                });
            }
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
        private void cbCoolingMode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            bool _IsAFWF = cbCoolingMode.SelectedIndex == 2;
            if (_IsAFWF != IsAFWF)
            {
                IsAFWF = _IsAFWF;
                UpdateSlotShieldState();
                SelectedSlots_SelectionChanged();
            }
            panelCoolDevice.Visibility = IsAFWF ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateSlotShieldState()
        {
            shEnvA.Visibility = IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shEnvB.Visibility = IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shEnvC.Visibility = IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shEnvD.Visibility = IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shOut1.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shOut2.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shOut3.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shOut4.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shOut5.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shOut6.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shIn1.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shIn2.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shIn3.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
            shTop.Visibility = !IsAFWF ? Visibility.Collapsed : Visibility.Visible;
        }

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
            if (!IsAFWF)
            {
                slots.Add(Configs.Configs.EnvA);
                slots.Add(Configs.Configs.EnvB);
                slots.Add(Configs.Configs.EnvC);
                slots.Add(Configs.Configs.EnvD);
            }
            else
            {
                var _outletSlots = Configs.Configs.OutletTemperature.Split(','); // 出风口温度最多6个
                var _inletSlots = Configs.Configs.InletTemperature.Split(','); // 进风口温度最多3个
                for (int i = 0; i < _outletSlots.Length && i < 6; i++)
                {
                    slots.Add(_outletSlots[i]);
                }
                for (int i = 0; i < _inletSlots.Length && i < 3; i++)
                {
                    slots.Add(_inletSlots[i]);
                }
                slots.Add(Configs.Configs.TopTemperature);
            }
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
            var selectDialog = new TempSlotSelectView(MaxSlotCount, IsAFWF) { WindowStartupLocation = WindowStartupLocation.CenterScreen };
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
            string[] outSlots = Configs.Configs.OutletTemperature.Split(",");
            string[] inSlots = Configs.Configs.InletTemperature.Split(",");
            shOut1.Status = outSlots.Length > 0 ? outSlots[0] : "";
            shOut2.Status = outSlots.Length > 1 ? outSlots[1] : "";
            shOut3.Status = outSlots.Length > 2 ? outSlots[2] : "";
            shOut4.Status = outSlots.Length > 3 ? outSlots[3] : "";
            shOut5.Status = outSlots.Length > 4 ? outSlots[4] : "";
            shOut6.Status = outSlots.Length > 5 ? outSlots[5] : "";

            shIn1.Status = inSlots.Length > 0 ? inSlots[0] : "";
            shIn2.Status = inSlots.Length > 1 ? inSlots[1] : "";
            shIn3.Status = inSlots.Length > 2 ? inSlots[2] : "";
            shTop.Status = Configs.Configs.TopTemperature;
        }
        #endregion

        #region 数据上传

        private void UploadData()
        {
            CommonTempRiseTestInfo configItem;
            int testIndex = Utils.ParseInt(cbTestCount.Text);
            var items = CommonTempRiseTestInfo.ReadFromDB(Configs.Configs.WorkflowID, cbTestPhase.Text, cbTestStatus.Text, cbCoolingMode.Text, testIndex, 1);
            if (items == null || items.Count == 0)
            {
                configItem = new CommonTempRiseTestInfo()
                {
                    TestingPhase = cbTestPhase.Text,
                    TestingStatus = cbTestStatus.Text,
                    WorkflowId = Configs.Configs.WorkflowID,
                    TestingIndex = testIndex,
                    CoolingMode = cbCoolingMode.Text,
                    DateTime = DateTime.Now,
                    TestingMode = 1,
                };
                if (!configItem.WriteToDB())
                {
                    MessageBox.Show("数据上传失败!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                configItem = items[0];
            }

            if (Table.Rows.Count == 0)
            {
                MessageBox.Show("暂无数据可以上传，请先采集数据!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 删除之前的时间数据
            CommonTempRiseTestRecordInfo.DeleteData(configItem.ID);
            // 将DataTable中的数据转成试验数据格式并且一条条上传
            var query = from row in Table.AsEnumerable()
                        select row;
            int i = 0;
            List<CommonTempRiseTestRecordInfo> list = new List<CommonTempRiseTestRecordInfo>();
            foreach (var item in query)
            {
                i++;
                var record = new CommonTempRiseTestRecordInfo()
                {
                    ID = configItem.ID,
                    Timestamp = item.Field<DateTime>("时间"),
                    Ua = item.Field<float?>("ua") ?? 0,
                    Ub = item.Field<float?>("ub") ?? 0,
                    Uc = item.Field<float?>("uc") ?? 0,
                    U3 = item.Field<float?>("u3") ?? 0,
                    Ia = item.Field<float?>("ia") ?? 0,
                    Ib = item.Field<float?>("ib") ?? 0,
                    Ic = item.Field<float?>("ic") ?? 0,
                    I3 = item.Field<float?>("i3") ?? 0,
                    P3 = item.Field<float?>("p3") ?? 0,
                    CoreTemp = item.Field<float?>(Configs.Configs.Core) ?? 0,
                    WindingTempA = item.Field<float?>(Configs.Configs.WindingA) ?? 0,
                    WindingTempB = item.Field<float?>(Configs.Configs.WindingB) ?? 0,
                    WindingTempC = item.Field<float?>(Configs.Configs.WindingC) ?? 0,
                    IsAFWF = IsAFWF,
                    WorkflowID = Configs.Configs.WorkflowID
                };
                if (!IsAFWF)
                {
                    record.EnvTempA = item.Field<float?>(Configs.Configs.EnvA) ?? 0;
                    record.EnvTempB = item.Field<float?>(Configs.Configs.EnvB) ?? 0;
                    record.EnvTempC = item.Field<float?>(Configs.Configs.EnvC) ?? 0;
                    record.EnvTempD = item.Field<float?>(Configs.Configs.EnvD) ?? 0;
                }
                else
                {
                    var outlets = Configs.Configs.OutletTemperature.Split(",");
                    var inlets = Configs.Configs.InletTemperature.Split(",");

                    record.Outlet1 = item.Field<float?>(outlets[0]) ?? 0;
                    record.Outlet2 = item.Field<float?>(outlets[1]) ?? 0;
                    record.Outlet3 = item.Field<float?>(outlets[2]) ?? 0;
                    record.Outlet4 = item.Field<float?>(outlets[3]) ?? 0;
                    record.Outlet5 = item.Field<float?>(outlets[4]) ?? 0;
                    record.Outlet6 = item.Field<float?>(outlets[5]) ?? 0;
                    record.Inlet1 = item.Field<float?>(inlets[0]) ?? 0;
                    record.Inlet2 = item.Field<float?>(inlets[1]) ?? 0;
                    record.Inlet3 = item.Field<float?>(inlets[2]) ?? 0;
                    record.TopTemp = item.Field<float?>(Configs.Configs.TopTemperature) ?? 0;
                    record.OutletWaterTemperature = item.Field<float?>("Outletwater") ?? 0;
                    record.InletWaterTemperature = item.Field<float?>("Inletwater") ?? 0;
                    record.AmbientTemperature1 = item.Field<float?>("Ambient1") ?? 0;
                    record.AmbientTemperature2 = item.Field<float?>("Ambient2") ?? 0;
                    record.OutletAirTemperature1 = item.Field<float?>("OutletAir1") ?? 0;
                    record.OutletAirTemperature2 = item.Field<float?>("OutletAir2") ?? 0;
                    record.OutletAirTemperature3 = item.Field<float?>("OutletAir3") ?? 0;
                    record.OutletAirTemperature4 = item.Field<float?>("OutletAir4") ?? 0;
                    record.WaterFlowRate = item.Field<float?>("Flow") ?? 0;
                }
                list.Add(record);
            }
            bool ret = CommonTempRiseTestRecordInfo.BatchInsertData(list);
            if (ret)
            {
                MessageBox.Show($"数据上传成功，共{Table.Rows.Count}条数据!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            else
            {
                MessageBox.Show($"数据上传出错，请检查或者重新尝试!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            UploadData();
        }

        private void btSetWorkflow_Click(object sender, RoutedEventArgs e)
        {
            var vs = WorkflowInfo.ReadFromDB(Configs.Configs.WorkflowID);
            if (vs == null || vs.Count == 0)
            {
                MessageBox.Show("工作令错误，请检查！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            float voltage = Utils.ParseFloat(tbTestingVoltage.Text);
            float current = Utils.ParseFloat(tbTestingCurrent.Text);
            WorkflowInfo workflowInfo = vs[0];
            workflowInfo.TempRiseHVCorrectionFactor = Utils.ParseFloatNull(cbHVCorrectionFact.Text);
            workflowInfo.TempRiseLVCorrectionFactor = Utils.ParseFloatNull(cbLVCorrectionFact.Text);
            workflowInfo.TempRiseRelativeTo = cbRelatedTo.Text;
            workflowInfo.TempRiseTestingVoltage = voltage;
            workflowInfo.TempRiseTestingCurrent = current;

            bool ret = workflowInfo.UpdateTempRiseFields();
            if (!ret)
            {
                MessageBox.Show("设置失败，请检查工作令和服务器连接！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("设置成功，数据页写入工作令！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region 获取工作令信息

        private void WorkflowUpdateEvent(object sender, TestEventArgs e)
        {
            GetWorkflowBaseInfo();
        }

        private void GetWorkflowBaseInfo()
        {
            Task.Run(() =>
            {
                var vs = WorkflowInfo.ReadFromDB(Configs.Configs.WorkflowID);
                WorkflowInfo? workflowInfo = (vs != null & vs.Count != 0) ? vs[0] : null;
                Dispatcher.Invoke(() =>
                {
                    if (workflowInfo == null)
                    {
                        cbHVCorrectionFact.SelectedIndex = 0;
                        cbLVCorrectionFact.SelectedIndex = 0;
                        cbRelatedTo.SelectedIndex = 0;
                        tbTestingVoltage.Text = "";
                        tbTestingCurrent.Text = "";
                    }
                    else
                    {
                        cbHVCorrectionFact.Text = workflowInfo.TempRiseHVCorrectionFactor.ToString();
                        cbLVCorrectionFact.Text = workflowInfo.TempRiseLVCorrectionFactor.ToString();
                        cbRelatedTo.Text = workflowInfo.TempRiseRelativeTo;
                        tbTestingVoltage.Text = workflowInfo.TempRiseTestingVoltage.ToString();
                        tbTestingCurrent.Text = workflowInfo.TempRiseTestingCurrent.ToString();
                    }
                });
            });
        }
        #endregion

        #region 监听水冷温度流量数据

        private UdpClient udpClient;
        private Thread listenThread;
        private bool isListening;

        private void StartListening()
        {
            udpClient = new UdpClient(8877);
            isListening = true;

            listenThread = new Thread(ListenForMessages);
            listenThread.Start();
        }

        private void ListenForMessages()
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                while (isListening)
                {
                    if (udpClient.Available > 0 && IsAFWF)
                    {
                        byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                        string receiveString = Encoding.ASCII.GetString(receiveBytes);
                        string[] values = receiveString.Split(';');
                        if (Configs.Configs.IsEnableVerboseDebug) 
                        {
                            Log.Info($"Received: {receiveString}");
                        }
                        if (values.Length >= 9)
                        {
                            CurrentCoolDeviceInfo.OutletWaterTemperature = Utils.ParseFloat(values[0]);
                            CurrentCoolDeviceInfo.InletWaterTemperature = Utils.ParseFloat(values[1]);
                            CurrentCoolDeviceInfo.WaterFlowRate = Utils.ParseFloat(values[2]);
                            CurrentCoolDeviceInfo.AmbientTemperature1 = Utils.ParseFloat(values[3]);
                            CurrentCoolDeviceInfo.AmbientTemperature2 = Utils.ParseFloat(values[4]);
                            CurrentCoolDeviceInfo.OutletAirTemperature1 = Utils.ParseFloat(values[5]);
                            CurrentCoolDeviceInfo.OutletAirTemperature2 = Utils.ParseFloat(values[6]);
                            CurrentCoolDeviceInfo.OutletAirTemperature3 = Utils.ParseFloat(values[7]);
                            CurrentCoolDeviceInfo.OutletAirTemperature4 = Utils.ParseFloat(values[8]);
                            Log.Info(CurrentCoolDeviceInfo.ToString());
                            Dispatcher.Invoke(() =>
                            {
                                tbOutletWater.Text = CurrentCoolDeviceInfo.OutletWaterTemperature + "";
                                tbInletWater.Text = CurrentCoolDeviceInfo.InletWaterTemperature + "";
                                tbFlow.Text = CurrentCoolDeviceInfo.WaterFlowRate + "";
                                tbEnvTemp1.Text = CurrentCoolDeviceInfo.AmbientTemperature1 + "";
                                tbEnvTemp2.Text = CurrentCoolDeviceInfo.AmbientTemperature2 + "";
                                tbOutletAir1.Text = CurrentCoolDeviceInfo.OutletAirTemperature1 + "";
                                tbOutletAir2.Text = CurrentCoolDeviceInfo.OutletAirTemperature2 + "";
                                tbOutletAir3.Text = CurrentCoolDeviceInfo.OutletAirTemperature3 + "";
                                tbOutletAir4.Text = CurrentCoolDeviceInfo.OutletAirTemperature4 + "";
                            });
                        }
                    }
                    else
                    {
                        Thread.Sleep(100); // Reduce CPU usage
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.Interrupted)
                {
                    Log.Info($"Socket exception: {ex.Message}");
                }
            }
            catch (ObjectDisposedException)
            {
                // This exception is expected when closing the UdpClient.
            }
        }

        private void StopListening()
        {
            isListening = false;

            // Close the UdpClient to stop the blocking Receive call.
            udpClient.Close();

            // Wait for the listening thread to finish.
            if (listenThread != null && listenThread.IsAlive)
            {
                listenThread.Join();
            }

            Log.Info("Stopped listening.");
        }
        #endregion

        private void MenuItemEditor_Click(object sender, RoutedEventArgs e)
        {
            // 获取右键菜单所在的行
            var selectedRow = (DataGridRow)dgTempRecord.ItemContainerGenerator.ContainerFromItem(dgTempRecord.SelectedItem);

            // 获取该行对应的数据项
            DataRowView rowView = (DataRowView)selectedRow.Item;
            DataRow row = rowView.Row;

            // 修改数据项
            // 假设数据表名为 dataTable，可以通过 row 修改对应的数据
            row["序号"] = 200; // 修改对应列的值

            // 如果需要保存到数据库或者其他操作，这里进行保存或提交的逻辑
        }

        private void MenuItemEnableEdit_Click(object sender, RoutedEventArgs e)
        {
            dgTempRecord.IsReadOnly = false;
        }

        private void MenuItemCloseEdit_Click(object sender, RoutedEventArgs e)
        {
            dgTempRecord.IsReadOnly = true;
        }
    }
}

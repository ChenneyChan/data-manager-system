using ABBDataManagerSystem.Bean.Base;
using ABBDataManagerSystem.Charts;
using ABBDataManagerSystem.Connector;
using ABBDataManagerSystem.Pages.Views;
using ABBDataManagerSystem.Tools;
using Microsoft.Win32;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MessageBox = HandyControl.Controls.MessageBox;
using HandyControl.Controls;

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
        /// 出风口温度5
        /// </summary>
        public float OutletAirTemperature5 { get; set; }

        /// <summary>
        /// 出风口温度6
        /// </summary>
        public float OutletAirTemperature6 { get; set; }

        /// <summary>
        /// 出风口温度7
        /// </summary>
        public float OutletAirTemperature7 { get; set; }

        /// <summary>
        /// 出风口温度8
        /// </summary>
        public float OutletAirTemperature8 { get; set; }

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
                   $"OutletAirTemperature5: {OutletAirTemperature5}, OutletAirTemperature6: {OutletAirTemperature6}, " +
                   $"OutletAirTemperature7: {OutletAirTemperature7}, OutletAirTemperature8: {OutletAirTemperature8}, " +
                   $"WaterFlowRate: {WaterFlowRate}";
        }
    }


    enum TempMode
    {
        COMMON = 0, // 常规模式，AN或者AF+相对于环境
        AF_AIR = 1, // AF+相对于进风口温度
        AFWF = 2,   // AFWF，水冷模式
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
        private Dictionary<int, string> ExtensionSlotMaps = new Dictionary<int, string>();

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
        private CoolDeviceInfo CoolDeviceInfo1 = new();
        private CoolDeviceInfo CoolDeviceInfo2 = new();
        private Object objLock = new object();
        private int Index = 0;
        private TempMode TempTestMode = TempMode.COMMON;
        private bool IsAutoScroll = true;

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
            tbTestCount.ValueChanged += tbTestCount_ValueChanged;
            cbTestCount.SelectionChanged += CbTestCount_SelectionChanged;
            cbCoolingMode.SelectionChanged += cbCoolingMode_SelectionChanged;
            cbRelatedTo.SelectionChanged += cbCoolingMode_SelectionChanged;
            panelCoolDevice.Visibility = TempTestMode == TempMode.AFWF ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateCommonInfo()
        {
            int Index = (int)tbTestCount.Value;
            string workflowId = Configs.Configs.WorkflowID;
            string CoolingMode = ((ComboBoxItem)cbCoolingMode.SelectedItem).Content.ToString();
            string TestingPhase = cbTestPhase.SelectedItem.ToString();
            Task.Run(() =>
            {
                var datas = TempRiseCommonInfo.ReadFromDB(workflowId, TestingPhase, Index, CoolingMode);
                if (datas.Count > 0)
                {
                    var data = datas[0];
                    Dispatcher.Invoke(() =>
                    {
                        tbTestingVoltage.Text = data.TempRiseTestingVoltage.ToString();
                        tbTestingCurrent.Text = data.TempRiseTestingCurrent.ToString();
                        cbHVCorrectionFact.Text = data.TempRiseHVCorrectionFactor.ToString();
                        cbLVCorrectionFact.Text = data.TempRiseLVCorrectionFactor.ToString();
                        cbRelatedTo.Text = data.TempRiseRelativeTo.ToString();
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        tbTestingVoltage.Text = "";
                        tbTestingCurrent.Text = "";
                        cbHVCorrectionFact.SelectedIndex = 0;
                        cbLVCorrectionFact.SelectedIndex = 0;
                        cbRelatedTo.SelectedIndex = 0;
                    });
                }
            });
        }

        private void tbTestCount_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            UpdateCommonInfo();
        }

        private void CbTestCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCommonInfo();
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
            cbRelatedTo.IsEnabled = !IsCollecting;
            cbCoolDeviceSource.IsEnabled = !IsCollecting;

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
            if (TempTestMode == TempMode.AFWF)
            {
                AddCoolTempToCharts();
            }
            tempCharts.ToogleLegends();
            GetWorkflowBaseInfo();
            Tools.EventManager.Instance.Subscribe("WorkflowSelected", WorkflowUpdateEvent);
            Tools.EventManager.Instance.Subscribe("PowerAnalyzer", EventHandler);
            // Set cool device selection from config
            cbCoolDeviceSource.SelectedIndex = Configs.Configs.CoolDeviceSelectedIndex;
            UpdateCurrentCoolDevice();
            StartListening();
            UpdateCommonInfo();
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
            if (cbCoolingMode.Text == "AF" && cbRelatedTo.Text == "进水口温度")
            {
                MessageBox.Show("冷却方式是“AF”时，相对于不可以是“进水口温度”！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (TempTestMode == TempMode.AFWF)
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
                "外循环出风口5",
                "外循环出风口6",
                "外循环出风口7",
                "外循环出风口8",
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
        private static string GetBindingPath(string path)
        {
            if (path == "") return "None";
            return path;
        }
        private void InitDataGrid()
        {
            Index = 0;
            dgTempRecord.ItemsSource = null;
            dgTempRecord.Columns.Clear();
            Table.Rows.Clear();
            Table.Columns.Clear();
            Table.Columns.Add("None", typeof(float)); // 一个默认的空字段

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

            if (TempTestMode == TempMode.COMMON)
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
                Binding = new Binding(GetBindingPath(Configs.Configs.WindingA)),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"绕组B",
                Binding = new Binding(GetBindingPath(Configs.Configs.WindingB)),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"绕组C",
                Binding = new Binding(GetBindingPath(Configs.Configs.WindingC)),
                MinWidth = 40
            });
            dgTempRecord.Columns.Add(new DataGridTextColumn()
            {
                Header = $"铁心",
                Binding = new Binding(GetBindingPath(Configs.Configs.Core)),
                MinWidth = 40
            });

            if (TempTestMode == TempMode.COMMON)
            {
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境A",
                    Binding = new Binding(GetBindingPath(Configs.Configs.EnvA)),
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境B",
                    Binding = new Binding(GetBindingPath(Configs.Configs.EnvB)),
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境C",
                    Binding = new Binding(GetBindingPath(Configs.Configs.EnvC)),
                    MinWidth = 40
                });
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"环境D",
                    Binding = new Binding(GetBindingPath(Configs.Configs.EnvD)),
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
                        Binding = new Binding(GetBindingPath(binding)),
                        Width = 40
                    });
                }
                for (int i = 1; i <= 3; i++)
                {
                    string binding = ins.Length >= i ? ins[i - 1] : "None";
                    dgTempRecord.Columns.Add(new DataGridTextColumn()
                    {
                        Header = $"进风口温度{i}",
                        Binding = new Binding(GetBindingPath(binding)),
                        Width = 40
                    });
                }
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"顶部温度",
                    Binding = new Binding(GetBindingPath(Configs.Configs.TopTemperature)),
                    Width = 40
                });
                if (TempTestMode == TempMode.AFWF)
                {
                    Dictionary<string, string> maps = new Dictionary<string, string>()
                    {
                        { "出水口温度", "Outletwater"},
                        { "回水口温度", "Inletwater"},
                        { "外循环出风口1", "OutletAir1"},
                        { "外循环出风口2", "OutletAir2"},
                        { "外循环出风口3", "OutletAir3"},
                        { "外循环出风口4", "OutletAir4"},
                        { "外循环出风口5", "OutletAir5"},
                        { "外循环出风口6", "OutletAir6"},
                        { "外循环出风口7", "OutletAir7"},
                        { "外循环出风口8", "OutletAir8"},
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
                            Width = item.Key.IndexOf("外循环环境温度") >= 0 ? 48 : 44
                        });
                        Table.Columns.Add(item.Value, typeof(float));
                    }
                }
            }

            for (int i = 0; i < SelectedSlots.Count; i++)
            {
                var slot = SelectedSlots[i];
                Table.Columns.Add($"Slot-{slot}", typeof(float));
            }

            foreach (var extensionItem in ExtensionSlotMaps)
            {
                dgTempRecord.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"其他{extensionItem.Key}",
                    Binding = new Binding(GetBindingPath(extensionItem.Value)),
                    Width = 40
                });
            }

            dgTempRecord.AutoGenerateColumns = false;
            dgTempRecord.ItemsSource = Table.DefaultView;

            AddSlotRow();
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
            if (TempTestMode == TempMode.AFWF)
            {
                newRow["Ambient1"] = CurrentCoolDeviceInfo.AmbientTemperature1;
                newRow["Ambient2"] = CurrentCoolDeviceInfo.AmbientTemperature2;
                newRow["Inletwater"] = CurrentCoolDeviceInfo.InletWaterTemperature;
                newRow["Outletwater"] = CurrentCoolDeviceInfo.OutletWaterTemperature;
                newRow["OutletAir1"] = CurrentCoolDeviceInfo.OutletAirTemperature1;
                newRow["OutletAir2"] = CurrentCoolDeviceInfo.OutletAirTemperature2;
                newRow["OutletAir3"] = CurrentCoolDeviceInfo.OutletAirTemperature3;
                newRow["OutletAir4"] = CurrentCoolDeviceInfo.OutletAirTemperature4;
                newRow["OutletAir5"] = CurrentCoolDeviceInfo.OutletAirTemperature5;
                newRow["OutletAir6"] = CurrentCoolDeviceInfo.OutletAirTemperature6;
                newRow["OutletAir7"] = CurrentCoolDeviceInfo.OutletAirTemperature7;
                newRow["OutletAir8"] = CurrentCoolDeviceInfo.OutletAirTemperature8;
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
                if (IsAutoScroll)
                {
                    ScrollToEnd(dgTempRecord);
                }
            });
        }

        private static void ScrollToEnd(Control control)
        {
            // 获取DataGrid的ScrollViewer
            var scrollViewer = control.Template.FindName("DG_ScrollViewer", control) as System.Windows.Controls.ScrollViewer;

            if (scrollViewer != null)
            {
                // 可以在这里操作ScrollViewer，例如滚动到底部
                scrollViewer.ScrollToEnd();
            }
        }

        private void AddSlotRow()
        {
            DataRow newRow = Table.NewRow();
            newRow["序号"] = 0;
            newRow["时间"] = DateTime.Now.Date;
            for (int i = 0; i < SelectedSlots.Count; i++)
            {
                var slot = SelectedSlots[i];
                newRow[$"Slot-{slot}"] = slot;
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
                tbIa.Text = Utils.FloatFormat(CurrentVoltageInfo.ia);
                tbIb.Text = Utils.FloatFormat(CurrentVoltageInfo.ib);
                tbIc.Text = Utils.FloatFormat(CurrentVoltageInfo.ic);
                tbI3.Text = Utils.FloatFormat(CurrentVoltageInfo.i3);
                tbUa.Text = Utils.FloatFormat(CurrentVoltageInfo.ua);
                tbUb.Text = Utils.FloatFormat(CurrentVoltageInfo.ub);
                tbUc.Text = Utils.FloatFormat(CurrentVoltageInfo.uc);
                tbU3.Text = Utils.FloatFormat(CurrentVoltageInfo.u3);
                tbP3.Text = Utils.FloatFormat(CurrentVoltageInfo.p3);
                tbFu.Text = Utils.FloatFormat(CurrentVoltageInfo.fu);
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
                    if (TempTestMode == TempMode.AFWF && CurrentCoolDeviceInfo != null)
                    {
                        coolTemp = new float[]
                        {
                            CurrentCoolDeviceInfo.OutletWaterTemperature,
                            CurrentCoolDeviceInfo.InletWaterTemperature,
                            CurrentCoolDeviceInfo.OutletAirTemperature1,
                            CurrentCoolDeviceInfo.OutletAirTemperature2,
                            CurrentCoolDeviceInfo.OutletAirTemperature3,
                            CurrentCoolDeviceInfo.OutletAirTemperature4,
                            CurrentCoolDeviceInfo.OutletAirTemperature5,
                            CurrentCoolDeviceInfo.OutletAirTemperature6,
                            CurrentCoolDeviceInfo.OutletAirTemperature7,
                            CurrentCoolDeviceInfo.OutletAirTemperature8,
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

        #region 温度槽位选择
        private void cbCoolingMode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var oldMode = TempTestMode;
            bool _IsAFWF = cbCoolingMode.SelectedIndex == 2;
            bool _IsAF_Air = cbCoolingMode.SelectedIndex == 1 && cbRelatedTo.SelectedIndex == 2;
            TempTestMode = _IsAFWF ? TempMode.AFWF : (_IsAF_Air ? TempMode.AF_AIR : TempMode.COMMON);
            if (TempTestMode != oldMode)
            {
                UpdateSlotShieldState();
                SelectedSlots_SelectionChanged();
            }
            panelCoolDevice.Visibility = TempTestMode == TempMode.AFWF ? Visibility.Visible : Visibility.Collapsed;
            if (sender == cbCoolingMode)
            {
                UpdateCommonInfo();
            }
        }

        private void UpdateSlotShieldState()
        {
            bool needEnv = TempTestMode == TempMode.COMMON;
            bool needAir = TempTestMode == TempMode.AF_AIR || TempTestMode == TempMode.AFWF;
            shEnvA.Visibility = !needEnv ? Visibility.Collapsed : Visibility.Visible;
            shEnvB.Visibility = !needEnv ? Visibility.Collapsed : Visibility.Visible;
            shEnvC.Visibility = !needEnv ? Visibility.Collapsed : Visibility.Visible;
            shEnvD.Visibility = !needEnv ? Visibility.Collapsed : Visibility.Visible;
            shOut1.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shOut2.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shOut3.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shOut4.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shOut5.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shOut6.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shIn1.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shIn2.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shIn3.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
            shTop.Visibility = !needAir ? Visibility.Collapsed : Visibility.Visible;
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
            if (TempTestMode == TempMode.COMMON)
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
            if (Configs.Configs.ExtensionSlots.Length > 0)
            {
                ExtensionSlotMaps.Clear();
                var _extensionSlots = Configs.Configs.ExtensionSlots.Split(',');
                int index = 0;
                foreach (var _extensionSlot in _extensionSlots)
                {
                    index++;
                    if (_extensionSlot.Trim().Length == 0)
                    {
                        continue;
                    }
                    slots.Add(_extensionSlot);
                    ExtensionSlotMaps.Add(index, _extensionSlot);
                }
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
            var selectDialog = new TempSlotSelectView(MaxSlotCount, TempTestMode != TempMode.AFWF) { WindowStartupLocation = WindowStartupLocation.CenterScreen };
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

            if (Configs.Configs.ExtensionSlots.Length > 0 && ExtensionSlotMaps.Count == 0)
            {
                var _extensionSlots = Configs.Configs.ExtensionSlots.Split(',');
                int index = 0;
                foreach (var _extensionSlot in _extensionSlots)
                {
                    index++;
                    if (_extensionSlot.Trim().Length == 0)
                    {
                        continue;
                    }
                    ExtensionSlotMaps.Add(index, _extensionSlot);
                }
            }

            UpdateExtensionSheild(1, shExtension1);
            UpdateExtensionSheild(2, shExtension2);
            UpdateExtensionSheild(3, shExtension3);
            UpdateExtensionSheild(4, shExtension4);
            UpdateExtensionSheild(5, shExtension5);
            UpdateExtensionSheild(6, shExtension6);
            UpdateExtensionSheild(7, shExtension7);
            UpdateExtensionSheild(8, shExtension8);
            UpdateExtensionSheild(9, shExtension9);
        }

        private void UpdateExtensionSheild(int key, Shield sh)
        {
            if (ExtensionSlotMaps.ContainsKey(key))
            {
                sh.Visibility = Visibility.Visible;
                sh.Status = ExtensionSlotMaps[key];
            }
            else
            {
                sh.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region 数据上传

        private void UploadDataAsync(int testIndex, string testPhase, string testStatus, string coolingMode, string workflowId, string remark, TempMode tempMode, DataRow[] rows)
        {
            CommonTempRiseTestInfo configItem;
            var items = CommonTempRiseTestInfo.ReadFromDB(workflowId, testPhase, testStatus, coolingMode, testIndex, 1);
            if (items == null || items.Count == 0)
            {
                configItem = new CommonTempRiseTestInfo()
                {
                    TestingPhase = testPhase,
                    TestingStatus = testStatus,
                    WorkflowId = workflowId,
                    TestingIndex = testIndex,
                    CoolingMode = coolingMode,
                    DateTime = DateTime.Now,
                    TestingMode = 1,
                    Remark = remark.Length > 0 ? remark : null,
                };
                if (!configItem.WriteToDB())
                {
                    Dispatcher.Invoke(() => MessageBox.Show("数据上传失败!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Error));
                    return;
                }
            }
            else
            {
                configItem = items[0];
            }

            if (rows.Length == 0)
            {
                Dispatcher.Invoke(() => MessageBox.Show("暂无数据可以上传，请先采集数据!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Warning));
                return;
            }

            // 删除之前的时间数据
            CommonTempRiseTestRecordInfo.DeleteData(configItem.ID);
            // 将DataTable中的数据转成试验数据格式并且一条条上传
            List<CommonTempRiseTestRecordInfo> list = new List<CommonTempRiseTestRecordInfo>();
            foreach (var item in rows)
            {
                var record = new CommonTempRiseTestRecordInfo()
                {
                    ID = configItem.ID,
                    Timestamp = item.Field<DateTime>("时间"),
                    Ua = item.Field<float?>("ua") ?? null,
                    Ub = item.Field<float?>("ub") ?? null,
                    Uc = item.Field<float?>("uc") ?? null,
                    U3 = item.Field<float?>("u3") ?? null,
                    Ia = item.Field<float?>("ia") ?? null,
                    Ib = item.Field<float?>("ib") ?? null,
                    Ic = item.Field<float?>("ic") ?? null,
                    I3 = item.Field<float?>("i3") ?? null,
                    P3 = item.Field<float?>("p3") ?? null,
                    CoreTemp = item.Field<float?>(GetBindingPath(Configs.Configs.Core)) ?? null,
                    WindingTempA = item.Field<float?>(GetBindingPath(Configs.Configs.WindingA)) ?? null,
                    WindingTempB = item.Field<float?>(GetBindingPath(Configs.Configs.WindingB)) ?? null,
                    WindingTempC = item.Field<float?>(GetBindingPath(Configs.Configs.WindingC)) ?? null,
                    IsAFWF = tempMode == TempMode.AFWF,
                    WorkflowID = workflowId
                };
                if (tempMode == TempMode.COMMON)
                {
                    record.EnvTempA = item.Field<float?>(GetBindingPath(Configs.Configs.EnvA)) ?? null;
                    record.EnvTempB = item.Field<float?>(GetBindingPath(Configs.Configs.EnvB)) ?? null;
                    record.EnvTempC = item.Field<float?>(GetBindingPath(Configs.Configs.EnvC)) ?? null;
                    record.EnvTempD = item.Field<float?>(GetBindingPath(Configs.Configs.EnvD)) ?? null;
                }
                else
                {
                    var outlets = Configs.Configs.OutletTemperature.Split(",");
                    var inlets = Configs.Configs.InletTemperature.Split(",");

                    record.Outlet1 = item.Field<float?>(GetBindingPath(outlets[0])) ?? null;
                    record.Outlet2 = item.Field<float?>(GetBindingPath(outlets[1])) ?? null;
                    record.Outlet3 = item.Field<float?>(GetBindingPath(outlets[2])) ?? null;
                    record.Outlet4 = item.Field<float?>(GetBindingPath(outlets[3])) ?? null;
                    record.Outlet5 = item.Field<float?>(GetBindingPath(outlets[4])) ?? null;
                    record.Outlet6 = item.Field<float?>(GetBindingPath(outlets[5])) ?? null;
                    record.Inlet1 = item.Field<float?>(GetBindingPath(inlets[0])) ?? null;
                    record.Inlet2 = item.Field<float?>(GetBindingPath(inlets[1])) ?? null;
                    record.Inlet3 = item.Field<float?>(GetBindingPath(inlets[2])) ?? null;
                    record.TopTemp = item.Field<float?>(GetBindingPath(Configs.Configs.TopTemperature)) ?? null;
                    if (tempMode == TempMode.AFWF)
                    {
                        record.OutletWaterTemperature = item.Field<float?>("Outletwater") ?? null;
                        record.InletWaterTemperature = item.Field<float?>("Inletwater") ?? null;
                        record.AmbientTemperature1 = item.Field<float?>("Ambient1") ?? null;
                        record.AmbientTemperature2 = item.Field<float?>("Ambient2") ?? null;
                        record.OutletAirTemperature1 = item.Field<float?>("OutletAir1") ?? null;
                        record.OutletAirTemperature2 = item.Field<float?>("OutletAir2") ?? null;
                        record.OutletAirTemperature3 = item.Field<float?>("OutletAir3") ?? null;
                        record.OutletAirTemperature4 = item.Field<float?>("OutletAir4") ?? null;
                        record.OutletAirTemperature5 = item.Field<float?>("OutletAir5") ?? null;
                        record.OutletAirTemperature6 = item.Field<float?>("OutletAir6") ?? null;
                        record.OutletAirTemperature7 = item.Field<float?>("OutletAir7") ?? null;
                        record.OutletAirTemperature8 = item.Field<float?>("OutletAir8") ?? null;
                        record.WaterFlowRate = item.Field<float?>("Flow") ?? null;
                    }
                }
                list.Add(record);
            }
            bool ret = CommonTempRiseTestRecordInfo.BatchInsertData(list);
            if (ret)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"数据上传成功，共{rows.Length}条数据!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    SetCommonInfo(false);
                });
                return;
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"数据上传出错，请检查或者重新尝试!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Warning);
                });
                return;
            }
        }

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            // 在UI线程读取控件值和DataTable（非线程安全）
            int testIndex = (int)tbTestCount.Value;
            string testPhase = cbTestPhase.Text;
            string testStatus = cbTestStatus.Text;
            string coolingMode = cbCoolingMode.Text;
            string workflowId = Configs.Configs.WorkflowID;
            string remark = tbRemark.Text;
            TempMode tempMode = TempTestMode;

            if (Table.Rows.Count == 0)
            {
                MessageBox.Show("暂无数据可以上传，请先采集数据!", "上传结果", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 在UI线程拷贝DataTable数据
            DataRow[] rows = Table.AsEnumerable().ToArray();

            btUpload.IsEnabled = false;
            btUpload.Content = "上传中...";

            Task.Run(() =>
            {
                UploadDataAsync(testIndex, testPhase, testStatus, coolingMode, workflowId, remark, tempMode, rows);
            }).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    btUpload.IsEnabled = true;
                    btUpload.Content = "上传数据";
                });
            });
        }


        private void SetCommonInfo(bool needMessageBox)
        {
            int Index = (int)tbTestCount.Value;
            string workflowId = Configs.Configs.WorkflowID;
            string CoolingMode = cbCoolingMode.Text;
            string TestingPhase = cbTestPhase.Text;

            var data = new TempRiseCommonInfo()
            {
                WorkflowID = workflowId,
                CoolingMode = CoolingMode,
                TestIndex = Index,
                TestingPhase = TestingPhase,
            };
            float? voltage = Utils.ParseFloatNull(tbTestingVoltage.Text);
            float? current = Utils.ParseFloatNull(tbTestingCurrent.Text);
            data.TempRiseHVCorrectionFactor = Utils.ParseFloatNull(cbHVCorrectionFact.Text);
            data.TempRiseLVCorrectionFactor = Utils.ParseFloatNull(cbLVCorrectionFact.Text);
            data.TempRiseRelativeTo = cbRelatedTo.Text;
            data.TempRiseTestingVoltage = voltage;
            data.TempRiseTestingCurrent = current;

            bool ret;
            var datas = TempRiseCommonInfo.ReadFromDB(workflowId, TestingPhase, Index, CoolingMode);
            if (datas.Count > 0)
            {
                ret = data.UpdateData();
            }
            else
            {
                ret = data.InsertDB();
            }

            if (!needMessageBox) { return; }
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
                WorkflowInfo? workflowInfo = Configs.Configs.WorkflowInfo;
                Dispatcher.Invoke(() =>
                {
                    if (workflowInfo != null)
                    {
                        bool isCommon = true;
                        if (workflowInfo.WorkflowType == "三绕组")
                        {
                            isCommon = workflowInfo.RatedPower1 == workflowInfo.RatedPower2;
                        }
                        cbTestPhase.Items.Clear();
                        if (isCommon)
                        {
                            cbTestPhase.Items.Add("空载");
                            cbTestPhase.Items.Add("负载");
                        }
                        else
                        {
                            cbTestPhase.Items.Add("空载");
                            cbTestPhase.Items.Add("负载（大容量）");
                            cbTestPhase.Items.Add("负载（小容量）");
                        }
                        cbTestPhase.SelectedIndex = 0;
                    }
                });
            });
        }
        #endregion

        #region 监听水冷温度流量数据

        private UdpClient udpClient1;
        private UdpClient udpClient2;
        private Thread listenThread1;
        private Thread listenThread2;
        private bool isListening;

        private void StartListening()
        {
            int port1 = Configs.Configs.CoolDevice1Port;
            int port2 = Configs.Configs.CoolDevice2Port;
            try
            {
                udpClient1 = new UdpClient(port1);
                Log.Info($"Cool device 1 listening on UDP port {port1}");
            }
            catch (Exception ex)
            {
                Log.Info($"Failed to bind UDP port {port1}: {ex.Message}");
                udpClient1 = null;
            }
            try
            {
                udpClient2 = new UdpClient(port2);
                Log.Info($"Cool device 2 listening on UDP port {port2}");
            }
            catch (Exception ex)
            {
                Log.Info($"Failed to bind UDP port {port2}: {ex.Message}");
                udpClient2 = null;
            }

            isListening = true;

            if (udpClient1 != null)
            {
                listenThread1 = new Thread(() => ListenForMessages(udpClient1, 0));
                listenThread1.Start();
            }
            if (udpClient2 != null)
            {
                listenThread2 = new Thread(() => ListenForMessages(udpClient2, 1));
                listenThread2.Start();
            }
        }

        private void ListenForMessages(UdpClient client, int deviceIndex)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                CoolDeviceInfo targetInfo = deviceIndex == 0 ? CoolDeviceInfo1 : CoolDeviceInfo2;
                while (isListening)
                {
                    if (client.Available > 0 && TempTestMode == TempMode.AFWF)
                    {
                        byte[] receiveBytes = client.Receive(ref remoteEndPoint);
                        string receiveString = Encoding.ASCII.GetString(receiveBytes);
                        string[] values = receiveString.Split(';');
                        if (Configs.Configs.IsEnableVerboseDebug)
                        {
                            Log.Info($"Received from device {deviceIndex + 1}: {receiveString}");
                        }
                        if (values.Length >= 9)
                        {
                            targetInfo.OutletWaterTemperature = Utils.ParseFloat(values[0]);
                            targetInfo.InletWaterTemperature = Utils.ParseFloat(values[1]);
                            targetInfo.WaterFlowRate = Utils.ParseFloat(values[2]);
                            targetInfo.AmbientTemperature1 = Utils.ParseFloat(values[3]);
                            targetInfo.AmbientTemperature2 = Utils.ParseFloat(values[4]);
                            targetInfo.OutletAirTemperature1 = Utils.ParseFloat(values[5]);
                            targetInfo.OutletAirTemperature2 = Utils.ParseFloat(values[6]);
                            targetInfo.OutletAirTemperature3 = Utils.ParseFloat(values[7]);
                            targetInfo.OutletAirTemperature4 = Utils.ParseFloat(values[8]);
                            // 设备2有8个出风温度（13个字段），设备1有4个出风温度（9个字段）
                            targetInfo.OutletAirTemperature5 = values.Length >= 10 ? Utils.ParseFloat(values[9]) : 0;
                            targetInfo.OutletAirTemperature6 = values.Length >= 11 ? Utils.ParseFloat(values[10]) : 0;
                            targetInfo.OutletAirTemperature7 = values.Length >= 12 ? Utils.ParseFloat(values[11]) : 0;
                            targetInfo.OutletAirTemperature8 = values.Length >= 13 ? Utils.ParseFloat(values[12]) : 0;
                            Log.Info($"Device {deviceIndex + 1}: {targetInfo}");

                            // Only update UI when this device is the currently selected source
                            if (deviceIndex == Configs.Configs.CoolDeviceSelectedIndex)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    tbOutletWater.Text = targetInfo.OutletWaterTemperature + "";
                                    tbInletWater.Text = targetInfo.InletWaterTemperature + "";
                                    tbFlow.Text = targetInfo.WaterFlowRate + "";
                                    tbEnvTemp1.Text = targetInfo.AmbientTemperature1 + "";
                                    tbEnvTemp2.Text = targetInfo.AmbientTemperature2 + "";
                                    tbOutletAir1.Text = targetInfo.OutletAirTemperature1 + "";
                                    tbOutletAir2.Text = targetInfo.OutletAirTemperature2 + "";
                                    tbOutletAir3.Text = targetInfo.OutletAirTemperature3 + "";
                                    tbOutletAir4.Text = targetInfo.OutletAirTemperature4 + "";
                                    tbOutletAir5.Text = targetInfo.OutletAirTemperature5 + "";
                                    tbOutletAir6.Text = targetInfo.OutletAirTemperature6 + "";
                                    tbOutletAir7.Text = targetInfo.OutletAirTemperature7 + "";
                                    tbOutletAir8.Text = targetInfo.OutletAirTemperature8 + "";
                                });
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.Interrupted)
                {
                    Log.Info($"Socket exception (device {deviceIndex + 1}): {ex.Message}");
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

            udpClient1?.Close();
            udpClient2?.Close();

            if (listenThread1 != null && listenThread1.IsAlive)
            {
                listenThread1.Join();
            }
            if (listenThread2 != null && listenThread2.IsAlive)
            {
                listenThread2.Join();
            }

            Log.Info("Stopped listening.");
        }

        private void UpdateCurrentCoolDevice()
        {
            int index = Configs.Configs.CoolDeviceSelectedIndex;
            CurrentCoolDeviceInfo = index == 0 ? CoolDeviceInfo1 : CoolDeviceInfo2;
            Log.Info($"Cool device source switched to device {index + 1}");
            Dispatcher.Invoke(() =>
            {
                panelCoolDeviceExpandInfo.Visibility = index == 0 ? Visibility.Collapsed : Visibility.Visible; 
            });
        }

        private void cbCoolDeviceSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            Configs.Configs.CoolDeviceSelectedIndex = cbCoolDeviceSource.SelectedIndex;
            UpdateCurrentCoolDevice();
            // Immediately refresh UI with the newly selected device's data
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
                tbOutletAir5.Text = CurrentCoolDeviceInfo.OutletAirTemperature5 + "";
                tbOutletAir6.Text = CurrentCoolDeviceInfo.OutletAirTemperature6 + "";
                tbOutletAir7.Text = CurrentCoolDeviceInfo.OutletAirTemperature7 + "";
                tbOutletAir8.Text = CurrentCoolDeviceInfo.OutletAirTemperature8 + "";
            });
        }
        #endregion

        private void MenuItemAddRow_Click(object sender, RoutedEventArgs e)
        {
            if (IsCollecting)
            {
                MessageBox.Show("采集过程中无法手动添加行，请先停止采集！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (Table.Columns.Count == 0)
            {
                MessageBox.Show("请先点击启动采集以初始化表格！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DataRow newRow = Table.NewRow();
            newRow["序号"] = ++Index;
            newRow["时间"] = DateTime.Now;
            Table.Rows.Add(newRow);
            ScrollToEnd(dgTempRecord);
        }

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

        private void MenuItemToggleAutoScroll_Click(object sender, RoutedEventArgs e)
        {
            IsAutoScroll = !IsAutoScroll;
        }

        private void btToggleRealTimeTemp_Click(object sender, RoutedEventArgs e)
        {
            if (SlotWrapPanel.Visibility == Visibility.Visible)
                SlotWrapPanel.Visibility = Visibility.Collapsed;
            else
                SlotWrapPanel.Visibility = Visibility.Visible;
        }

        private bool IsChartVisible = true;

        private void btToggleChart_Click(object sender, RoutedEventArgs e)
        {
            IsChartVisible = !IsChartVisible;
            if (IsChartVisible)
            {
                chartPanel.Visibility = Visibility.Visible;
                colDataGrid.Width = new GridLength(0, GridUnitType.Auto);
                colChart.Width = new GridLength(1, GridUnitType.Star);
                btToggleChart.Content = "隐藏折线图";
                dgTempRecord.MinWidth = 600;
            }
            else
            {
                chartPanel.Visibility = Visibility.Collapsed;
                colDataGrid.Width = new GridLength(1, GridUnitType.Star);
                colChart.Width = new GridLength(0, GridUnitType.Auto);
                btToggleChart.Content = "显示折线图";
                dgTempRecord.MinWidth = 0;
            }
        }
    }
}

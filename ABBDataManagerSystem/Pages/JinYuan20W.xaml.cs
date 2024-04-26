using ABBDataManagerSystem.Connector;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// JinYuan20W.xaml 的交互逻辑
    /// </summary>
    public partial class JinYuan20W : UserControl
    {
        private JinYuan20WCollector? Collector = null;
        private List<User> items = new List<User>();
        private ObservableCollection<MyItem> items2;
        private bool IsConneted = false;
        private bool IsCollecting = false;
        private ManualResetEvent? ResetEvent = null;
        private bool CommandChange = false;

        public JinYuan20W()
        {
            InitializeComponent();

            items.Add(new User() { Current = 101.2f, SortIndex = 42, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 39, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 42, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 39, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 42, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 39, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 42, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 39, Resistance = 222.2f });
            items.Add(new User() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            lvUsers.ItemsSource = items;

            // 数据模型
            items2 = new ObservableCollection<MyItem>
            {
                new MyItem { Name = "项目1", Description = "这是项目1的描述" },
                new MyItem { Name = "项目2", Description = "这是项目2的描述" },
                new MyItem { Name = "项目3", Description = "这是项目3的描述" }
            };

            // 数据绑定
            this.DataContext = new { Items = items2 };
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            Array.Sort(ports);
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
            UpdateControlEnableState();
        }


        public class User
        {
            public float Current { get; set; }
            public int SortIndex { get; set; }
            public float Resistance { get; set; }
        }

        private void btAddRecord_Click(object sender, RoutedEventArgs e)
        {
            items.Add(new User() { Current = 101.2f, SortIndex = 7, Resistance = 222.2f });
            lvUsers.Items.Refresh();

            items2.Add(new MyItem() { Name = "新增", Description = "这是新增的" });
        }

        public class MyItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private void btDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (items.Count > 0)
            {
                items.RemoveAt(items.Count - 1);
                lvUsers.Items.Refresh();
            }
            if (items2.Count > 0)
            {
                items2.RemoveAt(items2.Count - 1);
            }
        }

        private void swConnect_CheckedChange(object sender, RoutedEventArgs e)
        {
            if (swConnect.IsChecked == true)
            {
                IsConneted = true;
                Collector = new JinYuan20WCollector(cbSerialPort.SelectedItem.ToString(), Utils.ParseInt(cbBoundRate.Text));
                if (!Collector.Connect())
                {
                    Collector = null;
                    IsConneted = false;
                    swConnect.IsChecked = false;
                }
            }
            else
            {
                StopCollect();
                if (Collector != null)
                {
                    Collector.Disconnect();
                    Collector = null;
                }
                IsConneted = false;
                
            }
            UpdateControlEnableState();
        }

        private void UpdateControlEnableState()
        {
            cbBoundRate.IsEnabled = !IsConneted;
            cbSerialPort.IsEnabled = !IsConneted;

            btStart.IsEnabled = IsConneted && !IsCollecting;
            btStop.IsEnabled = IsConneted && IsCollecting;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            IsCollecting = false;
            if (Collector != null)
            {
                Collector.Disconnect();
                Collector = null;
            }
        }

        private void CollectDataOnce()
        {
            if (Collector != null)
            {
                if (CommandChange)
                {
                    CommandChange = false;

                }
                var packet = Collector.ReadPacket();
                if (packet != null)
                {
                }
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (!IsConneted || IsCollecting)
            {
                return;
            }
            // 创建一个ManualResetEvent，初始状态为未设置（false）  
            ResetEvent = new ManualResetEvent(false);
            IsCollecting = true;
            UpdateControlEnableState();
            Log.Info("Start 20W Collect...");

            // 创建一个新线程并执行任务  
            new Thread(() =>
            {
                while (IsCollecting)
                {
                    CollectDataOnce();
                    if (ResetEvent.WaitOne(JinYuan20WCollector.Interval))
                    {
                        // 线程没有超时被唤醒，说明要停止循环了
                    }
                }
                IsCollecting = false;
                ResetEvent = null;
                Log.Info("Temp Collector DONE");
            }).Start();
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            StopCollect();
        }

        private void StopCollect()
        {
            if (!IsCollecting)
            {
                return;
            }
            IsCollecting = false;
            if (ResetEvent != null)
            {
                ResetEvent.Set();
            }
            UpdateControlEnableState();
        }
    }

}

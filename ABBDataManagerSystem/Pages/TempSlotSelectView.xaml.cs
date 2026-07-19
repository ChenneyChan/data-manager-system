using ABBDataManagerSystem.Configs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// TempSlotSelectView.xaml 的交互逻辑
    /// </summary>
    public partial class TempSlotSelectView : Window
    {
        private readonly int MaxSlot = 36;
        public ObservableCollection<TemperatureChannelSetting> Channels { get; } = new();
        public List<string> SlotOptions { get; } = new();

        public TempSlotSelectView(int maxSlot = 36, TempMode tempMode = TempMode.COMMON)
        {
            InitializeComponent();
            MaxSlot = maxSlot;
            DataContext = this;
            tbMaxSlot.Text = MaxSlot.ToString();
            InitSlotOptions();
            InitView(tempMode);
        }

        private void InitSlotOptions()
        {
            SlotOptions.Clear();
            SlotOptions.Add(string.Empty);
            for (int i = 0; i < MaxSlot; i++)
            {
                SlotOptions.Add($"Slot-{i + 1}");
            }
        }

        private void InitView(TempMode tempMode)
        {
            Channels.Clear();
            var saved = Configs.Configs.TemperatureChannels ?? TemperatureChannelCatalog.CreateDefaultChannels();
            foreach (var item in saved.Select(item => item.Clone()).ToList())
            {
                Channels.Add(item);
            }
            for (int i = 0; i < Channels.Count; i++)
            {
                Channels[i].RowIndex = i + 1;
            }

            dgChannels.ItemsSource = Channels;
            var activeCount = Channels.Count(item => item.IsActive);
            if (activeCount == 0)
            {
                activeCount = tempMode == TempMode.COMMON ? 8 : 18;
            }
            nudChannelCount.Value = activeCount;
            ApplyActiveCount(activeCount);
        }

        private void ApplyActiveCount(int count)
        {
            if (count < 0)
            {
                count = 0;
            }
            if (count > Channels.Count)
            {
                count = Channels.Count;
            }

            for (int i = 0; i < Channels.Count; i++)
            {
                Channels[i].IsActive = i < count;
                Channels[i].RowIndex = i + 1;
                if (Channels[i].IsActive && string.IsNullOrWhiteSpace(Channels[i].Title))
                {
                    Channels[i].Title = Channels[i].RoleName;
                }
            }
        }

        private void nudChannelCount_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            ApplyActiveCount((int)nudChannelCount.Value);
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            var activeChannels = Channels.Where(item => item.IsActive).ToList();
            if (activeChannels.Count == 0)
            {
                MessageBox.Show("请至少启用一个通道。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var channel in activeChannels)
            {
                if (string.IsNullOrWhiteSpace(channel.Title))
                {
                    channel.Title = channel.RoleName;
                }
                if (string.IsNullOrWhiteSpace(channel.Probe))
                {
                    MessageBox.Show($"通道“{channel.RoleName}”未选择温度探头。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }

            Configs.Configs.TemperatureChannels = Channels.Select(item => item.Clone()).ToList();
            Configs.Configs.SaveToFile();
            DialogResult = true;
            Close();
        }
    }
}

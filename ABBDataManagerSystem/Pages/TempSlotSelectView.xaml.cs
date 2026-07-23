using ABBDataManagerSystem.Configs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages
{
    public partial class TempSlotSelectView : Window
    {
        private readonly int MaxSlot = 36;
        private int _customCounter = 0;
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

            _customCounter = Channels
                .Where(c => c.RoleKey.StartsWith("Custom"))
                .Select(c => { int.TryParse(c.RoleKey.Replace("Custom", ""), out var n); return n; })
                .DefaultIfEmpty(0)
                .Max();

            RefreshRowIndexes();
            dgChannels.ItemsSource = Channels;

           var activeCount = Channels.Count(item => item.IsActive);
           nudChannelCount.Maximum = Channels.Count;
            nudChannelCount.Value = activeCount;
            ApplyActiveCount(activeCount);
        }

        private void RefreshRowIndexes()
        {
            for (int i = 0; i < Channels.Count; i++)
            {
                Channels[i].RowIndex = i + 1;
            }
        }

        private void SyncNumericUpDown()
        {
            var count = Channels.Count(item => item.IsActive);
            nudChannelCount.Maximum = Channels.Count;
            nudChannelCount.Value = count;
        }

        private void ApplyActiveCount(int count)
        {
            if (count < 0) count = 0;
            if (count > Channels.Count) count = Channels.Count;

            for (int i = 0; i < Channels.Count; i++)
            {
                Channels[i].IsActive = i < count;
                if (Channels[i].IsActive && string.IsNullOrWhiteSpace(Channels[i].Title))
                {
                    Channels[i].Title = Channels[i].RoleName;
                }
            }
            SyncNumericUpDown();
        }

        private void nudChannelCount_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            ApplyActiveCount((int)nudChannelCount.Value);
        }

        private void CheckBox_IsActive_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext is TemperatureChannelSetting channel)
            {
                channel.IsActive = cb.IsChecked == true;
            }
            SyncNumericUpDown();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = tbNewChannelName.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("请输入通道名称。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _customCounter++;
            var channel = new TemperatureChannelSetting
            {
                RoleKey = $"Custom{_customCounter}",
                RoleName = name,
                Title = name,
                Probe = string.Empty,
                IsActive = true,
            };
            Channels.Add(channel);
            tbNewChannelName.Clear();
            RefreshRowIndexes();
            SyncNumericUpDown();
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgChannels.SelectedItem is TemperatureChannelSetting selected)
            {
                Channels.Remove(selected);
                RefreshRowIndexes();
                SyncNumericUpDown();
            }
            else
            {
                MessageBox.Show("请先选中要删除的通道行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                    MessageBox.Show($"通道\u201C{channel.RoleName}\u201D未选择温度探头。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

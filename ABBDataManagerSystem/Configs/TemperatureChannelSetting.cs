using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ABBDataManagerSystem.Configs
{
    public class TemperatureChannelSetting : INotifyPropertyChanged
    {
        private string roleKey = string.Empty;
        private string roleName = string.Empty;
        private string title = string.Empty;
        private string probe = string.Empty;
        private bool isActive = true;
        private int rowIndex = 0;

        public string RoleKey
        {
            get => roleKey;
            set
            {
                if (roleKey == value) return;
                roleKey = value;
                OnPropertyChanged();
            }
        }

        public string RoleName
        {
            get => roleName;
            set
            {
                if (roleName == value) return;
                roleName = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => title;
            set
            {
                if (title == value) return;
                title = value;
                OnPropertyChanged();
            }
        }

        public string Probe
        {
            get => probe;
            set
            {
                if (probe == value) return;
                probe = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive == value) return;
                isActive = value;
                OnPropertyChanged();
            }
        }

        public int RowIndex
        {
            get => rowIndex;
            set
            {
                if (rowIndex == value) return;
                rowIndex = value;
                OnPropertyChanged();
            }
        }

        public TemperatureChannelSetting Clone()
        {
            return new TemperatureChannelSetting
            {
                RoleKey = RoleKey,
                RoleName = RoleName,
                Title = Title,
                Probe = Probe,
                IsActive = IsActive,
                RowIndex = RowIndex,
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class TemperatureChannelCatalog
    {
        public static List<TemperatureChannelSetting> CreateDefaultChannels()
        {
            return new List<TemperatureChannelSetting>
            {
                Create("WindingA", "绕组A", ""),
                Create("WindingB", "绕组B", ""),
                Create("WindingC", "绕组C", ""),
                Create("Core", "铁心", ""),
                Create("EnvA", "环境A", ""),
                Create("EnvB", "环境B", ""),
                Create("EnvC", "环境C", ""),
                Create("EnvD", "环境D", ""),
                Create("Outlet1", "出风口温度1", ""),
                Create("Outlet2", "出风口温度2", ""),
                Create("Outlet3", "出风口温度3", ""),
                Create("Outlet4", "出风口温度4", ""),
                Create("Outlet5", "出风口温度5", ""),
                Create("Outlet6", "出风口温度6", ""),
                Create("Inlet1", "进风口温度1", ""),
                Create("Inlet2", "进风口温度2", ""),
                Create("Inlet3", "进风口温度3", ""),
                Create("TopTemperature", "壳内顶部温度", ""),
                Create("Extension1", "额外温度1", ""),
                Create("Extension2", "额外温度2", ""),
                Create("Extension3", "额外温度3", ""),
                Create("Extension4", "额外温度4", ""),
                Create("Extension5", "额外温度5", ""),
                Create("Extension6", "额外温度6", ""),
                Create("Extension7", "额外温度7", ""),
                Create("Extension8", "额外温度8", ""),
                Create("Extension9", "额外温度9", ""),
            };
        }

        public static List<TemperatureChannelSetting> NormalizeChannels(IEnumerable<TemperatureChannelSetting>? saved)
        {
            var defaults = CreateDefaultChannels();
            if (saved == null)
            {
                return defaults;
            }

            var lookup = new Dictionary<string, TemperatureChannelSetting>();
            foreach (var item in saved)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.RoleKey))
                {
                    continue;
                }
                lookup[item.RoleKey] = item;
            }

            foreach (var item in defaults)
            {
                if (!lookup.TryGetValue(item.RoleKey, out var savedItem))
                {
                    continue;
                }

                item.Title = string.IsNullOrWhiteSpace(savedItem.Title) ? item.Title : savedItem.Title;
                item.Probe = savedItem.Probe ?? string.Empty;
                item.IsActive = savedItem.IsActive;
            }

            return defaults;
        }

        private static TemperatureChannelSetting Create(string roleKey, string roleName, string probe)
        {
            return new TemperatureChannelSetting
            {
                RoleKey = roleKey,
                RoleName = roleName,
                Title = roleName,
                Probe = probe,
                IsActive = !string.IsNullOrWhiteSpace(probe),
            };
        }
    }
}

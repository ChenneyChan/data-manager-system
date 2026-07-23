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
        // 不预置任何语义通道：通道列表完全由用户自定义，所有通道地位平等。
        // 新装用户从空列表开始，通过 TempSlotSelectView 添加并命名通道。
        public static List<TemperatureChannelSetting> CreateDefaultChannels()
        {
            return new List<TemperatureChannelSetting>();
        }

        // 归一化保存的通道：按 RoleKey 去重，空标题回填 RoleName；不再依赖任何预设默认值。
        public static List<TemperatureChannelSetting> NormalizeChannels(IEnumerable<TemperatureChannelSetting>? saved)
        {
            var result = new List<TemperatureChannelSetting>();
            if (saved == null)
            {
                return result;
            }

            var seen = new HashSet<string>();
            foreach (var item in saved)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.RoleKey))
                {
                    continue;
                }
                if (!seen.Add(item.RoleKey))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.Title))
                {
                    item.Title = item.RoleName;
                }
                result.Add(item);
            }

            return result;
        }
    }
}

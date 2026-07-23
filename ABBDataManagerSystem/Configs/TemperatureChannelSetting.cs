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
       // 通道无预设语义：所有通道地位平等，使用通用 RoleKey（Channel1、Channel2...），
       // 用户可给通道的标题任意命名。
       // 当本地配置、DB 均为空时使用预设模板给用户一个合理起点初始通道列表，
       // 用户可在使用前完成探头分配，也会在这个基础上修改。
       public static List<TemperatureChannelSetting> CreateDefaultChannels()
       {
           return CreatePresetTemplate();
       }

       // 预设模板给用户一个合理的起点：通用 RoleKey，且名称直观，用户可按需重命名。
       // 通道默认为非激活（probe 为空），用户指定实物卡座号后才激活。
       public static List<TemperatureChannelSetting> CreatePresetTemplate()
       {
           return new List<TemperatureChannelSetting>
           {
               Create("Channel1", "绕组A"),
               Create("Channel2", "绕组B"),
               Create("Channel3", "绕组C"),
               Create("Channel4", "铁心"),
               Create("Channel5", "环境A"),
               Create("Channel6", "环境B"),
               Create("Channel7", "环境C"),
               Create("Channel8", "环境D"),
           };
       }

       private static TemperatureChannelSetting Create(string roleKey, string roleName)
       {
           return new TemperatureChannelSetting
           {
               RoleKey = roleKey,
               RoleName = roleName,
               Title = roleName,
               Probe = string.Empty,
               IsActive = false,
           };
       }

       // 归一化保存的通道：按 RoleKey 去重，空标题回填 RoleName。
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

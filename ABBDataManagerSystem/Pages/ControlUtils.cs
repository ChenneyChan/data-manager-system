using System.Windows;

namespace ABBDataManagerSystem.Pages
{
    internal class ControlUtils
    {
        public static bool ShowClearMessage()
        {
            var result = MessageBox.Show("确认清空当前试验数据？", "提醒", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}

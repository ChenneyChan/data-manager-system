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

        public static bool ShowUploadConfirm()
        {
            var result = MessageBox.Show("确认上传该数据么？如果确认，如果服务器已有数据则会被覆盖！", "提醒", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }
    }
}

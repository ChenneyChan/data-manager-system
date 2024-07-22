using ABBDataManagerSystem.Bean.Base;
using System.Windows;
using System.Windows.Controls;

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

        public static bool CheckWorkflowBeforeUpload()
        {
            if (!ControlUtils.ShowUploadConfirm())
            {
                return false;
            }
            if (Configs.Configs.WorkflowID.Length == 0)
            {
                MessageBox.Show("请先选择工作令！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public static void ShowUploadTips(bool uploadRet)
        {
            if (uploadRet)
            {
                MessageBox.Show("数据上传成功！", "上传结果", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("数据上传失败，请检查数据和服务器连接情况后重试！", "上传结果", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void HandleVoltageCurrentTextChange(object sender, Dictionary<string, TextBox> fields,
            List<VoltageCurrentLossDataInfo>? currentLossDataInfoList)
        {
            if (currentLossDataInfoList == null)
            {
                return;
            }
            string key = "";
            foreach (var item in fields)
            {
                if (item.Value == sender)
                {
                    key = item.Key;
                    break;
                }
            }
            if (key == "")
            {
                return;
            }
            var keys = key.Split("_");
            if (keys.Length < 2)
            {
                return;
            }
            int? index = Utils.ParseInt(keys[1]);
            if (index == null || index <= 0)
            {
                return;
            }
            if (currentLossDataInfoList.Count < index)
            {
                return;
            }
            var data = currentLossDataInfoList.ToArray()[(int)index - 1];

            var type = typeof(VoltageCurrentLossDataInfo);
            var field = type.GetField(keys[0]);
            if (field == null)
            {
                return;
            }
            field.SetValue(data, Utils.ParseFloatNull(((TextBox)sender).Text));
        }
    }
}

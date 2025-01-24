using ABBDataManagerSystem.Tools;
using MySql.Data.MySqlClient;
using System.Windows;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// WindowSettings.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSettings : Window
    {
        public WindowSettings()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Configs.Configs.LoadFromFile();
            tbDatabasePort.Text = Configs.Configs.Port + "";
            tbDatabaseName.Text = Configs.Configs.DatabaseName;
            tbDatabaseUserName.Text = Configs.Configs.Username;
            tbDatabasePassword.Password = Configs.Configs.Password;
            tbDatabaseIp.Text = Configs.Configs.Host;

            cbWorkStation.SelectedIndex = Configs.Configs.WorkStationNo - 1;
            cbEnableRatioInputMode.IsChecked = Configs.Configs.IsEnableRatioInputMode;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Configs.Configs.SaveToFile();
        }

        private void btComfirm_Click(object sender, RoutedEventArgs e)
        {
            Configs.Configs.Port = Utils.ParseInt(tbDatabasePort.Text);
            Configs.Configs.DatabaseName = tbDatabaseName.Text;
            Configs.Configs.Username = tbDatabaseUserName.Text;
            Configs.Configs.Password = tbDatabasePassword.Password;
            Configs.Configs.Host = tbDatabaseIp.Text;
            Configs.Configs.WorkStationNo = cbWorkStation.SelectedIndex + 1;
            Configs.Configs.IsEnableRatioInputMode = cbEnableRatioInputMode.IsChecked ?? false;
            DialogResult = true;

            Tools.EventManager.Instance.TriggerEvent("RatioInputModeChanged", this, new TestEventArgs() { obj = Configs.Configs.IsEnableRatioInputMode });
            Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btPing_Click(object sender, RoutedEventArgs e)
        {
            if (tbDatabaseIp.Text.Trim().Length == 0)
            {
                MessageBox.Show("IP地址错误！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Utils.IsPingable(tbDatabaseIp.Text))
            {
                MessageBox.Show("服务器地址网络检测通过！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                MessageBox.Show("服务器地址网络检测失败，请检查网络配置！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btDBTest_Click(object sender, RoutedEventArgs e)
        {
            string sqlConnectStr = "server=" + tbDatabaseIp.Text.Trim() + ";user=" + tbDatabaseUserName.Text.Trim()
                    + ";database=" + tbDatabaseName.Text.Trim() + ";port=" + Utils.ParseInt(tbDatabasePort.Text) + ";password=" + tbDatabasePassword.Password;

            if (CheckMySqlConnection(sqlConnectStr))
            {
                MessageBox.Show("数据库服务器连接检测通过！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                MessageBox.Show("数据库服务器连接检测失败，请检查数据库配置！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public static bool CheckMySqlConnection(string connectionString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open(); // 尝试打开连接
                    connection.Close();
                    return true; // 连接成功
                }
                catch (MySqlException)
                {
                    return false; // 连接失败
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            tbDatabasePassword.Text = Configs.Configs.Password;
            tbDatabaseIp.Text = Configs.Configs.Host;
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
            Configs.Configs.Password = tbDatabasePassword.Text;
            Configs.Configs.Host = tbDatabaseIp.Text;
            DialogResult = true;
            Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

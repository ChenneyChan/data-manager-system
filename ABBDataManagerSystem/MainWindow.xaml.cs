using ABBDataManagerSystem.Pages;
using ABBDataManagerSystem.PowerAnalyzer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ABBDataManagerSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Configs.Configs.LoadFromFile();
        }

        private void MenuItemSetting_Click(object sender, RoutedEventArgs e)
        {
            new WindowSettings().ShowDialog();
        }

        private void PowerAnalyzeTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window()
            {
                Title = "功率分析仪",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            var uc = new UCDeviceSearch();
            window.Content = uc;
            window.Show();
        }

        private void TemperatureTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window()
            {
                Title = "温度检测仪",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            var uc = new TempTestPage();
            window.Content = uc;
            window.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Configs.Configs.SaveToFile();     
        }
    }
}
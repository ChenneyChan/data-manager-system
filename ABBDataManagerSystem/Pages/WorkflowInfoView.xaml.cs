using ABBDataManagerSystem.Bean.Base;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// WorkflowInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class WorkflowInfoView : Window
    {
        private DataTable dt = new DataTable();

        public WorkflowInfoView()
        {
            InitializeComponent();
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WorkflowInfo.FillDataTable(dt, 20, 0);
            dataGrid.ItemsSource = dt.AsDataView();
        }
    }
}

using ABBDataManagerSystem.Bean.Base;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// WorkflowInfoView.xaml 的交互逻辑
    /// </summary>
    public partial class WorkflowInfoView : Window
    {
        private DataTable dt = new DataTable();
        private int TotalCount = 0;
        private int CurrentPage = 0; // 从0开始
        private int CountPerPage = 20;
        private int PageCount = 0;
        private bool IsFirst = true;

        public WorkflowInfoView()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTable();
            UpdatePagenation();
        }

        private void UpdateTable()
        {
            TotalCount = WorkflowInfo.GetTotalCount();
            PageCount = (int)Math.Ceiling((float)TotalCount / CountPerPage);
            if (TotalCount > 0)
            {
                dt.Rows.Clear();
                WorkflowInfo.FillDataTable(dt, CountPerPage, CurrentPage);
                if (IsFirst)
                {
                    dataGrid.ItemsSource = dt.AsDataView();
                    IsFirst = false;
                    foreach (var column in dataGrid.Columns)
                    {
                        if (!WorkflowInfo.FieldComments.ContainsKey(column.Header.ToString()))
                        {
                            column.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            column.Header = WorkflowInfo.FieldComments[column.Header.ToString()];
                        }
                    }
                }
            }
        }

        private void UpdatePagenation()
        {
            btNext.IsEnabled = CurrentPage + 1 < PageCount;
            btLast.IsEnabled = CurrentPage + 1 < PageCount;
            btPrev.IsEnabled = CurrentPage > 0;
            btFirst.IsEnabled = CurrentPage > 0;
            tbPageInfo.Text = $"共{PageCount}页，当前第{CurrentPage + 1}页";
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            // 获取选中行
            var selectedRow = dataGrid.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                // 通过列名获取值
                var columnValue = selectedRow["ID"];

                // 将值转换为相应的类型
                if (columnValue != null)
                {
                    // 根据需要转换为相应的类型
                    string? columnValueString = columnValue.ToString();
                    Configs.Configs.WorkflowID = columnValueString ?? "";
                    Tools.EventManager.Instance.TriggerEvent("WorkflowSelected", this, new Tools.TestEventArgs());
                }
                Close();
            }
            else
            {
                MessageBox.Show("请选择一个工作令", "选择结果", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btFirst_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 0;
            UpdateTable();
            UpdatePagenation();
        }

        private void btLast_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = PageCount - 1;
            UpdateTable();
            UpdatePagenation();
        }

        private void btPrev_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage -= 1;
            UpdateTable();
            UpdatePagenation();
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage += 1;
            UpdateTable();
            UpdatePagenation();
        }

        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 获取选中行
            var selectedRow = dataGrid.SelectedItem as DataRowView;
            if (selectedRow != null)
            {
                // 通过列名获取值
                var columnValue = selectedRow["ID"];

                // 将值转换为相应的类型
                if (columnValue != null)
                {
                    // 根据需要转换为相应的类型
                    string? columnValueString = columnValue.ToString();
                    Configs.Configs.WorkflowID = columnValueString ?? "";
                    Tools.EventManager.Instance.TriggerEvent("WorkflowSelected", this, new Tools.TestEventArgs());
                }
                Close();
            }
        }
    }
}

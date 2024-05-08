using System.Data;
using System.Windows;

namespace ABBDataManagerSystem.PowerAnalyzer
{
    /// <summary>
    /// HamonicInfo.xaml 的交互逻辑
    /// </summary>
    public partial class HarmonicInfo : Window
    {
        DataTable DataTableElementA;

        DataTable DataTableElementB;

        DataTable DataTableElementC;

        public HarmonicInfo()
        {
            InitializeComponent();

            DataTableElementA = new DataTable();
            DataTableElementB = new DataTable();
            DataTableElementC = new DataTable();
            DataTableElementA.Columns.Add("次数", typeof(string));
            DataTableElementA.Columns.Add("电压", typeof(float));
            DataTableElementA.Columns.Add("电流", typeof(float));
            DataTableElementA.Columns.Add("功率", typeof(float));

            DataTableElementA.Rows.Add("Total", 0, 0, 0);
            for (int i = 1; i <= 30; i++)
            {
                DataTableElementA.Rows.Add(i.ToString(), 0, 0, 0);
            }

            this.DataContext = new { DataTableElementA = DataTableElementA, DataTableElementB = DataTableElementA, DataTableElementC = DataTableElementA };
        }
    }
}

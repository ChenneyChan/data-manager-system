using System.Data;
using System.Windows;
using static ABBDataManagerSystem.PowerAnalyzer.UCPowerAanlyzer;

namespace ABBDataManagerSystem.PowerAnalyzer
{

    public delegate void OnClosed();

    /// <summary>
    /// HamonicInfo.xaml 的交互逻辑
    /// </summary>
    public partial class HarmonicInfo : Window
    {
        private OnClosed? onClosed = null;

        private int PhaseHarmonicCount = 0;

        DataTable DataTableElementA;

        DataTable DataTableElementB;

        DataTable DataTableElementC;

        public HarmonicInfo(int phaseHarmonicCount = 36, OnClosed? onClosed = null)
        {
            InitializeComponent();
            this.onClosed = onClosed;
            this.PhaseHarmonicCount = phaseHarmonicCount;

            DataTableElementA = InitDataTable();
            DataTableElementB = InitDataTable();
            DataTableElementC = InitDataTable();

            this.DataContext = new { DataTableElementA = DataTableElementA, DataTableElementB = DataTableElementB, DataTableElementC = DataTableElementC };
        }

        private DataTable InitDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("次数", typeof(string));
            dt.Columns.Add("电压", typeof(float));
            dt.Columns.Add("电流", typeof(float));
            dt.Columns.Add("功率", typeof(float));

            dt.Rows.Add("Total", 0, 0, 0);
            dt.Rows.Add("DC", 0, 0, 0);
            for (int i = 1; i <= PhaseHarmonicCount; i++)
            {
                dt.Rows.Add(i.ToString(), 0, 0, 0);
            }
            return dt;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (onClosed != null) onClosed();
        }

        public void HandleUpdate(List<FCODefine> items, int harmonicOffset, int phaseHarmonicCount)
        {
            int singlePhaseCount = 1 + 1 + PhaseHarmonicCount;
            int phaseTotalCount = 3 * singlePhaseCount; // （Total、DC、1~PhaseHarmonicCount）* 3（UK\IK\PK）
            for (int i = harmonicOffset; i < items.Count; i++)
            {
                int index = i - harmonicOffset;
                int phase = (int)(index / phaseTotalCount) + 1;
                DataTable? dt = null;
                switch (phase)
                {
                    case 1:
                        dt = DataTableElementA;
                        break;
                    case 2:
                        dt = DataTableElementB;
                        break;
                    case 3:
                        dt = DataTableElementC;
                        break;
                }
                if (dt == null)
                {
                    break;
                }
                int offset = index % phaseTotalCount;
                int ukikpk = (int)(offset / singlePhaseCount);
                string secondKey = ukikpk == 0 ? "电压" : (ukikpk == 1 ? "电流" : "功率");
                index = offset % singlePhaseCount;
                dt.Rows[index][secondKey] = items[i].Value != null ? items[i].Value : -1;
            }
        }
    }
}

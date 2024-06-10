using System.Windows;
using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// TempSlotSelectView.xaml 的交互逻辑
    /// </summary>
    public partial class TempSlotSelectView : Window
    {
        private int MaxSlot = 36;
        public TempSlotSelectView(int maxSlot = 36)
        {
            InitializeComponent();
            this.MaxSlot = maxSlot;
            InitView();
        }

        private void InitComboBox(ComboBox cb)
        {
            for (int i = 0; i < MaxSlot; i++)
            {
                cb.Items.Add($"Slot-{i + 1}");
            }
        }

        private void InitView()
        {
            InitComboBox(cbWindingA);
            InitComboBox(cbWindingB);
            InitComboBox(cbWindingC);
            InitComboBox(cbCore);
            InitComboBox(cbEnvA);
            InitComboBox(cbEnvB);
            InitComboBox(cbEnvC);
            InitComboBox(cbEnvD);
            cbWindingA.Text = Configs.Configs.WindingA;
            cbWindingB.Text = Configs.Configs.WindingB;
            cbWindingC.Text = Configs.Configs.WindingC;
            cbCore.Text = Configs.Configs.Core;
            cbEnvA.Text = Configs.Configs.EnvA;
            cbEnvB.Text = Configs.Configs.EnvB;
            cbEnvC.Text = Configs.Configs.EnvC;
            cbEnvD.Text = Configs.Configs.EnvD;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            Configs.Configs.WindingA = cbWindingA.Text;
            Configs.Configs.WindingB = cbWindingB.Text;
            Configs.Configs.WindingC = cbWindingC.Text;
            Configs.Configs.Core = cbCore.Text;
            Configs.Configs.EnvA = cbEnvA.Text;
            Configs.Configs.EnvB = cbEnvB.Text;
            Configs.Configs.EnvC = cbEnvC.Text;
            Configs.Configs.EnvD = cbEnvD.Text;

            DialogResult = true;
            Close();
        }
    }
}

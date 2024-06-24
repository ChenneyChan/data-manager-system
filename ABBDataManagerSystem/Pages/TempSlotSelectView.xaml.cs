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
        public TempSlotSelectView(int maxSlot = 36, bool isAFWF = false)
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
            InitComboBox(cbOutlet1);
            InitComboBox(cbOutlet2);
            InitComboBox(cbOutlet3);
            InitComboBox(cbOutlet4);
            InitComboBox(cbOutlet5);
            InitComboBox(cbOutlet6);
            InitComboBox(cbInlet1);
            InitComboBox(cbInlet2);
            InitComboBox(cbInlet3);
            InitComboBox(cbTop);
            cbWindingA.Text = Configs.Configs.WindingA;
            cbWindingB.Text = Configs.Configs.WindingB;
            cbWindingC.Text = Configs.Configs.WindingC;
            cbCore.Text = Configs.Configs.Core;
            cbEnvA.Text = Configs.Configs.EnvA;
            cbEnvB.Text = Configs.Configs.EnvB;
            cbEnvC.Text = Configs.Configs.EnvC;
            cbEnvD.Text = Configs.Configs.EnvD;
            var outlets = Configs.Configs.OutletTemperature.Split(",");
            var inlets = Configs.Configs.InletTemperature.Split(",");
            cbOutlet1.Text = outlets.Length > 0 ? outlets[0] : "";
            cbOutlet2.Text = outlets.Length > 1 ? outlets[1] : "";
            cbOutlet3.Text = outlets.Length > 2 ? outlets[2] : "";
            cbOutlet4.Text = outlets.Length > 3 ? outlets[3] : "";
            cbOutlet5.Text = outlets.Length > 4 ? outlets[4] : "";
            cbOutlet6.Text = outlets.Length > 5 ? outlets[5] : "";
            cbInlet1.Text = inlets.Length > 0 ? inlets[0] : "";
            cbInlet2.Text = inlets.Length > 1 ? inlets[1] : "";
            cbInlet3.Text = inlets.Length > 2 ? inlets[2] : "";
            cbTop.Text = Configs.Configs.TopTemperature;
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
            Configs.Configs.OutletTemperature = cbOutlet1.Text + "," + cbOutlet2.Text + "," + cbOutlet3.Text + "," + cbOutlet4.Text + "," + cbOutlet5.Text + "," + cbOutlet6.Text;
            Configs.Configs.InletTemperature = cbInlet1.Text + "," + cbInlet2.Text + "," + cbInlet3.Text;
            Configs.Configs.TopTemperature = cbTop.Text;
            DialogResult = true;
            Close();
        }
    }
}

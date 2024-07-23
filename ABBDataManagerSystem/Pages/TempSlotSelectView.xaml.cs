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

        private void InitComboBox(ComboBox cb, bool isExtension = false)
        {
            if (isExtension)
            {
                cb.Items.Add("");
            }
            for (int i = 0; i < MaxSlot; i++)
            {
                cb.Items.Add($"Slot-{i + 1}");
            }
        }

        private void InitView()
        {
            InitComboBox(cbWindingA, true);
            InitComboBox(cbWindingB, true);
            InitComboBox(cbWindingC, true);
            InitComboBox(cbCore, true);
            InitComboBox(cbEnvA, true);
            InitComboBox(cbEnvB, true);
            InitComboBox(cbEnvC, true);
            InitComboBox(cbEnvD, true);
            InitComboBox(cbOutlet1, true);
            InitComboBox(cbOutlet2, true);
            InitComboBox(cbOutlet3, true);
            InitComboBox(cbOutlet4, true);
            InitComboBox(cbOutlet5, true);
            InitComboBox(cbOutlet6, true);
            InitComboBox(cbInlet1, true);
            InitComboBox(cbInlet2, true);
            InitComboBox(cbInlet3, true);
            InitComboBox(cbTop, true);
            InitComboBox(cbExtension1, true);
            InitComboBox(cbExtension2, true);
            InitComboBox(cbExtension3, true);
            InitComboBox(cbExtension4, true);
            InitComboBox(cbExtension5, true);
            InitComboBox(cbExtension6, true);
            InitComboBox(cbExtension7, true);
            InitComboBox(cbExtension8, true);
            InitComboBox(cbExtension9, true);
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

            var extensions = Configs.Configs.ExtensionSlots.Split(",");
            cbExtension1.Text = extensions.Length > 0 ? extensions[0] : "";
            cbExtension2.Text = extensions.Length > 1 ? extensions[1] : "";
            cbExtension3.Text = extensions.Length > 2 ? extensions[2] : "";
            cbExtension4.Text = extensions.Length > 3 ? extensions[3] : "";
            cbExtension5.Text = extensions.Length > 4 ? extensions[4] : "";
            cbExtension6.Text = extensions.Length > 5 ? extensions[5] : "";
            cbExtension7.Text = extensions.Length > 6 ? extensions[6] : "";
            cbExtension8.Text = extensions.Length > 7 ? extensions[7] : "";
            cbExtension9.Text = extensions.Length > 8 ? extensions[8] : "";
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
            Configs.Configs.ExtensionSlots = cbExtension1.Text + "," + cbExtension2.Text + "," + cbExtension3.Text + "," + cbExtension4.Text + "," +
                cbExtension5.Text + "," + cbExtension6.Text + "," + cbExtension7.Text + "," + cbExtension8.Text + "," + cbExtension9.Text;
            DialogResult = true;
            Close();
        }
    }
}

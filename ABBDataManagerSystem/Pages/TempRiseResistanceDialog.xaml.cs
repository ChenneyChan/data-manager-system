using System.Windows;

namespace ABBDataManagerSystem.Pages
{
    /// <summary>
    /// TempRiseResistanceDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TempRiseResistanceDialog : Window
    {
        public int? Slot1VoltageType { get; set; } = null;
        public int? Slot2VoltageType { get; set; } = null;

        public TempRiseResistanceDialog()
        {
            InitializeComponent();
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ComboBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox2.SelectedIndex && comboBox1.SelectedIndex >= 0)
            {
                MessageBox.Show("通道一通道二需要选择不同的类型");
                comboBox1.SelectedIndex = -1;
                return;
            }
            Slot1VoltageType = comboBox1.SelectedIndex;
        }

        private void ComboBox2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox2.SelectedIndex && comboBox1.SelectedIndex >= 0)
            {
                MessageBox.Show("通道一通道二需要选择不同的类型");
                comboBox2.SelectedIndex = -1;
                return;
            }
            Slot2VoltageType = comboBox2.SelectedIndex;
        }
    }
}

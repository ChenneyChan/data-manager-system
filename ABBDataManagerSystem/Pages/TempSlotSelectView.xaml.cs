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
    /// TempSlotSelectView.xaml 的交互逻辑
    /// </summary>
    public partial class TempSlotSelectView : Window
    {
        private static readonly int MaxSlot = 36;
        public TempSlotSelectView()
        {
            InitializeComponent();
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
            InitComboBox(cbRaoZuA);
            InitComboBox(cbRaoZuB);
            InitComboBox(cbRaoZuC);
            InitComboBox(cbCore);
            InitComboBox(cbEnvA);
            InitComboBox(cbEnvB);
            InitComboBox(cbEnvC);
            InitComboBox(cbEnvD);
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

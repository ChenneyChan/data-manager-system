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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// TappingVoltageRatioFields.xaml 的交互逻辑
    /// </summary>
    public partial class TappingVoltageRatioFields : UserControl
    {
        private string _TappingIndex;

        public string TappingIndex
        {
            get { return _TappingIndex; }
            set { _TappingIndex = value; tbTappingIndex.Text = _TappingIndex.ToString(); }
        }

        public TappingVoltageRatioFields()
        {
            InitializeComponent();

            tbTappingIndex.Text = _TappingIndex;
        }
    }
}

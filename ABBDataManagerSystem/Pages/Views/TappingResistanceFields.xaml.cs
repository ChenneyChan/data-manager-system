using System.Windows.Controls;
using System.Windows.Media;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// TappingResistanceFields.xaml 的交互逻辑
    /// </summary>
    public partial class TappingResistanceFields : UserControl
    {
        private float _ValueAB = 0;

        public float ValueAB
        {
            get { return _ValueAB; }
            set { _ValueAB = value; UpdateDisplay(); }
        }

        private float _ValueBC = 0;

        public float ValueBC
        {
            get { return _ValueBC; }
            set { _ValueBC = value; UpdateDisplay(); }
        }

        private float _ValueCA = 0;

        public float ValueCA
        {
            get { return _ValueCA; }
            set { _ValueCA = value; UpdateDisplay(); }
        }

        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; UpdateDisplay(); }
        }

        private Brush OriginBackGroud;

        public TappingResistanceFields()
        {
            InitializeComponent();
            OriginBackGroud = Background;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            tbResistanceAB.Text = ZeroIsNull(Utils.FloatFormat(ValueAB));
            tbResistanceBC.Text = ZeroIsNull(Utils.FloatFormat(ValueBC));
            tbResistanceCA.Text = ZeroIsNull(Utils.FloatFormat(ValueCA));

            if (IsSelected)
            {
                Background = new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                Background = OriginBackGroud;
            }
        }

        private string ZeroIsNull(string value)
        {
            if (value == "0")
            {
                return "";
            }
            return value;
        }
    }
}

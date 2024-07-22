using System.Windows.Controls;
using System.Windows.Media;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// TappingVoltageRatioFields.xaml 的交互逻辑
    /// </summary>
    public partial class TappingVoltageRatioFields : UserControl
    {
        private string _TappingIndex = String.Empty;

        public string TappingIndex
        {
            get { return _TappingIndex; }
            set { _TappingIndex = value; UpdateDisplay(); }
        }

        private float? _ValueAB;

        public float? ValueAB
        {
            get { return _ValueAB; }
            set { _ValueAB = value; UpdateDisplay(); }
        }

        private float? _ValueBC;

        public float? ValueBC
        {
            get { return _ValueBC; }
            set { _ValueBC = value; UpdateDisplay(); }
        }

        private float? _ValueCA;

        public float? ValueCA
        {
            get { return _ValueCA; }
            set { _ValueCA = value; UpdateDisplay(); }
        }

        private float _TappingVoltage;

        public float TappingVoltage
        {
            get { return _TappingVoltage; }
            set { _TappingVoltage = value; UpdateDisplay(); }
        }

        private float _CalculatedRatio;

        public float CalculatedRatio
        {
            get { return _CalculatedRatio; }
            set { _CalculatedRatio = value; UpdateDisplay(); }
        }

        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; UpdateDisplay(); }
        }

        private Brush OriginBackGroud;

        public TappingVoltageRatioFields()
        {
            InitializeComponent();
            OriginBackGroud = Background;
            UpdateDisplay();
            tbErrorAB.TextChanged += TbError_TextChanged;
            tbErrorBC.TextChanged += TbError_TextChanged;
            tbErrorCA.TextChanged += TbError_TextChanged;
        }

        private void TbError_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == tbErrorAB)
            {
                _ValueAB = Utils.ParseFloatNull(tbErrorAB.Text);
            }
            else if (sender == tbErrorBC)
            {
                _ValueBC = Utils.ParseFloatNull(tbErrorBC.Text);
            } 
            else if (sender == tbErrorCA)
            {
                _ValueCA = Utils.ParseFloatNull(tbErrorCA.Text);
            }
        }

        private void UpdateDisplay()
        {
            tbTappingIndex.Text = _TappingIndex;
            tbErrorAB.Text = ValueAB == null ? "" : ZeroIsNull(Utils.FloatFormat((float)ValueAB));
            tbErrorBC.Text = ValueBC == null ? "" : ZeroIsNull(Utils.FloatFormat((float)ValueBC));
            tbErrorCA.Text = ValueCA == null ? "" : ZeroIsNull(Utils.FloatFormat((float)ValueCA));
            tbCalculatedRatio.Text = ZeroIsNull(Utils.FloatFormat(CalculatedRatio));
            tbTappingVoltage.Text = ZeroIsNull(Utils.FloatFormat(TappingVoltage));

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

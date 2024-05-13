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

        private float _ValueAB;

        public float ValueAB
        {
            get { return _ValueAB; }
            set { _ValueAB = value; UpdateDisplay(); }
        }

        private float _ValueBC;

        public float ValueBC
        {
            get { return _ValueBC; }
            set { _ValueBC = value; UpdateDisplay(); }
        }

        private float _ValueCA;

        public float ValueCA
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
        }

        private void UpdateDisplay()
        {
            tbTappingIndex.Text = _TappingIndex;
            tbErrorAB.Text = ZeroIsNull(Utils.FloatFormat(ValueAB));
            tbErrorBC.Text = ZeroIsNull(Utils.FloatFormat(ValueBC));
            tbErrorCA.Text = ZeroIsNull(Utils.FloatFormat(ValueCA));
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

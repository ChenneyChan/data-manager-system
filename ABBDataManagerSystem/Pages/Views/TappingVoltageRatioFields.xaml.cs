using System.Windows.Controls;

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

        private float _ValueAB;

        public float ValueAB
        {
            get { return _ValueAB; }
            set { _ValueAB = value; }
        }

        private float _ValueBC;

        public float ValueBC
        {
            get { return _ValueBC; }
            set { _ValueBC = value; }
        }

        private float _ValueCA;

        public float ValueCA
        {
            get { return _ValueCA; }
            set { _ValueCA = value; }
        }

        private float _TappingVoltage;

        public float TappingVoltage
        {
            get { return _TappingVoltage; }
            set { _TappingVoltage = value; }
        }

        private float _CalculatedRatio;

        public float CalculatedRatio
        {
            get { return _CalculatedRatio; }
            set { _CalculatedRatio = value; }
        }

        public TappingVoltageRatioFields()
        {
            InitializeComponent();
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

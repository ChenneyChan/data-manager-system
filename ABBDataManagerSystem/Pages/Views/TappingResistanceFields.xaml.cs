using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// TappingResistanceFields.xaml 的交互逻辑
    /// </summary>
    public partial class TappingResistanceFields : UserControl
    {
        public delegate void ActiveCallback(TappingResistanceFields obj, int activeIndex);

        private float? _ValueAB = null;

        public float? ValueAB
        {
            get { return _ValueAB; }
            set { _ValueAB = value; UpdateDisplay(); }
        }

        private float? _ValueBC = null;

        public float? ValueBC
        {
            get { return _ValueBC; }
            set { _ValueBC = value; UpdateDisplay(); }
        }

        private float? _ValueCA = null;

        public float? ValueCA
        {
            get { return _ValueCA; }
            set { _ValueCA = value; UpdateDisplay(); }
        }

        private int SelectedIndex = -1;

        public bool IsSelected
        {
            get { return SelectedIndex > 0; }
        }

        private ActiveCallback _ActiveCallback;

        public ActiveCallback ActiveEventCallback
        {
            get { return _ActiveCallback; }
            set { _ActiveCallback = value; }
        }

        private string _TappingIndex;

        public string TappingIndex
        {
            get { return _TappingIndex; }
            set { _TappingIndex = value; }
        }

        private Thickness NoneThickness = new Thickness(0);
        private Thickness ActiveThickness = new Thickness(2);

        public TappingResistanceFields()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        public void ResetSelection()
        {
            SelectedIndex = -1;
            UpdateDisplay();
        }

        public void UpdateValueForActiveItem(float value)
        {
            if (SelectedIndex == 1)
            {
                ValueAB = value;
            }
            else if (SelectedIndex == 2)
            {
                ValueBC = value;
            }
            else if (SelectedIndex == 3)
            {
                ValueCA = value;
            }
        }

        public float[] GetValues()
        {
            return new float[] { _ValueAB ?? 0, _ValueBC ?? 0, _ValueCA ?? 0 };
        }

        private void UpdateDisplay()
        {
            tbResistanceAB.Text = ValueAB == null ? "" : Utils.ZeroIsNull(Utils.FloatFormat((float)ValueAB));
            tbResistanceBC.Text = ValueBC == null ? "" : Utils.ZeroIsNull(Utils.FloatFormat((float)ValueBC));
            tbResistanceCA.Text = ValueCA == null ? "" : Utils.ZeroIsNull(Utils.FloatFormat((float)ValueCA));

            //tbResistanceAB.Background = SelectedIndex == 1 ? ActiveBackGroud : OriginBackGroud;
            //tbResistanceBC.Background = SelectedIndex == 2 ? ActiveBackGroud : OriginBackGroud;
            //tbResistanceCA.Background = SelectedIndex == 3 ? ActiveBackGroud : OriginBackGroud;

            tbResistanceAB.BorderThickness = SelectedIndex == 1 ? ActiveThickness : NoneThickness;
            tbResistanceBC.BorderThickness = SelectedIndex == 2 ? ActiveThickness : NoneThickness;
            tbResistanceCA.BorderThickness = SelectedIndex == 3 ? ActiveThickness : NoneThickness;
        }

        private void tbResistance_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            bool needNotify = true;
            if (sender == tbResistanceAB)
            {
                needNotify = SelectedIndex != 1;
                SelectedIndex = 1;
            }
            else if (sender == tbResistanceBC)
            {
                needNotify = SelectedIndex != 2;
                SelectedIndex = 2;
            }
            else if (sender == tbResistanceCA)
            {
                needNotify = SelectedIndex != 3;
                SelectedIndex = 3;
            }
            UpdateDisplay();
            if (needNotify && ActiveEventCallback != null)
            {
                ActiveEventCallback(this, SelectedIndex);
            }
        }
    }
}

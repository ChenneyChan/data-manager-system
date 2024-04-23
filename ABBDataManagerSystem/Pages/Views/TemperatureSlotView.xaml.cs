using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// TemperatureSlotView.xaml 的交互逻辑
    /// </summary>
    public partial class TemperatureSlotView : UserControl
    {

        private int _Slot = 0;

        public int Slot
        {
            get { return _Slot = 0; }
            set { _Slot = value; tbSlot.Text = $"Slot-{value}"; }
        }

        private float _Temperature;

        public float Temperature
        {
            get { return _Temperature; }
            set { _Temperature = value; tbTemp.Text = $"{Utils.FloatFormat(value)} ℃"; }
        }

        public TemperatureSlotView()
        {
            InitializeComponent();
            tbSlot.Text = $"Slot-{_Slot}";
            tbTemp.Text = $"{Utils.FloatFormat(_Temperature)} ℃";
        }
    }
}

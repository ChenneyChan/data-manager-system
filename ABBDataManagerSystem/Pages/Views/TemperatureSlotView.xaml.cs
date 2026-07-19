using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// TemperatureSlotView.xaml 的交互逻辑
    /// </summary>
    public partial class TemperatureSlotView : UserControl
    {
        private int _Slot = 0;
        private string _Title = string.Empty;
        private float _Temperature;

        public int Slot
        {
            get { return _Slot; }
            set
            {
                _Slot = value;
                tbSlot.Text = $"Slot-{value}";
            }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value ?? string.Empty;
                tbTitle.Text = string.IsNullOrWhiteSpace(_Title) ? $"Slot-{_Slot}" : _Title;
            }
        }

        public float Temperature
        {
            get { return _Temperature; }
            set
            {
                _Temperature = value;
                tbTemp.Text = $"{Utils.FloatFormat(value)} ℃";
            }
        }

        public TemperatureSlotView()
        {
            InitializeComponent();
            tbTitle.Text = "通道";
            tbSlot.Text = $"Slot-{_Slot}";
            tbTemp.Text = $"{Utils.FloatFormat(_Temperature)} ℃";
        }
    }
}

using ABBDataManagerSystem.Bean.Base;
using System.Windows.Controls;


namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// LoadNoloadInfo.xaml 的交互逻辑
    /// </summary>
    public partial class LoadNoloadInfo : UserControl
    {
        private Dictionary<string, TextBlock> _fields = new Dictionary<string, TextBlock>();

        private VoltageCurrentLossDataInfo? _currentLossDataInfo = null;

        public VoltageCurrentLossDataInfo? LossDataInfo
        {
            set { _currentLossDataInfo = value; UpdateValue(); }
            get { return _currentLossDataInfo; }
        }

        private string[] _headers = new string[] { };

        public string[] Headers
        {
            get { return _headers; }
            set { _headers = value; UpdateHeader(); }
        }


        public LoadNoloadInfo()
        {
            InitializeComponent();
            InitFields(3);
        }

        private void InitFields(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                int r = i + 1;
                AddField(r, 0, $"id_{i}");

                AddField(r, 1, $"ua_{i}");
                AddField(r, 2, $"ub_{i}");
                AddField(r, 3, $"uc_{i}");
                AddField(r, 4, $"u3_{i}");

                AddField(r, 5, $"pua_{i}");
                AddField(r, 6, $"pub_{i}");
                AddField(r, 7, $"puc_{i}");
                AddField(r, 8, $"pu3_{i}");

                AddField(r, 9, $"ia_{i}");
                AddField(r, 10, $"ib_{i}");
                AddField(r, 11, $"ic_{i}");
                AddField(r, 12, $"i3_{i}");

                AddField(r, 13, $"pa_{i}");
                AddField(r, 14, $"pb_{i}");
                AddField(r, 15, $"pc_{i}");
                AddField(r, 16, $"p3_{i}");

                AddField(r, 17, $"fu_{i}");
            }
        }

        private void AddField(int r, int c, string key)
        {
            var tb = new TextBlock()
            {
                Text = "",
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            };
            tb.SetValue(Grid.ColumnProperty, c);
            tb.SetValue(Grid.RowProperty, r);
            mainGrid.Children.Add(tb);

            _fields.Add(key, tb);
        }

        private void UpdateValue()
        {

        }

        private void UpdateHeader()
        {
            for (int i = 0; i < _headers.Length; i++)
            {
                var key = $"id_{i + 1}";
                if (_fields.ContainsKey(key))
                {
                    _fields[key].Text = _headers[i];
                }
            }
        }
    }
}

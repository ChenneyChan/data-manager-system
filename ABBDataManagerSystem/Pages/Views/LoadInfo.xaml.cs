using ABBDataManagerSystem.Bean.Base;
using System.Windows.Controls;


namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// LoadNoloadInfo.xaml 的交互逻辑
    /// </summary>
    public partial class LoadInfo : UserControl
    {
        private Dictionary<string, TextBlock> _fields = new Dictionary<string, TextBlock>();

        private List<VoltageCurrentLossDataInfo>? _currentLossDataInfoList = null;

        public List<VoltageCurrentLossDataInfo>? LossDataInfoList
        {
            set { _currentLossDataInfoList = value; UpdateValue(); }
            get { return _currentLossDataInfoList; }
        }

        private string[] _headers = new string[] { };

        public string[] Headers
        {
            get { return _headers; }
            set { _headers = value; UpdateHeader(); }
        }


        public LoadInfo()
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

                AddField(r, 5, $"ia_{i}");
                AddField(r, 6, $"ib_{i}");
                AddField(r, 7, $"ic_{i}");
                AddField(r, 8, $"i3_{i}");

                AddField(r, 9, $"pa_{i}");
                AddField(r, 10, $"pb_{i}");
                AddField(r, 11, $"pc_{i}");
                AddField(r, 12, $"p3_{i}");

                AddField(r, 13, $"fu_{i}");
            }
        }

        private void AddField(int r, int c, string key)
        {
            var tb = new TextBlock()
            {
                Text = "22230.00",
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
            if (_currentLossDataInfoList == null)
            {
                return;
            }
            for (int i = 0; i < _currentLossDataInfoList.Count; i++)
            {
                var item = _currentLossDataInfoList[i];
                if (item == null)
                {
                    continue; 
                }
                _fields[$"ua_{i + 1}"].Text = Utils.FloatFormat(item.ua ?? 0);
                _fields[$"ub_{i + 1}"].Text = Utils.FloatFormat(item.ub ?? 0);
                _fields[$"uc_{i + 1}"].Text = Utils.FloatFormat(item.uc ?? 0);
                _fields[$"u3_{i + 1}"].Text = Utils.FloatFormat(item.u3 ?? 0);

                _fields[$"ia_{i + 1}"].Text = Utils.FloatFormat(item.ia ?? 0);
                _fields[$"ib_{i + 1}"].Text = Utils.FloatFormat(item.ib ?? 0);
                _fields[$"ic_{i + 1}"].Text = Utils.FloatFormat(item.ic ?? 0);
                _fields[$"i3_{i + 1}"].Text = Utils.FloatFormat(item.i3 ?? 0);

                _fields[$"pa_{i + 1}"].Text = Utils.FloatFormat(item.pa ?? 0);
                _fields[$"pb_{i + 1}"].Text = Utils.FloatFormat(item.pb ?? 0);
                _fields[$"pc_{i + 1}"].Text = Utils.FloatFormat(item.pc ?? 0);
                _fields[$"p3_{i + 1}"].Text = Utils.FloatFormat(item.p3 ?? 0);

                _fields[$"fu_{i + 1}"].Text = Utils.FloatFormat(item.fU ?? 0);
            }
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

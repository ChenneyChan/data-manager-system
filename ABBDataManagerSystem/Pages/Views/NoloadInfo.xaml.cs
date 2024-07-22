using ABBDataManagerSystem.Bean.Base;
using System.Windows.Controls;


namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// LoadNoloadInfo.xaml 的交互逻辑
    /// </summary>
    public partial class NoloadInfo : UserControl
    {
        private Dictionary<string, TextBox> _fields = new Dictionary<string, TextBox>();

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


        public NoloadInfo()
        {
            InitializeComponent();
            InitFields(3);
        }

        private void InitFields(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                int r = i + 1;
                AddField(r, 0, $"id_{i}").IsReadOnly = true;

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

                AddField(r, 17, $"fU_{i}");
            }
        }

        private TextBox AddField(int r, int c, string key)
        {
            var tb = new TextBox()
            {
                Text = "2233.00",
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                BorderThickness = new System.Windows.Thickness(0),
                MinWidth = 60,
                TextAlignment = System.Windows.TextAlignment.Center,
            };
            tb.SetValue(Grid.ColumnProperty, c);
            tb.SetValue(Grid.RowProperty, r);
            tb.TextChanged += Tb_TextChanged;
            mainGrid.Children.Add(tb);

            _fields.Add(key, tb);
            return tb;
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlUtils.HandleVoltageCurrentTextChange(sender, _fields, _currentLossDataInfoList);
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

                _fields[$"pua_{i + 1}"].Text = Utils.FloatFormat(item.pua ?? 0);
                _fields[$"pub_{i + 1}"].Text = Utils.FloatFormat(item.pub ?? 0);
                _fields[$"puc_{i + 1}"].Text = Utils.FloatFormat(item.puc ?? 0);
                _fields[$"pu3_{i + 1}"].Text = Utils.FloatFormat(item.pu3 ?? 0);

                _fields[$"ia_{i + 1}"].Text = Utils.FloatFormat(item.ia ?? 0);
                _fields[$"ib_{i + 1}"].Text = Utils.FloatFormat(item.ib ?? 0);
                _fields[$"ic_{i + 1}"].Text = Utils.FloatFormat(item.ic ?? 0);
                _fields[$"i3_{i + 1}"].Text = Utils.FloatFormat(item.i3 ?? 0);

                _fields[$"pa_{i + 1}"].Text = Utils.FloatFormat(item.pa ?? 0);
                _fields[$"pb_{i + 1}"].Text = Utils.FloatFormat(item.pb ?? 0);
                _fields[$"pc_{i + 1}"].Text = Utils.FloatFormat(item.pc ?? 0);
                _fields[$"p3_{i + 1}"].Text = Utils.FloatFormat(item.p3 ?? 0);

                _fields[$"fU_{i + 1}"].Text = Utils.FloatFormat(item.fU ?? 0);
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

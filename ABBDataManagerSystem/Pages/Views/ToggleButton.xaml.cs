using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// ToggleButton.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleButton : UserControl
    {
        private BrushConverter brushConverter = new BrushConverter();
        private string _ButtonContent = string.Empty;

        public RoutedEventHandler? CheckedChange { set; get; } = null;

        public string ButtonContent
        {
            get { return _ButtonContent; }
            set { _ButtonContent = value; btContentButton.Content = _ButtonContent; }
        }

        private bool _IsChecked = false;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; UpdateBackground(); }
        }

        private float _GridMargin = 5;

        public float GridMargin
        {
            get { return _GridMargin; }
            set { _GridMargin = value; UpdateMargin(); }
        }


        public ToggleButton()
        {
            InitializeComponent();
            UpdateBackground();
            UpdateMargin();
            btContentButton.Content = _ButtonContent;
        }
       
        private void UpdateBackground()
        {
            if (_IsChecked)
            {
                btContentButton.Background = brushConverter.ConvertFromString("#bb646a") as Brush;
            } else
            {
                btContentButton.Background = brushConverter.ConvertFromString("#4ec64c") as Brush;
            }
        }

        private void btContentButton_Click(object sender, RoutedEventArgs e)
        {
            IsChecked = !IsChecked;
            UpdateBackground();
            if (CheckedChange != null) { CheckedChange(this, e); };
        }

        private void UpdateMargin()
        {
            mainGrid.Margin = new Thickness(_GridMargin);
        }
    }
}

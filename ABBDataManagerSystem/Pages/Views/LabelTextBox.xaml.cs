using System.ComponentModel;
using System.Windows.Controls;

namespace ABBDataManagerSystem.Pages.Views
{
    /// <summary>
    /// LabelTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class LabelTextBox : UserControl
    {
        public class LabelData : INotifyPropertyChanged
        {
            private string label = string.Empty;

            private string text = string.Empty;
            
            public string LabelText
            {
                get => this.label;
                set
                {
                    if (label != value)
                    {
                        label = value;
                        OnPropertyChanged(nameof(LabelText));
                    }
                }
            }

            public string Content
            {
                get => this.text;
                set
                {
                    if (text != value)
                    {
                        text = value;
                        OnPropertyChanged(nameof(Content));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Label
        {
            get { return ContextData.LabelText; }
            set { ContextData.LabelText = value; }
        }

        public string Text
        {
            get { return ContextData.Content; }
            set { ContextData.Content = value; }
        }

        LabelData ContextData = new LabelData();

        public LabelTextBox()
        {
            InitializeComponent();

            ContextData = new LabelData()
            {
                LabelText = this.Label,
                Content = this.Text
            };
            this.DataContext = ContextData;

            // 注册依赖属性的回调方法
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(
                UserControl.FontSizeProperty, typeof(UserControl));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, OnFontSizeChanged);
            }
        }

        private void OnFontSizeChanged(object? sender, EventArgs e)
        {
        }
    }
}

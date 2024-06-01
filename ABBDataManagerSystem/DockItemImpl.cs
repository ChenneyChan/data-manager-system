using DevZest.Windows.Docking;
using System.Windows.Controls;
using System.Windows.Media;

namespace ABBDataManagerSystem
{
    internal class DockItemImpl : DockItem
    {
        private float _Scale = 1.0f;

        public float Scale
        {
            get { return _Scale; }
            set { _Scale = value; UpdateTransform(); }
        }

        public DockItemImpl() : base()
        {
            Closing += DockItemImpl_Closing;
        }

        private void DockItemImpl_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // 1 流量 + 4 温度
            var sc = Content as ScrollViewer;
            if (sc != null && sc.Content != null)
            {
                var closeable = sc.Content as ICloseable;
                if (closeable != null) { closeable.Close(); }
            }
        }

        private void UpdateTransform()
        {
            var sc = Content as ScrollViewer;
            if (sc != null)
            {
                var uc = sc.Content as UserControl;
                if (uc != null)
                {
                    uc.RenderTransform = new ScaleTransform(_Scale, _Scale);
                }
            }
        }
    }
}

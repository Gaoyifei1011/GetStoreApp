using Windows.Foundation;
using Windows.Graphics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppHelper.UI.Controls
{
    public sealed partial class TrayMenuControl : ContentControl
    {
        public TrayMenuControl()
        {
            InitializeComponent();
        }

        public void SetXamlRoot(XamlRoot xamlRoot)
        {
            TrayMenuFlyout.XamlRoot = xamlRoot;
        }

        public void ShowMenuFlyout(PointInt32 point)
        {
            TrayMenuFlyout.ShowAt(null, new Point(point.X, point.Y));
        }
    }
}

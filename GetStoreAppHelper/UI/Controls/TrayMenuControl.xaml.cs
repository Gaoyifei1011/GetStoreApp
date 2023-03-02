using GetStoreAppHelper.Helpers;
using Windows.Foundation;
using Windows.Graphics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppHelper.UI.Controls
{
    /// <summary>
    /// 任务栏辅助部分：任务栏右键菜单浮出控件视图
    /// </summary>
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
            int x = DPICalcHelper.ConvertPixelToEpx(Program.ApplicationRoot.MainWindow.Handle, point.X);
            int y = DPICalcHelper.ConvertPixelToEpx(Program.ApplicationRoot.MainWindow.Handle, point.Y);
            TrayMenuFlyout.ShowAt(null, new Point(x, y));
        }
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace GetStoreApp.UI.Controls.Window
{
    /// <summary>
    /// 应用标题栏用户控件视图
    /// </summary>
    public sealed partial class AppTitleBarControl : Grid
    {
        public AppTitleBarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置标题栏状态
        /// </summary>
        public void SetTitlebarState(bool isWindowMaximized)
        {
            if (isWindowMaximized)
            {
                ViewModel.IsWindowMaximized = isWindowMaximized;
                ViewModel.IsWindowMaximizedToolTip = ViewModel.RestoreToolTip;
                VisualStateManager.GoToState(MaxButton, "WindowStateMaximized", false);
            }
            else
            {
                ViewModel.IsWindowMaximized = isWindowMaximized;
                ViewModel.IsWindowMaximizedToolTip = ViewModel.MaximizeToolTip;
                VisualStateManager.GoToState(MaxButton, "WindowStateNormal", false);
            }
        }

        /// <summary>
        /// 键盘点击Alt + 空格键时，弹出右键菜单
        /// </summary>
        public void ShowTitlebarMenu()
        {
            TitlebarMenuFlyout.ShowAt(null, new Point(0, 45));
        }
    }
}

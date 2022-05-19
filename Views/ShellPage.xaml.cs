using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using winui = Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);

            // 简单的颜色自定义
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // 完全自定义
            // 隐藏默认标题栏
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // 注册一个处理程序，当覆盖的标题控件的大小改变。
            // 例如，当应用程序移动到一个不同的DPI屏幕。
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // 注册标题栏可见性改变的处理程序。
            // 例如，在全屏模式下调用标题栏。
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            // 当窗口改变焦点时，注册一个处理程序
            Window.Current.Activated += Current_Activated;

            // 设置XAML元素为可拖动区域
            Window.Current.SetTitleBar(AppTitleBar);

            Loaded += OnLoaded;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, 0, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        //应用主页面及窗口设置
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetTitleBarControlColors();
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        //根据应用程序的未激活/激活状态更新TitleBar
        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            VisualStateManager.GoToState(
                this, e.WindowActivationState == CoreWindowActivationState.Deactivated ? WindowNotFocused.Name : WindowFocused.Name, false);
        }

        // 设置应用标题栏按钮颜色
        private void SetTitleBarControlColors()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            if (applicationView == null)
            {
                return;
            }

            var applicationTitleBar = applicationView.TitleBar;
            if (applicationTitleBar == null)
            {
                return;
            }

            Color bgColor = Colors.Transparent;
            Color fgColor = ((SolidColorBrush)Resources["ButtonForegroundColor"]).Color;
            Color inactivefgColor = ((SolidColorBrush)Resources["ButtonInactiveForegroundBrush"]).Color;
            Color hoverbgColor = ((SolidColorBrush)Resources["ButtonHoverBackgroundBrush"]).Color;
            Color hoverfgColor = ((SolidColorBrush)Resources["ButtonHoverForegroundBrush"]).Color;
            Color pressedbgColor = ((SolidColorBrush)Resources["ButtonPressedBackgroundBrush"]).Color;
            Color pressedfgColor = ((SolidColorBrush)Resources["ButtonPressedForegroundBrush"]).Color;

            applicationTitleBar.ButtonBackgroundColor = bgColor;
            applicationTitleBar.ButtonForegroundColor = fgColor;
            applicationTitleBar.ButtonInactiveBackgroundColor = bgColor;
            applicationTitleBar.ButtonInactiveForegroundColor = inactivefgColor;
            applicationTitleBar.ButtonHoverBackgroundColor = hoverbgColor;
            applicationTitleBar.ButtonHoverForegroundColor = hoverfgColor;
            applicationTitleBar.ButtonPressedBackgroundColor = pressedbgColor;
            applicationTitleBar.ButtonPressedForegroundColor = pressedfgColor;
        }

        private void NavigationView_DisplayModeChanged(winui.NavigationView sender, winui.NavigationViewDisplayModeChangedEventArgs args)
        {
            // 标题栏AppTitleBar距离左边栏间隔
            const int NormalTitleBarIndent = 48;
            int MinimalTitleBarIndent = 96;

            // ShellFrame距离上边栏间隔
            const int NormalFrameIndent = 0;
            const int MinimalFrameIndent = 45;

            // 如果后退按钮不可见，请减少AppTitleBar内容缩进。
            if (navigationView.IsBackButtonVisible.Equals(NavigationViewBackButtonVisible.Collapsed))
            {
                MinimalTitleBarIndent = 48;
            }

            Thickness AppTitleBarMargin = AppTitleBar.Margin;
            Thickness ShellFrameMargin = shellFrame.Margin;

            // 根据navigationView导航视图显示模式设置AppTitleBar标题栏左边距和ShellFrame距离上边栏边距
            if (sender.DisplayMode == winui.NavigationViewDisplayMode.Compact)
            {
                AppTitleBar.Margin = new Thickness(NormalTitleBarIndent, AppTitleBarMargin.Top, AppTitleBarMargin.Right, AppTitleBarMargin.Bottom);
                shellFrame.Margin = new Thickness(ShellFrameMargin.Left, NormalFrameIndent, ShellFrameMargin.Right, ShellFrameMargin.Bottom);
            }
            else if (sender.DisplayMode == winui.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(MinimalTitleBarIndent, AppTitleBarMargin.Top, AppTitleBarMargin.Right, AppTitleBarMargin.Bottom);
                shellFrame.Margin = new Thickness(ShellFrameMargin.Left, MinimalFrameIndent, ShellFrameMargin.Right, ShellFrameMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(NormalTitleBarIndent, AppTitleBarMargin.Top, AppTitleBarMargin.Right, AppTitleBarMargin.Bottom);
                shellFrame.Margin = new Thickness(ShellFrameMargin.Left, NormalFrameIndent, ShellFrameMargin.Right, ShellFrameMargin.Bottom);
            }
        }
    }
}

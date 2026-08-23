using GetStoreAppPinner.Services.Settings;
using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppPinner.Views.Pages
{
    /// <summary>
    /// 固定应用提示页面
    /// </summary>
    internal sealed partial class PinnerPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：属性、集合与事件

        private AppWindow AppWindow { get; }

        private ElementTheme _windowTheme;

        private ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowTheme)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第一部分：属性、集合与事件

        #region 第二部分：构造函数

        internal PinnerPage(AppWindow appWindow)
        {
            InitializeComponent();
            AppWindow = appWindow;
            WindowTheme = Enum.TryParse(ThemeService.AppTheme, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;
            SetTitleBarTheme(AppWindow.TitleBar, ActualTheme);
        }

        #endregion 第二部分：构造函数

        #region 第三部分：父类虚方法重写

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(AppWindow.TitleBar, sender.ActualTheme);
        }

        #endregion 第三部分：父类虚方法重写

        #region 第四部分：数据操作与业务逻辑

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(AppWindowTitleBar appWindowTitleBar, ElementTheme theme)
        {
            if (appWindowTitleBar is null)
            {
                return;
            }

            appWindowTitleBar.BackgroundColor = Colors.Transparent;
            appWindowTitleBar.ForegroundColor = Colors.Transparent;
            appWindowTitleBar.InactiveBackgroundColor = Colors.Transparent;
            appWindowTitleBar.InactiveForegroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                appWindowTitleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                appWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                appWindowTitleBar.ButtonHoverForegroundColor = Colors.Black;
                appWindowTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                appWindowTitleBar.ButtonPressedForegroundColor = Colors.Black;
                appWindowTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                appWindowTitleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                appWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                appWindowTitleBar.ButtonHoverForegroundColor = Colors.White;
                appWindowTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                appWindowTitleBar.ButtonPressedForegroundColor = Colors.White;
                appWindowTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        #endregion 第四部分：数据操作与业务逻辑
    }
}

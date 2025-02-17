using GetStoreAppWebView.Services.Controls.Settings;
using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppWebView.Pages
{
    /// <summary>
    /// 固定应用到任务栏提示页面
    /// </summary>
    public sealed partial class TaskbarPinPage : Page, INotifyPropertyChanged
    {
        private AppWindow AppWindow { get; }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TaskbarPinPage(AppWindow appWindow)
        {
            InitializeComponent();
            AppWindow = appWindow;
            WindowTheme = Enum.TryParse(ThemeService.AppTheme, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;

            SetTitleBarTheme(ActualTheme);
        }

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(9, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(6, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(15, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(10, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }
    }
}

using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Windowing;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace GetStoreAppWebView.Views.Controls
{
    /// <summary>
    /// 应用顶部标题栏区域控件
    /// </summary>
    public sealed partial class TitleControl : Grid, INotifyPropertyChanged
    {
        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                _isWindowMaximized = value;
                OnPropertyChanged();
            }
        }

        private bool _canGoBack = false;

        public bool CanGoBack
        {
            get { return _canGoBack; }

            set
            {
                _canGoBack = value;
                OnPropertyChanged();
            }
        }

        private bool _canGoForward = false;

        public bool CanGoForward
        {
            get { return _canGoForward; }

            set
            {
                _canGoForward = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading = true;

        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled = false;

        public bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TitleControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarColor(ActualTheme);
        }

        /// <summary>
        /// 控件加载完成时初始化控件位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            SetTitleBarColor(ActualTheme);

            if (RuntimeHelper.IsWebView2Installed)
            {
                if (Program.MainWindow.WebView2 is not null)
                {
                    Program.MainWindow.WebView2.Location = new Point(0, Program.MainWindow.MileXamlHost.Height);
                    Program.MainWindow.WebView2.Size = new Size(Program.MainWindow.ClientSize.Width, Program.MainWindow.ClientSize.Height - Program.MainWindow.MileXamlHost.Height);
                }
            }
            else
            {
                if (Program.MainWindow.WebBrowser is not null)
                {
                    Program.MainWindow.WebBrowser.Location = new Point(0, Program.MainWindow.MileXamlHost.Height);
                    Program.MainWindow.WebBrowser.Size = new Size(Program.MainWindow.ClientSize.Width, Program.MainWindow.ClientSize.Height - Program.MainWindow.MileXamlHost.Height);
                }
            }
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(Program.MainWindow.Handle, WindowMessage.WM_SYSCOMMAND, 0xF010, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage(Program.MainWindow.Handle, WindowMessage.WM_SYSCOMMAND, 0xF000, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 网页后退
        /// </summary>
        private void OnBrowserBackClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsWebView2Installed)
            {
                if (Program.MainWindow.WebView2 is not null)
                {
                    if (Program.MainWindow.WebView2.CanGoBack)
                    {
                        Program.MainWindow.WebView2.GoBack();
                    }
                }
            }
            else
            {
                if (Program.MainWindow.WebBrowser is not null)
                {
                    if (Program.MainWindow.WebBrowser.CanGoBack)
                    {
                        Program.MainWindow.WebBrowser.GoBack();
                    }
                }
            }
        }

        /// <summary>
        /// 网页前进
        /// </summary>
        private void OnBrowserForwardClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsWebView2Installed)
            {
                if (Program.MainWindow.WebView2 is not null)
                {
                    if (Program.MainWindow.WebView2.CanGoForward)
                    {
                        Program.MainWindow.WebView2.GoForward();
                    }
                }
            }
            else
            {
                if (Program.MainWindow.WebBrowser is not null)
                {
                    if (Program.MainWindow.WebBrowser.CanGoForward)
                    {
                        Program.MainWindow.WebBrowser.GoForward();
                    }
                }
            }
        }

        /// <summary>
        /// 网页刷新
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsWebView2Installed)
            {
                Program.MainWindow.WebView2?.Reload();
            }
            else
            {
                Program.MainWindow.WebBrowser?.Refresh();
            }
        }

        /// <summary>
        /// WebView2 ：打开下载窗口
        /// Winform WebBrowser ：打开系统下载文件夹
        /// </summary>
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsWebView2Installed)
            {
                if (Program.MainWindow.WebView2 is not null && Program.MainWindow.WebView2.CoreWebView2 is not null)
                {
                    Program.MainWindow.WebView2.CoreWebView2.OpenDefaultDownloadDialog();
                }
            }
            else
            {
                if (Program.MainWindow.WebBrowser is not null)
                {
                    Task.Run(() =>
                    {
                        Process.Start("explorer.exe", InfoHelper.UserDataPath.Downloads);
                    });
                }
            }
        }

        /// <summary>
        /// 显示浮出控件
        /// </summary>
        private void OnMoreClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 在浏览器中打开
        /// </summary>
        private void OnOpenWithBrowserClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("explorer.exe", "https://store.rg-adguard.net");
            });
        }

        /// <summary>
        /// 打开缓存文件夹
        /// </summary>
        private void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                if (RuntimeHelper.IsWebView2Installed)
                {
                    if (Directory.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView")))
                    {
                        Process.Start("explorer.exe", Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView"));
                    }
                    else
                    {
                        Process.Start("explorer.exe", Path.Combine(ApplicationData.Current.LocalFolder.Path));
                    }
                }
                else
                {
                    Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
                }
            });
        }

        /// <summary>
        ///  清理网页缓存
        /// </summary>
        private void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                if (RuntimeHelper.IsWebView2Installed)
                {
                    if (Program.MainWindow.WebView2 is not null && Program.MainWindow.WebView2.CoreWebView2 is not null)
                    {
                        CoreWebView2Profile coreWebView2Profile = Program.MainWindow.WebView2.CoreWebView2.Profile;
                        await coreWebView2Profile.ClearBrowsingDataAsync();
                    }
                }
                else
                {
                    Process.Start("RunDll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 255");
                }
            });
        }

        /// <summary>
        /// 设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarColor(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = Program.MainWindow.AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;

                if (InfoHelper.SystemVersion.Build < 22621)
                {
                    Program.MainWindow.BackColor = System.Drawing.Color.FromArgb(255, 240, 243, 249);
                }
            }
            else
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(30, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.White;

                if (InfoHelper.SystemVersion.Build < 22621)
                {
                    Program.MainWindow.BackColor = System.Drawing.Color.FromArgb(255, 20, 20, 20);
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

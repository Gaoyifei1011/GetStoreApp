using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace GetStoreAppWebView.Views.Controls
{
    /// <summary>
    /// 应用主窗口页面
    /// </summary>
    public sealed partial class TitleControl : Grid, INotifyPropertyChanged
    {
        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                _windowTheme = value;
                OnPropertyChanged();
            }
        }

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
        /// 窗口关闭
        /// </summary>
        public void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.Dispose();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        public void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        public void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        public void OnMoveClicked(object sender, RoutedEventArgs args)
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
        public void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            Program.MainWindow.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        public void OnSizeClicked(object sender, RoutedEventArgs args)
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
        public void OnBrowserBackClicked(object sender, RoutedEventArgs args)
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
        public void OnBrowserForwardClicked(object sender, RoutedEventArgs args)
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
        public void OnRefreshClicked(object sender, RoutedEventArgs args)
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
        public void OnDownloadClicked(object sender, RoutedEventArgs args)
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
        public void OnFlyoutShowClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 在浏览器中打开
        /// </summary>
        public void OnOpenWithBrowserClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("explorer.exe", "https://store.rg-adguard.net");
            });
        }

        /// <summary>
        /// 打开缓存文件夹
        /// </summary>
        public void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
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
        public void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

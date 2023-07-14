using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Web;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Dialogs.Web;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 访问网页版页面
    /// </summary>
    public sealed partial class WebPage : Page, INotifyPropertyChanged
    {
        private const string DefaultUrl = "https://store.rg-adguard.net/";

        public bool IsWebView2Installed { get; } = WebView2Helper.IsInstalled();

        private CoreWebView2 CoreWebView { get; set; }

        private bool _canGoBack = false;

        public bool CanGoBack
        {
            get { return _canGoBack; }

            set
            {
                _canGoBack = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoBack)));
            }
        }

        private bool _canGoForward = false;

        public bool CanGoForward
        {
            get { return _canGoForward; }

            set
            {
                _canGoForward = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoForward)));
            }
        }

        private Uri _source;

        public Uri Source
        {
            get { return _source; }

            set
            {
                _source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Source)));
            }
        }

        private bool _isLoading = true;

        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
            }
        }

        private bool _isClearWebCacheEnabled = false;

        public bool IsClearWebCacheEnabled
        {
            get { return _isClearWebCacheEnabled; }

            set
            {
                _isClearWebCacheEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsClearWebCacheEnabled)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WebPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            if (CoreWebView is not null)
            {
                try
                {
                    CoreWebView.NewWindowRequested -= OnNewWindowRequested;
                    CoreWebView.SourceChanged -= OnSourceChanged;
                    CoreWebView.DownloadStarting -= OnDownloadStarting;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.WARNING, "Unregister CoreWebView event failed.", e);
                    return;
                }
            }
        }

        /// <summary>
        /// 关闭浏览器内核
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            WebView?.Close();
        }

        /// <summary>
        /// 网页后退
        /// </summary>
        public void OnBrowserBackClicked(object sender, RoutedEventArgs args)
        {
            if (CoreWebView is not null && CoreWebView.CanGoBack)
            {
                CoreWebView.GoBack();
            }
        }

        /// <summary>
        /// 网页前进
        /// </summary>
        public void OnBrowserForwardClicked(object sender, RoutedEventArgs args)
        {
            if (CoreWebView is not null && CoreWebView.CanGoForward)
            {
                CoreWebView.GoForward();
            }
        }

        /// <summary>
        /// 网页刷新
        /// </summary>
        public void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            CoreWebView?.Reload();
        }

        /// <summary>
        /// 在浏览器中打开
        /// </summary>
        public async void OnOpenWithBrowserClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(Source);
        }

        /// <summary>
        /// 显示浮出控件
        /// </summary>
        public void OnFlyoutShowClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 打开缓存文件夹
        /// </summary>
        public async void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(string.Format(@"{0}\{1}", ApplicationData.Current.LocalFolder.Path, "EbWebView")));
            }
            catch (Exception)
            {
                await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
            }
        }

        /// <summary>
        ///  清理网页缓存
        /// </summary>
        public async void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
        {
            if (CoreWebView is not null)
            {
                CoreWebView2Profile coreWebView2Profile = CoreWebView.Profile;
                await coreWebView2Profile.ClearBrowsingDataAsync();
                new WebCacheCleanNotification().Show();
            }
        }

        /// <summary>
        /// 浏览器内核进程发生异常时对应的事件
        /// </summary>
        public async void OnCoreProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs args)
        {
            // 显示异常信息错误原因，弹出对话框
            await new CoreWebView2FailedDialog(args).ShowAsync();
        }

        /// <summary>
        /// 初始化CoreWebView2对象
        /// </summary>
        public void OnCoreWebView2Initialized(object sender, CoreWebView2InitializedEventArgs args)
        {
            WebView2 webView = sender as WebView2;

            if (CoreWebView is null && webView.CoreWebView2 is not null)
            {
                CoreWebView = webView.CoreWebView2;

                // 挂载对应的事件
                CoreWebView.NewWindowRequested += OnNewWindowRequested;
                CoreWebView.SourceChanged += OnSourceChanged;
                CoreWebView.DownloadStarting += OnDownloadStarting;

                IsClearWebCacheEnabled = true;
            }
        }

        /// <summary>
        /// 初始化页面信息
        /// </summary>
        public void OnWebView2Loaded(object sender, RoutedEventArgs args)
        {
            Source = new Uri(DefaultUrl);
        }

        /// <summary>
        /// 页面完成导航
        /// </summary>
        public void OnWebView2NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            IsLoading = false;
        }

        /// <summary>
        /// 页面开始导航
        /// </summary>
        public void OnWebView2NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
        }

        public void OnNavigatedFrom()
        {
            if (CoreWebView is not null)
            {
                try
                {
                    CoreWebView.NewWindowRequested -= OnNewWindowRequested;
                    CoreWebView.SourceChanged -= OnSourceChanged;
                    CoreWebView.DownloadStarting -= OnDownloadStarting;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.WARNING, "Unregister CoreWebView event failed.", e);
                    return;
                }
            }
        }

        /// <summary>
        /// 捕捉打开新窗口事件，并禁止弹窗
        /// </summary>
        private void OnNewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            CoreWebView.Navigate(args.Uri);
            args.Handled = true;
        }

        /// <summary>
        /// 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnSourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
        {
            CanGoBack = CoreWebView.CanGoBack;
            CanGoForward = CoreWebView.CanGoForward;
        }

        /// <summary>
        /// 当有任务要开始下载时触发这一事件
        /// </summary>
        private void OnDownloadStarting(CoreWebView2 sender, CoreWebView2DownloadStartingEventArgs args)
        {
            Deferral deferral = args.GetDeferral();

            SynchronizationContext.Current.Post(async (_) =>
            {
                using (deferral)
                {
                    // 隐藏默认的下载对话框
                    args.Handled = true;

                    // 使用应用内提供的下载方式（链接是通过网页抓取的，无法获得SHA1）
                    if (DownloadOptionsService.DownloadMode.InternalName == DownloadOptionsService.DownloadModeList[0].InternalName)
                    {
                        string DownloadFileName = Path.GetFileName(args.DownloadOperation.ResultFilePath);
                        string DownloadFilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, DownloadFileName);

                        BackgroundModel backgroundItem = new BackgroundModel
                        {
                            DownloadKey = UniqueKeyHelper.GenerateDownloadKey(DownloadFileName, DownloadFilePath),
                            FileName = DownloadFileName,
                            FileLink = args.DownloadOperation.Uri,
                            FilePath = DownloadFilePath,
                            TotalSize = 0,
                            FileSHA1 = "WebDownloadUnknown",
                            DownloadFlag = 1
                        };

                        backgroundItem.FilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, backgroundItem.FileName);
                        backgroundItem.DownloadKey = UniqueKeyHelper.GenerateDownloadKey(backgroundItem.FileName, backgroundItem.FilePath);

                        // 检查是否存在相同的任务记录
                        DuplicatedDataInfoArgs CheckResult = await DownloadXmlService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                        switch (CheckResult)
                        {
                            case DuplicatedDataInfoArgs.None:
                                {
                                    bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add");
                                    new DownloadCreateNotification(AddResult).Show();
                                    break;
                                }

                            case DuplicatedDataInfoArgs.Unfinished:
                                {
                                    ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Unfinished).ShowAsync();

                                    if (result is ContentDialogResult.Primary)
                                    {
                                        try
                                        {
                                            if (File.Exists(backgroundItem.FilePath))
                                            {
                                                File.Delete(backgroundItem.FilePath);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LogType.WARNING, "Delete duplicated unfinished downloaded file failed.", e);
                                        }
                                        finally
                                        {
                                            bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                            new DownloadCreateNotification(AddResult).Show();
                                        }
                                    }
                                    else if (result is ContentDialogResult.Secondary)
                                    {
                                        NavigationService.NavigateTo(typeof(DownloadPage));
                                    }
                                    break;
                                }

                            case DuplicatedDataInfoArgs.Completed:
                                {
                                    ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Completed).ShowAsync();

                                    if (result is ContentDialogResult.Primary)
                                    {
                                        try
                                        {
                                            if (File.Exists(backgroundItem.FilePath))
                                            {
                                                File.Delete(backgroundItem.FilePath);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LogType.WARNING, "Delete duplicated completed downloaded file failed.", e);
                                        }
                                        finally
                                        {
                                            bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                            new DownloadCreateNotification(AddResult).Show();
                                        }
                                    }
                                    else if (result is ContentDialogResult.Secondary)
                                    {
                                        NavigationService.NavigateTo(typeof(DownloadPage));
                                    }
                                    break;
                                }
                        }
                    }

                    // 使用浏览器下载
                    else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
                    {
                        await Launcher.LaunchUriAsync(new Uri(args.DownloadOperation.Uri));
                    }

                    args.DownloadOperation.Cancel();
                }
            }, null);
        }
    }
}

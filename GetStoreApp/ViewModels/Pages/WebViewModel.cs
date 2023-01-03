using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Web;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Dialogs.Web;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading;
using Windows.Foundation;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Pages
{
    public sealed class WebViewModel : ViewModelBase
    {
        private const string DefaultUrl = "https://store.rg-adguard.net/";

        public bool IsWebView2Installed => WebView2Helper.IsInstalled();

        private CoreWebView2 CoreWebView { get; set; }

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

        private Uri _source;

        public Uri Source
        {
            get { return _source; }

            set
            {
                _source = value;
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

        // 网页后退
        public IRelayCommand BrowserBackCommand => new RelayCommand(() =>
        {
            if (CoreWebView is not null && CoreWebView.CanGoBack)
            {
                CoreWebView.GoBack();
            }
        });

        // 网页前进
        public IRelayCommand BrowserForwardCommand => new RelayCommand(() =>
        {
            if (CoreWebView is not null && CoreWebView.CanGoForward)
            {
                CoreWebView.GoForward();
            }
        });

        // 网页刷新
        public IRelayCommand RefreshCommand => new RelayCommand<WebView2>((webView) =>
        {
            webView.Reload();
        });

        // 在浏览器中打开
        public IRelayCommand OpenWithBrowserCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(Source);
        });

        // 打开缓存文件夹
        public IRelayCommand OpenCacheFolderCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView")));
        });

        // 清理网页缓存
        public IRelayCommand ClearWebCacheCommand => new RelayCommand(async () =>
        {
            await CoreWebView.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
            new WebCacheCleanNotification(true).Show();
        });

        /// <summary>
        /// 浏览器内核进程发生异常时对应的事件
        /// </summary>
        public async void OnCoreProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs args)
        {
            // 显示异常信息错误原因，弹出对话框
            if (!Program.ApplicationRoot.IsDialogOpening)
            {
                Program.ApplicationRoot.IsDialogOpening = true;
                await new CoreWebView2FailedDialog(args).ShowAsync();
                Program.ApplicationRoot.IsDialogOpening = false;
            }
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

                IsEnabled = true;
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
                catch (Exception)
                {
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
                        DuplicatedDataInfoArgs CheckResult = await DownloadDBService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

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
                                    if (!Program.ApplicationRoot.IsDialogOpening)
                                    {
                                        Program.ApplicationRoot.IsDialogOpening = true;

                                        ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Unfinished).ShowAsync();

                                        if (result == ContentDialogResult.Primary)
                                        {
                                            try
                                            {
                                                if (File.Exists(backgroundItem.FilePath))
                                                {
                                                    File.Delete(backgroundItem.FilePath);
                                                }
                                            }
                                            finally
                                            {
                                                bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                                new DownloadCreateNotification(AddResult).Show();
                                            }
                                        }
                                        else if (result == ContentDialogResult.Secondary)
                                        {
                                            NavigationService.NavigateTo(typeof(DownloadPage));
                                        }
                                        Program.ApplicationRoot.IsDialogOpening = false;
                                    }
                                    break;
                                }

                            case DuplicatedDataInfoArgs.Completed:
                                {
                                    if (!Program.ApplicationRoot.IsDialogOpening)
                                    {
                                        Program.ApplicationRoot.IsDialogOpening = true;

                                        ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfoArgs.Completed).ShowAsync();

                                        if (result == ContentDialogResult.Primary)
                                        {
                                            try
                                            {
                                                if (File.Exists(backgroundItem.FilePath))
                                                {
                                                    File.Delete(backgroundItem.FilePath);
                                                }
                                            }
                                            finally
                                            {
                                                bool AddResult = await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                                new DownloadCreateNotification(AddResult).Show();
                                            }
                                        }
                                        else if (result == ContentDialogResult.Secondary)
                                        {
                                            NavigationService.NavigateTo(typeof(DownloadPage));
                                        }
                                        Program.ApplicationRoot.IsDialogOpening = false;
                                    }
                                    break;
                                }
                        }
                    }

                    // 使用浏览器下载
                    else if (DownloadOptionsService.DownloadMode == DownloadOptionsService.DownloadModeList[1])
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri(args.DownloadOperation.Uri));
                    }

                    args.DownloadOperation.Cancel();
                }
            }, null);
        }
    }
}

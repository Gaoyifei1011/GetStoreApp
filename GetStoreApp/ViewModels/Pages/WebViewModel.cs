using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.ViewModels;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Download;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using Windows.Foundation;

namespace GetStoreApp.ViewModels.Pages
{
    public class WebViewModel : ObservableRecipient, INavigationAware
    {
        private const string DefaultUrl = "https://store.rg-adguard.net/";

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public bool IsWebView2Installed => WebView2Helper.IsInstalled();

        private CoreWebView2 CoreWebView { get; set; }

        private bool _canGoBack = false;

        public bool CanGoBack
        {
            get { return _canGoBack; }

            set { SetProperty(ref _canGoBack, value); }
        }

        private bool _canGoForward = false;

        public bool CanGoForward
        {
            get { return _canGoForward; }

            set { SetProperty(ref _canGoForward, value); }
        }

        private Uri _source;

        public Uri Source
        {
            get { return _source; }

            set { SetProperty(ref _source, value); }
        }

        private bool _isLoading = true;

        public bool IsLoading
        {
            get { return _isLoading; }

            set { SetProperty(ref _isLoading, value); }
        }

        /// <summary>
        /// 浏览器内核进程发生异常时对应的事件
        /// </summary>
        public IRelayCommand CoreProcessFailedCommand => new RelayCommand<CoreWebView2ProcessFailedEventArgs>(async (args) =>
        {
            // 显示异常信息错误原因，弹出对话框
            await new CoreWebView2FailedDialog(args).ShowAsync();
        });

        /// <summary>
        /// 初始化CoreWebView2对象
        /// </summary>
        public IRelayCommand CoreWebView2InitializedCommand => new RelayCommand<WebView2>((webView) =>
        {
            if (CoreWebView is null && webView.CoreWebView2 is not null)
            {
                CoreWebView = webView.CoreWebView2;

                // 挂载对应的事件
                CoreWebView.NewWindowRequested += OnNewWindowRequested;
                CoreWebView.SourceChanged += OnSourceChanged;
                CoreWebView.DownloadStarting += OnDownloadStarting;
            }
        });

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

        // 页面开始导航
        public IRelayCommand NavigationStartingCommand => new RelayCommand(() =>
        {
            IsLoading = true;
        });

        // 页面完成导航
        public IRelayCommand NavigationCompletedCommand => new RelayCommand(() =>
        {
            IsLoading = false;
        });

        public void InitializeSource()
        {
            Source = new Uri(DefaultUrl);
        }

        public void OnNavigatedTo(object parameter)
        { }

        public void OnNavigatedFrom()
        {
            if (CoreWebView is not null)
            {
                CoreWebView.NewWindowRequested -= OnNewWindowRequested;
                CoreWebView.SourceChanged -= OnSourceChanged;
                CoreWebView.DownloadStarting -= OnDownloadStarting;
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

            System.Threading.SynchronizationContext.Current.Post(async (_) =>
            {
                using (deferral)
                {
                    // Hide the default download dialog.
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
                        DuplicatedDataInfo CheckResult = await DownloadDBService.CheckDuplicatedAsync(backgroundItem.DownloadKey);

                        switch (CheckResult)
                        {
                            case DuplicatedDataInfo.None: await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Add"); break;

                            case DuplicatedDataInfo.Unfinished:
                                {
                                    ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfo.Unfinished).ShowAsync();

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
                                            await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                        }
                                    }
                                    else if (result == ContentDialogResult.Secondary)
                                    {
                                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
                                    }
                                    break;
                                }

                            case DuplicatedDataInfo.Completed:
                                {
                                    ContentDialogResult result = await new DownloadNotifyDialog(DuplicatedDataInfo.Completed).ShowAsync();

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
                                            await DownloadSchedulerService.AddTaskAsync(backgroundItem, "Update");
                                        }
                                    }
                                    else if (result == ContentDialogResult.Secondary)
                                    {
                                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
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

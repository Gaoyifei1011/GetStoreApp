using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.AppUpdate;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 商店应用更新页面
    /// </summary>
    public sealed partial class AppUpdatePage : Page, INotifyPropertyChanged
    {
        private static readonly string AcquiringLicense = ResourceService.GetLocalized("AppUpdate/AcquiringLicense");
        private static readonly string Canceled = ResourceService.GetLocalized("AppUpdate/Canceled");
        private static readonly string Completed = ResourceService.GetLocalized("AppUpdate/Completed");
        private static readonly string Downloading = ResourceService.GetLocalized("AppUpdate/Downloading");
        private static readonly string Error = ResourceService.GetLocalized("AppUpdate/Error");
        private static readonly string Installing = ResourceService.GetLocalized("AppUpdate/Installing");
        private static readonly string InstallingSubInformation = ResourceService.GetLocalized("AppUpdate/InstallingSubInformation");
        private static readonly string Paused = ResourceService.GetLocalized("AppUpdate/Paused");
        private static readonly string Pending = ResourceService.GetLocalized("AppUpdate/Pending");
        private static readonly string ReadyToDownload = ResourceService.GetLocalized("AppUpdate/ReadyToDownload");
        private static readonly string RestoringData = ResourceService.GetLocalized("AppUpdate/RestoringData");
        private static readonly string Starting = ResourceService.GetLocalized("AppUpdate/Starting");

        private readonly AppInstallManager appInstallManager = new();
        private readonly PackageManager packageManager = new();

        private readonly Dictionary<string, AppInstallItem> AppInstallingDict = [];

        private string AppUpdateCountInfo { get; } = ResourceService.GetLocalized("AppUpdate/AppUpdateCountInfo");

        private bool _isInitialized;

        public bool IsInitialized
        {
            get { return _isInitialized; }

            set
            {
                if (!Equals(_isInitialized, value))
                {
                    _isInitialized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInitialized)));
                }
            }
        }

        private bool _isLoadedCompleted = true;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                if (!Equals(_isLoadedCompleted, value))
                {
                    _isLoadedCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadedCompleted)));
                }
            }
        }

        private bool _isUpdateEmpty = true;

        public bool IsUpdateEmpty
        {
            get { return _isUpdateEmpty; }

            set
            {
                if (!Equals(_isUpdateEmpty, value))
                {
                    _isUpdateEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdateEmpty)));
                }
            }
        }

        private ObservableCollection<AppUpdateModel> AppUpdateCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppUpdatePage()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 更新选定的应用
        /// </summary>
        private void OnUpdateExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string packageFamilyName && !string.IsNullOrEmpty(packageFamilyName))
            {
                Task.Run(async () =>
                {
                    AppInstallItem appInstallItem = null;
                    foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                    {
                        if (appUpdateItem.PackageFamilyName == packageFamilyName)
                        {
                            AutoResetEvent autoResetEvent = new(false);
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                appUpdateItem.AppInstallState = AppInstallState.Pending;
                                appUpdateItem.IsUpdating = true;
                                appUpdateItem.InstallInformation = Pending;
                                appUpdateItem.PercentComplete = 0;

                                autoResetEvent.Set();
                            });

                            autoResetEvent.WaitOne();
                            autoResetEvent.Dispose();

                            appInstallItem = await appInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);

                            if (appInstallItem is not null)
                            {
                                appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;
                                appInstallItem.Completed += OnAppInstallItemCompleted;

                                AppInstallingDict.TryAdd(appInstallItem.PackageFamilyName, appInstallItem);
                            }

                            break;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        private void OnCancelExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string packageFamilyName && !string.IsNullOrEmpty(packageFamilyName))
            {
                Task.Run(() =>
                {
                    if (AppInstallingDict.TryGetValue(packageFamilyName, out AppInstallItem appInstallItem))
                    {
                        try
                        {
                            appInstallItem.Completed -= OnAppInstallItemCompleted;
                            appInstallItem.StatusChanged -= OnAppInstallItemStatausChanged;
                            appInstallItem.Cancel();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Store app cancel install failed", e);
                        }

                        AppInstallingDict.Remove(packageFamilyName);
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                        {
                            if (appUpdateItem.PackageFamilyName == packageFamilyName)
                            {
                                appUpdateItem.AppInstallState = AppInstallState.Canceled;
                                appUpdateItem.IsUpdating = false;
                                appUpdateItem.InstallInformation = Canceled;
                                appUpdateItem.PercentComplete = 0;
                                break;
                            }
                        }
                    });
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：商店应用更新页面——挂载的事件

        /// <summary>
        /// 打开微软商店并更新微软商店应用
        /// </summary>
        private async void OnOpenStoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://downloadsandupdates"));
        }

        /// <summary>
        /// 设置当前系统的预览体验计划
        /// </summary>
        private async void OnInsiderProgramClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:windowsinsider"));
        }

        /// <summary>
        /// 检查商店应用更新
        /// </summary>
        private void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsLoadedCompleted = false;

            if (!IsInitialized) IsInitialized = true;

            Task.Run(async () =>
            {
                AppUpdateOptions updateOptions = new()
                {
                    AutomaticallyDownloadAndInstallUpdateIfFound = false,
                    AllowForcedAppRestart = false
                };
                IReadOnlyList<AppInstallItem> upgradableAppsList = await appInstallManager.SearchForAllUpdatesAsync(string.Empty, string.Empty, updateOptions);
                List<AppUpdateModel> appUpdateList = [];

                foreach (AppInstallItem upgradableApps in upgradableAppsList)
                {
                    // 判断是否已经添加到 AppUpdateCollection 中，没有则添加
                    bool isExisted = false;
                    foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                    {
                        if (appUpdateItem.PackageFamilyName.Equals(upgradableApps.PackageFamilyName))
                        {
                            isExisted = true;
                        }
                    }

                    if (!isExisted)
                    {
                        foreach (Package packageItem in packageManager.FindPackagesForUser(string.Empty))
                        {
                            if (packageItem.Id.FamilyName.Equals(upgradableApps.PackageFamilyName, StringComparison.OrdinalIgnoreCase))
                            {
                                AppInstallStatus appInstallStatus = upgradableApps.GetCurrentStatus();
                                string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus);
                                string installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));

                                appUpdateList.Add(new AppUpdateModel()
                                {
                                    AppInstallState = appInstallStatus.InstallState,
                                    DisplayName = packageItem.DisplayName,
                                    PublisherName = packageItem.PublisherDisplayName,
                                    InstallInformation = installInformation,
                                    InstallSubInformation = installSubInformation,
                                    IsUpdating = appInstallStatus.InstallState is AppInstallState.Pending ||
                                                 appInstallStatus.InstallState is AppInstallState.Starting ||
                                                 appInstallStatus.InstallState is AppInstallState.Downloading ||
                                                 appInstallStatus.InstallState is AppInstallState.RestoringData ||
                                                 appInstallStatus.InstallState is AppInstallState.Installing,
                                    PackageFamilyName = upgradableApps.PackageFamilyName,
                                    PercentComplete = appInstallStatus.PercentComplete,
                                    ProductId = upgradableApps.ProductId
                                });
                                break;
                            }
                        }
                    }
                }

                // 只添加未有的项
                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (AppUpdateModel appUpdateItem in appUpdateList)
                    {
                        AppUpdateCollection.Add(appUpdateItem);
                    }

                    IsLoadedCompleted = true;
                });
            });
        }

        /// <summary>
        /// 更新所有应用
        /// </summary>
        private void OnUpdateAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
            {
                if (!appUpdateItem.IsUpdating)
                {
                    appUpdateItem.AppInstallState = AppInstallState.Pending;
                    appUpdateItem.IsUpdating = true;
                    appUpdateItem.InstallInformation = Pending;
                    appUpdateItem.PercentComplete = 0;
                }
            }

            Task.Run(async () =>
            {
                foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                {
                    AppInstallItem appInstallItem = await appInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);

                    if (appInstallItem is not null && !AppInstallingDict.ContainsKey(appInstallItem.PackageFamilyName))
                    {
                        appInstallItem.Completed += OnAppInstallItemCompleted;
                        appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;

                        AppInstallingDict.TryAdd(appInstallItem.PackageFamilyName, appInstallItem);
                    }
                }
            });
        }

        /// <summary>
        /// 当前应用安装完成时发生的事件
        /// </summary>
        private void OnAppInstallItemCompleted(AppInstallItem sender, object args)
        {
            foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
            {
                if (appUpdateItem.PackageFamilyName == sender.PackageFamilyName)
                {
                    sender.Completed -= OnAppInstallItemCompleted;
                    sender.StatusChanged -= OnAppInstallItemStatausChanged;

                    AppInstallingDict.Remove(appUpdateItem.PackageFamilyName);
                    ToastNotificationService.Show(NotificationKind.AppUpdate, appUpdateItem.DisplayName);
                    break;
                }
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                {
                    if (appUpdateItem.PackageFamilyName == sender.PackageFamilyName)
                    {
                        try
                        {
                            AppUpdateCollection.Remove(appUpdateItem);
                            break;
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "AppUpdateCollection remove items failed", e);
                        }
                    }
                }
            });
        }

        #endregion 第二部分：商店应用更新页面——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 当前应用的安装状态发生更改时的事件
        /// </summary>
        private void OnAppInstallItemStatausChanged(AppInstallItem sender, object args)
        {
            foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
            {
                if (appUpdateItem.PackageFamilyName == sender.PackageFamilyName)
                {
                    AutoResetEvent autoResetEvent = new(false);
                    AppInstallStatus appInstallStatus = sender.GetCurrentStatus();
                    string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus);
                    string installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        appUpdateItem.AppInstallState = appInstallStatus.InstallState;
                        appUpdateItem.PercentComplete = appInstallStatus.PercentComplete;
                        appUpdateItem.InstallInformation = installInformation;
                        appUpdateItem.InstallSubInformation = installSubInformation;
                        appUpdateItem.IsUpdating = !(appInstallStatus.InstallState is AppInstallState.Canceled ||
                            appInstallStatus.InstallState is AppInstallState.Error ||
                            appInstallStatus.InstallState is AppInstallState.Paused ||
                            appInstallStatus.InstallState is AppInstallState.PausedLowBattery ||
                            appInstallStatus.InstallState is AppInstallState.PausedWiFiRecommended ||
                            appInstallStatus.InstallState is AppInstallState.PausedWiFiRequired);

                        autoResetEvent.Set();
                    });

                    autoResetEvent.WaitOne();
                    autoResetEvent.Dispose();
                }
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 获取应用安装的描述信息
        /// </summary>
        private static string GetInstallInformation(AppInstallState appInstallState, AppInstallStatus appInstallStatus)
        {
            return appInstallState switch
            {
                AppInstallState.AcquiringLicense => AcquiringLicense,
                AppInstallState.Canceled => Canceled,
                AppInstallState.Completed => Completed,
                AppInstallState.Downloading => string.Format(Downloading, appInstallStatus.PercentComplete),
                AppInstallState.Error => string.Format(Error, appInstallStatus.ErrorCode.HResult),
                AppInstallState.Installing => string.Format(Installing, appInstallStatus.PercentComplete),
                AppInstallState.Paused => Paused,
                AppInstallState.PausedLowBattery => Paused,
                AppInstallState.PausedWiFiRecommended => Paused,
                AppInstallState.PausedWiFiRequired => Paused,
                AppInstallState.Pending => Pending,
                AppInstallState.ReadyToDownload => ReadyToDownload,
                AppInstallState.RestoringData => RestoringData,
                AppInstallState.Starting => Starting,
                _ => string.Empty,
            };
        }
    }
}

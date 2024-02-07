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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 商店应用更新页面
    /// </summary>
    public sealed partial class AppUpdatePage : Page, INotifyPropertyChanged
    {
        private static readonly object appUpdateLock = new object();

        private static string AcquiringLicense = ResourceService.GetLocalized("AppUpdate/AcquiringLicense");
        private static string Canceled = ResourceService.GetLocalized("AppUpdate/Canceled");
        private static string Completed = ResourceService.GetLocalized("AppUpdate/Completed");
        private static string Downloading = ResourceService.GetLocalized("AppUpdate/Downloading");
        private static string Error = ResourceService.GetLocalized("AppUpdate/Error");
        private static string Installing = ResourceService.GetLocalized("AppUpdate/Installing");
        private static string InstallingSubInformation = ResourceService.GetLocalized("AppUpdate/InstallingSubInformation");
        private static string Paused = ResourceService.GetLocalized("AppUpdate/Paused");
        private static string Pending = ResourceService.GetLocalized("AppUpdate/Pending");
        private static string ReadyToDownload = ResourceService.GetLocalized("AppUpdate/ReadyToDownload");
        private static string RestoringData = ResourceService.GetLocalized("AppUpdate/RestoringData");
        private static string Starting = ResourceService.GetLocalized("AppUpdate/Starting");

        private AppInstallManager appInstallManager = new AppInstallManager();
        private PackageManager packageManager = new PackageManager();

        private Dictionary<string, AppInstallItem> AppInstallingDict = new Dictionary<string, AppInstallItem>();

        private bool _isInitialized;

        public bool IsInitialized
        {
            get { return _isInitialized; }

            set
            {
                _isInitialized = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoadedCompleted = true;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                _isLoadedCompleted = value;
                OnPropertyChanged();
            }
        }

        private bool _isUpdateEmpty = true;

        public bool IsUpdateEmpty
        {
            get { return _isUpdateEmpty; }

            set
            {
                _isUpdateEmpty = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AppUpdateModel> AppUpdateCollection { get; } = new ObservableCollection<AppUpdateModel>();

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
            string packageFamilyName = args.Parameter as string;

            if (packageFamilyName is not null)
            {
                Task.Run(async () =>
                {
                    AppInstallItem appInstallItem = null;
                    foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                    {
                        if (appUpdateItem.PackageFamilyName == packageFamilyName)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                lock (appUpdateLock)
                                {
                                    appUpdateItem.AppInstallState = AppInstallState.Pending;
                                    appUpdateItem.IsUpdating = true;
                                    appUpdateItem.InstallInformation = Pending;
                                    appUpdateItem.PercentComplete = 0;
                                }
                            });

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
            string packageFamilyName = args.Parameter as string;

            if (packageFamilyName is not null)
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
                        lock (appUpdateLock)
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
        private async void OnWIPSettingsClicked(object sender, RoutedEventArgs args)
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
                AppUpdateOptions updateOptions = new AppUpdateOptions();
                updateOptions.AutomaticallyDownloadAndInstallUpdateIfFound = false;
                updateOptions.AllowForcedAppRestart = false;
                IReadOnlyList<AppInstallItem> upgradableAppsList = await appInstallManager.SearchForAllUpdatesAsync(string.Empty, string.Empty, updateOptions);
                List<AppUpdateModel> appUpdateList = new List<AppUpdateModel>();

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
                                string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus.PercentComplete);
                                string installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));

                                appUpdateList.Add(new AppUpdateModel()
                                {
                                    AppInstallState = appInstallStatus.InstallState,
                                    DisplayName = packageItem.DisplayName,
                                    PublisherName = packageItem.PublisherDisplayName,
                                    InstallInformation = installInformation,
                                    InstallSubInformation = installSubInformation,
                                    IsUpdating = !(appInstallStatus.InstallState is AppInstallState.Canceled ||
                                        appInstallStatus.InstallState is AppInstallState.Error ||
                                        appInstallStatus.InstallState is AppInstallState.Paused ||
                                        appInstallStatus.InstallState is AppInstallState.PausedLowBattery ||
                                        appInstallStatus.InstallState is AppInstallState.PausedWiFiRecommended ||
                                        appInstallStatus.InstallState is AppInstallState.PausedWiFiRequired),
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
                    lock (appUpdateLock)
                    {
                        foreach (AppUpdateModel appUpdateItem in appUpdateList)
                        {
                            AppUpdateCollection.Add(appUpdateItem);
                        }
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
            Task.Run(async () =>
            {
                foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                {
                    if (!appUpdateItem.IsUpdating)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (appUpdateLock)
                            {
                                appUpdateItem.AppInstallState = AppInstallState.Pending;
                                appUpdateItem.IsUpdating = true;
                                appUpdateItem.InstallInformation = Pending;
                                appUpdateItem.PercentComplete = 0;
                            }
                        });
                    }
                }

                foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                {
                    AppInstallItem appInstallItem = await appInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);

                    if (appInstallItem != null && !AppInstallingDict.ContainsKey(appInstallItem.PackageFamilyName))
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
                lock (appUpdateLock)
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
                    AppInstallStatus appInstallStatus = sender.GetCurrentStatus();
                    string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus.PercentComplete);
                    string installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (appUpdateLock)
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
                        }
                    });
                }
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 本地化应用更新数量统计信息
        /// </summary>
        public string LocalizeAppUpdateCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("AppUpdate/AppUpdateEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("AppUpdate/AppUpdateCountInfo"), count);
            }
        }

        /// <summary>
        /// 获取应用安装的描述信息
        /// </summary>
        private string GetInstallInformation(AppInstallState appInstallState, double percentComplete)
        {
            return appInstallState switch
            {
                AppInstallState.AcquiringLicense => AcquiringLicense,
                AppInstallState.Canceled => Canceled,
                AppInstallState.Completed => Completed,
                AppInstallState.Downloading => string.Format(Downloading, percentComplete),
                AppInstallState.Error => Error,
                AppInstallState.Installing => string.Format(Installing, percentComplete),
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

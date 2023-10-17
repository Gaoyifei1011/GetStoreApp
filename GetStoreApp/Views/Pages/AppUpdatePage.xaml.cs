using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.AppUpdate;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 商店应用更新页面
    /// </summary>
    public sealed partial class AppUpdatePage : Page, INotifyPropertyChanged
    {
        private static readonly object AppUpdateDataListLock = new object();

        private static string AcquiringLicense = ResourceService.GetLocalized("AppUpdate/AcquiringLicense");

        private static string Canceled = ResourceService.GetLocalized("AppUpdate/Canceled");

        private static string Completed = ResourceService.GetLocalized("AppUpdate/Completed");

        private static string Downloading = ResourceService.GetLocalized("AppUpdate/Downloading");

        private static string Error = ResourceService.GetLocalized("AppUpdate/Error");

        private static string Installing = ResourceService.GetLocalized("AppUpdate/Installing");

        private static string Paused = ResourceService.GetLocalized("AppUpdate/Paused");

        private static string Pending = ResourceService.GetLocalized("AppUpdate/Pending");

        private static string ReadyToDownload = ResourceService.GetLocalized("AppUpdate/ReadyToDownload");

        private static string RestoringData = ResourceService.GetLocalized("AppUpdate/RestoringData");

        private static string Starting = ResourceService.GetLocalized("AppUpdate/Starting");

        private AppInstallManager AppInstallManager { get; } = new AppInstallManager();

        private PackageManager PackageManager { get; } = new PackageManager();

        private Dictionary<string, AppInstallItem> AppInstallingDict = new Dictionary<string, AppInstallItem>();

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

        public List<Package> PackageDataList { get; set; }

        public ObservableCollection<AppUpdateModel> AppUpdateDataList { get; } = new ObservableCollection<AppUpdateModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AppUpdatePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 更新选定的应用
        /// </summary>
        public void OnUpdateExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string packageFamilyName = args.Parameter as string;

            if (packageFamilyName is not null)
            {
                Task.Run(async () =>
                {
                    AppInstallItem appInstallItem = null;
                    foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
                    {
                        if (appUpdateItem.PackageFamilyName == packageFamilyName)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                lock (AppUpdateDataList)
                                {
                                    appUpdateItem.IsUpdating = true;
                                }
                            });

                            appInstallItem = await AppInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);

                            if (appInstallItem is not null)
                            {
                                appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;
                                appInstallItem.Completed += OnAppInstallItemCompleted;

                                AppInstallingDict.TryAdd(appInstallItem.PackageFamilyName, appInstallItem);

                                AppInstallStatus appInstallStatus = appInstallItem.GetCurrentStatus();

                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    lock (AppUpdateDataList)
                                    {
                                        // TODO: need to change
                                        appUpdateItem.InstallInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus.PercentComplete);
                                        appUpdateItem.InstallDetailInformation = "";
                                    }
                                });
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
        public void OnCancelExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string packageFamilyName = args.Parameter as string;

            if (packageFamilyName is not null)
            {
                Task.Run(() =>
                {
                    if (AppInstallingDict.TryGetValue(packageFamilyName, out AppInstallItem appInstallItem))
                    {
                        appInstallItem.Completed -= OnAppInstallItemCompleted;
                        appInstallItem.StatusChanged -= OnAppInstallItemStatausChanged;
                        appInstallItem.Cancel();

                        AppInstallingDict.Remove(packageFamilyName);

                        AppInstallStatus appInstallStatus = appInstallItem.GetCurrentStatus();

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (AppUpdateDataListLock)
                            {
                                foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
                                {
                                    if (appUpdateItem.PackageFamilyName == packageFamilyName)
                                    {
                                        appUpdateItem.IsUpdating = false;
                                        // TODO: need to change
                                        appUpdateItem.InstallInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus.PercentComplete);
                                        appUpdateItem.InstallDetailInformation = "";
                                        break;
                                    }
                                }
                            }
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 检查商店应用更新
        /// </summary>
        public void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsLoadedCompleted = false;
            Task.Run(async () =>
            {
                AppUpdateOptions updateOptions = new AppUpdateOptions();
                updateOptions.AutomaticallyDownloadAndInstallUpdateIfFound = false;
                updateOptions.AllowForcedAppRestart = false;
                IReadOnlyList<AppInstallItem> upgradableAppsList = await AppInstallManager.SearchForAllUpdatesAsync(string.Empty, string.Empty, updateOptions);
                List<AppUpdateModel> appUpdateList = new List<AppUpdateModel>();
                PackageDataList = PackageManager.FindPackagesForUser(string.Empty).ToList();

                if (PackageDataList is not null)
                {
                    foreach (AppInstallItem upgradableApps in upgradableAppsList)
                    {
                        // 判断是否已经添加到 AppUpdateDataList 中，没有则添加
                        if (!AppUpdateDataList.Any(item => item.PackageFamilyName == upgradableApps.PackageFamilyName))
                        {
                            Package package = PackageDataList.FirstOrDefault(item => item.Id.FamilyName.Equals(upgradableApps.PackageFamilyName, StringComparison.OrdinalIgnoreCase));
                            if (package is not null)
                            {
                                AppInstallStatus appInstallStatus = upgradableApps.GetCurrentStatus();
                                // TODO: need to change
                                appUpdateList.Add(new AppUpdateModel()
                                {
                                    DisplayName = package.DisplayName,
                                    PublisherName = package.PublisherDisplayName,
                                    InstallInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus.PercentComplete),
                                    InstallDetailInformation = "",
                                    IsUpdating = false,
                                    PackageFamilyName = upgradableApps.PackageFamilyName,
                                    ProductId = upgradableApps.ProductId
                                });
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    // 只添加未有的项
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (AppUpdateDataListLock)
                        {
                            foreach (AppUpdateModel appUpdateItem in appUpdateList)
                            {
                                AppUpdateDataList.Add(appUpdateItem);
                            }
                        }

                        IsLoadedCompleted = true;
                    });
                }
            });
        }

        /// <summary>
        /// 更新所有应用
        /// </summary>
        public void OnUpdateAllClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
                {
                    AppInstallItem appInstallItem = await AppInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);

                    if (appInstallItem != null && !AppInstallingDict.ContainsKey(appInstallItem.PackageFamilyName))
                    {
                        appInstallItem.Completed += OnAppInstallItemCompleted;
                        appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;

                        AppInstallingDict.TryAdd(appInstallItem.PackageFamilyName, appInstallItem);

                        AppInstallStatus appInstallStatus = appInstallItem.GetCurrentStatus();

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (AppUpdateDataListLock)
                            {
                                appUpdateItem.IsUpdating = true;
                                // TODO: need to change
                                appUpdateItem.InstallInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus.PercentComplete);
                                appUpdateItem.InstallDetailInformation = "";
                            }
                        });
                    }
                }
            });
        }

        /// <summary>
        /// 当前应用安装完成时发生的事件
        /// </summary>
        public void OnAppInstallItemCompleted(AppInstallItem sender, object args)
        {
            foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
            {
                if (appUpdateItem.PackageFamilyName == sender.PackageFamilyName)
                {
                    sender.Completed -= OnAppInstallItemCompleted;
                    sender.StatusChanged -= OnAppInstallItemStatausChanged;

                    AppInstallingDict.Remove(appUpdateItem.PackageFamilyName);
                    break;
                }

                ToastNotificationService.Show(NotificationKind.AppUpdate, appUpdateItem.DisplayName);
            }

            DispatcherQueue.TryEnqueue(() =>
            {
                lock (AppUpdateDataListLock)
                {
                    foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
                    {
                        if (appUpdateItem.PackageFamilyName == sender.PackageFamilyName)
                        {
                            try
                            {
                                AppUpdateDataList.Remove(appUpdateItem);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "AppUpdateDataList remove items failed", e);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 当前应用的安装状态发生更改时的事件
        /// </summary>
        public void OnAppInstallItemStatausChanged(AppInstallItem sender, object args)
        {
            foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
            {
                if (appUpdateItem.PackageFamilyName == sender.PackageFamilyName)
                {
                    AppInstallStatus appInstallStatus = sender.GetCurrentStatus();
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        appUpdateItem.AppInstallState = appInstallStatus.InstallState;
                        appUpdateItem.BytesDownload = appInstallStatus.BytesDownloaded;
                        appUpdateItem.DownloadSizeInBytes = appInstallStatus.DownloadSizeInBytes;
                        appUpdateItem.ErrorCode = appInstallStatus.ErrorCode;
                        appUpdateItem.PercentComplete = appInstallStatus.PercentComplete;
                        // TODO: need to change
                        appUpdateItem.InstallInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus.PercentComplete);
                        appUpdateItem.InstallDetailInformation = "";

                        if (appInstallStatus.InstallState is AppInstallState.Canceled ||
                            appInstallStatus.InstallState is AppInstallState.Error ||
                            appInstallStatus.InstallState is AppInstallState.Paused ||
                            appInstallStatus.InstallState is AppInstallState.PausedLowBattery ||
                            appInstallStatus.InstallState is AppInstallState.PausedWiFiRecommended ||
                            appInstallStatus.InstallState is AppInstallState.PausedWiFiRequired
                            )
                        {
                            appUpdateItem.IsUpdating = false;
                        }
                    });
                }
            }
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
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                AppInstallState.Downloading => string.Format(Installing, percentComplete),
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

        /// <summary>
        /// 获取应用安装的详细描述信息
        /// </summary>
        //private string GetInstallDetailedInformation(AppInstallState appInstallState, AppInstallStatus appInstallStatus)
        //{
        //    return string.Empty;
        //}
    }
}

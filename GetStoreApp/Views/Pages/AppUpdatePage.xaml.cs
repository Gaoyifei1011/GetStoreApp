using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.AppUpdate;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
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

        private readonly Lock appinstallingLock = new();
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
        private async void OnUpdateExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string packageFamilyName && !string.IsNullOrEmpty(packageFamilyName))
            {
                foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                {
                    if (appUpdateItem.PackageFamilyName == packageFamilyName)
                    {
                        appUpdateItem.AppInstallState = AppInstallState.Pending;
                        appUpdateItem.IsUpdating = true;
                        appUpdateItem.InstallInformation = Pending;
                        appUpdateItem.PercentComplete = 0;

                        await Task.Run(async () =>
                        {
                            AppInstallItem appInstallItem = null;

                            try
                            {
                                appInstallItem = await appInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, string.Format("Update app failed: package family name {0}", appUpdateItem.PackageFamilyName), e);
                            }

                            appinstallingLock.Enter();

                            try
                            {
                                if (appInstallItem is not null && !AppInstallingDict.ContainsKey(appInstallItem.PackageFamilyName))
                                {
                                    appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;
                                    appInstallItem.Completed += OnAppInstallItemCompleted;

                                    AppInstallingDict.TryAdd(appInstallItem.PackageFamilyName, appInstallItem);
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                appinstallingLock.Exit();
                            }
                        });

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        private async void OnCancelExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string packageFamilyName && !string.IsNullOrEmpty(packageFamilyName))
            {
                await Task.Run(() =>
                {
                    appinstallingLock.Enter();

                    try
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
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        appinstallingLock.Exit();
                    }
                });

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
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsLoadedCompleted = false;

            if (!IsInitialized) IsInitialized = true;

            List<AppUpdateModel> appUpdateList = [];
            await Task.Run(async () =>
            {
                try
                {
                    AppUpdateOptions updateOptions = new()
                    {
                        AutomaticallyDownloadAndInstallUpdateIfFound = false,
                        AllowForcedAppRestart = false
                    };
                    IReadOnlyList<AppInstallItem> upgradableAppsList = await appInstallManager.SearchForAllUpdatesAsync(string.Empty, string.Empty, updateOptions);

                    foreach (AppInstallItem upgradableAppItem in upgradableAppsList)
                    {
                        // 判断是否已经添加到 AppUpdateCollection 中，没有则添加
                        bool isExisted = false;
                        foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                        {
                            if (appUpdateItem.PackageFamilyName.Equals(upgradableAppItem.PackageFamilyName))
                            {
                                isExisted = true;
                            }
                        }

                        if (!isExisted)
                        {
                            foreach (Package packageItem in packageManager.FindPackagesForUser(string.Empty))
                            {
                                if (packageItem.Id.FamilyName.Equals(upgradableAppItem.PackageFamilyName, StringComparison.OrdinalIgnoreCase))
                                {
                                    AppInstallStatus appInstallStatus = upgradableAppItem.GetCurrentStatus();
                                    string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus);
                                    string installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));

                                    appUpdateList.Add(new AppUpdateModel()
                                    {
                                        AppInstallState = appInstallStatus.InstallState,
                                        DisplayName = packageItem.DisplayName,
                                        PublisherDisplayName = packageItem.PublisherDisplayName,
                                        InstallInformation = installInformation,
                                        InstallSubInformation = installSubInformation,
                                        IsUpdating = appInstallStatus.InstallState is AppInstallState.Pending ||
                                                     appInstallStatus.InstallState is AppInstallState.Starting ||
                                                     appInstallStatus.InstallState is AppInstallState.AcquiringLicense ||
                                                     appInstallStatus.InstallState is AppInstallState.Downloading ||
                                                     appInstallStatus.InstallState is AppInstallState.RestoringData ||
                                                     appInstallStatus.InstallState is AppInstallState.Installing,
                                        PackageFamilyName = upgradableAppItem.PackageFamilyName,
                                        PercentComplete = appInstallStatus.PercentComplete,
                                        ProductId = upgradableAppItem.ProductId
                                    });

                                    appinstallingLock.Enter();

                                    try
                                    {
                                        if (upgradableAppItem is not null && !AppInstallingDict.ContainsKey(upgradableAppItem.PackageFamilyName))
                                        {
                                            upgradableAppItem.StatusChanged += OnAppInstallItemStatausChanged;
                                            upgradableAppItem.Completed += OnAppInstallItemCompleted;

                                            AppInstallingDict.TryAdd(upgradableAppItem.PackageFamilyName, upgradableAppItem);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        appinstallingLock.Exit();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Check store update status failed", e);
                }
            });

            // 只添加未有的项
            foreach (AppUpdateModel appUpdateItem in appUpdateList)
            {
                AppUpdateCollection.Add(appUpdateItem);
            }

            IsLoadedCompleted = true;
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

                    appinstallingLock.Enter();

                    try
                    {
                        if (appInstallItem is not null && !AppInstallingDict.ContainsKey(appInstallItem.PackageFamilyName))
                        {
                            appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;
                            appInstallItem.Completed += OnAppInstallItemCompleted;

                            AppInstallingDict.TryAdd(appInstallItem.PackageFamilyName, appInstallItem);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        appinstallingLock.Exit();
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

                    appinstallingLock.Enter();

                    try
                    {
                        AppInstallingDict.Remove(appUpdateItem.PackageFamilyName);
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        appinstallingLock.Exit();
                    }

                    // 显示商店应用更新成功通知
                    if (appUpdateItem.AppInstallState is AppInstallState.Completed)
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppUpdateSuccessfully"), appUpdateItem.DisplayName));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    }

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
            DispatcherQueue.TryEnqueue(async () =>
            {
                foreach (AppUpdateModel appUpdateItem in AppUpdateCollection)
                {
                    if (appUpdateItem.PackageFamilyName == sender.PackageFamilyName)
                    {
                        AppInstallStatus appInstallStatus = null;
                        string installInformation = string.Empty;
                        string installSubInformation = string.Empty;

                        await Task.Run(() =>
                        {
                            try
                            {
                                appInstallStatus = sender.GetCurrentStatus();
                                installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus);
                                installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Get current app install status failed", e);
                            }
                        });

                        if (appInstallStatus is not null)
                        {
                            appUpdateItem.AppInstallState = appInstallStatus.InstallState;
                            appUpdateItem.PercentComplete = appInstallStatus.PercentComplete;
                            appUpdateItem.InstallInformation = installInformation;
                            appUpdateItem.InstallSubInformation = installSubInformation;
                            appUpdateItem.IsUpdating = appInstallStatus.InstallState is AppInstallState.Pending ||
                                         appInstallStatus.InstallState is AppInstallState.Starting ||
                                         appInstallStatus.InstallState is AppInstallState.AcquiringLicense ||
                                         appInstallStatus.InstallState is AppInstallState.Downloading ||
                                         appInstallStatus.InstallState is AppInstallState.RestoringData ||
                                         appInstallStatus.InstallState is AppInstallState.Installing;
                        }
                    }
                }
            });
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

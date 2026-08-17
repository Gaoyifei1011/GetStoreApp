using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
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

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 商店应用更新页面
    /// </summary>
    internal sealed partial class AppUpdatePage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AcquiringLicenseString = ResourceService.GetLocalized("AppUpdate/AcquiringLicense");
        private readonly string AppUpdateCountInfoString = ResourceService.GetLocalized("AppUpdate/AppUpdateCountInfo");
        private readonly string AppUpdateEmptyString = ResourceService.GetLocalized("AppUpdate/AppUpdateEmpty");
        private readonly string AppUpdateSuccessfullyString = ResourceService.GetLocalized("AppUpdate/AppUpdateSuccessfully");
        private readonly string CanceledString = ResourceService.GetLocalized("AppUpdate/Canceled");
        private readonly string CompletedString = ResourceService.GetLocalized("AppUpdate/Completed");
        private readonly string DownloadingString = ResourceService.GetLocalized("AppUpdate/Downloading");
        private readonly string ErrorString = ResourceService.GetLocalized("AppUpdate/Error");
        private readonly string InstallingString = ResourceService.GetLocalized("AppUpdate/Installing");
        private readonly string InstallingSubInformationString = ResourceService.GetLocalized("AppUpdate/InstallingSubInformation");
        private readonly string PausedString = ResourceService.GetLocalized("AppUpdate/Paused");
        private readonly string PendingString = ResourceService.GetLocalized("AppUpdate/Pending");
        private readonly string ReadyToDownloadString = ResourceService.GetLocalized("AppUpdate/ReadyToDownload");
        private readonly string RestoringDataString = ResourceService.GetLocalized("AppUpdate/RestoringData");
        private readonly string StartingString = ResourceService.GetLocalized("AppUpdate/Starting");
        private readonly Lock AppUpdateLock = new();
        private bool isInitialized;
        private AppInstallManager appInstallManager;
        private PackageManager packageManager;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private AppUpdateResultKind _appUpdateResultKind;

        private AppUpdateResultKind AppUpdateResultKind
        {
            get { return _appUpdateResultKind; }

            set
            {
                if (!Equals(_appUpdateResultKind, value))
                {
                    _appUpdateResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(AppUpdateResultKind)));
                }
            }
        }

        private bool _isCheckingUpdate;

        private bool IsCheckingUpdate
        {
            get { return _isCheckingUpdate; }

            set
            {
                if (!Equals(_isCheckingUpdate, value))
                {
                    _isCheckingUpdate = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCheckingUpdate)));
                }
            }
        }

        private string _appUpdateFailedContent;

        private string AppUpdateFailedContent
        {
            get { return _appUpdateFailedContent; }

            set
            {
                if (!string.Equals(_appUpdateFailedContent, value))
                {
                    _appUpdateFailedContent = value;
                    PropertyChanged?.Invoke(this, new(nameof(AppUpdateFailedContent)));
                }
            }
        }

        private List<AppUpdateModel> AppUpdateList { get; } = [];

        private ObservableCollection<AppUpdateModel> AppUpdateCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal AppUpdatePage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                AppUpdateResultKind = AppUpdateResultKind.NotCheckUpdate;
                appInstallManager = new();
                packageManager = new();
                MountAppUpdateEvent();
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 更新选定的应用
        /// </summary>
        private async void OnUpdateExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string packageFamilyName && !string.IsNullOrEmpty(packageFamilyName))
            {
                AppUpdateLock.Enter();

                try
                {
                    foreach (AppUpdateModel appUpdateItem in AppUpdateList)
                    {
                        if (string.Equals(appUpdateItem.PackageFamilyName, packageFamilyName))
                        {
                            appUpdateItem.IsOperating = true;
                            AppInstallItem appInstallItem = await UpdatAppAsync(appUpdateItem.PackageFamilyName);

                            // 安装更新添加成功
                            if (appInstallItem is not null)
                            {
                                appUpdateItem.IsOperating = false;
                                appUpdateItem.AppInstallState = AppInstallState.Pending;
                                appUpdateItem.IsUpdating = true;
                                appUpdateItem.InstallInformation = PendingString;
                                appUpdateItem.PercentComplete = 0;
                            }
                            // 安装更新添加失败
                            else
                            {
                                appUpdateItem.IsOperating = false;
                                appUpdateItem.AppInstallState = AppInstallState.Error;
                                appUpdateItem.IsUpdating = false;
                                appUpdateItem.InstallInformation = ErrorString;
                            }

                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    AppUpdateLock.Exit();
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
                AppUpdateLock.Enter();

                try
                {
                    foreach (AppUpdateModel appUpdateItem in AppUpdateList)
                    {
                        if (string.Equals(appUpdateItem.PackageFamilyName, packageFamilyName))
                        {
                            appUpdateItem.IsOperating = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    AppUpdateLock.Exit();
                }

                await CancelUpdateAppAsync(packageFamilyName);
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 打开微软商店并更新微软商店应用
        /// </summary>
        private void OnOpenStoreClicked(object sender, RoutedEventArgs args)
        {
            OpenStore();
        }

        /// <summary>
        /// 设置当前系统的预览体验计划
        /// </summary>
        private void OnInsiderProgramClicked(object sender, RoutedEventArgs args)
        {
            OpenInsiderProgram();
        }

        /// <summary>
        /// 检查商店应用更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            if (!IsCheckingUpdate)
            {
                IsCheckingUpdate = true;
                List<AppUpdateModel> appUpdateList = await GetAppUpdateListAsync(AppUpdateList);

                if (appUpdateList is not null)
                {
                    // 只添加未有的项
                    AppUpdateLock.Enter();

                    try
                    {
                        AppUpdateList.AddRange(appUpdateList);
                        AppUpdateList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                        AppUpdateCollection.Clear();
                        foreach (AppUpdateModel appUpdateItem in AppUpdateList)
                        {
                            AppUpdateCollection.Add(appUpdateItem);
                        }

                        if (AppUpdateList.Count > 0)
                        {
                            AppUpdateResultKind = AppUpdateResultKind.Successfully;
                            AppUpdateFailedContent = string.Empty;
                        }
                        else
                        {
                            AppUpdateResultKind = AppUpdateResultKind.Failed;
                            AppUpdateFailedContent = AppUpdateEmptyString;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }

                    AppUpdateLock.Exit();

                    IsCheckingUpdate = false;
                }
            }
        }

        /// <summary>
        /// 更新所有应用
        /// </summary>
        private async void OnUpdateAllClicked(object sender, RoutedEventArgs args)
        {
            AppUpdateLock.Enter();

            foreach (AppUpdateModel appUpdateItem in AppUpdateList)
            {
                if (!appUpdateItem.IsUpdating)
                {
                    appUpdateItem.IsOperating = true;
                }
            }

            foreach (AppUpdateModel appUpdateItem in AppUpdateList)
            {
                if (!appUpdateItem.IsUpdating && appUpdateItem.IsOperating)
                {
                    AppInstallItem appInstallItem = await UpdatAppAsync(appUpdateItem.PackageFamilyName);

                    // 安装更新添加成功
                    if (appInstallItem is not null)
                    {
                        appUpdateItem.IsOperating = false;
                        appUpdateItem.AppInstallState = AppInstallState.Pending;
                        appUpdateItem.IsUpdating = true;
                        appUpdateItem.InstallInformation = PendingString;
                        appUpdateItem.PercentComplete = 0;
                    }
                    // 安装更新添加失败
                    else
                    {
                        appUpdateItem.IsOperating = false;
                        appUpdateItem.AppInstallState = AppInstallState.Error;
                        appUpdateItem.IsUpdating = false;
                        appUpdateItem.InstallInformation = ErrorString;
                    }
                }
            }
        }

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            DismountAppUpdateEvent();
        }

        /// <summary>
        /// 当前应用的安装状态发生更改时的事件
        /// </summary>
        private void OnAppInstallItemStatusChanged(AppInstallManager sender, AppInstallManagerItemEventArgs args)
        {
            AppInstallStatus appInstallStatus = args.Item.GetCurrentStatus();

            if (appInstallStatus is not null)
            {
                string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus);
                string installSubInformation = string.Format(InstallingSubInformationString, VolumeSizeHelper.ConvertVolumeSizeToString(appInstallStatus.DownloadSizeInBytes), VolumeSizeHelper.ConvertVolumeSizeToString(appInstallStatus.BytesDownloaded));

                DispatcherQueue.TryEnqueue(() =>
                {
                    AppUpdateLock.Enter();

                    try
                    {
                        foreach (AppUpdateModel appUpdateItem in AppUpdateList)
                        {
                            if (string.Equals(appUpdateItem.PackageFamilyName, args.Item.PackageFamilyName, StringComparison.OrdinalIgnoreCase))
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

                                // 更新已取消
                                if (appInstallStatus.InstallState is AppInstallState.Canceled && appUpdateItem.IsOperating)
                                {
                                    appUpdateItem.IsOperating = false;
                                }
                                // 更新已完成
                                else if (appUpdateItem.AppInstallState is AppInstallState.Completed)
                                {
                                    ShowAppUpdateNotification(appUpdateItem.DisplayName);

                                    AppUpdateList.Remove(appUpdateItem);
                                    AppUpdateCollection.Remove(appUpdateItem);

                                    if (AppUpdateList.Count > 0)
                                    {
                                        AppUpdateResultKind = AppUpdateResultKind.Successfully;
                                        AppUpdateFailedContent = string.Empty;
                                    }
                                    else
                                    {
                                        AppUpdateResultKind = AppUpdateResultKind.Failed;
                                        AppUpdateFailedContent = AppUpdateEmptyString;
                                    }
                                }
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }

                    AppUpdateLock.Exit();
                });
            }
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 挂载应用更新事件
        /// </summary>
        private void MountAppUpdateEvent()
        {
            appInstallManager.ItemStatusChanged += OnAppInstallItemStatusChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
        }

        /// <summary>
        /// 卸载应用更新事件
        /// </summary>
        private void DismountAppUpdateEvent()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                appInstallManager.ItemStatusChanged -= OnAppInstallItemStatusChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppUpdatePage), nameof(DismountAppUpdateEvent), 1, e);
            }
        }

        /// <summary>
        /// 获取应用安装的描述信息
        /// </summary>
        private string GetInstallInformation(AppInstallState appInstallState, AppInstallStatus appInstallStatus)
        {
            return appInstallState switch
            {
                AppInstallState.AcquiringLicense => AcquiringLicenseString,
                AppInstallState.Canceled => CanceledString,
                AppInstallState.Completed => CompletedString,
                AppInstallState.Downloading => string.Format(DownloadingString, appInstallStatus.PercentComplete),
                AppInstallState.Error => string.Format(ErrorString, string.Format("0x{0:X8}", appInstallStatus.ErrorCode.HResult)),
                AppInstallState.Installing => string.Format(InstallingString, appInstallStatus.PercentComplete),
                AppInstallState.Paused => PausedString,
                AppInstallState.PausedLowBattery => PausedString,
                AppInstallState.PausedWiFiRecommended => PausedString,
                AppInstallState.PausedWiFiRequired => PausedString,
                AppInstallState.Pending => PendingString,
                AppInstallState.ReadyToDownload => ReadyToDownloadString,
                AppInstallState.RestoringData => RestoringDataString,
                AppInstallState.Starting => StartingString,
                _ => string.Empty,
            };
        }

        /// <summary>
        /// 更新应用
        /// </summary>
        private async Task<AppInstallItem> UpdatAppAsync(string packageFamilyName)
        {
            if (!string.IsNullOrEmpty(packageFamilyName))
            {
                return await Task.Run(async () =>
                {
                    try
                    {
                        return await appInstallManager.UpdateAppByPackageFamilyNameAsync(packageFamilyName);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppUpdatePage), nameof(UpdatAppAsync), 1, e);
                        return null;
                    }
                });
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 取消更新应用
        /// </summary>
        private async Task CancelUpdateAppAsync(string packageFamilyName)
        {
            if (!string.IsNullOrEmpty(packageFamilyName))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        foreach (AppInstallItem appInstallItem in appInstallManager.AppInstallItems)
                        {
                            if (string.Equals(packageFamilyName, appInstallItem.PackageFamilyName))
                            {
                                appInstallItem.Cancel();
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        private void OpenStore()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-windows-store://downloadsandupdates"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开预览体验计划
        /// </summary>
        private void OpenInsiderProgram()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("ms-settings:windowsinsider"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 获取应用更新信息
        /// </summary>
        private async Task<List<AppUpdateModel>> GetAppUpdateListAsync(List<AppUpdateModel> appUpdateList)
        {
            if (appUpdateList is not null)
            {
                return await Task.Run(async () =>
                {
                    List<AppUpdateModel> appUpdateList = [];

                    try
                    {
                        AppUpdateOptions updateOptions = new()
                        {
                            AutomaticallyDownloadAndInstallUpdateIfFound = false,
                            AllowForcedAppRestart = true,
                        };

                        IReadOnlyList<AppInstallItem> upgradableAppsList = await appInstallManager.SearchForAllUpdatesAsync(string.Empty, string.Empty, updateOptions);

                        foreach (AppInstallItem upgradableAppItem in upgradableAppsList)
                        {
                            // 判断是否已经添加到应用更新队列中，没有则添加
                            bool isExisted = false;
                            bool isUpdating = false;
                            AppUpdateLock.Enter();

                            try
                            {
                                foreach (AppUpdateModel appUpdateItem in appUpdateList)
                                {
                                    if (string.Equals(appUpdateItem.PackageFamilyName, upgradableAppItem.PackageFamilyName))
                                    {
                                        isExisted = true;
                                        isUpdating = appUpdateItem.IsUpdating;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }

                            AppUpdateLock.Exit();

                            if (isExisted)
                            {
                                // 已经检测到的应用暂未进行更新，取消自动安装当前更新
                                if (CancelAutoUpdateService.CancelAutoUpdate && !isUpdating)
                                {
                                    upgradableAppItem.Cancel();
                                }
                            }
                            else
                            {
                                if (CancelAutoUpdateService.CancelAutoUpdate)
                                {
                                    upgradableAppItem.Cancel();
                                }

                                foreach (Package packageItem in packageManager.FindPackagesForUser(string.Empty))
                                {
                                    if (string.Equals(packageItem.Id.FamilyName, upgradableAppItem.PackageFamilyName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        AppInstallStatus appInstallStatus = upgradableAppItem.GetCurrentStatus();
                                        string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus);
                                        string installSubInformation = string.Format(InstallingSubInformationString, VolumeSizeHelper.ConvertVolumeSizeToString(appInstallStatus.DownloadSizeInBytes), VolumeSizeHelper.ConvertVolumeSizeToString(appInstallStatus.BytesDownloaded));

                                        appUpdateList.Add(new()
                                        {
                                            LogoImage = packageItem.Logo,
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
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppUpdatePage), nameof(GetAppUpdateListAsync), 1, e);
                    }

                    return appUpdateList;
                });
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 显示应用更新通知
        /// </summary>
        private void ShowAppUpdateNotification(string displayName)
        {
            if (!string.IsNullOrEmpty(displayName))
            {
                Task.Run(() =>
                {
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(AppUpdateSuccessfullyString, displayName));
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                });
            }
        }

        /// <summary>
        /// 获取加载应用更新是否成功
        /// </summary>
        private Visibility GetAppUpdateSuccessfullyVisibility(AppUpdateResultKind appUpdateResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? appUpdateResultKind is AppUpdateResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : appUpdateResultKind is AppUpdateResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索应用更新是否成功
        /// </summary>
        private Visibility CheckAppUpdateResultKindVisibility(AppUpdateResultKind appUpdateResultKind, AppUpdateResultKind comparedAppUpdateResultKind)
        {
            return Equals(appUpdateResultKind, comparedAppUpdateResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取能否进行更新
        /// </summary>
        private bool GetCanUpdate(bool isCheckingUpdate, int count)
        {
            return !isCheckingUpdate && count > 0;
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}

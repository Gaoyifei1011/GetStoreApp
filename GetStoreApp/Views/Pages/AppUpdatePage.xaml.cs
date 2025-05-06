using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.AppUpdate;
using GetStoreApp.Services.Root;
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
    public sealed partial class AppUpdatePage : Page, INotifyPropertyChanged
    {
        private bool _isInitialized;

        private readonly string AcquiringLicense = ResourceService.GetLocalized("AppUpdate/AcquiringLicense");
        private readonly string Canceled = ResourceService.GetLocalized("AppUpdate/Canceled");
        private readonly string Completed = ResourceService.GetLocalized("AppUpdate/Completed");
        private readonly string Downloading = ResourceService.GetLocalized("AppUpdate/Downloading");
        private readonly string Error = ResourceService.GetLocalized("AppUpdate/Error");
        private readonly string Installing = ResourceService.GetLocalized("AppUpdate/Installing");
        private readonly string InstallingSubInformation = ResourceService.GetLocalized("AppUpdate/InstallingSubInformation");
        private readonly string Paused = ResourceService.GetLocalized("AppUpdate/Paused");
        private readonly string Pending = ResourceService.GetLocalized("AppUpdate/Pending");
        private readonly string ReadyToDownload = ResourceService.GetLocalized("AppUpdate/ReadyToDownload");
        private readonly string RestoringData = ResourceService.GetLocalized("AppUpdate/RestoringData");
        private readonly string Starting = ResourceService.GetLocalized("AppUpdate/Starting");
        private readonly string AppUpdateCountInfo = ResourceService.GetLocalized("AppUpdate/AppUpdateCountInfo");
        private readonly string AppUpdateEmpty = ResourceService.GetLocalized("AppUpdate/AppUpdateEmpty");
        private readonly string AppUpdateSuccessfully = ResourceService.GetLocalized("Notification/AppUpdateSuccessfully");

        private readonly Lock AppUpdateLock = new();

        private AppInstallManager appInstallManager;
        private PackageManager packageManager;

        private AppUpdateResultKind _appUpdateResultKind = AppUpdateResultKind.NotCheckUpdate;

        public AppUpdateResultKind AppUpdateResultKind
        {
            get { return _appUpdateResultKind; }

            set
            {
                if (!Equals(_appUpdateResultKind, value))
                {
                    _appUpdateResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppUpdateResultKind)));
                }
            }
        }

        private string _appUpdateFailedContent;

        public string AppUpdateFailedContent
        {
            get { return _appUpdateFailedContent; }

            set
            {
                if (!Equals(_appUpdateResultKind, value))
                {
                    _appUpdateFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppUpdateFailedContent)));
                }
            }
        }

        private List<AppUpdateModel> AppUpdateList { get; } = [];

        private ObservableCollection<AppUpdateModel> AppUpdateCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppUpdatePage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!_isInitialized)
            {
                _isInitialized = true;
                appInstallManager = new();
                packageManager = new();

                appInstallManager.ItemStatusChanged += OnAppInstallItemStatusChanged;
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

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
                        if (Equals(appUpdateItem.PackageFamilyName, packageFamilyName))
                        {
                            appUpdateItem.IsOperating = true;
                            AppInstallItem appInstallItem = await Task.Run(async () =>
                            {
                                try
                                {
                                    return await appInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, string.Format("Update app failed: package family name {0}", appUpdateItem.PackageFamilyName), e);
                                    return null;
                                }
                            });

                            // 安装更新添加成功
                            if (appInstallItem is not null)
                            {
                                appUpdateItem.IsOperating = false;
                                appUpdateItem.AppInstallState = AppInstallState.Pending;
                                appUpdateItem.IsUpdating = true;
                                appUpdateItem.InstallInformation = Pending;
                                appUpdateItem.PercentComplete = 0;
                            }
                            // 安装更新添加失败
                            else
                            {
                                appUpdateItem.IsOperating = false;
                                appUpdateItem.AppInstallState = AppInstallState.Error;
                                appUpdateItem.IsUpdating = false;
                                appUpdateItem.InstallInformation = Error;
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
                        if (Equals(appUpdateItem.PackageFamilyName, packageFamilyName))
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

                await Task.Run(() =>
                {
                    try
                    {
                        foreach (AppInstallItem appInstallItem in appInstallManager.AppInstallItems)
                        {
                            if (Equals(packageFamilyName, appInstallItem.PackageFamilyName))
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

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：商店应用更新页面——挂载的事件

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
            AppUpdateResultKind = AppUpdateResultKind.Querying;
            AppUpdateCollection.Clear();

            List<AppUpdateModel> appUpdateList = await Task.Run(async () =>
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
                            foreach (AppUpdateModel appUpdateItem in AppUpdateList)
                            {
                                if (Equals(appUpdateItem.PackageFamilyName, upgradableAppItem.PackageFamilyName))
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
                            if (!isUpdating)
                            {
                                upgradableAppItem.Cancel();
                            }
                        }
                        else
                        {
                            upgradableAppItem.Cancel();

                            foreach (Package packageItem in packageManager.FindPackagesForUser(string.Empty))
                            {
                                if (string.Equals(packageItem.Id.FamilyName, upgradableAppItem.PackageFamilyName, StringComparison.OrdinalIgnoreCase))
                                {
                                    AppInstallStatus appInstallStatus = upgradableAppItem.GetCurrentStatus();
                                    string installInformation = GetInstallInformation(appInstallStatus.InstallState, appInstallStatus);
                                    string installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));

                                    appUpdateList.Add(new AppUpdateModel()
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
                    LogService.WriteLog(LoggingLevel.Error, "Check store update status failed", e);
                }

                return appUpdateList;
            });

            // 只添加未有的项
            AppUpdateLock.Enter();

            try
            {
                foreach (AppUpdateModel appUpdateItem in appUpdateList)
                {
                    AppUpdateList.Add(appUpdateItem);
                }

                AppUpdateList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));

                foreach (AppUpdateModel appUpdateItem in AppUpdateList)
                {
                    AppUpdateCollection.Add(appUpdateItem);
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }

            AppUpdateLock.Exit();

            if (AppUpdateList.Count > 0)
            {
                AppUpdateResultKind = AppUpdateResultKind.Successfully;
                AppUpdateFailedContent = string.Empty;
            }
            else
            {
                AppUpdateResultKind = AppUpdateResultKind.Failed;
                AppUpdateFailedContent = AppUpdateEmpty;
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
                    AppInstallItem appInstallItem = await Task.Run(async () =>
                    {
                        try
                        {
                            return await appInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, string.Format("Update app failed: package family name {0}", appUpdateItem.PackageFamilyName), e);
                            return null;
                        }
                    });

                    // 安装更新添加成功
                    if (appInstallItem is not null)
                    {
                        appUpdateItem.IsOperating = false;
                        appUpdateItem.AppInstallState = AppInstallState.Pending;
                        appUpdateItem.IsUpdating = true;
                        appUpdateItem.InstallInformation = Pending;
                        appUpdateItem.PercentComplete = 0;
                    }
                    // 安装更新添加失败
                    else
                    {
                        appUpdateItem.IsOperating = false;
                        appUpdateItem.AppInstallState = AppInstallState.Error;
                        appUpdateItem.IsUpdating = false;
                        appUpdateItem.InstallInformation = Error;
                    }
                }
            }
        }

        #endregion 第三部分：商店应用更新页面——挂载的事件

        #region 第四部分：自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit(object sender, EventArgs args)
        {
            GlobalNotificationService.ApplicationExit -= OnApplicationExit;
            appInstallManager.ItemStatusChanged -= OnAppInstallItemStatusChanged;
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
                string installSubInformation = string.Format(InstallingSubInformation, FileSizeHelper.ConvertFileSizeToString(appInstallStatus.DownloadSizeInBytes), FileSizeHelper.ConvertFileSizeToString(appInstallStatus.BytesDownloaded));

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
                                    Task.Run(() =>
                                    {
                                        AppNotificationBuilder appNotificationBuilder = new();
                                        appNotificationBuilder.AddArgument("action", "OpenApp");
                                        appNotificationBuilder.AddText(string.Format(AppUpdateSuccessfully, appUpdateItem.DisplayName));
                                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                    });

                                    AppUpdateList.Remove(appUpdateItem);
                                    AppUpdateCollection.Remove(appUpdateItem);

                                    AppUpdateResultKind = AppUpdateResultKind.Failed;
                                    AppUpdateFailedContent = AppUpdateEmpty;
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

        #endregion 第四部分：自定义事件

        /// <summary>
        /// 获取应用安装的描述信息
        /// </summary>
        private string GetInstallInformation(AppInstallState appInstallState, AppInstallStatus appInstallStatus)
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

        /// <summary>
        /// 获取加载应用更新是否成功
        /// </summary>
        private Visibility GetAppUpdateSuccessfullyState(AppUpdateResultKind appUpdateResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? appUpdateResultKind is AppUpdateResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : appUpdateResultKind is AppUpdateResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索应用更新是否成功
        /// </summary>
        private Visibility CheckAppUpdateState(AppUpdateResultKind appUpdateResultKind, AppUpdateResultKind comparedAppUpdateResultKind)
        {
            return Equals(appUpdateResultKind, comparedAppUpdateResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在检查更新中
        /// </summary>

        private bool GetIsCheckingUpdate(AppUpdateResultKind appUpdateResultKind)
        {
            return !Equals(appUpdateResultKind, AppUpdateResultKind.Querying);
        }
    }
}

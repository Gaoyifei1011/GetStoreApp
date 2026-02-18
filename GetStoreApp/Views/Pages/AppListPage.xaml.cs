using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Management.Deployment;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.System;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理列表页
    /// </summary>
    public sealed partial class AppListPage : Page, INotifyPropertyChanged
    {
        private readonly string SignatureString = "AppList/Signature{0}";
        private readonly string NoString = ResourceService.GetLocalized("AppList/No");
        private readonly string OpenSettingsString = ResourceService.GetLocalized("AppList/OpenSettings");
        private readonly string PackageCountInfoString = ResourceService.GetLocalized("AppList/PackageCountInfo");
        private readonly string PackageEmptyDescriptionString = ResourceService.GetLocalized("AppList/PackageEmptyDescription");
        private readonly string PackageEmptyWithConditionDescriptionString = ResourceService.GetLocalized("AppList/PackageEmptyWithConditionDescription");
        private readonly string MoveFailed1String = ResourceService.GetLocalized("AppList/MoveFailed1");
        private readonly string MoveFailed2String = ResourceService.GetLocalized("AppList/MoveFailed2");
        private readonly string MoveFailed3String = ResourceService.GetLocalized("AppList/MoveFailed3");
        private readonly string MoveFailed4String = ResourceService.GetLocalized("AppList/MoveFailed4");
        private readonly string MoveFailed5String = ResourceService.GetLocalized("AppList/MoveFailed5");
        private readonly string MoveSuccessfullyString = ResourceService.GetLocalized("AppList/MoveSuccessfully");
        private readonly string NotAvailableString = ResourceService.GetLocalized("AppList/NotAvailable");
        private readonly string RepairFailed1String = ResourceService.GetLocalized("AppList/RepairFailed1");
        private readonly string RepairFailed2String = ResourceService.GetLocalized("AppList/RepairFailed2");
        private readonly string RepairFailed3String = ResourceService.GetLocalized("AppList/RepairFailed3");
        private readonly string RepairFailed4String = ResourceService.GetLocalized("AppList/RepairFailed4");
        private readonly string RepairFailed5String = ResourceService.GetLocalized("AppList/RepairFailed5");
        private readonly string RepairSuccessfullyString = ResourceService.GetLocalized("AppList/RepairSuccessfully");
        private readonly string ResetFailed1String = ResourceService.GetLocalized("AppList/ResetFailed1");
        private readonly string ResetFailed2String = ResourceService.GetLocalized("AppList/ResetFailed2");
        private readonly string ResetFailed3String = ResourceService.GetLocalized("AppList/ResetFailed3");
        private readonly string ResetFailed4String = ResourceService.GetLocalized("AppList/ResetFailed4");
        private readonly string ResetFailed5String = ResourceService.GetLocalized("AppList/ResetFailed5");
        private readonly string ResetSuccessfullyString = ResourceService.GetLocalized("AppList/ResetSuccessfully");
        private readonly string UninstallFailed1String = ResourceService.GetLocalized("AppList/UninstallFailed1");
        private readonly string UninstallFailed2String = ResourceService.GetLocalized("AppList/UninstallFailed2");
        private readonly string UninstallFailed3String = ResourceService.GetLocalized("AppList/UninstallFailed3");
        private readonly string UninstallFailed4String = ResourceService.GetLocalized("AppList/UninstallFailed4");
        private readonly string UninstallFailed5String = ResourceService.GetLocalized("AppList/UninstallFailed5");
        private readonly string UninstallSuccessfullyString = ResourceService.GetLocalized("AppList/UninstallSuccessfully");
        private readonly string YesString = ResourceService.GetLocalized("AppList/Yes");

        private bool isInitialized;
        private bool needToRefreshData;
        private PackageManager packageManager;
        private PackageDeploymentManager packageDeploymentManager;

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                if (!Equals(_isIncrease, value))
                {
                    _isIncrease = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncrease)));
                }
            }
        }

        private AppManagerResultKind _appManagerResultKind;

        public AppManagerResultKind AppManagerResultKind
        {
            get { return _appManagerResultKind; }

            set
            {
                if (!Equals(_appManagerResultKind, value))
                {
                    _appManagerResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppManagerResultKind)));
                }
            }
        }

        private bool _isAppFramework;

        public bool IsAppFramework
        {
            get { return _isAppFramework; }

            set
            {
                if (!Equals(_isAppFramework, value))
                {
                    _isAppFramework = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAppFramework)));
                }
            }
        }

        private AppSortRuleKind _selectedAppSortRuleKind = AppSortRuleKind.DisplayName;

        public AppSortRuleKind SelectedAppSortRuleKind
        {
            get { return _selectedAppSortRuleKind; }

            set
            {
                if (!Equals(_selectedAppSortRuleKind, value))
                {
                    _selectedAppSortRuleKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAppSortRuleKind)));
                }
            }
        }

        private bool _isStoreSignatureSelected = true;

        public bool IsStoreSignatureSelected
        {
            get { return _isStoreSignatureSelected; }

            set
            {
                if (!Equals(_isStoreSignatureSelected, value))
                {
                    _isStoreSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStoreSignatureSelected)));
                }
            }
        }

        private bool _isSystemSignatureSelected;

        public bool IsSystemSignatureSelected
        {
            get { return _isSystemSignatureSelected; }

            set
            {
                if (!Equals(_isSystemSignatureSelected, value))
                {
                    _isSystemSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemSignatureSelected)));
                }
            }
        }

        private bool _isEnterpriseSignatureSelected;

        public bool IsEnterpriseSignatureSelected
        {
            get { return _isEnterpriseSignatureSelected; }

            set
            {
                if (!Equals(_isEnterpriseSignatureSelected, value))
                {
                    _isEnterpriseSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnterpriseSignatureSelected)));
                }
            }
        }

        private bool _isDeveloperSignatureSelected;

        public bool IsDeveloperSignatureSelected
        {
            get { return _isDeveloperSignatureSelected; }

            set
            {
                if (!Equals(_isDeveloperSignatureSelected, value))
                {
                    _isDeveloperSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDeveloperSignatureSelected)));
                }
            }
        }

        private bool _isNoneSignatureSelected;

        public bool IsNoneSignatureSelected
        {
            get { return _isNoneSignatureSelected; }

            set
            {
                if (!Equals(_isNoneSignatureSelected, value))
                {
                    _isNoneSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoneSignatureSelected)));
                }
            }
        }

        private string _appManagerFailedContent;

        public string AppManagerFailedContent
        {
            get { return _appManagerFailedContent; }

            set
            {
                if (!string.Equals(_appManagerFailedContent, value))
                {
                    _appManagerFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppManagerFailedContent)));
                }
            }
        }

        private List<PackageModel> AppManagerList { get; } = [];

        private ObservableCollection<PackageModel> AppManagerCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppListPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;

                packageManager = new();
                packageDeploymentManager = PackageDeploymentManager.GetDefault();
                await GetAppListAsync();
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 打开应用
        /// </summary>
        private void OnOpenAppExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter.As<Package>() is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await package.GetAppListEntries()[0].LaunchAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnOpenAppExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用缓存目录
        /// </summary>
        private void OnOpenCacheFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter.As<Package>() is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (ApplicationData.GetForPackageFamily(package.Id.FamilyName) is ApplicationData applicationData)
                        {
                            await Launcher.LaunchFolderAsync(applicationData.LocalFolder);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnOpenCacheFolderExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用安装目录
        /// </summary>
        private void OnOpenInstalledFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter.As<Package>() is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnOpenInstalledFolderExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用清单文件
        /// </summary>
        private void OnOpenManifestExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter.As<Package>() is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (await global::Windows.Storage.StorageFile.GetFileFromPathAsync(Path.Combine(package.InstalledPath, "AppxManifest.xml")) is global::Windows.Storage.StorageFile file)
                        {
                            await Launcher.LaunchFileAsync(file);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnOpenManifestExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        private void OnOpenStoreExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter.As<Package>() is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={package.Id.FamilyName}"));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnOpenStoreExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 移动应用
        /// </summary>
        private async void OnMoveExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageModel package)
            {
                try
                {
                    PackageVolumeInfoDialog packageVolumeInfoDialog = new(package);
                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(packageVolumeInfoDialog);

                    if (contentDialogResult is ContentDialogResult.Primary && packageVolumeInfoDialog.SelectedPackageVolume is not null)
                    {
                        package.PackageOperationProgress = 0;
                        package.IsOperating = true;

                        (bool result, DeploymentResult deploymentResult, Exception exception) = await Task.Run(async () =>
                        {
                            try
                            {
                                // 移动目标应用，并获取移动进度
                                global::Windows.Management.Deployment.PackageVolume winRTPackageVolume = null;
                                foreach (global::Windows.Management.Deployment.PackageVolume winRTPackageVolumeItem in packageManager.FindPackageVolumes())
                                {
                                    if (string.Equals(winRTPackageVolumeItem.PackageStorePath, packageVolumeInfoDialog.SelectedPackageVolume.PackageVolume.PackageStorePath))
                                    {
                                        winRTPackageVolume = winRTPackageVolumeItem;
                                        break;
                                    }
                                }
                                IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> movePackageWithProgress = packageManager.MovePackageToVolumeAsync(package.Package.Id.FullName, DeploymentOptions.None, winRTPackageVolume);

                                // 更新移动进度
                                movePackageWithProgress.Progress = (result, progress) => OnPackageMoveProgress(result, progress, package);
                                return ValueTuple.Create<bool, DeploymentResult, Exception>(true, await movePackageWithProgress, null);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnMoveExecuteRequested), 1, e);
                                return ValueTuple.Create<bool, DeploymentResult, Exception>(false, null, e);
                            }
                        });

                        // 移动成功
                        if (result && deploymentResult is not null)
                        {
                            if (deploymentResult.ExtendedErrorCode is null)
                            {
                                // 显示 UWP 应用移动成功通知
                                await Task.Run(() =>
                                {
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(MoveSuccessfullyString, package.Package.DisplayName));
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });

                                package.IsOperating = false;
                            }
                            else
                            {
                                // 显示 UWP 应用移动失败通知
                                await Task.Run(() =>
                                {
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(MoveFailed1String, package.Package.DisplayName));
                                    appNotificationBuilder.AddText(MoveFailed2String);
                                    appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                                    {
                                        MoveFailed3String,
                                        string.Format(MoveFailed4String, deploymentResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(deploymentResult.ExtendedErrorCode.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                        string.Format(MoveFailed5String, deploymentResult.ErrorText)
                                    }));
                                    AppNotificationButton openSettingsButton = new(OpenSettingsString);
                                    openSettingsButton.Arguments.Add("action", "OpenSettings");
                                    appNotificationBuilder.AddButton(openSettingsButton);
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnMoveExecuteRequested), 2, deploymentResult.ExtendedErrorCode is not null ? deploymentResult.ExtendedErrorCode : new Exception());
                                });

                                package.IsOperating = false;
                            }
                        }
                        else
                        {
                            // 显示 UWP 应用移动失败通知
                            await Task.Run(() =>
                            {
                                AppNotificationBuilder appNotificationBuilder = new();
                                appNotificationBuilder.AddArgument("action", "OpenApp");
                                appNotificationBuilder.AddText(string.Format(MoveFailed1String, package.Package.DisplayName));
                                appNotificationBuilder.AddText(MoveFailed2String);
                                appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                                {
                                    MoveFailed3String,
                                    string.Format(MoveFailed4String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                    string.Format(MoveFailed5String, exception.Message)
                                }));
                                AppNotificationButton openSettingsButton = new(OpenSettingsString);
                                openSettingsButton.Arguments.Add("action", "OpenSettings");
                                appNotificationBuilder.AddButton(openSettingsButton);
                                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnMoveExecuteRequested), 3, exception is not null ? exception : new Exception());
                            });

                            package.IsOperating = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    package.IsOperating = false;
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnMoveExecuteRequested), 2, e);
                }
            }
        }

        /// <summary>
        /// 修复应用
        /// </summary>
        private async void OnRepairExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageModel package)
            {
                package.PackageOperationProgress = 0;
                package.IsOperating = true;

                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                {
                    try
                    {
                        // 修复目标应用，并获取修复进度
                        IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> repairPackageWithProgress = packageDeploymentManager.RepairPackageAsync(package.Package.Id.FullName);

                        // 更新修复进度
                        repairPackageWithProgress.Progress = (result, progress) => OnPackageRepairProgress(result, progress, package);
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, await repairPackageWithProgress, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnRepairExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                    }
                });

                // 修复成功
                if (result && packageDeploymentResult is not null)
                {
                    if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                    {
                        // 显示 UWP 应用修复成功通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RepairSuccessfullyString, package.Package.DisplayName));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });

                        package.IsOperating = false;
                    }
                    // 修复失败
                    else
                    {
                        // 显示 UWP 应用修复失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RepairFailed1String, package.Package.DisplayName));
                            appNotificationBuilder.AddText(RepairFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                RepairFailed3String,
                                string.Format(RepairFailed4String, packageDeploymentResult.ExtendedError is not null ? "0x" + Convert.ToString(packageDeploymentResult.ExtendedError.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(RepairFailed5String, packageDeploymentResult.ErrorText)
                            }));
                            AppNotificationButton openSettingsButton = new(OpenSettingsString);
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnRepairExecuteRequested), 2, packageDeploymentResult.ExtendedError is not null ? packageDeploymentResult.ExtendedError : new Exception());
                        });

                        package.IsOperating = false;
                    }
                }
                else
                {
                    // 显示 UWP 应用修复失败通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(RepairFailed1String, package.Package.DisplayName));
                        appNotificationBuilder.AddText(RepairFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            RepairFailed3String,
                            string.Format(RepairFailed4String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                            string.Format(RepairFailed5String, exception.Message)
                        }));
                        AppNotificationButton openSettingsButton = new(OpenSettingsString);
                        openSettingsButton.Arguments.Add("action", "OpenSettings");
                        appNotificationBuilder.AddButton(openSettingsButton);
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnRepairExecuteRequested), 3, exception is not null ? exception : new Exception());
                    });

                    package.IsOperating = false;
                }
            }
        }

        /// <summary>
        /// 重置应用
        /// </summary>
        private async void OnResetExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageModel package)
            {
                package.PackageOperationProgress = 0;
                package.IsOperating = true;

                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                {
                    try
                    {
                        // 重置目标应用，并获取重置进度
                        IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> resetPackageWithProgress = packageDeploymentManager.ResetPackageAsync(package.Package.Id.FullName);

                        // 更新重置进度
                        resetPackageWithProgress.Progress = (result, progress) => OnPackageResetProgress(result, progress, package);
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, await resetPackageWithProgress, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnResetExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                    }
                });

                // 重置成功
                if (result && packageDeploymentResult is not null)
                {
                    if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                    {
                        // 显示 UWP 应用重置成功通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResetSuccessfullyString, package.Package.DisplayName));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });

                        package.IsOperating = false;
                    }
                    // 重置失败
                    else
                    {
                        // 显示 UWP 应用重置失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResetFailed1String, package.Package.DisplayName));
                            appNotificationBuilder.AddText(ResetFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                ResetFailed3String,
                                string.Format(ResetFailed4String, packageDeploymentResult.ExtendedError is not null ? "0x" + Convert.ToString(packageDeploymentResult.ExtendedError.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(ResetFailed5String, packageDeploymentResult.ErrorText)
                            }));
                            AppNotificationButton openSettingsButton = new(OpenSettingsString);
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnResetExecuteRequested), 2, packageDeploymentResult.ExtendedError is not null ? packageDeploymentResult.ExtendedError : new Exception());
                        });

                        package.IsOperating = false;
                    }
                }
                else
                {
                    // 显示 UWP 应用重置失败通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(ResetFailed1String, package.Package.DisplayName));
                        appNotificationBuilder.AddText(ResetFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            ResetFailed3String,
                            string.Format(ResetFailed4String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                            string.Format(ResetFailed5String, exception.Message)
                        }));
                        AppNotificationButton openSettingsButton = new(OpenSettingsString);
                        openSettingsButton.Arguments.Add("action", "OpenSettings");
                        appNotificationBuilder.AddButton(openSettingsButton);
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnResetExecuteRequested), 3, exception is not null ? exception : new Exception());
                    });

                    package.IsOperating = false;
                }
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        private async void OnUninstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageModel package)
            {
                package.PackageOperationProgress = 0;
                package.IsOperating = true;

                try
                {
                    (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                    {
                        try
                        {
                            // 卸载目标应用，并获取卸载进度
                            IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> uninstallPackageWithProgress = packageDeploymentManager.RemovePackageByFullNameAsync(package.Package.Id.FullName, new());

                            // 更新卸载进度
                            uninstallPackageWithProgress.Progress = (result, progress) => OnPackageUninstallProgress(result, progress, package);
                            return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, await uninstallPackageWithProgress, null);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnUninstallExecuteRequested), 1, e);
                            return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                        }
                    });

                    // 卸载成功
                    if (result && packageDeploymentResult is not null)
                    {
                        if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                        {
                            // 显示 UWP 应用卸载成功通知
                            await Task.Run(() =>
                            {
                                AppNotificationBuilder appNotificationBuilder = new();
                                appNotificationBuilder.AddArgument("action", "OpenApp");
                                appNotificationBuilder.AddText(string.Format(UninstallSuccessfullyString, package.Package.DisplayName));
                                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            });

                            package.IsOperating = false;
                            AppManagerList.Remove(package);
                            AppManagerCollection.Remove(package);

                            AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                            if (AppManagerList.Count is 0)
                            {
                                AppManagerFailedContent = PackageEmptyDescriptionString;
                            }
                            else if (AppManagerCollection.Count is 0)
                            {
                                AppManagerFailedContent = PackageEmptyWithConditionDescriptionString;
                            }
                            else
                            {
                                AppManagerFailedContent = string.Empty;
                            }
                        }
                        // 卸载失败
                        else
                        {
                            // 显示 UWP 应用卸载失败通知
                            await Task.Run(() =>
                            {
                                AppNotificationBuilder appNotificationBuilder = new();
                                appNotificationBuilder.AddArgument("action", "OpenApp");
                                appNotificationBuilder.AddText(string.Format(UninstallFailed1String, package.Package.DisplayName));
                                appNotificationBuilder.AddText(UninstallFailed2String);
                                appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                                {
                                    UninstallFailed3String,
                                    string.Format(UninstallFailed4String, packageDeploymentResult.ExtendedError is not null ? "0x" + Convert.ToString(packageDeploymentResult.ExtendedError.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                    string.Format(UninstallFailed5String, packageDeploymentResult.ErrorText)
                                }));
                                AppNotificationButton openSettingsButton = new(OpenSettingsString);
                                openSettingsButton.Arguments.Add("action", "OpenSettings");
                                appNotificationBuilder.AddButton(openSettingsButton);
                                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnUninstallExecuteRequested), 2, packageDeploymentResult.ExtendedError is not null ? packageDeploymentResult.ExtendedError : new Exception());
                            });

                            package.IsOperating = false;
                        }
                    }
                    else
                    {
                        // 显示 UWP 应用卸载失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(UninstallFailed1String, package.Package.DisplayName));
                            appNotificationBuilder.AddText(UninstallFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                UninstallFailed3String,
                                string.Format(UninstallFailed4String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(UninstallFailed5String, exception.Message)
                            }));
                            AppNotificationButton openSettingsButton = new(OpenSettingsString);
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnUninstallExecuteRequested), 3, exception is not null ? exception : new Exception());
                        });

                        package.IsOperating = false;
                    }
                }
                catch (Exception e)
                {
                    package.IsOperating = false;
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnUninstallExecuteRequested), 2, e);
                }
            }
        }

        /// <summary>
        /// 查看应用信息
        /// </summary>
        private async void OnViewInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageModel package)
            {
                AppInformation appInformation = new();

                await Task.Run(() =>
                {
                    appInformation.DisplayName = package.DisplayName;

                    try
                    {
                        appInformation.PackageFamilyName = string.IsNullOrEmpty(package.Package.Id.FamilyName) ? NotAvailableString : package.Package.Id.FamilyName;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.PackageFamilyName = NotAvailableString;
                    }

                    try
                    {
                        appInformation.PackageFullName = string.IsNullOrEmpty(package.Package.Id.FullName) ? NotAvailableString : package.Package.Id.FullName;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.PackageFullName = NotAvailableString;
                    }

                    try
                    {
                        appInformation.Description = string.IsNullOrEmpty(package.Package.Description) ? NotAvailableString : package.Package.Description;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.Description = NotAvailableString;
                    }

                    appInformation.PublisherDisplayName = package.PublisherDisplayName;

                    try
                    {
                        appInformation.PublisherId = string.IsNullOrEmpty(package.Package.Id.PublisherId) ? NotAvailableString : package.Package.Id.PublisherId;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.PublisherId = NotAvailableString;
                    }

                    appInformation.Version = package.Version;
                    appInformation.InstallDate = package.InstallDate;

                    try
                    {
                        appInformation.Architecture = string.IsNullOrEmpty(Convert.ToString(package.Package.Id.Architecture)) ? NotAvailableString : Convert.ToString(package.Package.Id.Architecture);
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.Architecture = NotAvailableString;
                    }

                    appInformation.SignatureKind = ResourceService.GetLocalized(string.Format(SignatureString, Convert.ToString(package.SignatureKind)));

                    try
                    {
                        appInformation.ResourceId = string.IsNullOrEmpty(package.Package.Id.ResourceId) ? NotAvailableString : package.Package.Id.ResourceId;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.ResourceId = NotAvailableString;
                    }

                    try
                    {
                        appInformation.IsBundle = package.Package.IsBundle ? YesString : NoString;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsBundle = NotAvailableString;
                    }

                    try
                    {
                        appInformation.IsDevelopmentMode = package.Package.IsDevelopmentMode ? YesString : NoString;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsDevelopmentMode = NotAvailableString;
                    }

                    appInformation.IsFramework = package.IsFramework ? YesString : NoString;

                    try
                    {
                        appInformation.IsOptional = package.Package.IsOptional ? YesString : NoString;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsOptional = NotAvailableString;
                    }

                    try
                    {
                        appInformation.IsResourcePackage = package.Package.IsResourcePackage ? YesString : NoString;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsResourcePackage = NotAvailableString;
                    }

                    try
                    {
                        appInformation.IsStub = package.Package.IsStub ? YesString : NoString;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsStub = NotAvailableString;
                    }

                    try
                    {
                        appInformation.VerifyIsOK = package.Package.Status.VerifyIsOK() ? YesString : NoString;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.VerifyIsOK = NotAvailableString;
                    }

                    try
                    {
                        IReadOnlyList<AppListEntry> appListEntriesList = package.Package.GetAppListEntries();
                        for (int index = 0; index < appListEntriesList.Count; index++)
                        {
                            appInformation.AppListEntryList.Add(new AppListEntryModel()
                            {
                                DisplayName = appListEntriesList[index].DisplayInfo.DisplayName,
                                Description = appListEntriesList[index].DisplayInfo.Description,
                                AppUserModelId = appListEntriesList[index].AppUserModelId,
                                AppListEntry = appListEntriesList[index],
                                PackageFullName = package.Package.Id.FullName
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }

                    try
                    {
                        IReadOnlyList<Package> dependencyList = package.Package.Dependencies;

                        if (dependencyList.Count > 0)
                        {
                            for (int index = 0; index < dependencyList.Count; index++)
                            {
                                try
                                {
                                    appInformation.DependenciesList.Add(new PackageModel()
                                    {
                                        DisplayName = dependencyList[index].DisplayName,
                                        PublisherDisplayName = dependencyList[index].PublisherDisplayName,
                                        Version = Convert.ToString(new Version(dependencyList[index].Id.Version.Major, dependencyList[index].Id.Version.Minor, dependencyList[index].Id.Version.Build, dependencyList[index].Id.Version.Revision)),
                                        Package = dependencyList[index]
                                    });
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }

                        appInformation.DependenciesList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });

                if (MainWindow.Current.GetFrameContent() is AppManagerPage appManagerPage && Equals(appManagerPage.GetCurrentPageType(), appManagerPage.PageList[0]))
                {
                    appManagerPage.NavigateTo(appManagerPage.PageList[1], appInformation, true);
                }
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：应用管理页面——挂载的事件

        /// <summary>
        /// 打开设置中的安装的应用
        /// </summary>
        private void OnInstalledAppsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开开发者选项
        /// </summary>
        private void OnDeveloperOptionsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:developers"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开应用包存储卷设置
        /// </summary>
        private void OnPackageVolumeConfigurationClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.PackageVolume);
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private async void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerCollection.Clear();

                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        foreach (PackageModel packageItem in conditionWithSignatureKindList)
                        {
                            if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                            {
                                searchedList.Add(packageItem);
                            }
                        }

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnQuerySubmitted), 1, e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                if (AppManagerList.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyDescriptionString;
                }
                else if (AppManagerCollection.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyWithConditionDescriptionString;
                }
                else
                {
                    AppManagerFailedContent = string.Empty;
                }
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText))
            {
                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerCollection.Clear();

                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        searchedList.AddRange(conditionWithSignatureKindList);

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnTextChanged), 1, e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                if (AppManagerList.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyDescriptionString;
                }
                else if (AppManagerCollection.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyWithConditionDescriptionString;
                }
                else
                {
                    AppManagerFailedContent = string.Empty;
                }
            }
        }

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag is bool increase)
            {
                IsIncrease = increase;

                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerCollection.Clear();

                List<PackageModel> filterSortPackageList = [];

                try
                {
                    List<PackageModel> conditionWithFrameworkList = [];

                    // 根据选项是否筛选包含框架包的数据
                    if (IsAppFramework)
                    {
                        foreach (PackageModel packageItem in AppManagerList)
                        {
                            if (Equals(packageItem.IsFramework, IsAppFramework))
                            {
                                conditionWithFrameworkList.Add(packageItem);
                            }
                        }
                    }
                    else
                    {
                        conditionWithFrameworkList.AddRange(AppManagerList);
                    }

                    // 根据选项是否筛选包含特定签名类型的数据
                    List<PackageModel> conditionWithSignatureKindList = [];
                    foreach (PackageModel packageItem in conditionWithFrameworkList)
                    {
                        if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                        {
                            conditionWithSignatureKindList.Add(packageItem);
                        }
                        else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                        {
                            conditionWithSignatureKindList.Add(packageItem);
                        }
                        else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                        {
                            conditionWithSignatureKindList.Add(packageItem);
                        }
                        else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                        {
                            conditionWithSignatureKindList.Add(packageItem);
                        }
                        else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                        {
                            conditionWithSignatureKindList.Add(packageItem);
                        }
                    }

                    List<PackageModel> searchedList = [];

                    // 根据搜索内容筛选包含特定签名类型的数据
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        searchedList.AddRange(conditionWithSignatureKindList);
                    }
                    else
                    {
                        foreach (PackageModel packageItem in conditionWithSignatureKindList)
                        {
                            if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                            {
                                searchedList.Add(packageItem);
                            }
                        }
                    }

                    // 对过滤后的列表数据进行排序
                    switch (SelectedAppSortRuleKind)
                    {
                        case AppSortRuleKind.DisplayName:
                            {
                                if (IsIncrease)
                                {
                                    searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                }
                                else
                                {
                                    searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                }
                                break;
                            }
                        case AppSortRuleKind.PublisherName:
                            {
                                if (IsIncrease)
                                {
                                    searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                }
                                else
                                {
                                    searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                }
                                break;
                            }
                        case AppSortRuleKind.InstallDate:
                            {
                                if (IsIncrease)
                                {
                                    searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                }
                                else
                                {
                                    searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                }
                                break;
                            }
                    }

                    filterSortPackageList.AddRange(searchedList);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnSortWayClicked), 1, e);
                }

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                if (AppManagerList.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyDescriptionString;
                }
                else if (AppManagerCollection.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyWithConditionDescriptionString;
                }
                else
                {
                    AppManagerFailedContent = string.Empty;
                }
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private async void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag is AppSortRuleKind appSortRuleKind)
            {
                SelectedAppSortRuleKind = appSortRuleKind;

                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerCollection.Clear();

                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        if (string.IsNullOrEmpty(SearchText))
                        {
                            searchedList.AddRange(conditionWithSignatureKindList);
                        }
                        else
                        {
                            foreach (PackageModel packageItem in conditionWithSignatureKindList)
                            {
                                if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    searchedList.Add(packageItem);
                                }
                            }
                        }

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnSortRuleClicked), 1, e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                if (AppManagerList.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyDescriptionString;
                }
                else if (AppManagerCollection.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyWithConditionDescriptionString;
                }
                else
                {
                    AppManagerFailedContent = string.Empty;
                }
            }
        }

        /// <summary>
        /// 根据过滤方式对列表进行过滤
        /// </summary>
        private void OnFilterWayClicked(object sender, RoutedEventArgs args)
        {
            IsAppFramework = !IsAppFramework;
            needToRefreshData = true;
        }

        /// <summary>
        /// 根据签名规则进行过滤
        /// </summary>
        private void OnSignatureRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToggleButton>() is ToggleButton toggleButton && toggleButton.Tag is not null)
            {
                PackageSignatureKind signatureKind = toggleButton.Tag.As<PackageSignatureKind>();

                if (signatureKind is PackageSignatureKind.Store)
                {
                    IsStoreSignatureSelected = !IsStoreSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.System)
                {
                    IsSystemSignatureSelected = !IsSystemSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.Enterprise)
                {
                    IsEnterpriseSignatureSelected = !IsEnterpriseSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.Developer)
                {
                    IsDeveloperSignatureSelected = !IsDeveloperSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.None)
                {
                    IsNoneSignatureSelected = !IsNoneSignatureSelected;
                }

                needToRefreshData = true;
            }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetAppListAsync();
        }

        /// <summary>
        /// 浮出菜单关闭后更新数据
        /// </summary>
        private async void OnClosed(object sender, object args)
        {
            if (needToRefreshData)
            {
                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerCollection.Clear();

                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        if (string.IsNullOrEmpty(SearchText))
                        {
                            searchedList.AddRange(conditionWithSignatureKindList);
                        }
                        else
                        {
                            foreach (PackageModel packageItem in conditionWithSignatureKindList)
                            {
                                if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    searchedList.Add(packageItem);
                                }
                            }
                        }

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnClosed), 2, e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                if (AppManagerList.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyDescriptionString;
                }
                else if (AppManagerCollection.Count is 0)
                {
                    AppManagerFailedContent = PackageEmptyWithConditionDescriptionString;
                }
                else
                {
                    AppManagerFailedContent = string.Empty;
                }
            }

            needToRefreshData = false;
        }

        #endregion 第三部分：应用管理页面——挂载的事件

        #region 第四部分：应用管理页面——自定义事件

        /// <summary>
        /// 应用移动状态发生改变时触发的事件
        /// </summary>
        private void OnPackageMoveProgress(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, DeploymentProgress progress, PackageModel package)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                package.PackageOperationProgress = progress.percentage;
            });
        }

        /// <summary>
        /// 应用修复状态发生改变时触发的事件
        /// </summary>
        private void OnPackageRepairProgress(IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> result, PackageDeploymentProgress progress, PackageModel package)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                package.PackageOperationProgress = Convert.ToInt32(progress.Progress * 100);
            });
        }

        /// <summary>
        /// 应用重置状态发生改变时触发的事件
        /// </summary>
        private void OnPackageResetProgress(IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> result, PackageDeploymentProgress progress, PackageModel package)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                package.PackageOperationProgress = Convert.ToInt32(progress.Progress * 100);
            });
        }

        /// <summary>
        /// 应用卸载状态发生改变时触发的事件
        /// </summary>
        private void OnPackageUninstallProgress(IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> result, PackageDeploymentProgress progress, PackageModel package)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                package.PackageOperationProgress = Convert.ToInt32(progress.Progress * 100);
            });
        }

        #endregion 第四部分：应用管理页面——自定义事件

        /// <summary>
        /// 获取应用列表数据
        /// </summary>
        private async Task GetAppListAsync()
        {
            AppManagerResultKind = AppManagerResultKind.Loading;
            AppManagerList.Clear();
            AppManagerCollection.Clear();

            List<PackageModel> packageList = await Task.Run(() =>
            {
                List<PackageModel> packageList = [];

                try
                {
                    foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
                    {
                        packageList.Add(new PackageModel()
                        {
                            LogoImage = package.Logo,
                            IsFramework = GetIsFramework(package),
                            AppListEntryCount = GetAppListEntriesCount(package),
                            DisplayName = GetDisplayName(package),
                            InstallDate = GetInstallDate(package),
                            PublisherDisplayName = GetPublisherDisplayName(package),
                            Version = GetVersion(package),
                            SignatureKind = GetSignatureKind(package),
                            InstalledDate = GetInstalledDate(package),
                            Package = package,
                            IsOperating = false
                        });
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(OnRefreshClicked), 1, e);
                }

                return packageList;
            });

            AppManagerList.AddRange(packageList);

            if (AppManagerList.Count is 0)
            {
                AppManagerResultKind = AppManagerResultKind.Failed;
                AppManagerFailedContent = PackageEmptyDescriptionString;
            }
            else
            {
                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        if (string.IsNullOrEmpty(SearchText))
                        {
                            searchedList.AddRange(conditionWithSignatureKindList);
                        }
                        else
                        {
                            foreach (PackageModel packageItem in conditionWithSignatureKindList)
                            {
                                if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    searchedList.Add(packageItem);
                                }
                            }
                        }

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppListPage), nameof(GetAppListAsync), 2, e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescriptionString : string.Empty;
            }
        }

        /// <summary>
        /// 获取加载应用是否成功
        /// </summary>
        private Visibility GetAppManagerSuccessfullyState(AppManagerResultKind appManagerResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? appManagerResultKind is AppManagerResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : appManagerResultKind is AppManagerResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索应用是否成功
        /// </summary>
        private Visibility CheckAppManagerState(AppManagerResultKind appManagerResultKind, AppManagerResultKind comparedAppManagerResultKind)
        {
            return Equals(appManagerResultKind, comparedAppManagerResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(AppManagerResultKind appManagerResultKind)
        {
            return appManagerResultKind is not AppManagerResultKind.Loading;
        }

        /// <summary>
        /// 获取应用包是否为框架包
        /// </summary>
        private static bool GetIsFramework(Package package)
        {
            try
            {
                return package.IsFramework;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return false;
            }
        }

        /// <summary>
        /// 获取应用包的入口数
        /// </summary>
        private static int GetAppListEntriesCount(Package package)
        {
            try
            {
                return package.GetAppListEntries().Count;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return 0;
            }
        }

        /// <summary>
        /// 获取应用的显示名称
        /// </summary>
        private string GetDisplayName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.DisplayName) ? NotAvailableString : package.DisplayName;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return NotAvailableString;
            }
        }

        /// <summary>
        /// 获取应用的发布者显示名称
        /// </summary>
        private string GetPublisherDisplayName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.PublisherDisplayName) ? NotAvailableString : package.PublisherDisplayName;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return NotAvailableString;
            }
        }

        /// <summary>
        /// 获取应用的版本信息
        /// </summary>
        private static string GetVersion(Package package)
        {
            try
            {
                return Convert.ToString(new Version(package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision));
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return Convert.ToString(new Version());
            }
        }

        /// <summary>
        /// 获取应用的安装日期
        /// </summary>
        private static string GetInstallDate(Package package)
        {
            try
            {
                return package.InstalledDate.ToString("yyyy/MM/dd HH:mm");
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return DateTimeOffset.FromUnixTimeSeconds(0).ToString("yyyy/MM/dd HH:mm");
            }
        }

        /// <summary>
        /// 获取应用包签名方式
        /// </summary>
        private static PackageSignatureKind GetSignatureKind(Package package)
        {
            try
            {
                return package.SignatureKind;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return PackageSignatureKind.None;
            }
        }

        /// <summary>
        /// 获取应用包安装日期
        /// </summary>
        private static DateTimeOffset GetInstalledDate(Package package)
        {
            try
            {
                return package.InstalledDate;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return DateTimeOffset.FromUnixTimeSeconds(0);
            }
        }
    }
}

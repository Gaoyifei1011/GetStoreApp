using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Management.Deployment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用包存储卷设置页面
    /// </summary>
    internal sealed partial class SettingsPackageVolumePage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string NoString = ResourceService.GetLocalized("SettingsPackageVolume/No");
        private readonly string PackageVolumeCountInfoString = ResourceService.GetLocalized("SettingsPackageVolume/PackageVolumeCountInfo");
        private readonly string PackageVolumeEmptyString = ResourceService.GetLocalized("SettingsPackageVolume/PackageVolumeEmpty");
        private readonly string PackageVolumeFailedString = ResourceService.GetLocalized("SettingsPackageVolume/PackageVolumeFailed");
        private readonly string DismountedString = ResourceService.GetLocalized("SettingsPackageVolume/Dismounted");
        private readonly string DismountFailed1String = ResourceService.GetLocalized("SettingsPackageVolume/DismountFailed1");
        private readonly string DismountFailed2String = ResourceService.GetLocalized("SettingsPackageVolume/DismountFailed2");
        private readonly string DismountFailed3String = ResourceService.GetLocalized("SettingsPackageVolume/DismountFailed3");
        private readonly string DismountFailed4String = ResourceService.GetLocalized("SettingsPackageVolume/DismountFailed4");
        private readonly string DismountSuccessfullyString = ResourceService.GetLocalized("SettingsPackageVolume/DismountSuccessfully");
        private readonly string MountedString = ResourceService.GetLocalized("SettingsPackageVolume/Mounted");
        private readonly string MountFailed1String = ResourceService.GetLocalized("SettingsPackageVolume/MountFailed1");
        private readonly string MountFailed2String = ResourceService.GetLocalized("SettingsPackageVolume/MountFailed2");
        private readonly string MountFailed3String = ResourceService.GetLocalized("SettingsPackageVolume/MountFailed3");
        private readonly string MountFailed4String = ResourceService.GetLocalized("SettingsPackageVolume/MountFailed4");
        private readonly string MountSuccessfullyString = ResourceService.GetLocalized("SettingsPackageVolume/MountSuccessfully");
        private readonly string NotAvailableString = ResourceService.GetLocalized("SettingsPackageVolume/NotAvailable");
        private readonly string RemoveFailed1String = ResourceService.GetLocalized("SettingsPackageVolume/RemoveFailed1");
        private readonly string RemoveFailed2String = ResourceService.GetLocalized("SettingsPackageVolume/RemoveFailed2");
        private readonly string RemoveFailed3String = ResourceService.GetLocalized("SettingsPackageVolume/RemoveFailed3");
        private readonly string RemoveFailed4String = ResourceService.GetLocalized("SettingsPackageVolume/RemoveFailed4");
        private readonly string RemoveSuccessfullyString = ResourceService.GetLocalized("SettingsPackageVolume/RemoveSuccessfully");
        private readonly string SetDefaultFailed1String = ResourceService.GetLocalized("SettingsPackageVolume/SetDefaultFailed1");
        private readonly string SetDefaultFailed2String = ResourceService.GetLocalized("SettingsPackageVolume/SetDefaultFailed2");
        private readonly string SetDefaultFailed3String = ResourceService.GetLocalized("SettingsPackageVolume/SetDefaultFailed3");
        private readonly string SetDefaultFailed4String = ResourceService.GetLocalized("SettingsPackageVolume/SetDefaultFailed4");
        private readonly string SetDefaultSuccessfullyString = ResourceService.GetLocalized("SettingsPackageVolume/SetDefaultSuccessfully");
        private readonly string VolumeSpaceString = ResourceService.GetLocalized("SettingsPackageVolume/VolumeSpace");
        private readonly string YesString = ResourceService.GetLocalized("SettingsPackageVolume/Yes");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private PackageVolumeResultKind _packageVolumeResultKind;

        private PackageVolumeResultKind PackageVolumeResultKind
        {
            get { return _packageVolumeResultKind; }

            set
            {
                if (!Equals(_packageVolumeResultKind, value))
                {
                    _packageVolumeResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageVolumeResultKind)));
                }
            }
        }

        private string _packageVolumeFailedContent;

        private string PackageVolumeFailedContent
        {
            get { return _packageVolumeFailedContent; }

            set
            {
                if (!string.Equals(_packageVolumeFailedContent, value))
                {
                    _packageVolumeFailedContent = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageVolumeFailedContent)));
                }
            }
        }

        private ObservableCollection<PackageVolumeModel> PackageVolumeCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SettingsPackageVolumePage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            await GetPackageVolumeInfoAsync();
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 设置为默认卷
        /// </summary>
        private async void OnSetDefaultVolumeExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageVolumeModel packageVolume && !packageVolume.IsDefaultVolume)
            {
                packageVolume.IsOperating = true;
                (bool result, Exception exception) = await SetDefaultVolumeAsync(packageVolume.PackageVolume);
                packageVolume.IsOperating = false;
                ShowSetDefaultVolumeResultNotification(packageVolume, result, exception);

                if (result)
                {
                    foreach (PackageVolumeModel packageVolumeItem in PackageVolumeCollection)
                    {
                        packageVolumeItem.DefaultVolume = NoString;
                        packageVolumeItem.IsDefaultVolume = false;
                    }

                    packageVolume.DefaultVolume = YesString;
                    packageVolume.IsDefaultVolume = true;
                }
            }
        }

        /// <summary>
        /// 挂载卷
        /// </summary>
        private async void OnMountExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageVolumeModel packageVolume && packageVolume.IsOffline)
            {
                packageVolume.IsOperating = true;
                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await MountVolumeAsync(packageVolume.PackageVolume);
                packageVolume.IsOperating = false;
                ShowMountVolumeResultNotification(packageVolume, result, packageDeploymentResult, exception);

                if (result && packageDeploymentResult is not null && packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                {
                    packageVolume.Offline = MountedString;
                    packageVolume.IsOffline = false;
                }
            }
        }

        /// <summary>
        /// 卸载卷
        /// </summary>
        private async void OnDismountExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageVolumeModel packageVolume && !packageVolume.IsOffline)
            {
                packageVolume.IsOperating = true;
                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await DismountVolumeAsync(packageVolume.PackageVolume);
                packageVolume.IsOperating = false;
                ShowDismountVolumeResultNotification(packageVolume, result, packageDeploymentResult, exception);

                if (result && packageDeploymentResult is not null && packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                {
                    packageVolume.Offline = DismountedString;
                    packageVolume.IsOffline = true;
                }
            }
        }

        /// <summary>
        /// 移除卷
        /// </summary>
        private async void OnRemoveExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageVolumeModel packageVolume)
            {
                packageVolume.IsOperating = true;
                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await RemoveVolumeAync(packageVolume.PackageVolume);
                packageVolume.IsOperating = false;
                ShowRemoveVolumeResultNotification(packageVolume, result, packageDeploymentResult, exception);

                if (result && packageDeploymentResult is not null && packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                {
                    PackageVolumeCollection.Remove(packageVolume);

                    if (PackageVolumeCollection.Count is 0)
                    {
                        PackageVolumeResultKind = PackageVolumeResultKind.Failed;
                        PackageVolumeFailedContent = PackageVolumeEmptyString;
                    }
                    else
                    {
                        PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                        PackageVolumeFailedContent = string.Empty;
                    }
                }
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 添加存储卷
        /// </summary>
        private async void OnAddNewPackageVolumeClicked(object sender, RoutedEventArgs args)
        {
            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new PackageVolumeAddDialog());

            if (contentDialogResult is ContentDialogResult.Primary)
            {
                await GetPackageVolumeInfoAsync();
            }
        }

        /// <summary>
        /// 刷新应用包存储卷信息
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await GetPackageVolumeInfoAsync();
        }

        /// <summary>
        /// 设置说明
        /// </summary>
        private void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 获取应用包存储卷信息
        /// </summary>
        private async Task GetPackageVolumeInfoAsync()
        {
            if (PackageVolumeResultKind is not PackageVolumeResultKind.Loading)
            {
                PackageVolumeResultKind = PackageVolumeResultKind.Loading;
                PackageVolumeCollection.Clear();
                (bool result, List<PackageVolumeModel> packageVolumeList, Exception exception) = await GetPackageVolumeAsync();

                if (result)
                {
                    if (packageVolumeList.Count is 0)
                    {
                        PackageVolumeResultKind = PackageVolumeResultKind.Failed;
                        PackageVolumeFailedContent = PackageVolumeEmptyString;
                    }
                    else
                    {
                        foreach (PackageVolumeModel packageVolumeItem in packageVolumeList)
                        {
                            PackageVolumeCollection.Add(packageVolumeItem);
                        }

                        PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                        PackageVolumeFailedContent = string.Empty;
                    }
                }
                else
                {
                    PackageVolumeResultKind = PackageVolumeResultKind.Failed;
                    PackageVolumeFailedContent = string.Format(PackageVolumeFailedString, exception is not null ? exception.Message : NotAvailableString, exception is not null ? string.Format("0x{0:X8}", exception.HResult) : NotAvailableString);
                }
            }
        }

        /// <summary>
        /// 获取存储卷
        /// </summary>
        private async Task<(bool, List<PackageVolumeModel>, Exception)> GetPackageVolumeAsync()
        {
            return await Task.Run(async () =>
            {
                List<PackageVolumeModel> packageVolumeList = [];

                try
                {
                    PackageVolume defaultVolume = PackageVolume.GetDefault();
                    IList<PackageVolume> requestedPackageVolumeList = PackageVolume.FindPackageVolumes();

                    foreach (PackageVolume packageVolume in requestedPackageVolumeList)
                    {
                        if (packageVolume.IsAppxInstallSupported && packageVolume.IsFullTrustPackageSupported && packageVolume.SupportsHardLinks && !string.Equals(packageVolume.MountPoint, packageVolume.PackageStorePath))
                        {
                            double availableSpace = await packageVolume.GetAvailableSpaceAsync();
                            string displayName = string.Empty;
                            double totalSpace = 0;

                            if (!string.IsNullOrEmpty(packageVolume.MountPoint))
                            {
                                StorageFolder rootFolder = null;

                                try
                                {
                                    rootFolder = await StorageFolder.GetFolderFromPathAsync(packageVolume.MountPoint);
                                    displayName = rootFolder.DisplayName;
                                    if (rootFolder is not null)
                                    {
                                        IDictionary<string, object> propertiesDict = await rootFolder.Properties.RetrievePropertiesAsync((string[])["System.Capacity"]);

                                        if (propertiesDict.TryGetValue("System.Capacity", out object value))
                                        {
                                            totalSpace = Convert.ToDouble(value);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                            }

                            double usedPercentage = totalSpace is 0 ? 0 : (totalSpace - availableSpace) / totalSpace * 100;
                            string availableSpaceString = VolumeSizeHelper.ConvertVolumeSizeToString(availableSpace);
                            string totalSpaceString = VolumeSizeHelper.ConvertVolumeSizeToString(totalSpace);
                            bool isDefaultVolume = string.Equals(packageVolume.Name, defaultVolume.Name);

                            PackageVolumeModel packageVolumeItem = new()
                            {
                                IsOperating = false,
                                Name = string.Format("{0}[{1}]", displayName, packageVolume.PackageStorePath),
                                Space = string.Format(VolumeSpaceString, availableSpaceString, totalSpaceString),
                                PackageVolumeId = packageVolume.Name,
                                PackageVolumePath = packageVolume.PackageStorePath,
                                MountPoint = packageVolume.MountPoint,
                                PackageVolumeUsedPercentage = usedPercentage,
                                PackageVolume = packageVolume,
                                IsAvailableSpaceWarning = usedPercentage > 90,
                                IsAvailableSpaceError = usedPercentage > 95,
                                DefaultVolume = isDefaultVolume ? YesString : NoString,
                                IsDefaultVolume = isDefaultVolume,
                                IsAppxInstallSupported = packageVolume.IsAppxInstallSupported ? YesString : NoString,
                                IsFullTrustPackageSupported = packageVolume.IsFullTrustPackageSupported ? YesString : NoString,
                                Offline = packageVolume.IsOffline() ? YesString : NoString,
                                IsOffline = packageVolume.IsOffline(),
                                IsSystemVolume = packageVolume.IsSystemVolume ? YesString : NoString,
                                SupportedHardLinks = packageVolume.SupportsHardLinks ? YesString : NoString,
                            };

                            packageVolumeList.Add(packageVolumeItem);
                        }
                    }

                    return ValueTuple.Create<bool, List<PackageVolumeModel>, Exception>(true, packageVolumeList, null);
                }
                catch (Exception e)
                {
                    return ValueTuple.Create<bool, List<PackageVolumeModel>, Exception>(false, null, e);
                }
            });
        }

        /// <summary>
        /// 设置默认卷
        /// </summary>
        private async Task<(bool, Exception)> SetDefaultVolumeAsync(PackageVolume packageVolume)
        {
            if (packageVolume is null)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                try
                {
                    packageVolume.SetDefault();
                    return ValueTuple.Create<bool, Exception>(true, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(SetDefaultVolumeAsync), 1, e);
                    return ValueTuple.Create(true, e);
                }
            });
        }

        /// <summary>
        /// 挂载存储卷
        /// </summary>
        private async Task<(bool, PackageDeploymentResult, Exception)> MountVolumeAsync(PackageVolume packageVolume)
        {
            if (packageVolume is null)
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                try
                {
                    PackageDeploymentResult packageDeploymentResult = await packageVolume.SetOnlineAsync();
                    return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, packageDeploymentResult, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(MountVolumeAsync), 1, e);
                    return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                }
            });
        }

        /// <summary>
        /// 卸载存储卷
        /// </summary>
        private async Task<(bool, PackageDeploymentResult, Exception)> DismountVolumeAsync(PackageVolume packageVolume)
        {
            if (packageVolume is null)
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                try
                {
                    PackageDeploymentResult packageDeploymentResult = await packageVolume.SetOfflineAsync();
                    return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, packageDeploymentResult, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(DismountVolumeAsync), 1, e);
                    return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                }
            });
        }

        /// <summary>
        /// 移除存储卷
        /// </summary>
        private async Task<(bool, PackageDeploymentResult, Exception)> RemoveVolumeAync(PackageVolume packageVolume)
        {
            if (packageVolume is null)
            {
                return default;
            }

            return await Task.Run(async () =>
            {
                try
                {
                    PackageDeploymentResult packageDeploymentResult = await packageVolume.RemoveAsync();
                    return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, packageDeploymentResult, null);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(RemoveVolumeAync), 1, e);
                    return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                }
            });
        }

        /// <summary>
        /// 显示设置默认卷成功通知
        /// </summary>
        private void ShowSetDefaultVolumeResultNotification(PackageVolumeModel packageVolume, bool result, Exception exception)
        {
            if (packageVolume is not null)
            {
                Task.Run(() =>
                {
                    if (result)
                    {
                        // 显示应用包存储默认卷设置成功通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(SetDefaultSuccessfullyString, packageVolume.Name));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    }
                    else
                    {
                        // 显示应用包存储默认卷设置成功通知

                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(SetDefaultFailed1String, packageVolume.Name));
                        appNotificationBuilder.AddText(SetDefaultFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            string.Format(SetDefaultFailed3String, exception is not null ? string.Format("0x{0:X8}",exception.HResult) : NotAvailableString),
                            string.Format(SetDefaultFailed4String, exception is not null ? exception.Message : NotAvailableString)
                        }));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(ShowSetDefaultVolumeResultNotification), 1, exception is not null ? exception : new());
                    }
                });
            }
        }

        /// <summary>
        /// 显示挂载卷结果通知
        /// </summary>
        private void ShowMountVolumeResultNotification(PackageVolumeModel packageVolume, bool result, PackageDeploymentResult packageDeploymentResult, Exception exception)
        {
            if (packageVolume is not null)
            {
                Task.Run(() =>
                {
                    if (result && packageDeploymentResult is not null)
                    {
                        if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                        {
                            // 显示应用包存储卷挂载成功通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(MountSuccessfullyString, packageVolume.Name));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        }
                        else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                        {
                            string errorCode = packageDeploymentResult.Error is not null ? string.Format("0x{0:X8}", packageDeploymentResult.Error.HResult) : NotAvailableString;
                            string errorMessage = string.IsNullOrEmpty(packageDeploymentResult.ErrorText) ? packageDeploymentResult.Error is not null ? packageDeploymentResult.Error.Message : NotAvailableString : packageDeploymentResult.ErrorText;

                            // 显示应用包存储卷挂载失败通知
                            Task.Run(() =>
                            {
                                AppNotificationBuilder appNotificationBuilder = new();
                                appNotificationBuilder.AddArgument("action", "OpenApp");
                                appNotificationBuilder.AddText(string.Format(MountFailed1String, packageVolume.Name));
                                appNotificationBuilder.AddText(MountFailed2String);
                                appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                                {
                                    string.Format(MountFailed3String, errorCode),
                                    string.Format(MountFailed4String, errorMessage)
                                }));
                                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(ShowMountVolumeResultNotification), 1, exception is not null ? exception : new());
                            });
                        }
                    }
                    else
                    {
                        // 显示应用包存储卷挂载失败通知
                        Task.Run(() =>
                       {
                           AppNotificationBuilder appNotificationBuilder = new();
                           appNotificationBuilder.AddArgument("action", "OpenApp");
                           appNotificationBuilder.AddText(string.Format(MountFailed1String, packageVolume.Name));
                           appNotificationBuilder.AddText(MountFailed2String);
                           appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                           {
                                string.Format(MountFailed3String, exception is not null ? string.Format("0x{0:X8}",exception.HResult) : NotAvailableString),
                                string.Format(MountFailed4String, exception is not null ? exception.Message : NotAvailableString)
                           }));
                           ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                           LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(ShowMountVolumeResultNotification), 2, exception is not null ? exception : new());
                       });
                    }
                });
            }
        }

        /// <summary>
        /// 显示卸载卷结果通知
        /// </summary>
        private void ShowDismountVolumeResultNotification(PackageVolumeModel packageVolume, bool result, PackageDeploymentResult packageDeploymentResult, Exception exception)
        {
            if (packageVolume is not null)
            {
                Task.Run(() =>
                {
                    if (result && packageDeploymentResult is not null)
                    {
                        if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                        {
                            // 显示应用包存储卷卸载成功通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(DismountSuccessfullyString, packageVolume.Name));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        }
                        else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                        {
                            string errorCode = packageDeploymentResult.Error is not null ? string.Format("0x{0:X8}", packageDeploymentResult.Error.HResult) : NotAvailableString;
                            string errorMessage = string.IsNullOrEmpty(packageDeploymentResult.ErrorText) ? packageDeploymentResult.Error is not null ? packageDeploymentResult.Error.Message : NotAvailableString : packageDeploymentResult.ErrorText;

                            // 显示应用包存储卷卸载失败通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(DismountFailed1String, packageVolume.Name));
                            appNotificationBuilder.AddText(DismountFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                string.Format(DismountFailed3String, errorCode),
                                string.Format(DismountFailed4String, errorMessage)
                            }));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(ShowDismountVolumeResultNotification), 1, exception is not null ? exception : new());
                        }
                    }
                    else
                    {
                        // 显示应用包存储卷卸载失败通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(DismountFailed1String, packageVolume.Name));
                        appNotificationBuilder.AddText(DismountFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            string.Format(DismountFailed3String, exception is not null ? string.Format("0x{0:X8}",exception.HResult) : NotAvailableString),
                            string.Format(DismountFailed4String, exception is not null ? exception.Message : NotAvailableString)
                        }));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(ShowDismountVolumeResultNotification), 2, exception is not null ? exception : new());
                    }
                });
            }
        }

        /// <summary>
        /// 显示移除卷结果通知
        /// </summary>
        private void ShowRemoveVolumeResultNotification(PackageVolumeModel packageVolume, bool result, PackageDeploymentResult packageDeploymentResult, Exception exception)
        {
            if (packageVolume is not null)
            {
                Task.Run(() =>
                {
                    if (result && packageDeploymentResult is not null)
                    {
                        if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                        {
                            // 显示应用包存储卷移除成功通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RemoveSuccessfullyString, packageVolume.Name));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        }
                        else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                        {
                            string errorCode = packageDeploymentResult.Error is not null ? string.Format("0x{0:X8}", packageDeploymentResult.Error.HResult) : NotAvailableString;
                            string errorMessage = string.IsNullOrEmpty(packageDeploymentResult.ErrorText) ? packageDeploymentResult.Error is not null ? packageDeploymentResult.Error.Message : NotAvailableString : packageDeploymentResult.ErrorText;

                            // 显示应用包存储卷移除失败通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RemoveFailed1String, packageVolume.Name));
                            appNotificationBuilder.AddText(RemoveFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                string.Format(RemoveFailed3String, errorCode),
                                string.Format(RemoveFailed4String, errorMessage)
                            }));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(ShowRemoveVolumeResultNotification), 1, exception is not null ? exception : new());
                        }
                    }
                    else
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(RemoveFailed1String, packageVolume.Name));
                        appNotificationBuilder.AddText(RemoveFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            string.Format(RemoveFailed3String, exception is not null ? string.Format("0x{0:X8}",exception.HResult) : NotAvailableString),
                            string.Format(RemoveFailed4String, exception is not null ? exception.Message : NotAvailableString)
                        }));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(ShowRemoveVolumeResultNotification), 2, exception is not null ? exception : new());
                    }
                });
            }
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(PackageVolumeResultKind packageVolumeResultKind)
        {
            return packageVolumeResultKind is not PackageVolumeResultKind.Loading;
        }

        /// <summary>
        /// 获取包存储卷是否加载完成
        /// </summary>
        private Visibility GetPackageVolumeSuccessfullyVisibility(PackageVolumeResultKind packageVolumeResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? packageVolumeResultKind is PackageVolumeResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : packageVolumeResultKind is PackageVolumeResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查包存储卷是否加载成功
        /// </summary>
        private Visibility CheckPackageVolumeResultKindVisibility(PackageVolumeResultKind packageVolumeResultKind, PackageVolumeResultKind comparedPackageVolumeResultKind, bool needReverse)
        {
            return needReverse ? Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Collapsed : Visibility.Visible : Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        private string GetLocalizedPackageVolumeCountInfo(int packageVolumeCollectionCount)
        {
            return string.Format(PackageVolumeCountInfoString, packageVolumeCollectionCount);
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}

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
    public sealed partial class SettingsPackageVolumePage : Page, INotifyPropertyChanged
    {
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

        private PackageVolumeResultKind _packageVolumeResultKind = PackageVolumeResultKind.Loading;

        public PackageVolumeResultKind PackageVolumeResultKind
        {
            get { return _packageVolumeResultKind; }

            set
            {
                if (!Equals(_packageVolumeResultKind, value))
                {
                    _packageVolumeResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageVolumeResultKind)));
                }
            }
        }

        private string _packageVolumeFailedContent;

        public string PackageVolumeFailedContent
        {
            get { return _packageVolumeFailedContent; }

            set
            {
                if (!string.Equals(_packageVolumeFailedContent, value))
                {
                    _packageVolumeFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageVolumeFailedContent)));
                }
            }
        }

        private ObservableCollection<PackageVolumeModel> PackageVolumeCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPackageVolumePage()
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
            await GetPackageVolumeInfoAsync();
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 设置为默认卷
        /// </summary>
        private async void OnSetDefaultVolumeExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageVolumeModel packageVolume && !packageVolume.IsDefaultVolume)
            {
                packageVolume.IsOperating = true;
                (bool result, Exception exception) = await Task.Run(() =>
                {
                    try
                    {
                        packageVolume.PackageVolume.SetDefault();
                        return ValueTuple.Create<bool, Exception>(true, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnSetDefaultVolumeExecuteRequested), 1, e);
                        return ValueTuple.Create(true, e);
                    }
                });

                packageVolume.IsOperating = false;
                if (result)
                {
                    foreach (PackageVolumeModel packageVolumeItem in PackageVolumeCollection)
                    {
                        packageVolumeItem.DefaultVolume = NoString;
                        packageVolumeItem.IsDefaultVolume = false;
                    }

                    packageVolume.DefaultVolume = YesString;
                    packageVolume.IsDefaultVolume = true;

                    // 显示应用包存储默认卷设置成功通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(SetDefaultSuccessfullyString, packageVolume.Name));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    });
                }
                else
                {
                    // 显示应用包存储默认卷设置成功通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(SetDefaultFailed1String, packageVolume.Name));
                        appNotificationBuilder.AddText(SetDefaultFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            string.Format(SetDefaultFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                            string.Format(SetDefaultFailed4String, exception.Message)
                        }));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnSetDefaultVolumeExecuteRequested), 2, exception is not null ? exception : new Exception());
                    });
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
                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                {
                    try
                    {
                        PackageDeploymentResult packageDeploymentResult = await packageVolume.PackageVolume.SetOnlineAsync();
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, packageDeploymentResult, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnMountExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                    }
                });

                packageVolume.IsOperating = false;
                if (result && packageDeploymentResult is not null)
                {
                    if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                    {
                        packageVolume.Offline = MountedString;
                        packageVolume.IsOffline = false;

                        // 显示应用包存储卷挂载成功通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(MountSuccessfullyString, packageVolume.Name));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                    }
                    else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                    {
                        // 显示应用包存储卷挂载失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(MountFailed1String, packageVolume.Name));
                            appNotificationBuilder.AddText(MountFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                string.Format(MountFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(MountFailed4String, exception.Message)
                            }));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnMountExecuteRequested), 2, exception is not null ? exception : new Exception());
                        });
                    }
                }
                else
                {
                    // 显示应用包存储卷挂载失败通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(MountFailed1String, packageVolume.Name));
                        appNotificationBuilder.AddText(MountFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            string.Format(MountFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(MountFailed4String, exception.Message)
                        }));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnMountExecuteRequested), 2, exception is not null ? exception : new Exception());
                    });
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
                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                {
                    try
                    {
                        PackageDeploymentResult packageDeploymentResult = await packageVolume.PackageVolume.SetOfflineAsync();
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, packageDeploymentResult, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnMountExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                    }
                });

                packageVolume.IsOperating = false;
                if (result && packageDeploymentResult is not null)
                {
                    if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                    {
                        packageVolume.Offline = DismountedString;
                        packageVolume.IsOffline = true;

                        // 显示应用包存储卷卸载成功通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(DismountSuccessfullyString, packageVolume.Name));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                    }
                    else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                    {
                        // 显示应用包存储卷卸载失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(DismountFailed1String, packageVolume.Name));
                            appNotificationBuilder.AddText(DismountFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                string.Format(DismountFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(DismountFailed4String, exception.Message)
                            }));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnDismountExecuteRequested), 2, exception is not null ? exception : new Exception());
                        });
                    }
                }
                else
                {
                    // 显示应用包存储卷卸载失败通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(DismountFailed1String, packageVolume.Name));
                        appNotificationBuilder.AddText(DismountFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            string.Format(DismountFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(DismountFailed4String, exception.Message)
                        }));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnDismountExecuteRequested), 2, exception is not null ? exception : new Exception());
                    });
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
                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                {
                    try
                    {
                        PackageDeploymentResult packageDeploymentResult = await packageVolume.PackageVolume.RemoveAsync();
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, packageDeploymentResult, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnMountExecuteRequested), 1, e);
                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                    }
                });

                packageVolume.IsOperating = false;
                if (result && packageDeploymentResult is not null)
                {
                    if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
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

                        // 显示应用包存储卷移除成功通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RemoveSuccessfullyString, packageVolume.Name));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                    }
                    else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                    {
                        // 显示应用包存储卷移除失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RemoveFailed1String, packageVolume.Name));
                            appNotificationBuilder.AddText(RemoveFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                string.Format(RemoveFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(RemoveFailed4String, exception.Message)
                            }));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnRemoveExecuteRequested), 2, exception is not null ? exception : new Exception());
                        });
                    }
                }
                else
                {
                    // 显示应用包存储卷移除失败通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(RemoveFailed1String, packageVolume.Name));
                        appNotificationBuilder.AddText(RemoveFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            string.Format(RemoveFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString),
                                string.Format(RemoveFailed4String, exception.Message)
                        }));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsPackageVolumePage), nameof(OnRemoveExecuteRequested), 2, exception is not null ? exception : new Exception());
                    });
                }
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：应用包存储卷设置页面——挂载的事件

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

        #endregion 第三部分：应用包存储卷设置页面——挂载的事件

        /// <summary>
        /// 获取应用包存储卷信息
        /// </summary>
        private async Task GetPackageVolumeInfoAsync()
        {
            PackageVolumeResultKind = PackageVolumeResultKind.Loading;
            PackageVolumeCollection.Clear();

            (bool result, List<PackageVolumeModel> packageVolumeList, Exception exception) = await Task.Run(async () =>
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
                PackageVolumeFailedContent = string.Format(PackageVolumeFailedString, exception is not null ? exception.Message : NotAvailableString, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpperInvariant() : NotAvailableString);
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
        private Visibility GetPackageVolumeSuccessfullyState(PackageVolumeResultKind packageVolumeResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? packageVolumeResultKind is PackageVolumeResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : packageVolumeResultKind is PackageVolumeResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查包存储卷是否加载成功
        /// </summary>
        private Visibility CheckPackageVolumeState(PackageVolumeResultKind packageVolumeResultKind, PackageVolumeResultKind comparedPackageVolumeResultKind, bool needReverse)
        {
            return needReverse ? Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Collapsed : Visibility.Visible : Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        private string GetLocalizedPackageVolumeCountInfo(int packageVolumeCollectionCount)
        {
            return string.Format(PackageVolumeCountInfoString, packageVolumeCollectionCount);
        }
    }
}

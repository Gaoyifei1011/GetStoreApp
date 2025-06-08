using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;

// 抑制 CA1822，IDE0028，IDE0060警告
#pragma warning disable CA1822,IDE0028,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用包存储卷信息页面
    /// </summary>
    public sealed partial class PackageVolumeInfoPage : Page, INotifyPropertyChanged
    {
        private readonly string VolumeSpaceString = ResourceService.GetLocalized("PackageVolumeInfo/VolumeSpace");
        private readonly string OpenSettingsString = ResourceService.GetLocalized("PackageVolumeInfo/OpenSettings");
        private readonly string RemoveFailed1String = ResourceService.GetLocalized("PackageVolumeInfo/RemoveFailed1");
        private readonly string RemoveFailed2String = ResourceService.GetLocalized("PackageVolumeInfo/RemoveFailed2");
        private readonly string RemoveFailed3String = ResourceService.GetLocalized("PackageVolumeInfo/RemoveFailed3");
        private readonly string RemoveFailed4String = ResourceService.GetLocalized("PackageVolumeInfo/RemoveFailed4");
        private readonly string RemoveFailed5String = ResourceService.GetLocalized("PackageVolumeInfo/RemoveFailed5");
        private readonly string RemoveSuccessfullyString = ResourceService.GetLocalized("PackageVolumeInfo/RemoveSuccessfully");
        private readonly string UnknownString = ResourceService.GetLocalized("PackageVolumeInfo/Unknown");
        private PackageVolumeDialog PackageVolumeDialog;
        private PackageManager PackageManager;
        private PackageModel Package;

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

        private PackageVolumeModel _currentPackageVolume = new();

        public PackageVolumeModel CurrentPackageVolume
        {
            get { return _currentPackageVolume; }

            set
            {
                if (!Equals(_currentPackageVolume, value))
                {
                    _currentPackageVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPackageVolume)));
                }
            }
        }

        private PackageVolumeModel _selectedPackageVolume;

        public PackageVolumeModel SelectedPackageVolume
        {
            get { return _selectedPackageVolume; }

            set
            {
                if (!Equals(_selectedPackageVolume, value))
                {
                    _selectedPackageVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPackageVolume)));
                }
            }
        }

        private bool _isRemovePackageVolume;

        public bool IsRemovePackageVolume
        {
            get { return _isRemovePackageVolume; }

            set
            {
                if (!Equals(_isRemovePackageVolume, value))
                {
                    _isRemovePackageVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRemovePackageVolume)));
                }
            }
        }

        private bool _isRemovePackageVolumeEnabled;

        public bool IsRemovePackageVolumeEnabled
        {
            get { return _isRemovePackageVolumeEnabled; }

            set
            {
                if (!Equals(_isRemovePackageVolumeEnabled, value))
                {
                    _isRemovePackageVolumeEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRemovePackageVolumeEnabled)));
                }
            }
        }

        public ObservableCollection<PackageVolumeModel> PackageVolumeCollection = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public PackageVolumeInfoPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is List<object> argsList && argsList.Count is 3 && argsList[0] is PackageVolumeDialog packageVolumeDialog && argsList[1] is PackageManager packageManager && argsList[2] is PackageModel package)
            {
                PackageVolumeDialog = packageVolumeDialog;
                PackageManager = packageManager;
                Package = package;

                SelectedPackageVolume = null;
                packageVolumeDialog.IsPrimaryButtonEnabled = false;
                IsRemovePackageVolumeEnabled = false;
                (PackageVolumeModel currentPackageVolume, List<PackageVolumeModel> packageVolumeList) = await Task.Run(async () =>
                {
                    PackageVolumeModel currentPackageVolume = null;
                    List<PackageVolumeModel> packageVolumeList = [];
                    IReadOnlyList<PackageVolume> requestedPackageVolumeList = await PackageManager.GetPackageVolumesAsync();

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
                                        IDictionary<string, object> propertiesDict = await rootFolder.Properties.RetrievePropertiesAsync(new List<string> { "System.Capacity" });

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

                            double usedPercentage = totalSpace.Equals(0) ? 0 : (totalSpace - availableSpace) / totalSpace * 100;
                            string availableSpaceString = FileSizeHelper.ConvertFileSizeToString(availableSpace);
                            string totalSpaceString = FileSizeHelper.ConvertFileSizeToString(totalSpace);

                            PackageVolumeModel packageVolumeItem = new()
                            {
                                Name = string.Format("{0}[{1}]", displayName, packageVolume.PackageStorePath),
                                Space = string.Format(VolumeSpaceString, availableSpaceString, totalSpaceString),
                                PackageVolumeId = packageVolume.Name,
                                PackageVolumePath = packageVolume.PackageStorePath,
                                PackageVolumeUsedPercentage = usedPercentage,
                                WinRTPackageVolume = packageVolume,
                                IsAvailableSpaceWarning = usedPercentage > 90,
                                IsAvailableSpaceError = usedPercentage > 95,
                            };

                            if (currentPackageVolume is null && packageVolume.FindPackageForUser(string.Empty, Package.Package.Id.FullName).Count > 0)
                            {
                                currentPackageVolume = packageVolumeItem;
                            }
                            else
                            {
                                packageVolumeList.Add(packageVolumeItem);
                            }
                        }
                    }

                    return ValueTuple.Create(currentPackageVolume, packageVolumeList);
                });

                if (currentPackageVolume is not null)
                {
                    CurrentPackageVolume = currentPackageVolume;
                }

                if (packageVolumeList.Count is 0)
                {
                    PackageVolumeResultKind = PackageVolumeResultKind.Empty;
                }
                else
                {
                    PackageVolumeCollection.Clear();
                    foreach (PackageVolumeModel packageVolumeItem in packageVolumeList)
                    {
                        PackageVolumeCollection.Add(packageVolumeItem);
                    }

                    PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                }
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第二部分：应用包存储卷信息页面——挂载的事件

        /// <summary>
        /// 应用包存储卷选中项发生变化时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0)
            {
                SelectedPackageVolume = args.AddedItems[0] as PackageVolumeModel;
                IsRemovePackageVolumeEnabled = true;
                PackageVolumeDialog.IsPrimaryButtonEnabled = true;
            }
        }

        /// <summary>
        /// 刷新应用包存储卷
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            PackageVolumeResultKind = PackageVolumeResultKind.Loading;
            SelectedPackageVolume = null;
            IsRemovePackageVolumeEnabled = false;
            PackageVolumeDialog.IsPrimaryButtonEnabled = false;

            (PackageVolumeModel currentPackageVolume, List<PackageVolumeModel> packageVolumeList) = await Task.Run(async () =>
            {
                PackageVolumeModel currentPackageVolume = null;
                List<PackageVolumeModel> packageVolumeList = [];
                IReadOnlyList<PackageVolume> requestedPackageVolumeList = await PackageManager.GetPackageVolumesAsync();

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
                                    IDictionary<string, object> propertiesDict = await rootFolder.Properties.RetrievePropertiesAsync(new List<string> { "System.Capacity" });

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

                        double usedPercentage = totalSpace.Equals(0) ? 0 : (totalSpace - availableSpace) / totalSpace * 100;
                        string availableSpaceString = FileSizeHelper.ConvertFileSizeToString(availableSpace);
                        string totalSpaceString = FileSizeHelper.ConvertFileSizeToString(totalSpace);

                        PackageVolumeModel packageVolumeItem = new()
                        {
                            Name = string.Format("{0}[{1}]", displayName, packageVolume.PackageStorePath),
                            Space = string.Format(VolumeSpaceString, availableSpaceString, totalSpaceString),
                            PackageVolumeId = packageVolume.Name,
                            PackageVolumePath = packageVolume.PackageStorePath,
                            PackageVolumeUsedPercentage = usedPercentage,
                            WinRTPackageVolume = packageVolume,
                            IsAvailableSpaceWarning = usedPercentage > 90,
                            IsAvailableSpaceError = usedPercentage > 95,
                        };

                        if (currentPackageVolume is null && packageVolume.FindPackageForUser(string.Empty, Package.Package.Id.FullName).Count > 0)
                        {
                            currentPackageVolume = packageVolumeItem;
                        }
                        else
                        {
                            packageVolumeList.Add(packageVolumeItem);
                        }
                    }
                }

                return ValueTuple.Create(currentPackageVolume, packageVolumeList);
            });

            if (currentPackageVolume is not null)
            {
                CurrentPackageVolume = currentPackageVolume;
            }

            if (packageVolumeList.Count is 0)
            {
                PackageVolumeResultKind = PackageVolumeResultKind.Empty;
            }
            else
            {
                PackageVolumeCollection.Clear();
                foreach (PackageVolumeModel packageVolumeItem in packageVolumeList)
                {
                    PackageVolumeCollection.Add(packageVolumeItem);
                }

                PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
            }
        }

        /// <summary>
        /// 添加应用包存储卷
        /// </summary>
        private void OnAddPackageVolumeClicked(object sender, RoutedEventArgs args)
        {
            PackageVolumeDialog.NavigateTo(PackageVolumeDialog.PageList[1], null, true);
        }

        /// <summary>
        /// 移除应用包存储卷
        /// </summary>
        private async void OnRemovePackageVolumeClicked(object sender, RoutedEventArgs args)
        {
            if (SelectedPackageVolume is not null)
            {
                PackageVolumeResultKind = PackageVolumeResultKind.Operating;
                IsRemovePackageVolumeEnabled = false;
                PackageVolumeDialog.IsPrimaryButtonEnabled = false;
                IsRemovePackageVolume = true;

                (bool result, DeploymentResult deploymentResult, Exception exception) = await Task.Run(async () =>
                {
                    try
                    {
                        IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> removePackageWithProgress = PackageManager.RemovePackageVolumeAsync(SelectedPackageVolume.WinRTPackageVolume);
                        return ValueTuple.Create<bool, DeploymentResult, Exception>(true, await removePackageWithProgress, null);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeDialog), nameof(OnRemovePackageVolumeClicked), 1, e);
                        return ValueTuple.Create<bool, DeploymentResult, Exception>(false, null, e);
                    }
                });

                // 移除成功
                if (result && deploymentResult is not null)
                {
                    if (deploymentResult.ExtendedErrorCode is null)
                    {
                        // 显示应用包存储卷移除成功通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RemoveSuccessfullyString, SelectedPackageVolume.Name));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });

                        PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                        PackageVolumeCollection.Remove(SelectedPackageVolume);
                        SelectedPackageVolume = null;
                        IsRemovePackageVolumeEnabled = true;
                        PackageVolumeDialog.IsPrimaryButtonEnabled = false;
                        IsRemovePackageVolume = false;
                    }
                    else
                    {
                        // 显示 UWP 应用移动失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(RemoveFailed1String, SelectedPackageVolume.Name));
                            appNotificationBuilder.AddText(RemoveFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                RemoveFailed3String,
                                string.Format(RemoveFailed4String, deploymentResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(deploymentResult.ExtendedErrorCode.HResult, 16).ToUpper() : UnknownString),
                                string.Format(RemoveFailed5String, deploymentResult.ErrorText)
                            }));
                            AppNotificationButton openSettingsButton = new(OpenSettingsString);
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeDialog), nameof(OnRemovePackageVolumeClicked), 2, deploymentResult.ExtendedErrorCode is not null ? deploymentResult.ExtendedErrorCode : new Exception());
                        });

                        PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                        IsRemovePackageVolumeEnabled = true;
                        PackageVolumeDialog.IsPrimaryButtonEnabled = false;
                        IsRemovePackageVolume = false;
                    }
                }
                else
                {
                    // 显示 UWP 应用移动失败通知
                    await Task.Run(() =>
                    {
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(RemoveFailed1String, SelectedPackageVolume.Name));
                        appNotificationBuilder.AddText(RemoveFailed2String);
                        appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                        {
                            RemoveFailed3String,
                            string.Format(RemoveFailed4String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpper() : UnknownString),
                            string.Format(RemoveFailed5String, exception.Message)
                        }));
                        AppNotificationButton openSettingsButton = new(OpenSettingsString);
                        openSettingsButton.Arguments.Add("action", "OpenSettings");
                        appNotificationBuilder.AddButton(openSettingsButton);
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeDialog), nameof(OnRemovePackageVolumeClicked), 3, exception is not null ? exception : new Exception());
                    });

                    PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                    IsRemovePackageVolumeEnabled = true;
                    PackageVolumeDialog.IsPrimaryButtonEnabled = false;
                    IsRemovePackageVolume = false;
                }
            }
        }

        #endregion 第二部分：应用包存储卷信息页面——挂载的事件

        /// <summary>
        /// 获取是否正在加载中或操作中
        /// </summary>

        private bool GetIsLoadingOrOperating(PackageVolumeResultKind packageVolumeResultKind)
        {
            return !(packageVolumeResultKind is PackageVolumeResultKind.Loading || packageVolumeResultKind is PackageVolumeResultKind.Operating);
        }

        private bool GetIsOperating(PackageVolumeResultKind packageVolumeResultKind)
        {
            return packageVolumeResultKind is not PackageVolumeResultKind.Operating;
        }

        private bool GetIsRemovePackageVolumeEnabled(bool isElevated, bool isRemovePackageVolumeEnabled)
        {
            return isElevated && isRemovePackageVolumeEnabled;
        }

        /// <summary>
        /// 检查加载下载已完成文件是否成功
        /// </summary>
        private Visibility CheckPackageVolumeState(PackageVolumeResultKind packageVolumeResultKind, PackageVolumeResultKind comparedPackageVolumeResultKind, bool needReverse)
        {
            return needReverse ? Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Collapsed : Visibility.Visible : Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取包存储卷是否加载完成
        /// </summary>
        private Visibility GetPackageVolumeSuccessfullyState(PackageVolumeResultKind packageVolumeResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Visible : Visibility.Collapsed : packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;

// 抑制 CA1822，CS8305，IDE0028，IDE0060 警告
#pragma warning disable CA1822,CS8305,IDE0028,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 应用包添加卷对话框
    /// </summary>
    public sealed partial class PackageVolumeAddDialog : ContentDialog, INotifyPropertyChanged
    {
        private readonly string CreateFailed1String = ResourceService.GetLocalized("Dialog/CreateFailed1");
        private readonly string CreateFailed2String = ResourceService.GetLocalized("Dialog/CreateFailed2");
        private readonly string CreateFailed3String = ResourceService.GetLocalized("Dialog/CreateFailed3");
        private readonly string CreateFailed4String = ResourceService.GetLocalized("Dialog/CreateFailed4");
        private readonly string CreateSuccessfullyString = ResourceService.GetLocalized("Dialog/CreateSuccessfully");
        private readonly string UnknownString = ResourceService.GetLocalized("Dialog/Unknown");
        private readonly string VolumeSpaceString = ResourceService.GetLocalized("Dialog/VolumeSpace");
        private readonly PackageManager PackageManager;

        private bool _isPrimaryEnabled;

        public bool IsPrimaryEnabled
        {
            get { return _isPrimaryEnabled; }

            set
            {
                if (!Equals(_isPrimaryEnabled, value))
                {
                    _isPrimaryEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPrimaryEnabled)));
                }
            }
        }

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

        private string _selectedFolder;

        public string SelectedFolder
        {
            get { return _selectedFolder; }

            set
            {
                if (!Equals(_selectedFolder, value))
                {
                    _selectedFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFolder)));
                }
            }
        }

        private bool _useWindowsAppsFolderValue = true;

        public bool UseWindowsAppsFolderValue
        {
            get { return _useWindowsAppsFolderValue; }

            set
            {
                if (!Equals(_useWindowsAppsFolderValue, value))
                {
                    _useWindowsAppsFolderValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseWindowsAppsFolderValue)));
                }
            }
        }

        private bool _setDefaultVolumeValue;

        public bool SetDefaultVolumeValue
        {
            get { return _setDefaultVolumeValue; }

            set
            {
                if (!Equals(_setDefaultVolumeValue, value))
                {
                    _setDefaultVolumeValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SetDefaultVolumeValue)));
                }
            }
        }

        private bool _isAddingPackageVolume;

        public bool IsAddingPackageVolume
        {
            get { return _isAddingPackageVolume; }

            set
            {
                if (!Equals(_isAddingPackageVolume, value))
                {
                    _isAddingPackageVolume = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAddingPackageVolume)));
                }
            }
        }

        public ObservableCollection<PackageVolumeModel> PackageVolumeCollection = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public PackageVolumeAddDialog(PackageManager packageManager)
        {
            InitializeComponent();
            PackageManager = packageManager;
        }

        #region 第一部分：应用包添加卷对话框——挂载的事件

        /// <summary>
        /// 打开对话框时触发的事件
        /// </summary>
        private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            SelectedPackageVolume = null;
            SelectedFolder = string.Empty;
            UseWindowsAppsFolderValue = true;
            SetDefaultVolumeValue = false;
            IsPrimaryEnabled = false;
            await GetAvailablePackageVolumeAsync();
        }

        /// <summary>
        /// 应用包可用存储卷选中项发生变化时触发的事件
        /// </summary>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0)
            {
                SelectedPackageVolume = args.AddedItems[0] as PackageVolumeModel;
                IsPrimaryEnabled = true;
                if (UseWindowsAppsFolderValue && SelectedPackageVolume is not null && SelectedPackageVolume.WinRTPackageVolume is not null)
                {
                    SelectedFolder = Path.Combine(SelectedPackageVolume.WinRTPackageVolume.MountPoint, "WindowsApps");
                }
                else
                {
                    if (!string.IsNullOrEmpty(SelectedFolder) && SelectedPackageVolume is not null && SelectedPackageVolume.WinRTPackageVolume is not null)
                    {
                        string rootPath = Path.GetPathRoot(SelectedFolder);
                        SelectedFolder = SelectedFolder.Replace(rootPath, SelectedPackageVolume.WinRTPackageVolume.MountPoint);
                    }
                }
            }
        }

        /// <summary>
        /// 选择存放的文件夹
        /// </summary>
        private async void OnSelectFolderClicked(object sender, RoutedEventArgs args)
        {
            if (SelectedPackageVolume is not null)
            {
                try
                {
                    FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                    {
                        SuggestedStartLocation = PickerLocationId.Downloads
                    };

                    if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                    {
                        string rootPath = Path.GetPathRoot(pickFolderResult.Path);
                        SelectedFolder = pickFolderResult.Path.Replace(rootPath, SelectedPackageVolume.WinRTPackageVolume.MountPoint);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeAddPage), nameof(OnSelectFolderClicked), 1, e);
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FolderPicker));
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SelectPackageVolumeEmpty));
            }
        }

        /// <summary>
        /// 使用 WindowsApps 默认目录
        /// </summary>
        private void OnUseWindowsAppsFolderToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                UseWindowsAppsFolderValue = toggleSwitch.IsOn;
                if (SelectedPackageVolume is not null && SelectedPackageVolume.WinRTPackageVolume is not null)
                {
                    SelectedFolder = Path.Combine(SelectedPackageVolume.WinRTPackageVolume.MountPoint, "WindowsApps");
                }
            }
        }

        /// <summary>
        /// 设置为默认卷
        /// </summary>

        private void OnSetDefaultVolumeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                SetDefaultVolumeValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 刷新应用包可用存储卷
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            SelectedPackageVolume = null;
            SelectedFolder = string.Empty;
            IsPrimaryEnabled = false;
            await GetAvailablePackageVolumeAsync();
        }

        /// <summary>
        /// 保存添加的存储卷
        /// </summary>
        private async void OnPrimaryButtonClicked(object sender, ContentDialogButtonClickEventArgs args)
        {
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                PackageVolumeResultKind = PackageVolumeResultKind.Operating;
                IsAddingPackageVolume = true;

                if (!string.IsNullOrEmpty(SelectedFolder) && SelectedPackageVolume is not null)
                {
                    (bool result, PackageVolume packageVolume, Exception exception) = await Task.Run(async () =>
                    {
                        try
                        {
                            IAsyncOperation<PackageVolume> packageVolumeProgress = PackageManager.AddPackageVolumeAsync(SelectedFolder);
                            return ValueTuple.Create<bool, PackageVolume, Exception>(true, await packageVolumeProgress, null);
                        }
                        catch (Exception e)
                        {
                            return ValueTuple.Create<bool, PackageVolume, Exception>(false, null, e);
                        }
                    });

                    if (result)
                    {
                        // 显示存储卷添加成功的通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(CreateSuccessfullyString, string.Format("{0}[{1}]", SelectedPackageVolume.Name, SelectedFolder)));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                        PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                        IsAddingPackageVolume = false;

                        if (SetDefaultVolumeValue)
                        {
                            // 设置默认卷
                            await Task.Run(() =>
                            {
                                try
                                {
                                    PackageManager.SetDefaultPackageVolume(packageVolume);
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                            });
                        }
                    }
                    else
                    {
                        // 显示存储卷添加失败的通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(CreateFailed1String, string.Format("{0}[{1}]", SelectedPackageVolume.Name, SelectedFolder)));
                            appNotificationBuilder.AddText(CreateFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                            string.Format(CreateFailed3String, exception is not null ? "0x" + Convert.ToString(exception.HResult, 16).ToUpper() : UnknownString),
                            string.Format(CreateFailed4String, exception.Message)
                            }));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeDialog), nameof(OnPrimaryButtonClicked), 1, exception is not null ? exception : new Exception());
                        });
                        PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                        IsAddingPackageVolume = false;
                    }
                }
                else
                {
                    // 显示选择的文件夹为空的通知
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SelectFolderEmpty));
                    });
                    PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                    IsAddingPackageVolume = false;
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                contentDialogButtonClickDeferral.Complete();
            }
        }

        #endregion 第一部分：应用包添加卷对话框——挂载的事件

        /// <summary>
        /// 获取应用包可用存储卷信息
        /// </summary>
        private async Task GetAvailablePackageVolumeAsync()
        {
            List<PackageVolumeModel> packageVolumeList = await Task.Run(async () =>
            {
                List<PackageVolumeModel> packageVolumeList = [];
                IReadOnlyList<PackageVolume> requestedPackageVolumeList = await PackageManager.GetPackageVolumesAsync();

                foreach (PackageVolume packageVolume in requestedPackageVolumeList)
                {
                    if (string.Equals(packageVolume.MountPoint, packageVolume.PackageStorePath, StringComparison.OrdinalIgnoreCase))
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
                            Name = displayName,
                            Space = string.Format(VolumeSpaceString, availableSpaceString, totalSpaceString),
                            PackageVolumeId = packageVolume.Name,
                            PackageVolumePath = packageVolume.PackageStorePath,
                            PackageVolumeUsedPercentage = usedPercentage,
                            WinRTPackageVolume = packageVolume,
                            IsAvailableSpaceWarning = usedPercentage > 90,
                            IsAvailableSpaceError = usedPercentage > 95,
                        };

                        packageVolumeList.Add(packageVolumeItem);
                    }
                }

                return packageVolumeList;
            });

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
        /// 获取是否正在加载中或操作中
        /// </summary>

        private bool GetIsLoadingOrOperating(PackageVolumeResultKind packageVolumeResultKind)
        {
            return !(packageVolumeResultKind is PackageVolumeResultKind.Loading || packageVolumeResultKind is PackageVolumeResultKind.Operating);
        }

        /// <summary>
        /// 获取是否正在操作中
        /// </summary>
        private bool GetIsOperating(PackageVolumeResultKind packageVolumeResultKind)
        {
            return packageVolumeResultKind is not PackageVolumeResultKind.Operating;
        }

        /// <summary>
        /// 检查包可用存储卷是否加载成功
        /// </summary>
        private Visibility CheckPackageVolumeState(PackageVolumeResultKind packageVolumeResultKind, PackageVolumeResultKind comparedPackageVolumeResultKind, bool needReverse)
        {
            return needReverse ? Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Collapsed : Visibility.Visible : Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取包可用存储卷是否加载完成
        /// </summary>
        private Visibility GetPackageVolumeSuccessfullyState(PackageVolumeResultKind packageVolumeResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Visible : Visibility.Collapsed : packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

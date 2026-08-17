using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Management.Deployment;
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
using Windows.Storage;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 应用包添加卷对话框
    /// </summary>
    internal sealed partial class PackageVolumeAddDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string CreateFailed1String = ResourceService.GetLocalized("Dialog/CreateFailed1");
        private readonly string CreateFailed2String = ResourceService.GetLocalized("Dialog/CreateFailed2");
        private readonly string CreateFailed3String = ResourceService.GetLocalized("Dialog/CreateFailed3");
        private readonly string CreateFailed4String = ResourceService.GetLocalized("Dialog/CreateFailed4");
        private readonly string CreateSuccessfullyString = ResourceService.GetLocalized("Dialog/CreateSuccessfully");
        private readonly string NotAvailableString = ResourceService.GetLocalized("Dialog/NotAvailable");
        private readonly string VolumeSpaceString = ResourceService.GetLocalized("Dialog/VolumeSpace");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private bool _isItemSelected;

        private bool IsItemSelected
        {
            get { return _isItemSelected; }

            set
            {
                if (!Equals(_isItemSelected, value))
                {
                    _isItemSelected = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsItemSelected)));
                }
            }
        }

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

        private PackageVolumeModel _selectedPackageVolume;

        private PackageVolumeModel SelectedPackageVolume
        {
            get { return _selectedPackageVolume; }

            set
            {
                if (!Equals(_selectedPackageVolume, value))
                {
                    _selectedPackageVolume = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedPackageVolume)));
                }
            }
        }

        private string _selectedFolder;

        private string SelectedFolder
        {
            get { return _selectedFolder; }

            set
            {
                if (!string.Equals(_selectedFolder, value))
                {
                    _selectedFolder = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedFolder)));
                }
            }
        }

        private bool _useWindowsAppsFolder = true;

        private bool UseWindowsAppsFolder
        {
            get { return _useWindowsAppsFolder; }

            set
            {
                if (!Equals(_useWindowsAppsFolder, value))
                {
                    _useWindowsAppsFolder = value;
                    PropertyChanged?.Invoke(this, new(nameof(UseWindowsAppsFolder)));
                }
            }
        }

        private bool _setDefaultVolume;

        private bool SetDefaultVolume
        {
            get { return _setDefaultVolume; }

            set
            {
                if (!Equals(_setDefaultVolume, value))
                {
                    _setDefaultVolume = value;
                    PropertyChanged?.Invoke(this, new(nameof(SetDefaultVolume)));
                }
            }
        }

        private ObservableCollection<PackageVolumeModel> PackageVolumeCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal PackageVolumeAddDialog()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 打开内容对话框后发生的事件
        /// </summary>
        private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            SelectedPackageVolume = null;
            SelectedFolder = string.Empty;
            UseWindowsAppsFolder = true;
            SetDefaultVolume = false;
            IsItemSelected = false;
            await GetPackageVolumeAsync();
        }

        /// <summary>
        /// 加载完成前禁用关闭对话框
        /// </summary>
        private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (PackageVolumeResultKind is PackageVolumeResultKind.Loading)
            {
                args.Cancel = true;
            }
        }

        /// <summary>
        /// 应用包可用存储卷选中项发生变化时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ListView))]
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ListView listView && !Equals(SelectedPackageVolume, listView.SelectedItem))
            {
                SelectedPackageVolume = listView.SelectedItem is PackageVolumeModel packageVolume ? packageVolume : null;

                if (SelectedPackageVolume is not null && SelectedPackageVolume.WinRTPackageVolume is not null)
                {
                    IsItemSelected = true;
                    if (UseWindowsAppsFolder)
                    {
                        SelectedFolder = Path.Combine(SelectedPackageVolume.WinRTPackageVolume.MountPoint, "WindowsApps");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(SelectedFolder))
                        {
                            string rootPath = Path.GetPathRoot(SelectedFolder);
                            SelectedFolder = SelectedFolder.Replace(rootPath, SelectedPackageVolume.WinRTPackageVolume.MountPoint);
                        }
                    }
                }
                else
                {
                    SelectedFolder = null;
                    IsItemSelected = false;
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
                    if (await SelectSaveFolderAsync(SelectedPackageVolume) is PickFolderResult pickFolderResult)
                    {
                        string rootPath = Path.GetPathRoot(pickFolderResult.Path);
                        string saveFolder = pickFolderResult.Path.Replace(rootPath, SelectedPackageVolume.WinRTPackageVolume.MountPoint);
                        if (!string.IsNullOrEmpty(saveFolder))
                        {
                            SelectedFolder = saveFolder;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeAddDialog), nameof(OnSelectFolderClicked), 1, e);
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FolderPicker));
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectPackageVolumeEmpty));
            }
        }

        /// <summary>
        /// 使用 WindowsApps 默认目录
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnUseWindowsAppsFolderToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(UseWindowsAppsFolder, toggleSwitch.IsOn))
            {
                UseWindowsAppsFolder = toggleSwitch.IsOn;
                if (SelectedPackageVolume is not null && SelectedPackageVolume.WinRTPackageVolume is not null)
                {
                    SelectedFolder = Path.Combine(SelectedPackageVolume.WinRTPackageVolume.MountPoint, "WindowsApps");
                }
            }
        }

        /// <summary>
        /// 设置为默认卷
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnSetDefaultVolumeToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(SetDefaultVolume, toggleSwitch.IsOn))
            {
                SetDefaultVolume = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 刷新应用包可用存储卷
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            SelectedPackageVolume = null;
            SelectedFolder = string.Empty;
            IsItemSelected = false;
            await GetPackageVolumeAsync();
        }

        /// <summary>
        /// 保存添加的存储卷
        /// </summary>
        private async void OnSaveClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ContentDialogButtonClickDeferral contentDialogButtonClickDeferral = args.GetDeferral();

            try
            {
                PackageVolumeResultKind = PackageVolumeResultKind.Operating;

                if (!string.IsNullOrEmpty(SelectedFolder) && SelectedPackageVolume is not null)
                {
                    (bool result, PackageVolume packageVolume, Exception exception) = await AddPackageVolumeAsync(SelectedFolder);
                    ShowAddPackageVolumeResultNotification(SelectedPackageVolume, false, SelectedFolder, exception);

                    if (result)
                    {
                        if (SetDefaultVolume)
                        {
                            await SetDefaultVolumeAsync(packageVolume);
                        }
                    }
                    else
                    {
                        PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
                    }
                }
                else
                {
                    // 显示选择的文件夹为空的通知
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectFolderEmpty));
                    });
                    PackageVolumeResultKind = PackageVolumeResultKind.Successfully;
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

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 获取应用包可用存储卷信息
        /// </summary>
        private async Task GetPackageVolumeAsync()
        {
            if (PackageVolumeResultKind is not PackageVolumeResultKind.Loading)
            {
                PackageVolumeResultKind = PackageVolumeResultKind.Loading;
                List<PackageVolumeModel> packageVolumeList = await GetPackageVolumeListAsync();

                if (packageVolumeList.Count is 0)
                {
                    PackageVolumeResultKind = PackageVolumeResultKind.Failed;
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

        /// <summary>
        /// 获取应用包存储卷信息
        /// </summary>
        private async Task<List<PackageVolumeModel>> GetPackageVolumeListAsync()
        {
            return await Task.Run(async () =>
            {
                List<PackageVolumeModel> packageVolumeList = [];
                IReadOnlyList<global::Windows.Management.Deployment.PackageVolume> requestedPackageVolumeList = await new global::Windows.Management.Deployment.PackageManager().GetPackageVolumesAsync();

                foreach (global::Windows.Management.Deployment.PackageVolume packageVolume in requestedPackageVolumeList)
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

                        packageVolumeList.Add(new()
                        {
                            Name = displayName,
                            Space = string.Format(VolumeSpaceString, availableSpaceString, totalSpaceString),
                            PackageVolumeId = packageVolume.Name,
                            PackageVolumePath = packageVolume.PackageStorePath,
                            PackageVolumeUsedPercentage = usedPercentage,
                            WinRTPackageVolume = packageVolume,
                            IsAvailableSpaceWarning = usedPercentage > 90,
                            IsAvailableSpaceError = usedPercentage > 95,
                        });
                    }
                }

                return packageVolumeList;
            });
        }

        /// <summary>
        /// 添加存储卷
        /// </summary>
        private async Task<(bool, PackageVolume, Exception)> AddPackageVolumeAsync(string selectedFolder)
        {
            if (!string.IsNullOrEmpty(selectedFolder))
            {
                return await Task.Run(async () =>
                {
                    try
                    {
                        IAsyncOperation<PackageVolume> packageVolumeProgress = PackageVolume.AddAsync(selectedFolder);
                        return ValueTuple.Create<bool, PackageVolume, Exception>(true, await packageVolumeProgress, null);
                    }
                    catch (Exception e)
                    {
                        return ValueTuple.Create<bool, PackageVolume, Exception>(false, null, e);
                    }
                });
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 选择存放的文件夹
        /// </summary>
        private async Task<PickFolderResult> SelectSaveFolderAsync(PackageVolumeModel packageVolume)
        {
            if (packageVolume is not null && packageVolume.WinRTPackageVolume is not null)
            {
                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                {
                    SuggestedStartLocation = PickerLocationId.Downloads
                };

                return await folderPicker.PickSingleFolderAsync();
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 显示添加存储卷结果通知
        /// </summary>
        private void ShowAddPackageVolumeResultNotification(PackageVolumeModel packageVolume, bool addResult, string saveFolder, Exception exception)
        {
            if (packageVolume is not null)
            {
                Task.Run(() =>
                {
                    if (addResult)
                    {
                        // 显示存储卷添加成功的通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(CreateSuccessfullyString, string.Format("{0}[{1}]", packageVolume.Name, saveFolder)));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    }
                    else
                    {
                        if (exception is not null)
                        {
                            // 显示存储卷添加失败的通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(CreateFailed1String, string.Format("{0}[{1}]", packageVolume.Name, saveFolder)));
                            appNotificationBuilder.AddText(CreateFailed2String);
                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                string.Format(CreateFailed3String, exception is not null ? string.Format("0x{0:X8}",exception.HResult) : NotAvailableString),
                                string.Format(CreateFailed4String, exception is not null ? exception.Message : NotAvailableString)
                            }));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(PackageVolumeAddDialog), nameof(ShowAddPackageVolumeResultNotification), 1, exception is not null ? exception : new());
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 设置默认卷
        /// </summary>
        private async Task SetDefaultVolumeAsync(PackageVolume packageVolume)
        {
            if (packageVolume is not null)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        packageVolume.SetDefault();
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });
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
        /// 获取是否加载成功并且选项是否已经选中
        /// </summary>
        private bool GetIsLoadSuccessfullyAndItemSelected(PackageVolumeResultKind packageVolumeResultKind, bool isItemSelected)
        {
            return packageVolumeResultKind is PackageVolumeResultKind.Successfully && isItemSelected;
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
        private Visibility CheckPackageVolumeResultKindVisibility(PackageVolumeResultKind packageVolumeResultKind, PackageVolumeResultKind comparedPackageVolumeResultKind, bool needReverse)
        {
            return needReverse ? Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Collapsed : Visibility.Visible : Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取包可用存储卷是否加载完成
        /// </summary>
        private Visibility GetPackageVolumeSuccessfullyVisibility(PackageVolumeResultKind packageVolumeResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Visible : Visibility.Collapsed : packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}

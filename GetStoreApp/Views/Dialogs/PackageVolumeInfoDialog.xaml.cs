using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Management.Deployment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Storage;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 应用包存储卷信息对话框
    /// </summary>
    internal sealed partial class PackageVolumeInfoDialog : ContentDialog, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string VolumeSpaceString = ResourceService.GetLocalized("Dialog/VolumeSpace");
        private readonly global::Windows.Management.Deployment.PackageManager packageManager = new();
        private readonly PackageModel Package;

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

        private PackageVolumeModel _currentPackageVolume = new();

        private PackageVolumeModel CurrentPackageVolume
        {
            get { return _currentPackageVolume; }

            set
            {
                if (!Equals(_currentPackageVolume, value))
                {
                    _currentPackageVolume = value;
                    PropertyChanged?.Invoke(this, new(nameof(CurrentPackageVolume)));
                }
            }
        }

        private PackageVolumeModel _selectedPackageVolume;

        internal PackageVolumeModel SelectedPackageVolume
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

        private bool _isRemovingPackageVolume;

        private bool IsRemovingPackageVolume
        {
            get { return _isRemovingPackageVolume; }

            set
            {
                if (!Equals(_isRemovingPackageVolume, value))
                {
                    _isRemovingPackageVolume = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRemovingPackageVolume)));
                }
            }
        }

        private bool _isRemovePackageVolumeEnabled;

        private bool IsRemovePackageVolumeEnabled
        {
            get { return _isRemovePackageVolumeEnabled; }

            set
            {
                if (!Equals(_isRemovePackageVolumeEnabled, value))
                {
                    _isRemovePackageVolumeEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsRemovePackageVolumeEnabled)));
                }
            }
        }

        private ObservableCollection<PackageVolumeModel> PackageVolumeCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal PackageVolumeInfoDialog(PackageModel package)
        {
            InitializeComponent();
            Package = package;
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 打开内容对话框后发生的事件
        /// </summary>
        private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            SelectedPackageVolume = null;
            IsRemovePackageVolumeEnabled = false;
            IsItemSelected = false;
            await GetPackageVolumeInfoAsync();
        }

        /// <summary>
        /// 应用包存储卷选中项发生变化时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ListView))]
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ListView listView && !Equals(SelectedPackageVolume, listView.SelectedItem))
            {
                SelectedPackageVolume = listView.SelectedItem is PackageVolumeModel packageVolume ? packageVolume : null;

                if (SelectedPackageVolume is not null)
                {
                    IsRemovePackageVolumeEnabled = true;
                    IsItemSelected = true;
                }
                else
                {
                    IsRemovePackageVolumeEnabled = false;
                    IsItemSelected = false;
                }
            }
        }

        /// <summary>
        /// 刷新应用包存储卷
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            SelectedPackageVolume = null;
            IsRemovePackageVolumeEnabled = false;
            IsItemSelected = false;
            await GetPackageVolumeInfoAsync();
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 获取应用包存储卷信息
        /// </summary>
        private async Task GetPackageVolumeInfoAsync()
        {
            if (PackageVolumeResultKind is not PackageVolumeResultKind.Loading)
            {
                PackageVolumeResultKind = PackageVolumeResultKind.Loading;

                (PackageVolumeModel currentPackageVolume, List<PackageVolumeModel> packageVolumeList) = await GetCurrentPackageAndPackageVolumeListAsync();

                if (currentPackageVolume is not null)
                {
                    CurrentPackageVolume = currentPackageVolume;
                }

                if (packageVolumeList is null || packageVolumeList.Count is 0)
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
        private async Task<(PackageVolumeModel, List<PackageVolumeModel>)> GetCurrentPackageAndPackageVolumeListAsync()
        {
            return await Task.Run(async () =>
            {
                PackageVolumeModel currentPackageVolume = null;
                List<PackageVolumeModel> packageVolumeList = [];
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

                        PackageVolumeModel packageVolumeItem = new()
                        {
                            Name = string.Format("{0}[{1}]", displayName, packageVolume.PackageStorePath),
                            Space = string.Format(VolumeSpaceString, availableSpaceString, totalSpaceString),
                            PackageVolumeId = packageVolume.Name,
                            PackageVolumePath = packageVolume.PackageStorePath,
                            PackageVolumeUsedPercentage = usedPercentage,
                            PackageVolume = packageVolume,
                            IsAvailableSpaceWarning = usedPercentage > 90,
                            IsAvailableSpaceError = usedPercentage > 95,
                        };

                        global::Windows.Management.Deployment.PackageVolume winRTPackageVolume = null;
                        foreach (global::Windows.Management.Deployment.PackageVolume winRTPackageVolumeItem in packageManager.FindPackageVolumes())
                        {
                            if (string.Equals(winRTPackageVolumeItem.PackageStorePath, packageVolume.PackageStorePath))
                            {
                                winRTPackageVolume = winRTPackageVolumeItem;
                                break;
                            }
                        }
                        if (currentPackageVolume is null && winRTPackageVolume.FindPackageForUser(string.Empty, Package.Package.Id.FullName).Count > 0)
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
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>
        private bool GetIsLoading(PackageVolumeResultKind packageVolumeResultKind)
        {
            return packageVolumeResultKind is not PackageVolumeResultKind.Loading;
        }

        /// <summary>
        /// 检查包存储卷是否加载成功
        /// </summary>
        private Visibility CheckPackageVolumeResultKindVisibility(PackageVolumeResultKind packageVolumeResultKind, PackageVolumeResultKind comparedPackageVolumeResultKind, bool needReverse)
        {
            return needReverse ? Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Collapsed : Visibility.Visible : Equals(packageVolumeResultKind, comparedPackageVolumeResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取包存储卷是否加载完成
        /// </summary>
        private Visibility GetPackageVolumeSuccessfullyVisibility(PackageVolumeResultKind packageVolumeResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Visible : Visibility.Collapsed : packageVolumeResultKind is PackageVolumeResultKind.Successfully || packageVolumeResultKind is PackageVolumeResultKind.Operating ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}

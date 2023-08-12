using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.UWPApp;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;
using WinRT;

namespace GetStoreApp.UI.Pages
{
    /// <summary>
    /// 应用信息列表页面
    /// </summary>
    public sealed partial class AppListPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized = false;

        private PackageManager PackageManager { get; } = new PackageManager();

        public string SearchText { get; set; } = string.Empty;

        private bool _isLoadedCompleted = false;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                _isLoadedCompleted = value;
                OnPropertyChanged();
            }
        }

        private bool _isPackageEmpty = true;

        public bool IsPackageEmpty
        {
            get { return _isPackageEmpty; }

            set
            {
                _isPackageEmpty = value;
                OnPropertyChanged();
            }
        }

        private bool _isPackageEmptyWithCondition = true;

        public bool IsPackageEmptyWithCondition
        {
            get { return _isPackageEmptyWithCondition; }

            set
            {
                _isPackageEmptyWithCondition = value;
                OnPropertyChanged();
            }
        }

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                _isIncrease = value;
                OnPropertyChanged();
            }
        }

        private bool _isFramework = false;

        public bool IsFramework
        {
            get { return _isFramework; }

            set
            {
                _isFramework = value;
                OnPropertyChanged();
            }
        }

        private bool _isStorePackage = false;

        public bool IsStorePackage
        {
            get { return _isStorePackage; }

            set
            {
                _isStorePackage = value;
                OnPropertyChanged();
            }
        }

        private bool _isSideLoadedPackage = false;

        public bool IsSideLoadedPackage
        {
            get { return _isSideLoadedPackage; }

            set
            {
                _isSideLoadedPackage = value;
                OnPropertyChanged();
            }
        }

        private AppListRuleSeletedType _selectedType = AppListRuleSeletedType.PackageName;

        public AppListRuleSeletedType SelectedType
        {
            get { return _selectedType; }

            set
            {
                _selectedType = value;
                OnPropertyChanged();
            }
        }

        // 查看应用信息
        public XamlUICommand ViewInformationCommand { get; } = new XamlUICommand();

        // 打开应用
        public XamlUICommand OpenAppCommand { get; } = new XamlUICommand();

        // 打开商店
        public XamlUICommand OpenStoreCommand { get; } = new XamlUICommand();

        // 获取应用安装包
        public XamlUICommand GetPackageCommand { get; } = new XamlUICommand();

        // 打开应用清单文件
        public XamlUICommand OpenManifestCommand { get; } = new XamlUICommand();

        // 打开应用安装目录
        public XamlUICommand OpenInstalledFolderCommand { get; } = new XamlUICommand();

        // 卸载应用
        public XamlUICommand UnInstallCommand { get; } = new XamlUICommand();

        private List<Package> MatchResultList;

        public ObservableCollection<PackageModel> UWPAppDataList { get; } = new ObservableCollection<PackageModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AppListPage()
        {
            InitializeComponent();

            ViewInformationCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter.As<Package>();
                UWPAppPage uwpAppPage = NavigationService.NavigationFrame.Content.As<UWPAppPage>();
                if (package is not null && uwpAppPage is not null)
                {
                    uwpAppPage.ShowAppInformation(package);
                }
            };

            OpenAppCommand.ExecuteRequested += async (sender, args) =>
            {
                Package package = args.Parameter.As<Package>();

                if (package is not null)
                {
                    try
                    {
                        await package.GetAppListEntries()[0].LaunchAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.ERROR, string.Format("Open app {0} failed", package.DisplayName), e);
                    }
                }
            };

            OpenStoreCommand.ExecuteRequested += async (sender, args) =>
            {
                string packageFamilyName = args.Parameter as string;
                if (packageFamilyName is not null)
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={packageFamilyName}"));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.ERROR, string.Format("Open microsoft store {0} failed", packageFamilyName), e);
                    }
                }
            };

            GetPackageCommand.ExecuteRequested += (sender, args) =>
            {
                string packageFamilyName = args.Parameter as string;
                if (packageFamilyName is not null)
                {
                    NavigationService.NavigateTo(typeof(StorePage), new object[] { AppNaviagtionArgs.Store, "PackageFamilyName", "Retail", packageFamilyName });
                }
            };

            OpenManifestCommand.ExecuteRequested += async (sender, args) =>
            {
                Package package = args.Parameter.As<Package>();
                if (package is not null)
                {
                    try
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(Path.Combine(package.InstalledPath, "AppxManifest.xml"));
                        if (file is not null)
                        {
                            await Launcher.LaunchFileAsync(file);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.ERROR, string.Format("{0}'s AppxManifest.xml file open failed", package.DisplayName), e);
                    }
                }
            };

            OpenInstalledFolderCommand.ExecuteRequested += async (sender, args) =>
            {
                StorageFolder folder = args.Parameter.As<StorageFolder>();
                if (folder is not null)
                {
                    await Launcher.LaunchFolderAsync(folder);
                }
            };

            UnInstallCommand.ExecuteRequested += async (sender, args) =>
            {
                string packageFullName = args.Parameter as string;
                if (packageFullName is not null)
                {
                    await PackageManager.RemovePackageAsync(packageFullName);
                }
            };
        }

        /// <summary>
        /// 本地化应用管理记录数量统计信息
        /// </summary>
        public string LocalizeUWPAppCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("UWPApp/PackageEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("UWPApp/PackageCountInfo"), count);
            }
        }

        public bool IsItemChecked(AppListRuleSeletedType seletedType, AppListRuleSeletedType comparedType)
        {
            return seletedType == comparedType;
        }

        /// <summary>
        /// 初始化已安装应用信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                await Task.Delay(500);
                await GetInstalledAppsAsync();
                await InitializeDataAsync();
                IsPackageEmpty = MatchResultList.Count is 0;
                IsPackageEmptyWithCondition = UWPAppDataList.Count is 0;
                IsLoadedCompleted = true;
                isInitialized = true;
            }
        }

        /// <summary>
        /// 显示排序规则
        /// </summary>
        public void OnSortClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender.As<FrameworkElement>());
        }

        /// <summary>
        /// 显示过滤规则
        /// </summary>
        public void OnFilterClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender.As<FrameworkElement>());
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsLoadedCompleted = false;
            SearchText = string.Empty;
            await Task.Delay(500);
            await GetInstalledAppsAsync();
            await InitializeDataAsync();
            IsPackageEmpty = MatchResultList.Count is 0;
            IsPackageEmptyWithCondition = UWPAppDataList.Count is 0;
            IsLoadedCompleted = true;
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 加载系统已安装的应用信息
        /// </summary>
        private async Task GetInstalledAppsAsync()
        {
            await Task.Run(() =>
            {
                MatchResultList = PackageManager.FindPackagesForUser(string.Empty).ToList();
            });
        }

        private async Task InitializeDataAsync(bool hasSearchText = false)
        {
            UWPAppDataList.Clear();
            if (MatchResultList is not null)
            {
                if (hasSearchText)
                {
                    await Task.Run(() =>
                    {
                        List<Package> matchWithConditionList = MatchResultList.Where(matchItem =>
                            matchItem.DisplayName.Contains(SearchText) ||
                            matchItem.Description.Contains(SearchText) ||
                            matchItem.PublisherDisplayName.Contains(SearchText)).ToList();

                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            foreach (Package matchwithConditionItem in matchWithConditionList)
                            {
                                try
                                {
                                    if (File.Exists(matchwithConditionItem.Logo.OriginalString))
                                    {
                                        UWPAppDataList.Add(new PackageModel()
                                        {
                                            IsUnInstalling = false,
                                            Package = matchwithConditionItem
                                        });
                                    }
                                    await Task.Delay(1);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        });
                    });
                }
                else
                {
                    foreach (Package matchItem in MatchResultList.OrderBy(item => item.PublisherDisplayName).ToList())
                    {
                        try
                        {
                            if (File.Exists(matchItem.Logo.OriginalString))
                            {
                                UWPAppDataList.Add(new PackageModel()
                                {
                                    IsUnInstalling = false,
                                    Package = matchItem
                                });
                            }
                            await Task.Delay(1);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }
}

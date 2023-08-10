using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.PackageManager;
using GetStoreApp.Services.Root;
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

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理页面
    /// </summary>
    public sealed partial class PackageManagerPage : Page, INotifyPropertyChanged
    {
        private bool isInitialized = false;

        private PackageManager PackageManager { get; } = new PackageManager();

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

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private List<Package> MatchResultList;

        // 打开应用
        public XamlUICommand OpenAppCommand { get; } = new XamlUICommand();

        public XamlUICommand OpenStoreCommand { get; } = new XamlUICommand();

        public XamlUICommand OpenManifestCommand { get; } = new XamlUICommand();

        public XamlUICommand OpenInstalledFolderCommand { get; } = new XamlUICommand();

        public XamlUICommand UnInstallCommand { get; } = new XamlUICommand();

        public ObservableCollection<PackageModel> PackageManagerDataList { get; } = new ObservableCollection<PackageModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public PackageManagerPage()
        {
            InitializeComponent();

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

            PropertyChanged += OnPackageManagerPropertyChanged;
        }

        /// <summary>
        /// 本地化应用管理记录数量统计信息
        /// </summary>
        public string LocalizePackageManagerCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("PackageManager/PackageEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("PackageManager/PackageCountInfo"), count);
            }
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
                IsPackageEmptyWithCondition = PackageManagerDataList.Count is 0;
                IsLoadedCompleted = true;
                isInitialized = true;
            }
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        public void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (IsLoadedCompleted)
            {
            }
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        public async void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
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
            IsPackageEmptyWithCondition = PackageManagerDataList.Count is 0;
            IsLoadedCompleted = true;
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        public void OnPackageManagerPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SearchText))
            {
                //if (SearchText == string.Empty && MatchResultList is not null)
                //{
                //    InitializeDataAsync();
                //}
            }
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
            PackageManagerDataList.Clear();
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
                                        PackageManagerDataList.Add(new PackageModel()
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
                                PackageManagerDataList.Add(new PackageModel()
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

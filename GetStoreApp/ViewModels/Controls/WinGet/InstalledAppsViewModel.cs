using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：已安装应用控件视图模型
    /// </summary>
    public sealed class InstalledAppsViewModel : ViewModelBase
    {
        private bool _isInitialized = false;

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

        private List<MatchResult> SearchResultList;

        public ObservableCollection<InstalledAppsModel> InstalledAppsDataList { get; set; } = new ObservableCollection<InstalledAppsModel>();

        // 更新已安装应用数据
        public IRelayCommand RefreshCommand => new RelayCommand(async () =>
        {
            InstalledAppsDataList.Clear();
            IsLoadedCompleted = false;
            await Task.Delay(500);
            await GetInstalledAppsAsync();
            IsLoadedCompleted = true;
        });

        // 卸载应用
        public IRelayCommand UnInstallCommand => new RelayCommand<string>((id) =>
        {
        });

        /// <summary>
        /// 初始化已安装应用信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!_isInitialized)
            {
                InstalledAppsDataList.Clear();
                await Task.Delay(500);
                await GetInstalledAppsAsync();
                IsLoadedCompleted = true;
                _isInitialized = true;
            }
        }

        /// <summary>
        /// 加载系统已安装的应用信息
        /// </summary>
        private async Task GetInstalledAppsAsync()
        {
            try
            {
                PackageManager packageManager = WinGetService.CreatePackageManager();
                PackageCatalogReference searchCatalogReference = packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);

                ConnectResult connectResult = await searchCatalogReference.ConnectAsync();
                PackageCatalog installedCatalog = connectResult.PackageCatalog;

                if (installedCatalog is not null)
                {
                    FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                    FindPackagesResult findResult = await installedCatalog.FindPackagesAsync(findPackagesOptions);

                    SearchResultList = findResult.Matches.ToList();

                    foreach (MatchResult matchItem in SearchResultList)
                    {
                        InstalledAppsDataList.Add(new InstalledAppsModel()
                        {
                            AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                            AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                            AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                            AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                            IsUnInstalling = false,
                        });
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

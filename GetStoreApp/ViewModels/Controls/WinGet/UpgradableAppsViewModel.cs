using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.ViewModels.Base;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.WinGet
{
    public sealed class UpgradableAppsViewModel : ViewModelBase
    {
        private PackageManager UpgradableAppsManager { get; set; }

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

        private bool _isUpgradableAppsEmpty;

        public bool IsUpgradableAppsEmpty
        {
            get { return _isUpgradableAppsEmpty; }

            set
            {
                _isUpgradableAppsEmpty = value;
                OnPropertyChanged();
            }
        }

        private List<MatchResult> MatchResultList;

        public ObservableCollection<UpgradableAppsModel> UpgradableAppsDataList { get; } = new ObservableCollection<UpgradableAppsModel>();

        // 应用升级
        public XamlUICommand UpdateCommand { get; } = new XamlUICommand();

        /// <summary>
        /// 更新可升级应用数据
        /// </summary>
        public async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsLoadedCompleted = false;
            await Task.Delay(500);
            await GetUpgradableAppsAsync();
            InitializeData();
            IsUpgradableAppsEmpty = MatchResultList.Count is 0;
            IsLoadedCompleted = true;
        }

        public UpgradableAppsViewModel()
        {
            UpdateCommand.ExecuteRequested += (sender, args) =>
            {
            };
        }

        /// <summary>
        /// 初始化可升级应用信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                UpgradableAppsManager = WinGetService.CreatePackageManager();
            }
            catch (Exception)
            {
                return;
            }
            await Task.Delay(500);
            await GetUpgradableAppsAsync();
            InitializeData();
            IsUpgradableAppsEmpty = MatchResultList.Count is 0;
            IsLoadedCompleted = true;
        }

        /// <summary>
        /// 加载系统可升级的应用信息
        /// </summary>
        private async Task GetUpgradableAppsAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    List<PackageCatalogReference> packageCatalogReferences = UpgradableAppsManager.GetPackageCatalogs().ToList();
                    CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetService.CreateCreateCompositePackageCatalogOptions();
                    PackageCatalogReference searchCatalogReference = UpgradableAppsManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
                    foreach (PackageCatalogReference catalogReference in packageCatalogReferences)
                    {
                        createCompositePackageCatalogOptions.Catalogs.Add(catalogReference);
                    }
                    createCompositePackageCatalogOptions.CompositeSearchBehavior = CompositeSearchBehavior.LocalCatalogs;
                    PackageCatalogReference packageCatalogReference = UpgradableAppsManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
                    ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                    PackageCatalog upgradableCatalog = connectResult.PackageCatalog;

                    if (upgradableCatalog is not null)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        FindPackagesResult findResult = await upgradableCatalog.FindPackagesAsync(findPackagesOptions);
                        var result = findResult.Matches.ToList();

                        MatchResultList = findResult.Matches.ToList().Where(item => item.CatalogPackage.IsUpdateAvailable == true).ToList();
                    }
                });
            }
            catch (Exception) { }
        }

        private void InitializeData()
        {
            UpgradableAppsDataList.Clear();
            if (MatchResultList is not null)
            {
                foreach (MatchResult matchItem in MatchResultList)
                {
                    UpgradableAppsDataList.Add(new UpgradableAppsModel()
                    {
                        AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                        AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                        AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                        AppCurrentVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                        AppNewestVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                    });
                }
            }
        }
    }
}

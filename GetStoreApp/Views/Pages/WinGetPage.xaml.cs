using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page
    {
        public WinGetPage()
        {
            InitializeComponent();
            ViewModel.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ViewModel.SelectedIndex))
            {
                switch (ViewModel.SelectedIndex)
                {
                    // 搜索应用页面
                    case 0:
                        {
                            break;
                        }
                    // 已安装应用页面
                    case 1:
                        {
                            InstalledApps.ViewModel.Equals(ViewModel);
                            break;
                        }
                    // 可升级应用页面
                    case 2:
                        {
                            break;
                        }
                }
            }
        }

        private unsafe void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
        {
            switch (Convert.ToString((sender as Button).Tag))
            {
                case "PackageManager":
                    {
                        PackageManager packageManager = WinGetService.CreatePackageManager();
                        IReadOnlyList<PackageCatalogReference> catalogs = packageManager.GetPackageCatalogs().ToList();
                        Debug.WriteLine(catalogs.Count);
                        foreach (PackageCatalogReference item in catalogs)
                        {
                            Debug.WriteLine("===========================");
                            Debug.WriteLine("Name:" + item.Info.Name);
                            Debug.WriteLine("Argument:" + item.Info.Argument);
                            Debug.WriteLine("LastUpdateTime" + item.Info.LastUpdateTime);
                            Debug.WriteLine("Type:" + item.Info.Type);
                        }
                        break;
                    }
                case "InstallOptions":
                    {
                        GetInstalledPackagesAsync().Wait();
                        break;

                    }
                case "UnInstallOptions":
                    {
                        UninstallOptions uninstallOptions = WinGetService.CreateUninstallOptions();
                        break;
                    }
                case "FindPackagesOptions":
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        break;
                    }
                case "PackageMatchFilter":
                    {
                        PackageMatchFilter packageMatchFilter = WinGetService.CreatePacakgeMatchFilter();
                        break;
                    }
                case "CreateCompositePackageCatalogOptions":
                    {
                        CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetService.CreateCreateCompositePackageCatalogOptions();
                        break;
                    }
            }
        }

        private async Task GetInstalledPackagesAsync()
        {
            PackageManager packageManager = WinGetService.CreatePackageManager();
            PackageCatalogReference installedSearchCatalogReference = packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);

            ConnectResult connectResult = await installedSearchCatalogReference.ConnectAsync();
            PackageCatalog installedCatalog = connectResult.PackageCatalog;

            if(installedCatalog is not null)
            {
                FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                FindPackagesResult findResult = await installedCatalog.FindPackagesAsync(findPackagesOptions);

                List<MatchResult> matchResults = findResult.Matches.ToList();

                foreach (var item in matchResults)
                {
                    string result = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n",item.CatalogPackage.Name,item.CatalogPackage.Id,item.CatalogPackage.InstalledVersion.Version,item.CatalogPackage.InstalledVersion.Id,item.CatalogPackage.InstalledVersion.Channel,item.CatalogPackage.InstalledVersion.Publisher,item.CatalogPackage.InstalledVersion.DisplayName);
                    System.IO.File.AppendAllText(@"D:\0014.txt", result);
                }
            }
        }
    }
}

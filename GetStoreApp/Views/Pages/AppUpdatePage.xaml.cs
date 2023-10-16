using GetStoreApp.Models.Controls.AppUpdate;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using Windows.Management.Deployment;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 商店应用更新页面
    /// </summary>
    public sealed partial class AppUpdatePage : Page, INotifyPropertyChanged
    {
        private static readonly object AppUpdateDataListLock = new object();

        private AppInstallManager AppInstallManager { get; } = new AppInstallManager();

        private PackageManager PackageManager { get; } = new PackageManager();

        private Dictionary<string, AppInstallItem> AppInstallingDict = new Dictionary<string, AppInstallItem>();

        private bool _isLoadedCompleted = true;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                _isLoadedCompleted = value;
                OnPropertyChanged();
            }
        }

        private bool _isUpdateEmpty = true;

        public bool IsUpdateEmpty
        {
            get { return _isUpdateEmpty; }

            set
            {
                _isUpdateEmpty = value;
                OnPropertyChanged();
            }
        }

        public List<Package> PackageDataList { get; set; }

        public ObservableCollection<AppUpdateModel> AppUpdateDataList { get; } = new ObservableCollection<AppUpdateModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AppUpdatePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 更新选定的应用
        /// </summary>
        public void OnUpdateExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string packageFamilyName = args.Parameter as string;

            if (packageFamilyName is not null)
            {
                Task.Run(async () =>
                {
                    AppInstallItem appInstallItem = null;
                    foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
                    {
                        if (appUpdateItem.PackageFamilyName == packageFamilyName)
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                lock (AppUpdateDataList)
                                {
                                    appUpdateItem.IsUpdating = true;
                                }
                            });

                            appInstallItem = await AppInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);
                            break;
                        }
                    }

                    if (appInstallItem is not null)
                    {
                        appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;
                        appInstallItem.Completed += OnAppInstallItemCompleted;

                        AppInstallingDict.TryAdd(appInstallItem.PackageFamilyName, appInstallItem);
                    }
                });
            }
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        public void OnCancelExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string packageFamilyName = args.Parameter as string;

            if (packageFamilyName is not null)
            {
                Task.Run(() =>
                {
                    if (AppInstallingDict.TryGetValue(packageFamilyName, out AppInstallItem appInstallItem))
                    {
                        appInstallItem.Completed -= OnAppInstallItemCompleted;
                        appInstallItem.StatusChanged -= OnAppInstallItemStatausChanged;
                        appInstallItem.Cancel();

                        AppInstallingDict.Remove(packageFamilyName);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (AppUpdateDataListLock)
                            {
                                foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
                                {
                                    if (appUpdateItem.PackageFamilyName == packageFamilyName)
                                    {
                                        appUpdateItem.IsUpdating = false;
                                        break;
                                    }
                                }
                            }
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 检查商店应用更新
        /// </summary>
        public void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            IsLoadedCompleted = false;
            Task.Run(async () =>
            {
                AppUpdateOptions updateOptions = new AppUpdateOptions();
                updateOptions.AutomaticallyDownloadAndInstallUpdateIfFound = false;
                updateOptions.AllowForcedAppRestart = false;
                IReadOnlyList<AppInstallItem> upgradableAppsList = await AppInstallManager.SearchForAllUpdatesAsync(string.Empty, string.Empty, updateOptions);
                List<AppUpdateModel> appUpdateList = new List<AppUpdateModel>();
                PackageDataList = PackageManager.FindPackagesForUser(string.Empty).ToList();

                if (PackageDataList is not null)
                {
                    foreach (AppInstallItem upgradableApps in upgradableAppsList)
                    {
                        Package package = PackageDataList.FirstOrDefault(item => item.Id.FamilyName.Equals(upgradableApps.PackageFamilyName, StringComparison.OrdinalIgnoreCase));
                        if (package is not null)
                        {
                            appUpdateList.Add(new AppUpdateModel()
                            {
                                DisplayName = package.DisplayName,
                                PublisherName = package.PublisherDisplayName,
                                InstallState = string.Empty,
                                IsUpdating = false,
                                PackageFamilyName = upgradableApps.PackageFamilyName,
                                ProductId = upgradableApps.ProductId
                            });
                        }
                        else
                        {
                            continue;
                        }
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (AppUpdateDataListLock)
                        {
                            AppUpdateDataList.Clear();
                            foreach (AppUpdateModel appUpdateItem in appUpdateList)
                            {
                                AppUpdateDataList.Add(appUpdateItem);
                            }
                        }

                        IsLoadedCompleted = true;
                    });
                }
            });
        }

        /// <summary>
        /// 更新所有应用
        /// </summary>
        public void OnUpdateAllClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                foreach (AppUpdateModel appUpdateItem in AppUpdateDataList)
                {
                    AppInstallItem appInstallItem = await AppInstallManager.UpdateAppByPackageFamilyNameAsync(appUpdateItem.PackageFamilyName);

                    if (appInstallItem != null && !AppInstallingDict.ContainsKey(appInstallItem.PackageFamilyName))
                    {
                        appInstallItem.Completed += OnAppInstallItemCompleted;
                        appInstallItem.StatusChanged += OnAppInstallItemStatausChanged;

                        AppInstallingDict.Add(appInstallItem.PackageFamilyName, appInstallItem);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (AppUpdateDataListLock)
                            {
                                appUpdateItem.IsUpdating = true;
                            }
                        });
                    }
                }
            });
        }

        /// <summary>
        /// 当前应用安装完成时发生的事件
        /// </summary>
        public void OnAppInstallItemCompleted(AppInstallItem sender, object args)
        {
        }

        /// <summary>
        /// 当前应用的安装状态发生更改时的事件
        /// </summary>
        public void OnAppInstallItemStatausChanged(AppInstallItem sender, object args)
        {
        }

        /// <summary>
        /// 本地化应用更新数量统计信息
        /// </summary>
        public string LocalizeAppUpdateCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("AppUpdate/AppUpdateEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("AppUpdate/AppUpdateCountInfo"), count);
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.UI.Text;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 应用版本信息页面
    /// </summary>
    internal sealed partial class WinGetAppsVersionInfoPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AuthorString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Author");
        private readonly string CopyRightLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/CopyRightLink");
        private readonly string CopyRightString = ResourceService.GetLocalized("WinGetAppsVersionInfo/CopyRight");
        private readonly string DescriptionString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Description");
        private readonly string DisplayNameString = ResourceService.GetLocalized("WinGetAppsVersionInfo/DisplayName");
        private readonly string LicenseLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/LicenseLink");
        private readonly string LicenseString = ResourceService.GetLocalized("WinGetAppsVersionInfo/License");
        private readonly string LocaleString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Locale");
        private readonly string NoSpecificLocaleString = ResourceService.GetLocalized("WinGetAppsVersionInfo/NoSpecificLocale");
        private readonly string NotAvailableString = ResourceService.GetLocalized("WinGetAppsVersionInfo/NotAvailable");
        private readonly string PackageLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PackageLink");
        private readonly string PrivacyLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PrivacyLink");
        private readonly string PublisherLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PublisherLink");
        private readonly string PublisherString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Publisher");
        private readonly string PublisherSupportLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PublisherSupportLink");
        private readonly string PurchaseLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PurchaseLink");
        private readonly string ReleaseNotesLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/ReleaseNotesLink");
        private readonly string ReleaseNotesString = ResourceService.GetLocalized("WinGetAppsVersionInfo/ReleaseNotes");
        private readonly string VersionString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Version");
        private readonly string WinGetAppsVersionCountInfoString = ResourceService.GetLocalized("WinGetAppsVersionInfo/WinGetAppsVersionCountInfo");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private WinGetPage WinGetPage { get; set; }

        private WinGetAppsVersionDialog WinGetAppsVersionDialog { get; set; }

        private SearchAppsModel SearchApps { get; set; }

        private UpgradableAppsModel UpgradableApps { get; set; }

        private bool _isLoadCompleted;

        private bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsLoadCompleted)));
                }
            }
        }

        private AvailableVersionModel _selectedItem;

        private AvailableVersionModel SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
                }
            }
        }

        private string _displayName;

        private string DisplayName
        {
            get { return _displayName; }

            set
            {
                if (!string.Equals(_displayName, value))
                {
                    _displayName = value;
                    PropertyChanged?.Invoke(this, new(nameof(DisplayName)));
                }
            }
        }

        private string _description;

        private string Description
        {
            get { return _description; }

            set
            {
                if (!string.Equals(_description, value))
                {
                    _description = value;
                    PropertyChanged?.Invoke(this, new(nameof(Description)));
                }
            }
        }

        private ObservableCollection<AvailableVersionModel> WinGetAppsVersionCollection { get; } = [];

        private ObservableCollection<WinGetAppInfoModel> WinGetAppInfoCollection { get; } = [];

        private ObservableCollection<string> TagCollection { get; } = [];

        private ObservableCollection<ContentLinkInfo> DocumentationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal WinGetAppsVersionInfoPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is ReadOnlyCollection<object> argsCollection && argsCollection.Count is 3 && WinGetPage is null && argsCollection[0] is WinGetPage winGetPage && argsCollection[1] is WinGetAppsVersionDialog winGetAppsVersionDialog)
            {
                WinGetPage = winGetPage;
                WinGetAppsVersionDialog = winGetAppsVersionDialog;
                await InitializeDataAsync(argsCollection[2]);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 选中项发生变化后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ListView))]
        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ListView listView && !Equals(SelectedItem, listView.SelectedItem))
            {
                SelectedItem = listView.SelectedItem is AvailableVersionModel availableVersion ? availableVersion : null;

                if (SelectedItem is not null)
                {
                    await InitializeVersionInformationAsync(SelectedItem);
                }
            }
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>

        private async void OnCopyInformationClicked(object sender, RoutedEventArgs args)
        {
            string copyInformation = await GetCopyInformationStringAsync(DisplayName, Description, WinGetAppInfoCollection.AsReadOnly());
            if (!string.IsNullOrEmpty(copyInformation))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyInformation);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 下载当前版本应用
        /// </summary>
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            if (SearchApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                NavigateOptionsPage(PackageOperationKind.Download, SearchApps, null, SelectedItem.Version, SelectedItem.PackageVersionId);
            }
        }

        /// <summary>
        /// 安装当前版本应用
        /// </summary>
        private void OnInstallClicked(object sender, RoutedEventArgs args)
        {
            if (SearchApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                NavigateOptionsPage(PackageOperationKind.Install, SearchApps, null, SelectedItem.Version, SelectedItem.PackageVersionId);
            }
        }

        /// <summary>
        /// 修复当前版本应用
        /// </summary>
        private void OnRepairClicked(object sender, RoutedEventArgs args)
        {
            if (SearchApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                NavigateOptionsPage(PackageOperationKind.Repair, SearchApps, null, SelectedItem.Version, SelectedItem.PackageVersionId);
            }
        }

        /// <summary>
        /// 更新当前版本应用
        /// </summary>
        private void OnUpgradeClicked(object sender, RoutedEventArgs args)
        {
            if (UpgradableApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                NavigateOptionsPage(PackageOperationKind.Upgrade, null, UpgradableApps, SelectedItem.Version, SelectedItem.PackageVersionId);
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async Task InitializeDataAsync(object args)
        {
            // 搜索应用
            if (args is SearchAppsModel searchApps)
            {
                SearchApps = searchApps;
            }
            // 可更新应用
            else if (args is UpgradableAppsModel upgradableApps)
            {
                UpgradableApps = upgradableApps;
            }

            DisplayName = NotAvailableString;
            Description = NotAvailableString;

            if (!IsLoadCompleted)
            {
                if (SearchApps is not null)
                {
                    // 获取当前应用可用版本
                    if (await GetAvailableVersionAysnc(SearchApps.CatalogPackage, false) is ReadOnlyCollection<AvailableVersionModel> availableVersionCollection && availableVersionCollection.Count > 0)
                    {
                        await UpdateAvailableVersionListAsync(availableVersionCollection);
                    }
                }
                else if (UpgradableApps is not null)
                {
                    // 获取当前应用可用版本
                    if (await GetAvailableVersionAysnc(UpgradableApps.CatalogPackage, true) is ReadOnlyCollection<AvailableVersionModel> availableVersionCollection && availableVersionCollection.Count > 0)
                    {
                        await UpdateAvailableVersionListAsync(availableVersionCollection);
                    }
                }
            }

            IsLoadCompleted = true;
        }

        /// <summary>
        /// 获取应用可用版本
        /// </summary>
        private async Task<ReadOnlyCollection<AvailableVersionModel>> GetAvailableVersionAysnc(CatalogPackage catalogPackage, bool isUpgrade)
        {
            if (catalogPackage is null)
            {
                return default;
            }

            return await Task.Run(() =>
            {
                bool hasDefaultVersion = false;
                List<AvailableVersionModel> availableVersionList = [];

                for (int subIndex = 0; subIndex < catalogPackage.AvailableVersions.Count; subIndex++)
                {
                    PackageVersionId packageVersionId = catalogPackage.AvailableVersions[subIndex];

                    if (!string.IsNullOrEmpty(packageVersionId.Version))
                    {
                        // 获取大于已安装应用版本的所有版本
                        if (isUpgrade)
                        {
                            if (catalogPackage.InstalledVersion.CompareToVersion(packageVersionId.Version) is CompareResult.Lesser)
                            {
                                (bool isDefaultVersion, bool checkedHasDefaultVersion) = CheckDefaultVersion(catalogPackage.DefaultInstallVersion, packageVersionId);
                                if (checkedHasDefaultVersion && !hasDefaultVersion)
                                {
                                    hasDefaultVersion = true;
                                }

                                // 添加所有已经获取到的所有版本
                                availableVersionList.Add(new()
                                {
                                    IsDefaultVersion = isDefaultVersion,
                                    Version = packageVersionId.Version,
                                    PackageVersionId = packageVersionId
                                });
                            }
                        }
                        else
                        {
                            (bool isDefaultVersion, bool checkedHasDefaultVersion) = CheckDefaultVersion(catalogPackage.DefaultInstallVersion, packageVersionId);
                            if (checkedHasDefaultVersion && !hasDefaultVersion)
                            {
                                hasDefaultVersion = true;
                            }

                            // 添加所有已经获取到的所有版本
                            availableVersionList.Add(new()
                            {
                                IsDefaultVersion = isDefaultVersion,
                                Version = packageVersionId.Version,
                                PackageVersionId = packageVersionId
                            });
                        }
                    }
                }

                // 没有默认版本，把默认版本添加在第一项
                if (!hasDefaultVersion)
                {
                    availableVersionList.Insert(0, new()
                    {
                        IsDefaultVersion = true,
                        Version = catalogPackage.DefaultInstallVersion.Version,
                        PackageVersionId = null,
                        PackageVersionInfo = catalogPackage.DefaultInstallVersion
                    });
                }

                return availableVersionList.AsReadOnly();
            });
        }

        /// <summary>
        /// 初始化对应版本信息
        /// </summary>
        private async Task InitializeVersionInformationAsync(AvailableVersionModel availableVersion)
        {
            (PackageVersionInfo packageVersionInfo, CatalogPackageMetadata catalogPackageMetadata) = await Task.Run(() =>
            {
                PackageVersionInfo packageVersionInfo = null;
                CatalogPackageMetadata catalogPackageMetadata = null;

                if (SearchApps is not null)
                {
                    packageVersionInfo = availableVersion is not null && availableVersion.PackageVersionId is not null ? SearchApps.CatalogPackage.GetPackageVersionInfo(availableVersion.PackageVersionId) : availableVersion.PackageVersionInfo;

                    if (packageVersionInfo is not null)
                    {
                        catalogPackageMetadata = packageVersionInfo.GetCatalogPackageMetadata();
                    }
                }
                else if (UpgradableApps is not null)
                {
                    packageVersionInfo = availableVersion is not null && availableVersion.PackageVersionId is not null ? UpgradableApps.CatalogPackage.GetPackageVersionInfo(availableVersion.PackageVersionId) : availableVersion.PackageVersionInfo;

                    if (packageVersionInfo is not null)
                    {
                        catalogPackageMetadata = packageVersionInfo.GetCatalogPackageMetadata();
                    }
                }

                return ValueTuple.Create(packageVersionInfo, catalogPackageMetadata);
            });

            if (packageVersionInfo is not null && catalogPackageMetadata is not null)
            {
                DisplayName = string.IsNullOrEmpty(catalogPackageMetadata.PackageName) ? NotAvailableString : catalogPackageMetadata.PackageName;
                Description = string.IsNullOrEmpty(catalogPackageMetadata.Description) ? NotAvailableString : catalogPackageMetadata.Description;

                WinGetAppInfoCollection.Clear();
                if (!string.IsNullOrEmpty(packageVersionInfo.Version))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.String,
                        ClassName = VersionString,
                        StringContent = packageVersionInfo.Version
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.PackageUrl) && Uri.TryCreate(catalogPackageMetadata.PackageUrl, new(), out Uri packageLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = PackageLinkString,
                        UriContent = packageLinkUri
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.Author))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.String,
                        ClassName = AuthorString,
                        StringContent = catalogPackageMetadata.Author
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.Publisher))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.String,
                        ClassName = PublisherString,
                        StringContent = catalogPackageMetadata.Publisher
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.PublisherUrl) && Uri.TryCreate(catalogPackageMetadata.PublisherUrl, new(), out Uri publisherLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = PublisherLinkString,
                        UriContent = publisherLinkUri
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.PublisherSupportUrl) && Uri.TryCreate(catalogPackageMetadata.PublisherSupportUrl, new(), out Uri publisherSupportLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = PublisherSupportLinkString,
                        UriContent = publisherSupportLinkUri
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.Locale))
                {
                    try
                    {
                        WinGetAppInfoCollection.Add(new()
                        {
                            WinGetAppInfoKind = WinGetAppInfoKind.String,
                            ClassName = LocaleString,
                            StringContent = catalogPackageMetadata.Locale.Contains("Neutral", StringComparison.OrdinalIgnoreCase) ? NoSpecificLocaleString : new CultureInfo(catalogPackageMetadata.Locale).DisplayName
                        });
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.Copyright))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.String,
                        ClassName = CopyRightString,
                        StringContent = catalogPackageMetadata.Copyright
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.CopyrightUrl) && Uri.TryCreate(catalogPackageMetadata.CopyrightUrl, new(), out Uri copyRightLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = CopyRightLinkString,
                        UriContent = copyRightLinkUri
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.License))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.String,
                        ClassName = LicenseString,
                        StringContent = catalogPackageMetadata.License
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.LicenseUrl) && Uri.TryCreate(catalogPackageMetadata.LicenseUrl, new(), out Uri licenseLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = LicenseLinkString,
                        UriContent = licenseLinkUri
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.PrivacyUrl) && Uri.TryCreate(catalogPackageMetadata.PrivacyUrl, new(), out Uri privacyLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = PrivacyLinkString,
                        UriContent = privacyLinkUri
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.PurchaseUrl) && Uri.TryCreate(catalogPackageMetadata.PurchaseUrl, new(), out Uri purchaseLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = PurchaseLinkString,
                        UriContent = purchaseLinkUri
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.ReleaseNotes))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.String,
                        ClassName = ReleaseNotesString,
                        StringContent = catalogPackageMetadata.ReleaseNotes
                    });
                }
                if (!string.IsNullOrEmpty(catalogPackageMetadata.ReleaseNotesUrl) && Uri.TryCreate(catalogPackageMetadata.ReleaseNotesUrl, new(), out Uri releaseNotesLinkUri))
                {
                    WinGetAppInfoCollection.Add(new()
                    {
                        WinGetAppInfoKind = WinGetAppInfoKind.Uri,
                        ClassName = ReleaseNotesLinkString,
                        UriContent = releaseNotesLinkUri
                    });
                }

                TagCollection.Clear();
                foreach (string tag in catalogPackageMetadata.Tags)
                {
                    TagCollection.Add(tag);
                }

                DocumentationCollection.Clear();
                for (int index = 0; index < catalogPackageMetadata.Documentations.Count; index++)
                {
                    Documentation documentation = catalogPackageMetadata.Documentations[index];
                    if (!string.IsNullOrEmpty(documentation.DocumentUrl) && Uri.TryCreate(documentation.DocumentUrl, new(), out Uri documentUrlUri))
                    {
                        DocumentationCollection.Add(new() { DisplayText = documentation.DocumentLabel, Uri = documentUrlUri });
                    }
                }
            }
            else
            {
                DisplayName = NotAvailableString;
                Description = NotAvailableString;
                WinGetAppInfoCollection.Clear();
                TagCollection.Clear();
                DocumentationCollection.Clear();
            }
        }

        /// <summary>
        /// 检查默认版本信息
        /// </summary>
        private (bool, bool) CheckDefaultVersion(PackageVersionInfo packageVersionInfo, PackageVersionId packageVersionId)
        {
            bool isDefaultVersion = false;
            bool hasDefaultVersion = false;

            // 判断是否等同于默认版本
            if (packageVersionInfo.CompareToVersion(packageVersionId.Version) is CompareResult.Equal)
            {
                isDefaultVersion = true;
                if (!hasDefaultVersion)
                {
                    hasDefaultVersion = true;
                }
            }

            return ValueTuple.Create(isDefaultVersion, hasDefaultVersion);
        }

        /// <summary>
        /// 更新可用版本列表信息
        /// </summary>
        private async Task UpdateAvailableVersionListAsync(ReadOnlyCollection<AvailableVersionModel> availableVersionCollection)
        {
            if (availableVersionCollection is not null && availableVersionCollection.Count > 0)
            {
                foreach (AvailableVersionModel availableVersionItem in availableVersionCollection)
                {
                    WinGetAppsVersionCollection.Add(availableVersionItem);

                    if (availableVersionItem.IsDefaultVersion)
                    {
                        SelectedItem = availableVersionItem;
                        await InitializeVersionInformationAsync(availableVersionItem);
                    }
                }
            }
        }

        /// <summary>
        /// 获取复制信息
        /// </summary>
        private async Task<string> GetCopyInformationStringAsync(string displayName, string description, ReadOnlyCollection<WinGetAppInfoModel> winGetAppInfoCollection)
        {
            return await Task.Run(() =>
            {
                try
                {
                    List<string> copyInformationList =
                    [
                        string.Format("{0}\t{1}", DisplayNameString, string.IsNullOrEmpty(displayName) ? NotAvailableString : displayName),
                        string.Format("{0}\t{1}", DescriptionString, string.IsNullOrEmpty(description) ? NotAvailableString : description),
                    ];
                    foreach (WinGetAppInfoModel winGetAppInfo in winGetAppInfoCollection)
                    {
                        if (winGetAppInfo.WinGetAppInfoKind is WinGetAppInfoKind.String)
                        {
                            copyInformationList.Add(string.Format("{0}\t{1}", winGetAppInfo.ClassName, string.IsNullOrEmpty(winGetAppInfo.StringContent) ? NotAvailableString : winGetAppInfo.StringContent));
                        }
                        else if (winGetAppInfo.WinGetAppInfoKind is WinGetAppInfoKind.Uri)
                        {
                            copyInformationList.Add(string.Format("{0}\t{1}", winGetAppInfo.ClassName, string.IsNullOrEmpty(winGetAppInfo.UriContent.AbsoluteUri) ? NotAvailableString : winGetAppInfo.UriContent.AbsoluteUri));
                        }
                    }
                    return string.Join(Environment.NewLine, copyInformationList);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetAppsVersionInfoPage), nameof(GetCopyInformationStringAsync), 1, e);
                    return default;
                }
            });
        }

        /// <summary>
        /// 导航到选项页面
        /// </summary>
        private void NavigateOptionsPage(PackageOperationKind packageOperationKind, SearchAppsModel searchApps, UpgradableAppsModel upgradableApps, string version, PackageVersionId packageVersionId)
        {
            switch (packageOperationKind)
            {
                case PackageOperationKind.Download:
                    {
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageCollection[1], new ReadOnlyCollection<object>([WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                        {
                            PackageOperationKind = packageOperationKind,
                            AppID = searchApps.AppID,
                            AppName = searchApps.AppName,
                            AppVersion = version,
                            PackagePath = WinGetConfigService.DefaultDownloadFolder,
                            PackageOperationProgress = 0,
                            PackageDownloadProgressState = PackageDownloadProgressState.Queued,
                            PackageVersionId = packageVersionId,
                            DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            PackageDownloadProgress = null,
                            SearchApps = searchApps,
                         }]), true);
                        break;
                    }
                case PackageOperationKind.Install:
                    {
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageCollection[1], new ReadOnlyCollection<object>([WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                        {
                            PackageOperationKind = packageOperationKind,
                            AppID = searchApps.AppID,
                            AppName = searchApps.AppName,
                            AppVersion = version,
                            PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                            PackageOperationProgress = 0,
                            PackageInstallProgressState = PackageInstallProgressState.Queued,
                            PackageVersionId = packageVersionId,
                            DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            PackageInstallProgress = null,
                            SearchApps = searchApps,
                        }]), true);
                        break;
                    }

                case PackageOperationKind.Repair:
                    {
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageCollection[1], new ReadOnlyCollection<object>([WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                        {
                            PackageOperationKind = packageOperationKind,
                            AppID = searchApps.AppID,
                            AppName = searchApps.AppName,
                            AppVersion = version,
                            PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                            PackageOperationProgress = 0,
                            PackageRepairProgressState = PackageRepairProgressState.Queued,
                            PackageVersionId = packageVersionId,
                            DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            PackageRepairProgress = null,
                            SearchApps = searchApps,
                        }]), true);
                        break;
                    }
                case PackageOperationKind.Upgrade:
                    {
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageCollection[1], new ReadOnlyCollection<object>([WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                        {
                            PackageOperationKind = packageOperationKind,
                            AppID = upgradableApps.AppID,
                            AppName = upgradableApps.AppName,
                            AppVersion = version,
                            PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                            PackageOperationProgress = 0,
                            PackageInstallProgressState = PackageInstallProgressState.Queued,
                            PackageVersionId = packageVersionId,
                            DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                            PackageInstallProgress = null,
                            UpgradableApps = upgradableApps,
                        }]), true);
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取显示的应用类型
        /// </summary>
        private Visibility GetWinGetAppsVisibility(object winGetApps)
        {
            return winGetApps is null ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}

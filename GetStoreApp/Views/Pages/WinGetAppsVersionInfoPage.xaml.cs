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

        private string _version;

        private string Version
        {
            get { return _version; }

            set
            {
                if (!string.Equals(_version, value))
                {
                    _version = value;
                    PropertyChanged?.Invoke(this, new(nameof(Version)));
                }
            }
        }

        private bool _isPackageLinkExisted;

        private bool IsPackageLinkExisted
        {
            get { return _isPackageLinkExisted; }

            set
            {
                if (!Equals(_isPackageLinkExisted, value))
                {
                    _isPackageLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPackageLinkExisted)));
                }
            }
        }

        private Uri _packageLink;

        private Uri PackageLink
        {
            get { return _packageLink; }

            set
            {
                if (!string.Equals(_packageLink, value))
                {
                    _packageLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageLink)));
                }
            }
        }

        private string _author;

        private string Author
        {
            get { return _author; }

            set
            {
                if (!string.Equals(_author, value))
                {
                    _author = value;
                    PropertyChanged?.Invoke(this, new(nameof(Author)));
                }
            }
        }

        private string _publisher;

        private string Publisher
        {
            get { return _publisher; }

            set
            {
                if (!string.Equals(_publisher, value))
                {
                    _publisher = value;
                    PropertyChanged?.Invoke(this, new(nameof(Publisher)));
                }
            }
        }

        private bool _isPublisherLinkExisted;

        private bool IsPublisherLinkExisted
        {
            get { return _isPublisherLinkExisted; }

            set
            {
                if (!Equals(_isPublisherLinkExisted, value))
                {
                    _isPublisherLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPublisherLinkExisted)));
                }
            }
        }

        private Uri _publisherLink;

        private Uri PublisherLink
        {
            get { return _publisherLink; }

            set
            {
                if (!Equals(_publisherLink, value))
                {
                    _publisherLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(PublisherLink)));
                }
            }
        }

        private bool _isPublisherSupportLinkExisted;

        private bool IsPublisherSupportLinkExisted
        {
            get { return _isPublisherSupportLinkExisted; }

            set
            {
                if (!Equals(_isPublisherSupportLinkExisted, value))
                {
                    _isPublisherSupportLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPublisherSupportLinkExisted)));
                }
            }
        }

        private Uri _publisherSupportLink;

        private Uri PublisherSupportLink
        {
            get { return _publisherSupportLink; }

            set
            {
                if (!Equals(_publisherSupportLink, value))
                {
                    _publisherSupportLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(PublisherSupportLink)));
                }
            }
        }

        private string _locale;

        private string Locale
        {
            get { return _locale; }

            set
            {
                if (!string.Equals(_locale, value))
                {
                    _locale = value;
                    PropertyChanged?.Invoke(this, new(nameof(Locale)));
                }
            }
        }

        private string _copyRight;

        private string CopyRight
        {
            get { return _copyRight; }

            set
            {
                if (!string.Equals(_copyRight, value))
                {
                    _copyRight = value;
                    PropertyChanged?.Invoke(this, new(nameof(CopyRight)));
                }
            }
        }

        private bool _isCopyRightLinkExisted;

        private bool IsCopyRightLinkExisted
        {
            get { return _isCopyRightLinkExisted; }

            set
            {
                if (!Equals(_isCopyRightLinkExisted, value))
                {
                    _isCopyRightLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsCopyRightLinkExisted)));
                }
            }
        }

        private Uri _copyRightLink;

        private Uri CopyRightLink
        {
            get { return _copyRightLink; }

            set
            {
                if (!Equals(_copyRightLink, value))
                {
                    _copyRightLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(CopyRightLink)));
                }
            }
        }

        private string _license;

        private string License
        {
            get { return _license; }

            set
            {
                if (!string.Equals(_license, value))
                {
                    _license = value;
                    PropertyChanged?.Invoke(this, new(nameof(License)));
                }
            }
        }

        private bool _isLicenseLinkExisted;

        private bool IsLicenseLinkExisted
        {
            get { return _isLicenseLinkExisted; }

            set
            {
                if (!Equals(_isLicenseLinkExisted, value))
                {
                    _isLicenseLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsLicenseLinkExisted)));
                }
            }
        }

        private Uri _licenseLink;

        private Uri LicenseLink
        {
            get { return _licenseLink; }

            set
            {
                if (!Equals(_licenseLink, value))
                {
                    _licenseLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(LicenseLink)));
                }
            }
        }

        private bool _isPrivacyLinkExisted;

        private bool IsPrivacyLinkExisted
        {
            get { return _isPrivacyLinkExisted; }

            set
            {
                if (!Equals(_isPrivacyLinkExisted, value))
                {
                    _isPrivacyLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPrivacyLinkExisted)));
                }
            }
        }

        private Uri _privacyLink;

        private Uri PrivacyLink
        {
            get { return _privacyLink; }

            set
            {
                if (!Equals(_privacyLink, value))
                {
                    _privacyLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(PrivacyLink)));
                }
            }
        }

        private bool _isPurchaseLinkExisted;

        private bool IsPurchaseLinkExisted
        {
            get { return _isPurchaseLinkExisted; }

            set
            {
                if (!Equals(_isPurchaseLinkExisted, value))
                {
                    _isPurchaseLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsPurchaseLinkExisted)));
                }
            }
        }

        private Uri _purchaseLink;

        private Uri PurchaseLink
        {
            get { return _purchaseLink; }

            set
            {
                if (!Equals(_purchaseLink, value))
                {
                    _purchaseLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(PurchaseLink)));
                }
            }
        }

        private string _releaseNotes;

        private string ReleaseNotes
        {
            get { return _releaseNotes; }

            set
            {
                if (!string.Equals(_releaseNotes, value))
                {
                    _releaseNotes = value;
                    PropertyChanged?.Invoke(this, new(nameof(ReleaseNotes)));
                }
            }
        }

        private bool _isReleaseNotesLinkExisted;

        private bool IsReleaseNotesLinkExisted
        {
            get { return _isReleaseNotesLinkExisted; }

            set
            {
                if (!Equals(_isReleaseNotesLinkExisted, value))
                {
                    _isReleaseNotesLinkExisted = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsReleaseNotesLinkExisted)));
                }
            }
        }

        private Uri _releaseNotesLink;

        private Uri ReleaseNotesLink
        {
            get { return _releaseNotesLink; }

            set
            {
                if (!Equals(_releaseNotesLink, value))
                {
                    _releaseNotesLink = value;
                    PropertyChanged?.Invoke(this, new(nameof(ReleaseNotesLink)));
                }
            }
        }

        private ObservableCollection<AvailableVersionModel> WinGetAppsVersionCollection { get; } = [];

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

            if (args.Parameter is List<object> argsList && argsList.Count is 3 && WinGetPage is null && argsList[0] is WinGetPage winGetPage && argsList[1] is WinGetAppsVersionDialog winGetAppsVersionDialog)
            {
                WinGetPage = winGetPage;
                WinGetAppsVersionDialog = winGetAppsVersionDialog;
                await InitializeDataAsync(argsList[2]);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 选中项发生变化时触发的事件
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
            string copyInformation = await GetCopyInformationStringAsync(DisplayName, Description, Version, PackageLink, Author, Publisher, PublisherLink, PublisherSupportLink, Locale, CopyRight, CopyRightLink, License, LicenseLink, PrivacyLink, PurchaseLink, ReleaseNotes, ReleaseNotesLink);
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
                NavigateOptionsPage(PackageOperationKind.Download, null, UpgradableApps, SelectedItem.Version, SelectedItem.PackageVersionId);
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
            Version = NotAvailableString;
            PackageLink = null;
            IsPackageLinkExisted = false;
            Author = NotAvailableString;
            Publisher = NotAvailableString;
            PublisherLink = null;
            IsPublisherLinkExisted = false;
            PublisherSupportLink = null;
            IsPublisherLinkExisted = false;
            Locale = NotAvailableString;
            CopyRight = NotAvailableString;
            CopyRightLink = null;
            IsCopyRightLinkExisted = false;
            License = NotAvailableString;
            LicenseLink = null;
            IsLicenseLinkExisted = false;
            PrivacyLink = null;
            IsPrivacyLinkExisted = false;
            PurchaseLink = null;
            IsPrivacyLinkExisted = false;
            ReleaseNotes = NotAvailableString;
            ReleaseNotesLink = null;
            IsReleaseNotesLinkExisted = false;

            if (!IsLoadCompleted)
            {
                if (SearchApps is not null)
                {
                    // 获取当前应用可用版本
                    List<AvailableVersionModel> availableVersionList = await GetAvailableVersionAysnc(SearchApps.CatalogPackage, false);
                    if (availableVersionList is not null)
                    {
                        await UpdateAvailableVersionListAsync(availableVersionList);
                    }
                }
                else if (UpgradableApps is not null)
                {
                    // 获取当前应用可用版本
                    List<AvailableVersionModel> availableVersionList = await GetAvailableVersionAysnc(UpgradableApps.CatalogPackage, true);
                    if (availableVersionList is not null)
                    {
                        await UpdateAvailableVersionListAsync(availableVersionList);
                    }
                }
            }

            IsLoadCompleted = true;
        }

        /// <summary>
        /// 获取应用可用版本
        /// </summary>
        private async Task<List<AvailableVersionModel>> GetAvailableVersionAysnc(CatalogPackage catalogPackage, bool isUpgrade)
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
                                (bool isDefaultVersion, hasDefaultVersion) = CheckDefaultVersion(catalogPackage.DefaultInstallVersion, packageVersionId);

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
                            (bool isDefaultVersion, hasDefaultVersion) = CheckDefaultVersion(catalogPackage.DefaultInstallVersion, packageVersionId);

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

                return availableVersionList;
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
                Version = string.IsNullOrEmpty(packageVersionInfo.Version) ? NotAvailableString : packageVersionInfo.Version;
                if (Uri.TryCreate(catalogPackageMetadata.PackageUrl, new(), out Uri packageLinkUri))
                {
                    IsPackageLinkExisted = true;
                    PackageLink = packageLinkUri;
                }
                else
                {
                    IsPackageLinkExisted = false;
                    PackageLink = null;
                }
                Author = string.IsNullOrEmpty(catalogPackageMetadata.Author) ? NotAvailableString : catalogPackageMetadata.Author;
                Publisher = string.IsNullOrEmpty(catalogPackageMetadata.Publisher) ? NotAvailableString : packageVersionInfo.Publisher;
                if (Uri.TryCreate(catalogPackageMetadata.PublisherUrl, new(), out Uri publisherLinkUri))
                {
                    IsPublisherLinkExisted = true;
                    PublisherLink = publisherLinkUri;
                }
                else
                {
                    IsPublisherLinkExisted = false;
                    PublisherLink = null;
                }
                if (Uri.TryCreate(catalogPackageMetadata.PublisherSupportUrl, new(), out Uri appPublisherSupportLinkUri))
                {
                    IsPublisherSupportLinkExisted = true;
                    PublisherSupportLink = appPublisherSupportLinkUri;
                }
                else
                {
                    IsPublisherLinkExisted = false;
                    PublisherLink = null;
                }
                if (string.IsNullOrEmpty(catalogPackageMetadata.Locale))
                {
                    Locale = NotAvailableString;
                }
                else
                {
                    try
                    {
                        // 无特定区域
                        if (catalogPackageMetadata.Locale.Contains("Neutral", StringComparison.OrdinalIgnoreCase))
                        {
                            Locale = NoSpecificLocaleString;
                        }
                        else
                        {
                            Locale = new CultureInfo(catalogPackageMetadata.Locale).DisplayName;
                        }
                    }
                    catch (Exception e)
                    {
                        Locale = NotAvailableString;
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }
                CopyRight = string.IsNullOrEmpty(catalogPackageMetadata.Copyright) ? NotAvailableString : catalogPackageMetadata.Copyright;
                if (Uri.TryCreate(catalogPackageMetadata.CopyrightUrl, new(), out Uri copyRightLinkUri))
                {
                    IsCopyRightLinkExisted = true;
                    CopyRightLink = copyRightLinkUri;
                }
                else
                {
                    IsCopyRightLinkExisted = false;
                    CopyRightLink = null;
                }
                License = string.IsNullOrEmpty(catalogPackageMetadata.License) ? NotAvailableString : catalogPackageMetadata.License;
                if (Uri.TryCreate(catalogPackageMetadata.LicenseUrl, new(), out Uri licenseLinkUri))
                {
                    IsLicenseLinkExisted = true;
                    LicenseLink = licenseLinkUri;
                }
                else
                {
                    IsLicenseLinkExisted = false;
                    LicenseLink = null;
                }
                if (Uri.TryCreate(catalogPackageMetadata.PrivacyUrl, new(), out Uri privacyLinkUri))
                {
                    IsPrivacyLinkExisted = true;
                    PrivacyLink = privacyLinkUri;
                }
                else
                {
                    IsPrivacyLinkExisted = false;
                    PrivacyLink = null;
                }
                if (Uri.TryCreate(catalogPackageMetadata.PurchaseUrl, new(), out Uri purchaseLinkUri))
                {
                    IsPurchaseLinkExisted = true;
                    PurchaseLink = purchaseLinkUri;
                }
                else
                {
                    IsPurchaseLinkExisted = false;
                    PurchaseLink = null;
                }
                ReleaseNotes = string.IsNullOrEmpty(catalogPackageMetadata.ReleaseNotes) ? NotAvailableString : catalogPackageMetadata.ReleaseNotes;
                if (Uri.TryCreate(catalogPackageMetadata.ReleaseNotesUrl, new(), out Uri releaseNotesLinkUri))
                {
                    IsReleaseNotesLinkExisted = true;
                    ReleaseNotesLink = releaseNotesLinkUri;
                }
                else
                {
                    IsReleaseNotesLinkExisted = false;
                    ReleaseNotesLink = null;
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
                    if (Uri.TryCreate(documentation.DocumentUrl, new(), out Uri documentUrlUri))
                    {
                        DocumentationCollection.Add(new() { DisplayText = documentation.DocumentLabel, Uri = documentUrlUri });
                    }
                }
            }
            else
            {
                DisplayName = NotAvailableString;
                Description = NotAvailableString;
                Version = NotAvailableString;
                PackageLink = null;
                IsPackageLinkExisted = false;
                Author = NotAvailableString;
                Publisher = NotAvailableString;
                PublisherLink = null;
                IsPublisherLinkExisted = false;
                PublisherSupportLink = null;
                IsPublisherLinkExisted = false;
                Locale = NotAvailableString;
                CopyRight = NotAvailableString;
                CopyRightLink = null;
                IsCopyRightLinkExisted = false;
                License = NotAvailableString;
                LicenseLink = null;
                IsLicenseLinkExisted = false;
                PrivacyLink = null;
                IsPrivacyLinkExisted = false;
                PurchaseLink = null;
                IsPrivacyLinkExisted = false;
                ReleaseNotes = NotAvailableString;
                ReleaseNotesLink = null;
                IsReleaseNotesLinkExisted = false;
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
        private async Task UpdateAvailableVersionListAsync(List<AvailableVersionModel> availableVersionList)
        {
            if (availableVersionList is not null)
            {
                foreach (AvailableVersionModel availableVersionItem in availableVersionList)
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
        private async Task<string> GetCopyInformationStringAsync(string displayName, string description, string version, Uri packageLink, string author, string publisher, Uri publisherLink, Uri publisherSupportLink, string locale, string copyRight, Uri copyRightLink, string license, Uri licenseLink, Uri privacyLink, Uri purchaseLink, string releaseNotes, Uri releaseNotesLink)
        {
            return await Task.Run(() =>
            {
                try
                {
                    List<string> copyInformationList = [];
                    copyInformationList.Add(string.Format("{0}\t{1}", DisplayNameString, string.IsNullOrEmpty(displayName) ? NotAvailableString : displayName));
                    copyInformationList.Add(string.Format("{0}\t{1}", DescriptionString, string.IsNullOrEmpty(description) ? NotAvailableString : description));
                    copyInformationList.Add(string.Format("{0}\t{1}", VersionString, string.IsNullOrEmpty(version) ? NotAvailableString : version));
                    copyInformationList.Add(string.Format("{0}\t{1}", PackageLinkString, packageLink is not null ? packageLink.AbsoluteUri : NotAvailableString));
                    copyInformationList.Add(string.Format("{0}\t{1}", AuthorString, string.IsNullOrEmpty(author) ? NotAvailableString : author));
                    copyInformationList.Add(string.Format("{0}\t{1}", PublisherString, string.IsNullOrEmpty(publisher) ? NotAvailableString : publisher));
                    copyInformationList.Add(string.Format("{0}\t{1}", PublisherLinkString, publisherLink is not null ? publisherLink.AbsoluteUri : NotAvailableString));
                    copyInformationList.Add(string.Format("{0}\t{1}", PublisherSupportLinkString, publisherSupportLink is not null ? publisherSupportLink.AbsoluteUri : NotAvailableString));
                    copyInformationList.Add(string.Format("{0}\t{1}", LocaleString, string.IsNullOrEmpty(locale) ? NotAvailableString : locale));
                    copyInformationList.Add(string.Format("{0}\t{1}", CopyRightString, string.IsNullOrEmpty(copyRight) ? NotAvailableString : copyRight));
                    copyInformationList.Add(string.Format("{0}\t{1}", CopyRightLinkString, copyRightLink is not null ? copyRightLink.AbsoluteUri : NotAvailableString));
                    copyInformationList.Add(string.Format("{0}\t{1}", LicenseString, string.IsNullOrEmpty(license) ? NotAvailableString : license));
                    copyInformationList.Add(string.Format("{0}\t{1}", LicenseLinkString, licenseLink is not null ? licenseLink.AbsoluteUri : NotAvailableString));
                    copyInformationList.Add(string.Format("{0}\t{1}", PrivacyLinkString, privacyLink is not null ? privacyLink.AbsoluteUri : NotAvailableString));
                    copyInformationList.Add(string.Format("{0}\t{1}", PurchaseLinkString, purchaseLink is not null ? purchaseLink.AbsoluteUri : NotAvailableString));
                    copyInformationList.Add(string.Format("{0}\t{1}", ReleaseNotesString, string.IsNullOrEmpty(releaseNotes) ? NotAvailableString : releaseNotes));
                    copyInformationList.Add(string.Format("{0}\t{1}", ReleaseNotesLinkString, releaseNotesLink is not null ? releaseNotesLink.AbsoluteUri : NotAvailableString));
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
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
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
                         }}, true);
                        break;
                    }
                case PackageOperationKind.Install:
                    {
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
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
                        }}, true);
                        break;
                    }

                case PackageOperationKind.Repair:
                    {
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
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
                        }}, true);
                        break;
                    }
                case PackageOperationKind.Upgrade:
                    {
                        WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
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
                        }}, true);
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

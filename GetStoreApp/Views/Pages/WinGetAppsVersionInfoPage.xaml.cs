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
using Windows.UI.Text;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 应用版本信息页面
    /// </summary>
    public sealed partial class WinGetAppsVersionInfoPage : Page, INotifyPropertyChanged
    {
        private readonly string AuthorString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Author");
        private readonly string CopyRightLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/CopyRightLink");
        private readonly string CopyRightString = ResourceService.GetLocalized("WinGetAppsVersionInfo/CopyRight");
        private readonly string DescriptionString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Description");
        private readonly string DisplayNameString = ResourceService.GetLocalized("WinGetAppsVersionInfo/DisplayName");
        private readonly string LicenseLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/LicenseLink");
        private readonly string LicenseString = ResourceService.GetLocalized("WinGetAppsVersionInfo/License");
        private readonly string LocaleString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Locale");
        private readonly string NoSpecificLocaleString = ResourceService.GetLocalized("WinGetAppsVersionInfo/NoSpecificLocale");
        private readonly string PackageLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PackageLink");
        private readonly string PrivacyLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PrivacyLink");
        private readonly string PublisherLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PublisherLink");
        private readonly string PublisherString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Publisher");
        private readonly string PublisherSupportLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PublisherSupportLink");
        private readonly string PurchaseLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/PurchaseLink");
        private readonly string ReleaseNotesLinkString = ResourceService.GetLocalized("WinGetAppsVersionInfo/ReleaseNotesLink");
        private readonly string ReleaseNotesString = ResourceService.GetLocalized("WinGetAppsVersionInfo/ReleaseNotes");
        private readonly string UnknownString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Unknown");
        private readonly string VersionString = ResourceService.GetLocalized("WinGetAppsVersionInfo/Version");
        private readonly string WinGetAppsVersionCountInfoString = ResourceService.GetLocalized("WinGetAppsVersionInfo/WinGetAppsVersionCountInfo");

        private WinGetPage WinGetPage { get; set; }

        private WinGetAppsVersionDialog WinGetAppsVersionDialog { get; set; }

        private SearchAppsModel SearchApps { get; set; }

        private UpgradableAppsModel UpgradableApps { get; set; }

        private bool _isLoadCompleted;

        public bool IsLoadCompleted
        {
            get { return _isLoadCompleted; }

            set
            {
                if (!Equals(_isLoadCompleted, value))
                {
                    _isLoadCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadCompleted)));
                }
            }
        }

        private AvailableVersionModel _selectedItem;

        public AvailableVersionModel SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }

            set
            {
                if (!string.Equals(_displayName, value))
                {
                    _displayName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
                }
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }

            set
            {
                if (!string.Equals(_description, value))
                {
                    _description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
                }
            }
        }

        private string _version;

        public string Version
        {
            get { return _version; }

            set
            {
                if (!string.Equals(_version, value))
                {
                    _version = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Version)));
                }
            }
        }

        private bool _isPackageLinkExisted;

        public bool IsPackageLinkExisted
        {
            get { return _isPackageLinkExisted; }

            set
            {
                if (!Equals(_isPackageLinkExisted, value))
                {
                    _isPackageLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPackageLinkExisted)));
                }
            }
        }

        private Uri _packageLink;

        public Uri PackageLink
        {
            get { return _packageLink; }

            set
            {
                if (!string.Equals(_packageLink, value))
                {
                    _packageLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageLink)));
                }
            }
        }

        private string _author;

        public string Author
        {
            get { return _author; }

            set
            {
                if (!string.Equals(_author, value))
                {
                    _author = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Author)));
                }
            }
        }

        private string _publisher;

        public string Publisher
        {
            get { return _publisher; }

            set
            {
                if (!string.Equals(_publisher, value))
                {
                    _publisher = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Publisher)));
                }
            }
        }

        private bool _isPublisherLinkExisted;

        public bool IsPublisherLinkExisted
        {
            get { return _isPublisherLinkExisted; }

            set
            {
                if (!Equals(_isPublisherLinkExisted, value))
                {
                    _isPublisherLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPublisherLinkExisted)));
                }
            }
        }

        private Uri _publisherLink;

        public Uri PublisherLink
        {
            get { return _publisherLink; }

            set
            {
                if (!Equals(_publisherLink, value))
                {
                    _publisherLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublisherLink)));
                }
            }
        }

        private bool _isPublisherSupportLinkExisted;

        public bool IsPublisherSupportLinkExisted
        {
            get { return _isPublisherSupportLinkExisted; }

            set
            {
                if (!Equals(_isPublisherSupportLinkExisted, value))
                {
                    _isPublisherSupportLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPublisherSupportLinkExisted)));
                }
            }
        }

        private Uri _publisherSupportLink;

        public Uri PublisherSupportLink
        {
            get { return _publisherSupportLink; }

            set
            {
                if (!Equals(_publisherSupportLink, value))
                {
                    _publisherSupportLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublisherSupportLink)));
                }
            }
        }

        private string _locale;

        public string Locale
        {
            get { return _locale; }

            set
            {
                if (!string.Equals(_locale, value))
                {
                    _locale = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Locale)));
                }
            }
        }

        private string _copyRight;

        public string CopyRight
        {
            get { return _copyRight; }

            set
            {
                if (!string.Equals(_copyRight, value))
                {
                    _copyRight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CopyRight)));
                }
            }
        }

        private bool _isCopyRightLinkExisted;

        public bool IsCopyRightLinkExisted
        {
            get { return _isCopyRightLinkExisted; }

            set
            {
                if (!Equals(_isCopyRightLinkExisted, value))
                {
                    _isCopyRightLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCopyRightLinkExisted)));
                }
            }
        }

        private Uri _copyRightLink;

        public Uri CopyRightLink
        {
            get { return _copyRightLink; }

            set
            {
                if (!Equals(_copyRightLink, value))
                {
                    _copyRightLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CopyRightLink)));
                }
            }
        }

        private string _license;

        public string License
        {
            get { return _license; }

            set
            {
                if (!string.Equals(_license, value))
                {
                    _license = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(License)));
                }
            }
        }

        private bool _isLicenseLinkExisted;

        public bool IsLicenseLinkExisted
        {
            get { return _isLicenseLinkExisted; }

            set
            {
                if (!Equals(_isLicenseLinkExisted, value))
                {
                    _isLicenseLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLicenseLinkExisted)));
                }
            }
        }

        private Uri _licenseLink;

        public Uri LicenseLink
        {
            get { return _licenseLink; }

            set
            {
                if (!Equals(_licenseLink, value))
                {
                    _licenseLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LicenseLink)));
                }
            }
        }

        private bool _isPrivacyLinkExisted;

        public bool IsPrivacyLinkExisted
        {
            get { return _isPrivacyLinkExisted; }

            set
            {
                if (!Equals(_isPrivacyLinkExisted, value))
                {
                    _isPrivacyLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPrivacyLinkExisted)));
                }
            }
        }

        private Uri _privacyLink;

        public Uri PrivacyLink
        {
            get { return _privacyLink; }

            set
            {
                if (!Equals(_privacyLink, value))
                {
                    _privacyLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrivacyLink)));
                }
            }
        }

        private bool _isPurchaseLinkExisted;

        public bool IsPurchaseLinkExisted
        {
            get { return _isPurchaseLinkExisted; }

            set
            {
                if (!Equals(_isPurchaseLinkExisted, value))
                {
                    _isPurchaseLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPurchaseLinkExisted)));
                }
            }
        }

        private Uri _purchaseLink;

        public Uri PurchaseLink
        {
            get { return _purchaseLink; }

            set
            {
                if (!Equals(_purchaseLink, value))
                {
                    _purchaseLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PurchaseLink)));
                }
            }
        }

        private string _releaseNotes;

        public string ReleaseNotes
        {
            get { return _releaseNotes; }

            set
            {
                if (!string.Equals(_releaseNotes, value))
                {
                    _releaseNotes = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReleaseNotes)));
                }
            }
        }

        private bool _isReleaseNotesLinkExisted;

        public bool IsReleaseNotesLinkExisted
        {
            get { return _isReleaseNotesLinkExisted; }

            set
            {
                if (!Equals(_isReleaseNotesLinkExisted, value))
                {
                    _isReleaseNotesLinkExisted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReleaseNotesLinkExisted)));
                }
            }
        }

        private Uri _releaseNotesLink;

        public Uri ReleaseNotesLink
        {
            get { return _releaseNotesLink; }

            set
            {
                if (!Equals(_releaseNotesLink, value))
                {
                    _releaseNotesLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReleaseNotesLink)));
                }
            }
        }

        private ObservableCollection<AvailableVersionModel> WinGetAppsVersionCollection { get; } = [];

        private ObservableCollection<string> TagCollection { get; } = [];

        private ObservableCollection<ContentLinkInfo> DocumentionCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetAppsVersionInfoPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

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

                // 搜索应用
                if (argsList[2] is SearchAppsModel searchApps)
                {
                    SearchApps = searchApps;
                }
                // 可更新应用
                else if (argsList[2] is UpgradableAppsModel upgradableApps)
                {
                    UpgradableApps = upgradableApps;
                }

                DisplayName = UnknownString;
                Description = UnknownString;
                Version = UnknownString;
                PackageLink = null;
                IsPackageLinkExisted = false;
                Author = UnknownString;
                Publisher = UnknownString;
                PublisherLink = null;
                IsPublisherLinkExisted = false;
                PublisherSupportLink = null;
                IsPublisherLinkExisted = false;
                Locale = UnknownString;
                CopyRight = UnknownString;
                CopyRightLink = null;
                IsCopyRightLinkExisted = false;
                License = UnknownString;
                LicenseLink = null;
                IsLicenseLinkExisted = false;
                PrivacyLink = null;
                IsPrivacyLinkExisted = false;
                PurchaseLink = null;
                IsPrivacyLinkExisted = false;
                ReleaseNotes = UnknownString;
                ReleaseNotesLink = null;
                IsReleaseNotesLinkExisted = false;

                if (!IsLoadCompleted)
                {
                    if (SearchApps is not null)
                    {
                        bool hasDefaultVersion = false;

                        // 获取当前应用可用版本
                        List<AvailableVersionModel> availableVersionList = await Task.Run(() =>
                        {
                            List<AvailableVersionModel> availableVersionList = [];

                            for (int subIndex = 0; subIndex < SearchApps.CatalogPackage.AvailableVersions.Count; subIndex++)
                            {
                                PackageVersionId packageVersionId = SearchApps.CatalogPackage.AvailableVersions[subIndex];

                                if (!string.IsNullOrEmpty(packageVersionId.Version))
                                {
                                    bool isDefaultVersion = false;

                                    // 判断是否等同于默认版本
                                    if (SearchApps.CatalogPackage.DefaultInstallVersion.CompareToVersion(packageVersionId.Version) is CompareResult.Equal)
                                    {
                                        isDefaultVersion = true;
                                        if (!hasDefaultVersion)
                                        {
                                            hasDefaultVersion = true;
                                        }
                                    }

                                    // 添加所有已经获取到的所有版本
                                    availableVersionList.Add(new AvailableVersionModel()
                                    {
                                        IsDefaultVersion = isDefaultVersion,
                                        Version = packageVersionId.Version,
                                        PackageVersionId = packageVersionId
                                    });
                                }
                            }

                            // 没有默认版本，把默认版本添加在第一项
                            if (!hasDefaultVersion)
                            {
                                availableVersionList.Insert(0, new AvailableVersionModel()
                                {
                                    IsDefaultVersion = true,
                                    Version = SearchApps.CatalogPackage.DefaultInstallVersion.Version,
                                    PackageVersionId = null,
                                    PackageVersionInfo = SearchApps.CatalogPackage.DefaultInstallVersion
                                });
                            }

                            return availableVersionList;
                        });

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
                    else if (UpgradableApps is not null)
                    {
                        bool hasDefaultVersion = false;

                        // 获取当前应用可用版本
                        List<AvailableVersionModel> availableVersionList = await Task.Run(() =>
                        {
                            List<AvailableVersionModel> availableVersionList = [];

                            for (int subIndex = 0; subIndex < UpgradableApps.CatalogPackage.AvailableVersions.Count; subIndex++)
                            {
                                PackageVersionId packageVersionId = UpgradableApps.CatalogPackage.AvailableVersions[subIndex];

                                if (!string.IsNullOrEmpty(packageVersionId.Version))
                                {
                                    // 获取大于已安装应用版本的所有版本
                                    if (UpgradableApps.CatalogPackage.InstalledVersion.CompareToVersion(packageVersionId.Version) is CompareResult.Lesser)
                                    {
                                        bool isDefaultVersion = false;

                                        // 判断是否等同于默认版本
                                        if (UpgradableApps.CatalogPackage.DefaultInstallVersion.CompareToVersion(packageVersionId.Version) is CompareResult.Equal)
                                        {
                                            isDefaultVersion = true;
                                            if (!hasDefaultVersion)
                                            {
                                                hasDefaultVersion = true;
                                            }
                                        }

                                        // 添加所有已经获取到的所有版本
                                        availableVersionList.Add(new AvailableVersionModel()
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
                                availableVersionList.Insert(0, new AvailableVersionModel()
                                {
                                    IsDefaultVersion = true,
                                    Version = SearchApps.CatalogPackage.DefaultInstallVersion.Version,
                                    PackageVersionId = null,
                                    PackageVersionInfo = SearchApps.CatalogPackage.DefaultInstallVersion
                                });
                            }

                            return availableVersionList;
                        });

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

                IsLoadCompleted = true;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：WinGet 应用版本信息页面——挂载的事件

        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private async void OnItemClick(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is AvailableVersionModel availableVersion && !Equals(SelectedItem, availableVersion))
            {
                SelectedItem = availableVersion;
                await InitializeVersionInformationAsync(availableVersion);
            }
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>

        private async void OnCopyInformationClicked(object sender, RoutedEventArgs args)
        {
            List<string> copyInformationList = await Task.Run(() =>
            {
                List<string> copyInformationList = [];

                copyInformationList.Add(string.Format("{0}\t{1}", DisplayNameString, DisplayName));
                copyInformationList.Add(string.Format("{0}\t{1}", DescriptionString, Description));
                copyInformationList.Add(string.Format("{0}\t{1}", VersionString, Version));
                copyInformationList.Add(string.Format("{0}\t{1}", PackageLinkString, PackageLink));
                copyInformationList.Add(string.Format("{0}\t{1}", AuthorString, Author));
                copyInformationList.Add(string.Format("{0}\t{1}", PublisherString, Publisher));
                copyInformationList.Add(string.Format("{0}\t{1}", PublisherLinkString, PublisherLink));
                copyInformationList.Add(string.Format("{0}\t{1}", PublisherSupportLinkString, PublisherSupportLink));
                copyInformationList.Add(string.Format("{0}\t{1}", LocaleString, Locale));
                copyInformationList.Add(string.Format("{0}\t{1}", CopyRightString, CopyRight));
                copyInformationList.Add(string.Format("{0}\t{1}", CopyRightLinkString, CopyRightLink));
                copyInformationList.Add(string.Format("{0}\t{1}", LicenseString, License));
                copyInformationList.Add(string.Format("{0}\t{1}", LicenseLinkString, LicenseLink));
                copyInformationList.Add(string.Format("{0}\t{1}", PrivacyLinkString, PrivacyLink));
                copyInformationList.Add(string.Format("{0}\t{1}", PurchaseLinkString, PurchaseLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ReleaseNotesString, ReleaseNotes));
                copyInformationList.Add(string.Format("{0}\t{1}", ReleaseNotesLinkString, ReleaseNotesLink));

                return copyInformationList;
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyInformationList));
            await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
        }

        /// <summary>
        /// 下载当前版本应用
        /// </summary>
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            if (SearchApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Download,
                    AppID = SearchApps.AppID,
                    AppName = SearchApps.AppName,
                    AppVersion = SelectedItem.Version,
                    PackagePath = WinGetConfigService.DefaultDownloadFolder,
                    PackageOperationProgress = 0,
                    PackageDownloadProgressState = PackageDownloadProgressState.Queued,
                    PackageVersionId = SelectedItem.PackageVersionId,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageDownloadProgress = null,
                    SearchApps = SearchApps,
                 }}, true);
            }
        }

        /// <summary>
        /// 安装当前版本应用
        /// </summary>
        private void OnInstallClicked(object sender, RoutedEventArgs args)
        {
            if (SearchApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Install,
                    AppID = SearchApps.AppID,
                    AppName = SearchApps.AppName,
                    AppVersion = SelectedItem.Version,
                    PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                    PackageOperationProgress = 0,
                    PackageInstallProgressState = PackageInstallProgressState.Queued,
                    PackageVersionId = SelectedItem.PackageVersionId,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageInstallProgress = null,
                    SearchApps = SearchApps,
                }}, true);
            }
        }

        /// <summary>
        /// 修复当前版本应用
        /// </summary>
        private void OnRepairClicked(object sender, RoutedEventArgs args)
        {
            if (SearchApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Repair,
                    AppID = SearchApps.AppID,
                    AppName = SearchApps.AppName,
                    AppVersion = SelectedItem.Version,
                    PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                    PackageOperationProgress = 0,
                    PackageRepairProgressState = PackageRepairProgressState.Queued,
                    PackageVersionId = SelectedItem.PackageVersionId,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageRepairProgress = null,
                    SearchApps = SearchApps,
                }}, true);
            }
        }

        /// <summary>
        /// 更新当前版本应用
        /// </summary>
        private void OnUpgradeClicked(object sender, RoutedEventArgs args)
        {
            if (UpgradableApps is not null && SelectedItem is not null && WinGetAppsVersionDialog is not null)
            {
                WinGetAppsVersionDialog.NavigateTo(WinGetAppsVersionDialog.PageList[1], new List<object>(){ WinGetPage, WinGetAppsVersionDialog, new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Upgrade,
                    AppID = UpgradableApps.AppID,
                    AppName = UpgradableApps.AppName,
                    AppVersion = SelectedItem.Version,
                    PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                    PackageOperationProgress = 0,
                    PackageInstallProgressState = PackageInstallProgressState.Queued,
                    PackageVersionId = SelectedItem.PackageVersionId,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageInstallProgress = null,
                    UpgradableApps = UpgradableApps,
                }}, true);
            }
        }

        #endregion 第二部分：WinGet 应用版本信息页面——挂载的事件

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
                    packageVersionInfo = availableVersion.PackageVersionId is not null ? SearchApps.CatalogPackage.GetPackageVersionInfo(availableVersion.PackageVersionId) : availableVersion.PackageVersionInfo;

                    if (packageVersionInfo is not null)
                    {
                        catalogPackageMetadata = packageVersionInfo.GetCatalogPackageMetadata();
                    }
                }
                else if (UpgradableApps is not null)
                {
                    packageVersionInfo = availableVersion.PackageVersionId is not null ? SearchApps.CatalogPackage.GetPackageVersionInfo(availableVersion.PackageVersionId) : availableVersion.PackageVersionInfo;

                    if (packageVersionInfo is not null)
                    {
                        catalogPackageMetadata = packageVersionInfo.GetCatalogPackageMetadata();
                    }
                }

                return ValueTuple.Create(packageVersionInfo, catalogPackageMetadata);
            });

            if (packageVersionInfo is not null && catalogPackageMetadata is not null)
            {
                DisplayName = string.IsNullOrEmpty(catalogPackageMetadata.PackageName) ? UnknownString : catalogPackageMetadata.PackageName;
                Description = string.IsNullOrEmpty(catalogPackageMetadata.Description) ? UnknownString : catalogPackageMetadata.Description;
                Version = string.IsNullOrEmpty(packageVersionInfo.Version) ? UnknownString : packageVersionInfo.Version;
                if (Uri.TryCreate(catalogPackageMetadata.PackageUrl, new UriCreationOptions(), out Uri packageLinkUri))
                {
                    IsPackageLinkExisted = true;
                    PackageLink = packageLinkUri;
                }
                else
                {
                    IsPackageLinkExisted = false;
                    PackageLink = null;
                }
                Author = string.IsNullOrEmpty(catalogPackageMetadata.Author) ? UnknownString : catalogPackageMetadata.Author;
                Publisher = string.IsNullOrEmpty(catalogPackageMetadata.Publisher) ? UnknownString : packageVersionInfo.Publisher;
                if (Uri.TryCreate(catalogPackageMetadata.PublisherUrl, new UriCreationOptions(), out Uri publisherLinkUri))
                {
                    IsPublisherLinkExisted = true;
                    PublisherLink = publisherLinkUri;
                }
                else
                {
                    IsPublisherLinkExisted = false;
                    PublisherLink = null;
                }
                if (Uri.TryCreate(catalogPackageMetadata.PublisherSupportUrl, new UriCreationOptions(), out Uri appPublisherSupportLinkUri))
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
                    Locale = UnknownString;
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
                        Locale = UnknownString;
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }
                CopyRight = string.IsNullOrEmpty(catalogPackageMetadata.Copyright) ? UnknownString : catalogPackageMetadata.Copyright;
                if (Uri.TryCreate(catalogPackageMetadata.CopyrightUrl, new UriCreationOptions(), out Uri copyRightLinkUri))
                {
                    IsCopyRightLinkExisted = true;
                    CopyRightLink = copyRightLinkUri;
                }
                else
                {
                    IsCopyRightLinkExisted = false;
                    CopyRightLink = null;
                }
                License = string.IsNullOrEmpty(catalogPackageMetadata.License) ? UnknownString : catalogPackageMetadata.License;
                if (Uri.TryCreate(catalogPackageMetadata.LicenseUrl, new UriCreationOptions(), out Uri licenseLinkUri))
                {
                    IsLicenseLinkExisted = true;
                    LicenseLink = licenseLinkUri;
                }
                else
                {
                    IsLicenseLinkExisted = false;
                    LicenseLink = null;
                }
                if (Uri.TryCreate(catalogPackageMetadata.PrivacyUrl, new UriCreationOptions(), out Uri privacyLinkUri))
                {
                    IsPrivacyLinkExisted = true;
                    PrivacyLink = privacyLinkUri;
                }
                else
                {
                    IsPrivacyLinkExisted = false;
                    PrivacyLink = null;
                }
                if (Uri.TryCreate(catalogPackageMetadata.PurchaseUrl, new UriCreationOptions(), out Uri purchaseLinkUri))
                {
                    IsPurchaseLinkExisted = true;
                    PurchaseLink = purchaseLinkUri;
                }
                else
                {
                    IsPurchaseLinkExisted = false;
                    PurchaseLink = null;
                }
                ReleaseNotes = string.IsNullOrEmpty(catalogPackageMetadata.ReleaseNotes) ? UnknownString : catalogPackageMetadata.ReleaseNotes;
                if (Uri.TryCreate(catalogPackageMetadata.ReleaseNotesUrl, new UriCreationOptions(), out Uri releaseNotesLinkUri))
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

                DocumentionCollection.Clear();
                for (int index = 0; index < catalogPackageMetadata.Documentations.Count; index++)
                {
                    Documentation documentation = catalogPackageMetadata.Documentations[index];
                    if (Uri.TryCreate(documentation.DocumentUrl, new UriCreationOptions(), out Uri documentUrlUri))
                    {
                        DocumentionCollection.Add(new ContentLinkInfo() { DisplayText = documentation.DocumentLabel, Uri = documentUrlUri });
                    }
                }
            }
            else
            {
                DisplayName = UnknownString;
                Description = UnknownString;
                Version = UnknownString;
                PackageLink = null;
                IsPackageLinkExisted = false;
                Author = UnknownString;
                Publisher = UnknownString;
                PublisherLink = null;
                IsPublisherLinkExisted = false;
                PublisherSupportLink = null;
                IsPublisherLinkExisted = false;
                Locale = UnknownString;
                CopyRight = UnknownString;
                CopyRightLink = null;
                IsCopyRightLinkExisted = false;
                License = UnknownString;
                LicenseLink = null;
                IsLicenseLinkExisted = false;
                PrivacyLink = null;
                IsPrivacyLinkExisted = false;
                PurchaseLink = null;
                IsPrivacyLinkExisted = false;
                ReleaseNotes = UnknownString;
                ReleaseNotesLink = null;
                IsReleaseNotesLinkExisted = false;
                TagCollection.Clear();
                DocumentionCollection.Clear();
            }
        }

        /// <summary>
        /// 获取显示的应用类型
        /// </summary>
        private Visibility GetWinGetAppsType(object wingetApps)
        {
            return wingetApps is null ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

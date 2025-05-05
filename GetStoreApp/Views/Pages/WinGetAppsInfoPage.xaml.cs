using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
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
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 应用版本信息（应用信息）页面
    /// </summary>
    public sealed partial class WinGetAppsInfoPage : Page, INotifyPropertyChanged
    {
        private readonly string WinGetAppsVersion = ResourceService.GetLocalized("WinGet/WinGetAppsVersion");
        private readonly string Unknown = ResourceService.GetLocalized("WinGet/Unknown");
        private bool isInitialized;

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
                if (!Equals(_displayName, value))
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
                if (!Equals(_description, value))
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
                if (!Equals(_version, value))
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
                if (!Equals(_packageLink, value))
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
                if (!Equals(_author, value))
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
                if (!Equals(_publisher, value))
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
                if (!Equals(_locale, value))
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
                if (!Equals(_copyRight, value))
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
                if (!Equals(_license, value))
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
                if (!Equals(_releaseNotes, value))
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

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetAppsInfoPage()
        {
            InitializeComponent();

            DisplayName = Unknown;
            Description = Unknown;
            Version = Unknown;
            PackageLink = null;
            IsPackageLinkExisted = false;
            Author = Unknown;
            Publisher = Unknown;
            PublisherLink = null;
            IsPublisherLinkExisted = false;
            PublisherSupportLink = null;
            IsPublisherLinkExisted = false;
            Locale = Unknown;
            CopyRight = Unknown;
            CopyRightLink = null;
            IsCopyRightLinkExisted = false;
            License = Unknown;
            LicenseLink = null;
            IsLicenseLinkExisted = false;
            PrivacyLink = null;
            IsPrivacyLinkExisted = false;
            PurchaseLink = null;
            IsPrivacyLinkExisted = false;
            ReleaseNotes = Unknown;
            ReleaseNotesLink = null;
            IsReleaseNotesLinkExisted = false;
        }

        #region 第一部分：重载父类事件

        /// <summary>
        /// 导航到该页面发生的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized && !IsLoadCompleted && args.Parameter is SearchAppsModel searchApps)
            {
                isInitialized = true;

                // 获取当前应用可用版本
                List<AvailableVersionModel> availableVersionList = await Task.Run(() =>
                {
                    List<AvailableVersionModel> availableVersionList = [];

                    for (int subIndex = 0; subIndex < searchApps.CatalogPackage.AvailableVersions.Count; subIndex++)
                    {
                        PackageVersionId packageVersionId = searchApps.CatalogPackage.AvailableVersions[subIndex];
                        PackageVersionInfo packageVersionInfo = searchApps.CatalogPackage.GetPackageVersionInfo(packageVersionId);

                        if (!string.IsNullOrEmpty(packageVersionId.Version))
                        {
                            bool isDefaultVersion = false;

                            // 判断是否等同于默认版本
                            if (searchApps.CatalogPackage.DefaultInstallVersion.CompareToVersion(packageVersionId.Version) is CompareResult.Equal)
                            {
                                isDefaultVersion = true;
                            }

                            availableVersionList.Add(new AvailableVersionModel()
                            {
                                IsDefaultVersion = isDefaultVersion,
                                Version = packageVersionInfo.Version,
                                PackageVersionInfo = packageVersionInfo,
                            });
                        }
                    }

                    return availableVersionList;
                });

                foreach (AvailableVersionModel availableVersionItem in availableVersionList)
                {
                    WinGetAppsVersionCollection.Add(availableVersionItem);

                    if (availableVersionItem.IsDefaultVersion)
                    {
                        SelectedItem = availableVersionItem;
                        InitializeVersionInformation(availableVersionItem);
                    }
                }

                IsLoadCompleted = true;
            }
        }

        #endregion 第一部分：重载父类事件

        #region 第二部分：WinGet 应用版本信息（应用信息）页面——挂载的事件

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is AvailableVersionModel availableVersionItem && !Equals(SelectedItem, availableVersionItem))
            {
                SelectedItem = availableVersionItem;
                InitializeVersionInformation(availableVersionItem);
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

                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/AppName"), DisplayName));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/Description"), Description));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/AppVersion"), Version));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/PackageLink"), PackageLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/Author"), Author));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/AppPublisher"), Publisher));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/PublisherLink"), PublisherLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/PublisherSupportLink"), PublisherSupportLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/Locale"), Locale));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/CopyRight"), CopyRight));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/CopyRightLink"), CopyRightLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/License"), License));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/LicenseLink"), LicenseLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/PrivacyLink"), PrivacyLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/PurchaseLink"), PurchaseLink));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/ReleaseNotes"), ReleaseNotes));
                copyInformationList.Add(string.Format("{0}\t{1}", ResourceService.GetLocalized("WinGet/ReleaseNotesLink"), ReleaseNotesLink));

                return copyInformationList;
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyInformationList));
            await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetAppInformation, copyResult));
        }

        #endregion 第二部分：WinGet 应用版本信息（应用信息）页面——挂载的事件

        /// <summary>
        /// 初始化对应版本信息
        /// </summary>
        private void InitializeVersionInformation(AvailableVersionModel availableVersion)
        {
            if (availableVersion.PackageVersionInfo is not null && availableVersion.PackageVersionInfo.GetCatalogPackageMetadata() is CatalogPackageMetadata catalogPackageMetadata)
            {
                DisplayName = string.IsNullOrEmpty(catalogPackageMetadata.PackageName) ? Unknown : catalogPackageMetadata.PackageName;
                Description = string.IsNullOrEmpty(catalogPackageMetadata.Description) ? Unknown : catalogPackageMetadata.Description;
                Version = string.IsNullOrEmpty(availableVersion.PackageVersionInfo.Version) ? Unknown : availableVersion.PackageVersionInfo.Version;
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
                Author = string.IsNullOrEmpty(catalogPackageMetadata.Author) ? Unknown : catalogPackageMetadata.Author;
                Publisher = string.IsNullOrEmpty(catalogPackageMetadata.Publisher) ? Unknown : availableVersion.PackageVersionInfo.Publisher;
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
                    Locale = Unknown;
                }
                else
                {
                    try
                    {
                        Locale = new CultureInfo(catalogPackageMetadata.Locale).DisplayName;
                    }
                    catch (Exception e)
                    {
                        Locale = Unknown;
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }
                CopyRight = string.IsNullOrEmpty(catalogPackageMetadata.Copyright) ? Unknown : catalogPackageMetadata.Copyright;
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
                License = string.IsNullOrEmpty(catalogPackageMetadata.License) ? Unknown : catalogPackageMetadata.License;
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
                ReleaseNotes = string.IsNullOrEmpty(catalogPackageMetadata.ReleaseNotes) ? Unknown : catalogPackageMetadata.ReleaseNotes;
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
            }
        }
    }
}

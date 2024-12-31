using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.UI.Backdrop;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Ole32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shlwapi;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppInstaller.Pages
{
    /// <summary>
    /// 应用主页面
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private static readonly Guid CLSID_AppxFactory = new("5842A140-FF9F-4166-8F5C-62F5B7B0C781");
        private static readonly Guid CLSID_AppxBundleFactory = new("378E0446-5384-43B7-8877-E7DBDD883446");

        private bool _showMoreEnabled;

        public bool ShowMoreEnabled
        {
            get { return _showMoreEnabled; }

            set
            {
                if (!Equals(_showMoreEnabled, value))
                {
                    _showMoreEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowMoreEnabled)));
                }
            }
        }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                }
            }
        }

        private BitmapImage _packageIconImage;

        public BitmapImage PackageIconImage
        {
            get { return _packageIconImage; }

            set
            {
                if (!Equals(_packageIconImage, value))
                {
                    _packageIconImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageIconImage)));
                }
            }
        }

        private string _packageName;

        public string PackageName
        {
            get { return _packageName; }

            set
            {
                if (!Equals(_packageName, value))
                {
                    _packageName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageName)));
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

        private Version _version;

        public Version Version
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

        private bool _isInstalling;

        public bool IsInstalling
        {
            get { return _isInstalling; }

            set
            {
                if (!Equals(_isInstalling, value))
                {
                    _isInstalling = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalling)));
                }
            }
        }

        private double _installProgressValue;

        public double InstallProgressValue
        {
            get { return _installProgressValue; }

            set
            {
                if (!Equals(_installProgressValue, value))
                {
                    _installProgressValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallProgressValue)));
                }
            }
        }

        private bool _isInstallWaiting;

        public bool IsInstallWaiting
        {
            get { return _isInstallWaiting; }

            set
            {
                if (!Equals(_isInstallWaiting, value))
                {
                    _isInstallWaiting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstallWaiting)));
                }
            }
        }

        private bool _isInstallFailed;

        public bool IsInstallFailed
        {
            get { return _isInstallFailed; }

            set
            {
                if (!Equals(_isInstallFailed, value))
                {
                    _isInstallFailed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstallFailed)));
                }
            }
        }

        private string _installStateString;

        public string InstallStateString
        {
            get { return _installStateString; }

            set
            {
                if (!Equals(_installStateString, value))
                {
                    _installStateString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallStateString)));
                }
            }
        }

        private string _installStateVisible;

        public string InstallStateVisible
        {
            get { return _installStateVisible; }

            set
            {
                if (!Equals(_installStateVisible, value))
                {
                    _installStateVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallStateVisible)));
                }
            }
        }

        private string _installFailedInformation;

        public string InstallFailedInformation
        {
            get { return _installFailedInformation; }

            set
            {
                if (!Equals(_installFailedInformation, value))
                {
                    _installFailedInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallFailedInformation)));
                }
            }
        }

        private ObservableCollection<DependencyModel> DependencyCollection { get; } = [];

        private ObservableCollection<string> CapabilitiesCollection { get; } = [];

        private List<string> AddDependencyList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage()
        {
            InitializeComponent();
            Background = new MicaBrush(MicaKind.BaseAlt, true);
            Program.SetTitleBarTheme(ActualTheme);
            Program.SetClassicMenuTheme(ActualTheme);

            for (int i = 0; i < 5; i++)
            {
                CapabilitiesCollection.Add("Test" + i.ToString());
            }
        }

        #region 第一部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.MainAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_CLOSE, 0);
        }

        #endregion 第一部分：窗口右键菜单事件

        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            Program.SetTitleBarTheme(sender.ActualTheme);
            Program.SetClassicMenuTheme(sender.ActualTheme);
        }

        #region 第二部分：应用安装器主页面——挂载的事件

        /// <summary>
        /// 查看更多 / 更少信息
        /// </summary>
        private void OnViewInformationClicked(object sender, RoutedEventArgs args)
        {
            ShowMoreEnabled = !ShowMoreEnabled;
        }

        /// <summary>
        /// 添加依赖包
        /// </summary>
        private void OnAddDependencyClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private void OnInstallAppClicked(object sender, RoutedEventArgs args)
        {
        }

        #endregion 第二部分：应用安装器主页面——挂载的事件

        /// <summary>
        /// 解析应用包
        /// </summary>
        public static void ParsePackagedApp(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (ShlwapiLibrary.SHCreateStreamOnFileEx(filePath, STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE, 0, false, null, out IStream fileStream) is 0)
                {
                    string extensionName = Path.GetExtension(filePath);

                    // 解析以 appx 或 msix 格式结尾的单个应用包
                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out IntPtr appxFactoryPtr) is 0)
                        {
                            IAppxFactory3 appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.Unwrap);

                            if (appxFactory is not null)
                            {
                                // 读取应用包内容
                                if (appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                {
                                    // 读取应用包清单
                                    if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0)
                                    {
                                        // 获取应用包定义的应用程序列表
                                        if (appxManifestReader.GetApplications(out IAppxManifestApplicationsEnumerator applicationsEnumerator) is 0)
                                        {
                                            while (applicationsEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                            {
                                                if (applicationsEnumerator.GetCurrent(out IAppxManifestApplication appxManifestApplication) is 0)
                                                {
                                                    appxManifestApplication.GetAppUserModelId(out string appUserModelId);
                                                    appxManifestApplication.GetStringValue("AppListEntry", out string appListEntry);
                                                    appxManifestApplication.GetStringValue("BackgroundColor", out string backgroundColor);
                                                    appxManifestApplication.GetStringValue("DefaultSize", out string defaultSize);
                                                    appxManifestApplication.GetStringValue("Description", out string description);
                                                    appxManifestApplication.GetStringValue("EntryPoint", out string entryPoint);
                                                    appxManifestApplication.GetStringValue("Executable", out string executable);
                                                    appxManifestApplication.GetStringValue("ForegroundText", out string foregroundText);
                                                    appxManifestApplication.GetStringValue("ID", out string id);
                                                    appxManifestApplication.GetStringValue("LockScreenLogo", out string lockScreenLogo);
                                                    appxManifestApplication.GetStringValue("LockScreenNotification", out string lockScreenNotification);
                                                    appxManifestApplication.GetStringValue("Logo", out string logo);
                                                    appxManifestApplication.GetStringValue("MinWidth", out string minWidth);
                                                    appxManifestApplication.GetStringValue("ShortName", out string shortName);
                                                    appxManifestApplication.GetStringValue("SmallLogo", out string smallLogo);
                                                    appxManifestApplication.GetStringValue("Square150x150Logo", out string square150x150Logo);
                                                    appxManifestApplication.GetStringValue("Square30x30Logo", out string square30x30Logo);
                                                    appxManifestApplication.GetStringValue("Square310x310Logo", out string square310x310Logo);
                                                    appxManifestApplication.GetStringValue("Square44x44Logo", out string square44x44Logo);
                                                    appxManifestApplication.GetStringValue("Square70x70Logo", out string square70x70Logo);
                                                    appxManifestApplication.GetStringValue("Square71x71Logo", out string square71x71Logo);
                                                    appxManifestApplication.GetStringValue("StartPage", out string startPage);
                                                    appxManifestApplication.GetStringValue("Tall150x310Logo", out string tall150x310Logo);
                                                    appxManifestApplication.GetStringValue("VisualGroup", out string visualGroup);
                                                    appxManifestApplication.GetStringValue("WideLogo", out string wideLogo);
                                                    appxManifestApplication.GetStringValue("Wide310x150Logo", out string wide310x150Logo);
                                                }

                                                applicationsEnumerator.MoveNext(out _);
                                            }
                                        }

                                        // 获取应用包定义的功能列表
                                        appxManifestReader.GetCapabilities(out APPX_CAPABILITIES capabilities);

                                        // 获取应用包定义的静态依赖项列表
                                        if (appxManifestReader.GetPackageDependencies(out IAppxManifestPackageDependenciesEnumerator dependenciesEnumerator) is 0)
                                        {
                                            while (dependenciesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                            {
                                                if (dependenciesEnumerator.GetCurrent(out IAppxManifestPackageDependency2 appxManifestPackageDependency) is 0)
                                                {
                                                    appxManifestPackageDependency.GetMinVersion(out ulong dependencyMinVersion);
                                                    appxManifestPackageDependency.GetName(out string dependencyName);
                                                    appxManifestPackageDependency.GetPublisher(out string dependencyPublisher);
                                                    appxManifestPackageDependency.GetMaxMajorVersionTested(out ushort dependencyMaxMajorVersionTested);
                                                }

                                                dependenciesEnumerator.MoveNext(out _);
                                            }
                                        }

                                        // 获取应用包定义的包标识符
                                        if (appxManifestReader.GetPackageId(out IAppxManifestPackageId2 packageId) is 0)
                                        {
                                            packageId.GetArchitecture2(out ProcessorArchitecture architecture);
                                            packageId.GetName(out string name);
                                            packageId.GetPackageFamilyName(out string packageFamilyName);
                                            packageId.GetPackageFullName(out string packageFullName);
                                            packageId.GetPublisher(out string publisher);
                                            packageId.GetResourceId(out string resourceId);
                                            packageId.GetVersion(out ulong version);
                                        }

                                        // 获取应用包定义的先决条件：最小系统版本号和最大测试系统版本号
                                        appxManifestReader.GetPrerequisite("OSMinVersion", out ulong minVersion);
                                        appxManifestReader.GetPrerequisite("OSMaxVersionTested", out ulong maxVersion);

                                        // 获取应用包的属性
                                        if (appxManifestReader.GetProperties(out IAppxManifestProperties packageProperties) is 0)
                                        {
                                            packageProperties.GetBoolValue("Framework", out bool isFramework);

                                            packageProperties.GetStringValue("Description", out string description);
                                            packageProperties.GetStringValue("DisplayName", out string displayName);
                                            packageProperties.GetStringValue("Logo", out string logo);
                                            packageProperties.GetStringValue("PublisherDisplayName", out string publisherDisplayName);
                                        }

                                        // 获取应用包定义的资源
                                        if (appxManifestReader.GetResources(out IAppxManifestResourcesEnumerator appxManifestResourcesEnumerator) is 0)
                                        {
                                            while (appxManifestResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                            {
                                                appxManifestResourcesEnumerator.GetCurrent(out string resource);
                                                appxManifestResourcesEnumerator.MoveNext(out _);
                                            }
                                        }

                                        // 获取应用包定义的限定资源
                                        if (appxManifestReader.GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
                                        {
                                            while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                            {
                                                if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                                                {
                                                }

                                                appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 解析以 appxbundle 或 msixbundle 格式结尾的应用包
                    else if (extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Ole32Library.CoCreateInstance(CLSID_AppxBundleFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxBundleFactory2).GUID, out IntPtr appxBundleFactoryPtr) is 0)
                        {
                            IAppxBundleFactory2 appxBundleFactory = (IAppxBundleFactory2)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxBundleFactoryPtr, CreateObjectFlags.Unwrap);
                            if (appxBundleFactory is not null)
                            {
                                // 读取捆绑包的二进制文件内容
                                if (appxBundleFactory.CreateBundleReader2(fileStream, null, out IAppxBundleReader appxBundleReader) is 0)
                                {
                                    // 读取捆绑包的二进制文件
                                    appxBundleReader.GetPayloadPackages(out IAppxFilesEnumerator appxFilesEnumerator);

                                    Dictionary<string, IAppxFile> bundleFileDict = [];
                                    Architecture osArchitecture = RuntimeInformation.ProcessArchitecture;
                                    IStream parseFileStream = null;

                                    while (appxFilesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                    {
                                        appxFilesEnumerator.GetCurrent(out IAppxFile appxFile);
                                        appxFile.GetName(out string packageFileName);
                                        string packageFileExtensionName = Path.GetExtension(packageFileName);

                                        if (packageFileExtensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase))
                                        {
                                            bundleFileDict.Add(packageFileName, appxFile);
                                        }

                                        appxFilesEnumerator.MoveNext(out _);
                                    }

                                    if (osArchitecture is Architecture.X86)
                                    {
                                        foreach (string bundleFileName in bundleFileDict.Keys)
                                        {
                                            if (bundleFileName.Contains("x86", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bundleFileDict[bundleFileName].GetStream(out parseFileStream);
                                            }
                                        }
                                    }
                                    else if (osArchitecture is Architecture.X64)
                                    {
                                        foreach (string bundleFileName in bundleFileDict.Keys)
                                        {
                                            if (bundleFileName.Contains("x64", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bundleFileDict[bundleFileName].GetStream(out parseFileStream);
                                            }
                                        }
                                    }
                                    else if (osArchitecture is Architecture.Arm64)
                                    {
                                        foreach (string bundleFileName in bundleFileDict.Keys)
                                        {
                                            if (bundleFileName.Contains("arm64", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bundleFileDict[bundleFileName].GetStream(out parseFileStream);
                                            }
                                        }
                                    }
                                    else if (osArchitecture is Architecture.Arm)
                                    {
                                        foreach (string bundleFileName in bundleFileDict.Keys)
                                        {
                                            if (bundleFileName.Contains("arm", StringComparison.OrdinalIgnoreCase))
                                            {
                                                bundleFileDict[bundleFileName].GetStream(out parseFileStream);
                                            }
                                        }
                                    }

                                    if (parseFileStream is null && bundleFileDict.Count > 0)
                                    {
                                        foreach (KeyValuePair<string, IAppxFile> appxFileItem in bundleFileDict)
                                        {
                                            appxFileItem.Value.GetStream(out parseFileStream);
                                            if (parseFileStream is not null)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    // 解析捆绑包里面包含的二进制安装文件
                                    if (parseFileStream is not null)
                                    {
                                        if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out IntPtr appxFactoryPtr) is 0)
                                        {
                                            IAppxFactory3 appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.Unwrap);
                                            if (appxFactory is not null)
                                            {
                                                // 读取应用包内容
                                                if (appxFactory.CreatePackageReader2(parseFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                                {
                                                    // 读取应用包清单
                                                    if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0)
                                                    {
                                                        // 获取应用包定义的应用程序列表
                                                        if (appxManifestReader.GetApplications(out IAppxManifestApplicationsEnumerator applicationsEnumerator) is 0)
                                                        {
                                                            while (applicationsEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                            {
                                                                if (applicationsEnumerator.GetCurrent(out IAppxManifestApplication appxManifestApplication) is 0)
                                                                {
                                                                    appxManifestApplication.GetAppUserModelId(out string appUserModelId);
                                                                    appxManifestApplication.GetStringValue("AppListEntry", out string appListEntry);
                                                                    appxManifestApplication.GetStringValue("BackgroundColor", out string backgroundColor);
                                                                    appxManifestApplication.GetStringValue("DefaultSize", out string defaultSize);
                                                                    appxManifestApplication.GetStringValue("Description", out string description);
                                                                    appxManifestApplication.GetStringValue("EntryPoint", out string entryPoint);
                                                                    appxManifestApplication.GetStringValue("Executable", out string executable);
                                                                    appxManifestApplication.GetStringValue("ForegroundText", out string foregroundText);
                                                                    appxManifestApplication.GetStringValue("ID", out string id);
                                                                    appxManifestApplication.GetStringValue("LockScreenLogo", out string lockScreenLogo);
                                                                    appxManifestApplication.GetStringValue("LockScreenNotification", out string lockScreenNotification);
                                                                    appxManifestApplication.GetStringValue("Logo", out string logo);
                                                                    appxManifestApplication.GetStringValue("MinWidth", out string minWidth);
                                                                    appxManifestApplication.GetStringValue("ShortName", out string shortName);
                                                                    appxManifestApplication.GetStringValue("SmallLogo", out string smallLogo);
                                                                    appxManifestApplication.GetStringValue("Square150x150Logo", out string square150x150Logo);
                                                                    appxManifestApplication.GetStringValue("Square30x30Logo", out string square30x30Logo);
                                                                    appxManifestApplication.GetStringValue("Square310x310Logo", out string square310x310Logo);
                                                                    appxManifestApplication.GetStringValue("Square44x44Logo", out string square44x44Logo);
                                                                    appxManifestApplication.GetStringValue("Square70x70Logo", out string square70x70Logo);
                                                                    appxManifestApplication.GetStringValue("Square71x71Logo", out string square71x71Logo);
                                                                    appxManifestApplication.GetStringValue("StartPage", out string startPage);
                                                                    appxManifestApplication.GetStringValue("Tall150x310Logo", out string tall150x310Logo);
                                                                    appxManifestApplication.GetStringValue("VisualGroup", out string visualGroup);
                                                                    appxManifestApplication.GetStringValue("WideLogo", out string wideLogo);
                                                                    appxManifestApplication.GetStringValue("Wide310x150Logo", out string wide310x150Logo);
                                                                }

                                                                applicationsEnumerator.MoveNext(out _);
                                                            }
                                                        }

                                                        // 获取应用包定义的功能列表
                                                        appxManifestReader.GetCapabilities(out APPX_CAPABILITIES capabilities);

                                                        // 获取应用包定义的静态依赖项列表
                                                        if (appxManifestReader.GetPackageDependencies(out IAppxManifestPackageDependenciesEnumerator dependenciesEnumerator) is 0)
                                                        {
                                                            while (dependenciesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                            {
                                                                if (dependenciesEnumerator.GetCurrent(out IAppxManifestPackageDependency2 appxManifestPackageDependency) is 0)
                                                                {
                                                                    appxManifestPackageDependency.GetMinVersion(out ulong dependencyMinVersion);
                                                                    appxManifestPackageDependency.GetName(out string dependencyName);
                                                                    appxManifestPackageDependency.GetPublisher(out string dependencyPublisher);
                                                                    appxManifestPackageDependency.GetMaxMajorVersionTested(out ushort dependencyMaxMajorVersionTested);
                                                                }

                                                                dependenciesEnumerator.MoveNext(out _);
                                                            }
                                                        }

                                                        // 获取应用包定义的包标识符
                                                        if (appxManifestReader.GetPackageId(out IAppxManifestPackageId2 packageId) is 0)
                                                        {
                                                            packageId.GetArchitecture2(out ProcessorArchitecture architecture);
                                                            packageId.GetName(out string name);
                                                            packageId.GetPackageFamilyName(out string packageFamilyName);
                                                            packageId.GetPackageFullName(out string packageFullName);
                                                            packageId.GetPublisher(out string publisher);
                                                            packageId.GetResourceId(out string resourceId);
                                                            packageId.GetVersion(out ulong version);
                                                        }

                                                        // 获取应用包定义的先决条件：最小系统版本号和最大测试系统版本号
                                                        appxManifestReader.GetPrerequisite("OSMinVersion", out ulong minVersion);
                                                        appxManifestReader.GetPrerequisite("OSMaxVersionTested", out ulong maxVersion);

                                                        // 获取应用包的属性
                                                        if (appxManifestReader.GetProperties(out IAppxManifestProperties packageProperties) is 0)
                                                        {
                                                            packageProperties.GetBoolValue("Framework", out bool isFramework);

                                                            packageProperties.GetStringValue("Description", out string description);
                                                            packageProperties.GetStringValue("DisplayName", out string displayName);
                                                            packageProperties.GetStringValue("Logo", out string logo);
                                                            packageProperties.GetStringValue("PublisherDisplayName", out string publisherDisplayName);
                                                        }

                                                        // 获取应用包定义的资源
                                                        if (appxManifestReader.GetResources(out IAppxManifestResourcesEnumerator appxManifestResourcesEnumerator) is 0)
                                                        {
                                                            while (appxManifestResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                            {
                                                                appxManifestResourcesEnumerator.GetCurrent(out string resource);
                                                                appxManifestResourcesEnumerator.MoveNext(out _);
                                                            }
                                                        }

                                                        // 获取应用包定义的限定资源
                                                        if (appxManifestReader.GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
                                                        {
                                                            while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                            {
                                                                if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                                                                {
                                                                }

                                                                appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // 解析以 appinstaller 格式结尾的应用包
                    else if (extensionName.Equals(".appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        // 读取应用包内容失败，请检查文件是否存在问题
                        if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out IntPtr appxFactoryPtr) is 0)
                        {
                            IAppxFactory3 appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.Unwrap);
                            appxFactory?.CreateAppInstallerReader(fileStream, null, out IntPtr appInstallerReader);
                        }
                    }

                    Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                }
            }
        }
    }
}

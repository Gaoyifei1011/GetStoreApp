using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.UI.Backdrop;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Ole32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shlwapi;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WinRT.Interop;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppInstaller.Pages
{
    /// <summary>
    /// 应用主页面
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private readonly Guid CLSID_AppxFactory = new("5842A140-FF9F-4166-8F5C-62F5B7B0C781");
        private readonly Guid CLSID_AppxBundleFactory = new("378E0446-5384-43B7-8877-E7DBDD883446");
        private readonly PackageManager packageManager = new();
        private readonly AppActivationArguments appActivationArguments;

        private string fileName = string.Empty;
        private IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installPackageWithProgress;

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

        private bool _isParseSuccessfully;

        public bool IsParseSuccessfully
        {
            get { return _isParseSuccessfully; }

            set
            {
                if (!Equals(_isParseSuccessfully, value))
                {
                    _isParseSuccessfully = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsParseSuccessfully)));
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

        private Version _minVersion;

        public Version MinVersion
        {
            get { return _minVersion; }

            set
            {
                if (!Equals(_minVersion, value))
                {
                    _minVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinVersion)));
                }
            }
        }

        private Version _maxVersionTested;

        public Version MaxVersionTested
        {
            get { return _maxVersionTested; }

            set
            {
                if (!Equals(_maxVersionTested, value))
                {
                    _maxVersionTested = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxVersionTested)));
                }
            }
        }

        private string _packageFamilyName;

        public string PackageFamilyName
        {
            get { return _packageFamilyName; }

            set
            {
                if (!Equals(_packageFamilyName, value))
                {
                    _packageFamilyName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageFamilyName)));
                }
            }
        }

        private string _packageFullName;

        public string PackageFullName
        {
            get { return _packageFullName; }

            set
            {
                if (!Equals(_packageFullName, value))
                {
                    _packageFullName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageFullName)));
                }
            }
        }

        private string _supportedArchitecture;

        public string SupportedArchitecture
        {
            get { return _supportedArchitecture; }

            set
            {
                if (!Equals(_supportedArchitecture, value))
                {
                    _supportedArchitecture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SupportedArchitecture)));
                }
            }
        }

        private string _isFramework;

        public string IsFramework
        {
            get { return _isFramework; }

            set
            {
                if (!Equals(_isFramework, value))
                {
                    _isFramework = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFramework)));
                }
            }
        }

        private string _packageType;

        public string PackageType
        {
            get { return _packageType; }

            set
            {
                if (!Equals(_packageType, value))
                {
                    _packageType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageType)));
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

        private List<string> DependencyList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage(AppActivationArguments activationArguments)
        {
            InitializeComponent();
            Background = new MicaBrush(MicaKind.BaseAlt, true);
            appActivationArguments = activationArguments;
            Program.SetTitleBarTheme(ActualTheme);
            Program.SetClassicMenuTheme(ActualTheme);
        }

        #region 第一部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MOVE, 0);
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
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_CLOSE, 0);
        }

        #endregion 第一部分：窗口右键菜单事件

        #region 第二部分：应用安装器主页面——挂载的事件

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            Program.SetTitleBarTheme(sender.ActualTheme);
            Program.SetClassicMenuTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 主页面初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            IsLoadCompleted = false;
            IsParseSuccessfully = false;
            PackageIconImage = null;
            PackageName = string.Empty;
            Publisher = string.Empty;
            Version = new Version();
            MinVersion = new Version();
            MaxVersionTested = new Version();
            PackageFamilyName = string.Empty;
            PackageFullName = string.Empty;
            SupportedArchitecture = string.Empty;
            IsFramework = string.Empty;
            PackageType = string.Empty;
            IsLoadCompleted = true;

            // 正常启动
            if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                return;
            }
            // 从文件处启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                FileActivatedEventArgs fileActivatedEventArgs = appActivationArguments.Data as FileActivatedEventArgs;

                // 只解析读取到的第一个文件
                if (fileActivatedEventArgs.Files.Count > 0)
                {
                    Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(() =>
                    {
                        return ParsePackagedApp(fileActivatedEventArgs.Files[0].Path);
                    });

                    IsParseSuccessfully = parseResult.Item1;

                    if (IsParseSuccessfully)
                    {
                    }
                }

                IsLoadCompleted = true;
            }
        }

        /// <summary>
        /// 选择安装包
        /// </summary>
        private async void OnOpenPackageClicked(object sender, RoutedEventArgs args)
        {
            // 先使用 FileOpenPicker，FileOpenPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件，否则提示打开自定义文件选取框失败
            bool result = false;

            // 使用 FileOpenPicker
            try
            {
                FileOpenPicker fileOpenPicker = new();
                InitializeWithWindow.Initialize(fileOpenPicker, Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id));
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;

                if (await fileOpenPicker.PickSingleFileAsync() is StorageFile storageFile)
                {
                    fileName = storageFile.Path;
                }
                result = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Open fileOpenPicker failed", e);
            }

            // 使用 IFileDialog
            if (!result)
            {
                try
                {
                    OpenFileDialog openFileDialog = new(Program.CoreAppWindow.Id)
                    {
                        Description = ResourceService.GetLocalized("Installer/SelectPackage"),
                    };

                    if (openFileDialog.ShowDialog())
                    {
                        fileName = openFileDialog.SelectedFile;
                    }

                    result = true;
                    openFileDialog.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                }
            }
        }

        /// <summary>
        /// 添加依赖包
        /// </summary>
        private async void OnAddDependencyClicked(object sender, RoutedEventArgs args)
        {
            // 先使用 FileOpenPicker，FileOpenPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件，否则提示打开自定义文件选取框失败
            bool result = false;

            // 使用 FileOpenPicker
            try
            {
                FileOpenPicker fileOpenPicker = new();
                InitializeWithWindow.Initialize(fileOpenPicker, Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id));
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;

                IReadOnlyList<StorageFile> filesList = await fileOpenPicker.PickMultipleFilesAsync();

                if (filesList is not null)
                {
                    foreach (StorageFile file in filesList)
                    {
                        DependencyList.Add(file.Path);
                    }
                }

                result = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Open fileOpenPicker failed", e);
            }

            // 使用 IFileDialog
            if (!result)
            {
                try
                {
                    OpenFileDialog openFileDialog = new(Program.CoreAppWindow.Id)
                    {
                        Description = ResourceService.GetLocalized("Installer/SelectDependencyPackage"),
                        AllowMultiSelect = true
                    };

                    if (openFileDialog.ShowDialog())
                    {
                        fileName = openFileDialog.SelectedFile;
                    }

                    result = true;
                    openFileDialog.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                }
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallAppClicked(object sender, RoutedEventArgs args)
        {
            await Task.Run(() =>
            {
                string extensionName = Path.GetExtension(fileName);

                if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        List<Uri> dependencyUriList = [];

                        foreach (string dependencyItem in DependencyList)
                        {
                            dependencyUriList.Add(new Uri(dependencyItem));
                        }

                        // 安装目标应用，并获取安装进度
                        installPackageWithProgress = packageManager.AddPackageAsync(new Uri(fileName), dependencyUriList, DeploymentOptions.ForceUpdateFromAnyVersion | DeploymentOptions.ForceTargetApplicationShutdown);

                        // 更新安装进度
                        installPackageWithProgress.Progress = (result, progress) => OnInstallPackageProgressing(result, progress);

                        // 应用安装过程已结束
                        installPackageWithProgress.Completed = (result, status) => OnInstallPackageCompleted(result, status);
                    }
                    // 安装失败显示失败信息
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                    }
                }
                else if (extensionName.Equals(".appinstaller"))
                {
                    try
                    {
                        // 安装目标应用，并获取安装进度
                        IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installPackageWithProgress = packageManager.AddPackageByAppInstallerFileAsync(new Uri(fileName), AddPackageByAppInstallerOptions.ForceTargetAppShutdown, null);

                        // 更新安装进度
                        installPackageWithProgress.Progress = (result, progress) => OnInstallPackageProgressing(result, progress);

                        // 应用安装过程已结束
                        installPackageWithProgress.Completed = (result, status) => OnInstallPackageCompleted(result, status);
                    }
                    // 安装失败显示失败信息
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                    }
                }
            });
        }

        /// <summary>
        /// 取消安装
        /// </summary>
        private void OnCancelInstallClicked(object sender, RoutedEventArgs args)
        {
            if (installPackageWithProgress is not null)
            {
                Task.Run(() =>
                {
                    installPackageWithProgress.Cancel();
                    installPackageWithProgress.Close();
                    installPackageWithProgress = null;
                });
            }
        }

        #endregion 第二部分：应用安装器主页面——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 应用安装状态发生改变时触发的事件
        /// </summary>
        private async void OnInstallPackageProgressing(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, DeploymentProgress progress)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                InstallProgressValue = progress.percentage;
            });
        }

        /// <summary>
        /// 应用安装完成时触发的事件
        /// </summary>
        private async void OnInstallPackageCompleted(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, AsyncStatus status)
        {
            // 安装完成
            if (status is AsyncStatus.Completed)
            {
                //// 显示安装成功通知
                //AppNotificationBuilder appNotificationBuilder = new();
                //appNotificationBuilder.AddArgument("action", "OpenApp");
                //appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallSuccessfully"), completedFile.Name));
                //ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
            }
            // 安装错误
            else if (status is AsyncStatus.Error)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                });

                //// 显示安装失败通知
                //AppNotificationBuilder appNotificationBuilder = new();
                //appNotificationBuilder.AddArgument("action", "OpenApp");
                //appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallFailed1"), completedFile.Name));
                //appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallFailed2"), result.ErrorCode.Message));
                //ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
            }

            result.Close();
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 解析应用包
        /// </summary>
        public Tuple<bool, Dictionary<string, object>> ParsePackagedApp(string filePath)
        {
            bool parseResult = false;
            Dictionary<string, object> parseDict = [];

            try
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
                                    parseResult = true;

                                    // 读取应用包内容
                                    if (appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                    {
                                        // 读取应用包清单
                                        if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0)
                                        {
                                            // 获取应用包定义的应用程序列表
                                            if (appxManifestReader.GetApplications(out IAppxManifestApplicationsEnumerator applicationsEnumerator) is 0)
                                            {
                                                List<Dictionary<string, string>> applicationInfoList = [];

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

                                                        Dictionary<string, string> applictionInfoDict = [];
                                                        applictionInfoDict.TryAdd("AppUserModelId", appUserModelId);
                                                        applictionInfoDict.TryAdd("ID", id);
                                                        applictionInfoDict.TryAdd("ShortName", shortName);
                                                        applicationInfoList.Add(applictionInfoDict);
                                                    }

                                                    applicationsEnumerator.MoveNext(out _);
                                                }

                                                parseDict.TryAdd("Application", applicationInfoList);
                                            }

                                            // 获取应用包定义的功能列表
                                            appxManifestReader.GetCapabilities(out APPX_CAPABILITIES capabilities);
                                            parseDict.TryAdd("Capabilities", capabilities);

                                            // 获取应用包定义的静态依赖项列表
                                            if (appxManifestReader.GetPackageDependencies(out IAppxManifestPackageDependenciesEnumerator dependenciesEnumerator) is 0)
                                            {
                                                List<Dictionary<string, object>> dependencyInfoList = [];

                                                while (dependenciesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                {
                                                    if (dependenciesEnumerator.GetCurrent(out IAppxManifestPackageDependency2 appxManifestPackageDependency) is 0)
                                                    {
                                                        appxManifestPackageDependency.GetMinVersion(out ulong dependencyMinVersion);
                                                        appxManifestPackageDependency.GetName(out string dependencyName);
                                                        appxManifestPackageDependency.GetPublisher(out string dependencyPublisher);
                                                        appxManifestPackageDependency.GetMaxMajorVersionTested(out ushort dependencyMaxMajorVersionTested);

                                                        Dictionary<string, object> dependencyInfoDict = [];

                                                        PackageVersion dependencyMinPackageVersion = new(dependencyMinVersion);
                                                        dependencyInfoDict.TryAdd("DependencyMinVersion", new Version(dependencyMinPackageVersion.Major, dependencyMinPackageVersion.Minor, dependencyMinPackageVersion.Build, dependencyMinPackageVersion.Revision));
                                                        dependencyInfoDict.TryAdd("DependencyName", dependencyName);
                                                        dependencyInfoDict.TryAdd("DependencyPublisher", dependencyPublisher);

                                                        PackageVersion dependencyMaxPackageMajorVersionTested = new(dependencyMaxMajorVersionTested);
                                                        dependencyInfoDict.TryAdd("DependencyMaxMajorVersionTested", new Version(dependencyMaxPackageMajorVersionTested.Major, dependencyMaxPackageMajorVersionTested.Minor, dependencyMaxPackageMajorVersionTested.Build, dependencyMaxPackageMajorVersionTested.Revision));
                                                        dependencyInfoList.Add(dependencyInfoDict);
                                                    }

                                                    dependenciesEnumerator.MoveNext(out _);
                                                }

                                                parseDict.TryAdd("Dependency", dependencyInfoList);
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

                                                parseDict.TryAdd("ProcessorArchitecture", architecture);
                                                parseDict.TryAdd("Name", name);
                                                parseDict.TryAdd("PackageFamilyName", packageFamilyName);
                                                parseDict.TryAdd("PackageFullName", packageFullName);
                                                parseDict.TryAdd("Publisher", publisher);
                                                parseDict.TryAdd("ResourceId", resourceId);

                                                PackageVersion packageVersion = new(version);
                                                parseDict.TryAdd("Version", new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
                                            }

                                            // 获取应用包定义的先决条件：最小系统版本号和最大测试系统版本号
                                            appxManifestReader.GetPrerequisite("OSMinVersion", out ulong minVersion);
                                            appxManifestReader.GetPrerequisite("OSMaxVersionTested", out ulong maxMajorVersionTested);

                                            PackageVersion minPackageVersion = new(minVersion);
                                            parseDict.TryAdd("MinVersion", new Version(minPackageVersion.Major, minPackageVersion.Minor, minPackageVersion.Build, minPackageVersion.Revision));
                                            PackageVersion maxPackageMajorVersionTested = new(maxMajorVersionTested);
                                            parseDict.TryAdd("MaxMajorVersionTested", new Version(maxPackageMajorVersionTested.Major, maxPackageMajorVersionTested.Minor, maxPackageMajorVersionTested.Build, maxPackageMajorVersionTested.Revision));

                                            // 获取应用包的属性
                                            if (appxManifestReader.GetProperties(out IAppxManifestProperties packageProperties) is 0)
                                            {
                                                packageProperties.GetBoolValue("Framework", out bool isFramework);

                                                packageProperties.GetStringValue("Description", out string description);
                                                packageProperties.GetStringValue("DisplayName", out string displayName);
                                                packageProperties.GetStringValue("Logo", out string logo);
                                                packageProperties.GetStringValue("PublisherDisplayName", out string publisherDisplayName);

                                                parseDict.TryAdd("IsFramework", isFramework);
                                                parseDict.TryAdd("Description", description);
                                                parseDict.TryAdd("PublisherDisplayName", publisherDisplayName);
                                            }

                                            // 获取应用包定义的资源
                                            if (appxManifestReader.GetResources(out IAppxManifestResourcesEnumerator appxManifestResourcesEnumerator) is 0)
                                            {
                                                List<string> resourceList = [];

                                                while (appxManifestResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                {
                                                    appxManifestResourcesEnumerator.GetCurrent(out string resource);
                                                    resourceList.Add(resource);

                                                    appxManifestResourcesEnumerator.MoveNext(out _);
                                                }

                                                parseDict.TryAdd("Resource", resourceList);
                                            }

                                            // 获取应用包定义的限定资源
                                            if (appxManifestReader.GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
                                            {
                                                List<string> qualifiedResourceList = [];

                                                while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                {
                                                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                                                    {
                                                        qualifiedResourceList.Add(language);
                                                    }

                                                    appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                                                }

                                                parseDict.TryAdd("QualifiedResource", qualifiedResourceList);
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
                                                    parseResult = true;

                                                    // 读取应用包内容
                                                    if (appxFactory.CreatePackageReader2(parseFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                                    {
                                                        // 读取应用包清单
                                                        if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0)
                                                        {
                                                            // 获取应用包定义的应用程序列表
                                                            if (appxManifestReader.GetApplications(out IAppxManifestApplicationsEnumerator applicationsEnumerator) is 0)
                                                            {
                                                                List<Dictionary<string, string>> applicationInfoList = [];

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

                                                                        Dictionary<string, string> applictionInfoDict = [];
                                                                        applictionInfoDict.TryAdd("AppUserModelId", appUserModelId);
                                                                        applictionInfoDict.TryAdd("ID", id);
                                                                        applictionInfoDict.TryAdd("ShortName", shortName);
                                                                        applicationInfoList.Add(applictionInfoDict);
                                                                    }

                                                                    applicationsEnumerator.MoveNext(out _);
                                                                }

                                                                parseDict.TryAdd("Application", applicationInfoList);
                                                            }

                                                            // 获取应用包定义的功能列表
                                                            appxManifestReader.GetCapabilities(out APPX_CAPABILITIES capabilities);
                                                            parseDict.TryAdd("Capabilities", capabilities);

                                                            // 获取应用包定义的静态依赖项列表
                                                            if (appxManifestReader.GetPackageDependencies(out IAppxManifestPackageDependenciesEnumerator dependenciesEnumerator) is 0)
                                                            {
                                                                List<Dictionary<string, object>> dependencyInfoList = [];

                                                                while (dependenciesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                                {
                                                                    if (dependenciesEnumerator.GetCurrent(out IAppxManifestPackageDependency2 appxManifestPackageDependency) is 0)
                                                                    {
                                                                        appxManifestPackageDependency.GetMinVersion(out ulong dependencyMinVersion);
                                                                        appxManifestPackageDependency.GetName(out string dependencyName);
                                                                        appxManifestPackageDependency.GetPublisher(out string dependencyPublisher);
                                                                        appxManifestPackageDependency.GetMaxMajorVersionTested(out ushort dependencyMaxMajorVersionTested);

                                                                        Dictionary<string, object> dependencyInfoDict = [];

                                                                        PackageVersion dependencyMinPackageVersion = new(dependencyMinVersion);
                                                                        dependencyInfoDict.TryAdd("DependencyMinVersion", new Version(dependencyMinPackageVersion.Major, dependencyMinPackageVersion.Minor, dependencyMinPackageVersion.Build, dependencyMinPackageVersion.Revision));
                                                                        dependencyInfoDict.TryAdd("DependencyName", dependencyName);
                                                                        dependencyInfoDict.TryAdd("DependencyPublisher", dependencyPublisher);

                                                                        PackageVersion dependencyMaxPackageMajorVersionTested = new(dependencyMaxMajorVersionTested);
                                                                        dependencyInfoDict.TryAdd("DependencyMaxMajorVersionTested", new Version(dependencyMaxPackageMajorVersionTested.Major, dependencyMaxPackageMajorVersionTested.Minor, dependencyMaxPackageMajorVersionTested.Build, dependencyMaxPackageMajorVersionTested.Revision));
                                                                        dependencyInfoList.Add(dependencyInfoDict);
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

                                                                parseDict.TryAdd("ProcessorArchitecture", architecture);
                                                                parseDict.TryAdd("Name", name);
                                                                parseDict.TryAdd("PackageFamilyName", packageFamilyName);
                                                                parseDict.TryAdd("PackageFullName", packageFullName);
                                                                parseDict.TryAdd("Publisher", publisher);
                                                                parseDict.TryAdd("ResourceId", resourceId);

                                                                PackageVersion packageVersion = new(version);
                                                                parseDict.TryAdd("Version", new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
                                                            }

                                                            // 获取应用包定义的先决条件：最小系统版本号和最大测试系统版本号
                                                            appxManifestReader.GetPrerequisite("OSMinVersion", out ulong minVersion);
                                                            appxManifestReader.GetPrerequisite("OSMaxVersionTested", out ulong maxMajorVersionTested);

                                                            PackageVersion minPackageVersion = new(minVersion);
                                                            parseDict.TryAdd("MinVersion", new Version(minPackageVersion.Major, minPackageVersion.Minor, minPackageVersion.Build, minPackageVersion.Revision));
                                                            PackageVersion maxPackageMajorVersionTested = new(maxMajorVersionTested);
                                                            parseDict.TryAdd("MaxMajorVersionTested", new Version(maxPackageMajorVersionTested.Major, maxPackageMajorVersionTested.Minor, maxPackageMajorVersionTested.Build, maxPackageMajorVersionTested.Revision));

                                                            // 获取应用包的属性
                                                            if (appxManifestReader.GetProperties(out IAppxManifestProperties packageProperties) is 0)
                                                            {
                                                                packageProperties.GetBoolValue("Framework", out bool isFramework);

                                                                packageProperties.GetStringValue("Description", out string description);
                                                                packageProperties.GetStringValue("DisplayName", out string displayName);
                                                                packageProperties.GetStringValue("Logo", out string logo);
                                                                packageProperties.GetStringValue("PublisherDisplayName", out string publisherDisplayName);

                                                                parseDict.TryAdd("IsFramework", isFramework);
                                                                parseDict.TryAdd("Description", description);
                                                                parseDict.TryAdd("PublisherDisplayName", publisherDisplayName);
                                                            }

                                                            // 获取应用包定义的资源
                                                            if (appxManifestReader.GetResources(out IAppxManifestResourcesEnumerator appxManifestResourcesEnumerator) is 0)
                                                            {
                                                                List<string> resourceList = [];

                                                                while (appxManifestResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                                {
                                                                    appxManifestResourcesEnumerator.GetCurrent(out string resource);
                                                                    resourceList.Add(resource);

                                                                    appxManifestResourcesEnumerator.MoveNext(out _);
                                                                }

                                                                parseDict.TryAdd("Resource", resourceList);
                                                            }

                                                            // 获取应用包定义的限定资源
                                                            if (appxManifestReader.GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
                                                            {
                                                                List<string> qualifiedResourceList = [];

                                                                while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                                                                {
                                                                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                                                                    {
                                                                        qualifiedResourceList.Add(language);
                                                                    }

                                                                    appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                                                                }

                                                                parseDict.TryAdd("QualifiedResource", qualifiedResourceList);
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, string.Format("Parse package {0} failed", filePath), e);
            }

            return Tuple.Create(parseResult, parseDict);
        }
    }
}

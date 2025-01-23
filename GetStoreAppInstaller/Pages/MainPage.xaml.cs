using GetStoreAppInstaller.Extensions.DataType.Enums;
using GetStoreAppInstaller.Extensions.DataType.Methods;
using GetStoreAppInstaller.Extensions.PriExtract;
using GetStoreAppInstaller.Helpers.Controls.Extensions;
using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.Services.Controls.Settings;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.UI.Backdrop;
using GetStoreAppInstaller.UI.TeachingTips;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Ole32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.SHCore;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Globalization;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using WinRT;
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

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
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

        private bool _canDragFile;

        public bool CanDragFile
        {
            get { return _canDragFile; }

            set
            {
                if (!Equals(_canDragFile, value))
                {
                    _canDragFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanDragFile)));
                }
            }
        }

        private bool _isParseEmpty;

        public bool IsParseEmpty
        {
            get { return _isParseEmpty; }

            set
            {
                if (!Equals(_isParseEmpty, value))
                {
                    _isParseEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsParseEmpty)));
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

        private string _publisherDisplayName;

        public string PublisherDisplayName
        {
            get { return _publisherDisplayName; }

            set
            {
                if (!Equals(_publisherDisplayName, value))
                {
                    _publisherDisplayName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublisherDisplayName)));
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

        private string _packageDescription;

        public string PackageDescription
        {
            get { return _packageDescription; }

            set
            {
                if (!Equals(_packageDescription, value))
                {
                    _packageDescription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageDescription)));
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

        private bool _isCancelInstall;

        public bool IsCancelInstall
        {
            get { return _isCancelInstall; }

            set
            {
                if (!Equals(_isCancelInstall, value))
                {
                    _isCancelInstall = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCancelInstall)));
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

        private ObservableCollection<TargetDeviceFamilyModel> TargetDeviceFamilyCollection { get; } = [];

        private ObservableCollection<DependencyModel> DependencyCollection { get; } = [];

        private ObservableCollection<string> CapabilitiesCollection { get; } = [];

        private ObservableCollection<ApplicationModel> ApplicationCollection { get; } = [];

        private ObservableCollection<Language> LanguageCollection { get; } = [];

        private ObservableCollection<InstallDependencyModel> InstallDependencyCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage(AppActivationArguments activationArguments)
        {
            InitializeComponent();
            appActivationArguments = activationArguments;

            WindowTheme = ThemeService.AppTheme.Equals(ThemeService.ThemeList[0])
                ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark
                : Enum.TryParse(ThemeService.AppTheme.Key, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;

            Program.SetTitleBarTheme(ActualTheme);
            Program.SetClassicMenuTheme(ActualTheme);

            if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[1]))
            {
                Background = new MicaBrush(MicaKind.Base, AlwaysShowBackdropService.AlwaysShowBackdropValue);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[2]))
            {
                Background = new MicaBrush(MicaKind.BaseAlt, AlwaysShowBackdropService.AlwaysShowBackdropValue);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[3]))
            {
                Background = new DesktopAcrylicBrush(DesktopAcrylicKind.Default, AlwaysShowBackdropService.AlwaysShowBackdropValue, true);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[4]))
            {
                Background = new DesktopAcrylicBrush(DesktopAcrylicKind.Base, AlwaysShowBackdropService.AlwaysShowBackdropValue, true);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[5]))
            {
                Background = new DesktopAcrylicBrush(DesktopAcrylicKind.Thin, AlwaysShowBackdropService.AlwaysShowBackdropValue, true);
            }
            else
            {
                VisualStateManager.GoToState(this, "BackgroundDefault", false);
            }
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 拖拽安装包
        /// </summary>
        protected override async void OnDragEnter(DragEventArgs args)
        {
            base.OnDragEnter(args);
            DragOperationDeferral deferral = args.GetDeferral();

            try
            {
                IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                if (dragItemsList.Count is 1)
                {
                    string extensionName = Path.GetExtension(dragItemsList[0].Name);

                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        args.AcceptedOperation = DataPackageOperation.Copy;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/OpenPackage");
                    }
                    else if (extensionName.Equals(".appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        args.AcceptedOperation = DataPackageOperation.Copy;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/OpenInstallerFile");
                    }
                    else
                    {
                        args.AcceptedOperation = DataPackageOperation.None;
                        args.DragUIOverride.IsCaptionVisible = true;
                        args.DragUIOverride.IsContentVisible = false;
                        args.DragUIOverride.IsGlyphVisible = true;
                        args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/UnsupportedFileType");
                    }
                }
                else
                {
                    args.AcceptedOperation = DataPackageOperation.None;
                    args.DragUIOverride.IsCaptionVisible = true;
                    args.DragUIOverride.IsContentVisible = false;
                    args.DragUIOverride.IsGlyphVisible = true;
                    args.DragUIOverride.Caption = ResourceService.GetLocalized("Installer/UnsupportedMultiFiles");
                }

                args.Handled = true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Update drag enter information failed", e);
            }
            finally
            {
                deferral.Complete();
            }
        }

        /// <summary>
        /// 拖动文件完成后获取文件信息
        /// </summary>
        protected override async void OnDrop(DragEventArgs args)
        {
            base.OnDrop(args);
            DragOperationDeferral deferral = args.GetDeferral();
            DataPackageView view = args.DataView;
            fileName = string.Empty;

            if (view.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> dragItemsList = await args.DataView.GetStorageItemsAsync();

                if (dragItemsList.Count > 0)
                {
                    try
                    {
                        fileName = dragItemsList[0].Path;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Read file {0} information failed", dragItemsList[0].Path), e);
                    }
                }
            }

            deferral.Complete();

            if (!string.IsNullOrEmpty(fileName))
            {
                IsParseEmpty = false;
                ResetResult();

                Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                {
                    return await ParsePackagedAppAsync(fileName);
                });

                await UpdateResultAsync(parseResult);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：窗口右键菜单事件

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

        #endregion 第二部分：窗口右键菜单事件

        #region 第三部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 删除依赖包
        /// </summary>
        private void OnDeleteDependencyPackageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string dependencyFullName && !string.IsNullOrEmpty(dependencyFullName))
            {
                foreach (InstallDependencyModel installDependencyItem in InstallDependencyCollection)
                {
                    if (installDependencyItem.DependencyFullName.Equals(dependencyFullName))
                    {
                        InstallDependencyCollection.Remove(installDependencyItem);
                        break;
                    }
                }
            }
        }

        #endregion 第三部分：XamlUICommand 命令调用时挂载的事件

        #region 第四部分：应用安装器主页面——挂载的事件

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
            IsParseEmpty = true;
            CanDragFile = true;
            fileName = string.Empty;

            // 正常启动
            if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                LaunchActivatedEventArgs launchActivatedEventArgs = appActivationArguments.Data is WinRT.IInspectable inspectable ? LaunchActivatedEventArgs.FromAbi(inspectable.ThisPtr) : appActivationArguments.Data as LaunchActivatedEventArgs;

                if (!string.IsNullOrEmpty(launchActivatedEventArgs.Arguments))
                {
                    string executableFileName = Path.GetFileName(Environment.ProcessPath);
                    string[] argumentsArray = launchActivatedEventArgs.Arguments.Split(' ');

                    if (launchActivatedEventArgs.Arguments.Contains(executableFileName))
                    {
                        if (argumentsArray.Length >= 2)
                        {
                            fileName = argumentsArray[1];
                        }
                    }
                    else
                    {
                        if (argumentsArray.Length >= 1)
                        {
                            fileName = argumentsArray[0];
                        }
                    }

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        IsParseEmpty = false;
                        ResetResult();

                        Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                        {
                            return await ParsePackagedAppAsync(fileName);
                        });

                        await UpdateResultAsync(parseResult);
                    }
                }
            }
            // 从文件处启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.File)
            {
                FileActivatedEventArgs fileActivatedEventArgs = appActivationArguments.Data as FileActivatedEventArgs;

                // 只解析读取到的第一个文件
                if (fileActivatedEventArgs.Files.Count > 0)
                {
                    fileName = fileActivatedEventArgs.Files[0].Path;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        IsParseEmpty = false;
                        ResetResult();

                        Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                        {
                            return await ParsePackagedAppAsync(fileName);
                        });

                        await UpdateResultAsync(parseResult);
                    }
                }
            }
            // 从共享目标处启动
            else if (appActivationArguments.Kind is ExtendedActivationKind.ShareTarget)
            {
                ShareTargetActivatedEventArgs shareTargetActivatedEventArgs = appActivationArguments.Data as ShareTargetActivatedEventArgs;
                ShareOperation shareOperation = shareTargetActivatedEventArgs.ShareOperation;
                shareOperation.ReportCompleted();

                if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
                {
                    IReadOnlyList<IStorageItem> sharedFilesList = await shareOperation.Data.GetStorageItemsAsync();

                    if (sharedFilesList.Count > 0)
                    {
                        fileName = sharedFilesList[0].Path;

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            IsParseEmpty = false;
                            ResetResult();

                            Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                            {
                                return await ParsePackagedAppAsync(fileName);
                            });

                            await UpdateResultAsync(parseResult);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 选择安装包
        /// </summary>
        private async void OnOpenPackageClicked(object sender, RoutedEventArgs args)
        {
            // 先使用 FileOpenPicker，FileOpenPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件，否则提示打开自定义文件选取框失败
            bool result = false;
            bool hasSelectFile = false;

            // 使用 FileOpenPicker
            try
            {
                FileOpenPicker fileOpenPicker = new();
                InitializeWithWindow.Initialize(fileOpenPicker, Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id));
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");
                fileOpenPicker.FileTypeFilter.Add(".appinstaller");

                if (await fileOpenPicker.PickSingleFileAsync() is StorageFile storageFile)
                {
                    fileName = storageFile.Path;
                    hasSelectFile = true;
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
                        UseCustomFilterTypes = true
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add("*.appx");
                    openFileDialog.FileTypeFilter.Add("*.msix");
                    openFileDialog.FileTypeFilter.Add("*.appxbundle");
                    openFileDialog.FileTypeFilter.Add("*.msixbundle");
                    openFileDialog.FileTypeFilter.Add("*.appinstaller");

                    if (openFileDialog.ShowDialog())
                    {
                        fileName = openFileDialog.SelectedFile;
                        hasSelectFile = true;
                    }

                    result = true;
                    openFileDialog.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                }
            }

            if (result && hasSelectFile)
            {
                IsParseEmpty = false;
                ResetResult();

                Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                {
                    return await ParsePackagedAppAsync(fileName);
                });

                await UpdateResultAsync(parseResult);
            }
        }

        /// <summary>
        /// 选择其他的安装包
        /// </summary>
        private async void OnOpenOtherPackageClicked(object sender, RoutedEventArgs args)
        {
            // 先使用 FileOpenPicker，FileOpenPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件，否则提示打开自定义文件选取框失败
            bool result = false;
            bool hasSelectFile = false;

            // 使用 FileOpenPicker
            try
            {
                FileOpenPicker fileOpenPicker = new();
                InitializeWithWindow.Initialize(fileOpenPicker, Win32Interop.GetWindowFromWindowId(Program.CoreAppWindow.Id));
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");
                fileOpenPicker.FileTypeFilter.Add(".appinstaller");

                if (await fileOpenPicker.PickSingleFileAsync() is StorageFile storageFile)
                {
                    fileName = storageFile.Path;
                    hasSelectFile = true;
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
                        UseCustomFilterTypes = true
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add("*.appx");
                    openFileDialog.FileTypeFilter.Add("*.msix");
                    openFileDialog.FileTypeFilter.Add("*.appxbundle");
                    openFileDialog.FileTypeFilter.Add("*.msixbundle");
                    openFileDialog.FileTypeFilter.Add("*.appinstaller");

                    if (openFileDialog.ShowDialog())
                    {
                        fileName = openFileDialog.SelectedFile;
                        hasSelectFile = true;
                    }

                    result = true;
                    openFileDialog.Dispose();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                }
            }

            if (result && hasSelectFile)
            {
                ResetResult();

                Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                {
                    return await ParsePackagedAppAsync(fileName);
                });

                await UpdateResultAsync(parseResult);
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
                fileOpenPicker.FileTypeFilter.Clear();
                fileOpenPicker.FileTypeFilter.Add(".appx");
                fileOpenPicker.FileTypeFilter.Add(".msix");
                fileOpenPicker.FileTypeFilter.Add(".appxbundle");
                fileOpenPicker.FileTypeFilter.Add(".msixbundle");

                IReadOnlyList<StorageFile> filesList = await fileOpenPicker.PickMultipleFilesAsync();

                if (filesList is not null)
                {
                    foreach (StorageFile file in filesList)
                    {
                        Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                        {
                            return await ParseDependencyAppAsync(file.Path);
                        });

                        if (parseResult.Item1)
                        {
                            Dictionary<string, object> parseDict = parseResult.Item2;

                            InstallDependencyCollection.Add(new InstallDependencyModel()
                            {
                                DependencyName = file.Name,
                                DependencyVersion = parseDict.TryGetValue("Version", out object versionObj) && versionObj is Version version ? version : new Version(),
                                DependencyPublisher = parseDict.TryGetValue("PublisherDisplayName", out object publisherDisplayNameObj) && publisherDisplayNameObj is string publisherDisplayName ? publisherDisplayName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown")),
                                DependencyFullName = parseDict.TryGetValue("PackageFullName", out object packageFullNameObj) && packageFullNameObj is string packageFullName ? packageFullName : Guid.NewGuid().ToString(),
                                DependencyPath = file.Path
                            });
                        }
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
                        AllowMultiSelect = true,
                        UseCustomFilterTypes = true
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add("*.appx");
                    openFileDialog.FileTypeFilter.Add("*.msix");
                    openFileDialog.FileTypeFilter.Add("*.appxbundle");
                    openFileDialog.FileTypeFilter.Add("*.msixbundle");

                    if (openFileDialog.ShowDialog())
                    {
                        foreach (string fileItem in openFileDialog.SelectedFileList)
                        {
                            InstallDependencyCollection.Add(new InstallDependencyModel()
                            {
                                DependencyName = Path.GetFileName(fileItem),
                                DependencyVersion = new Version(),
                                DependencyPublisher = string.Empty,
                                DependencyFullName = Guid.NewGuid().ToString(),
                                DependencyPath = fileItem
                            });
                        }
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
        /// 清空依赖包
        /// </summary>
        private void OnClearDependencyClicked(object sender, RoutedEventArgs args)
        {
            InstallDependencyCollection.Clear();
        }

        /// <summary>
        /// 复制错误原因
        /// </summary>
        private async void OnCopyErrorInformationClicked(object sender, RoutedEventArgs args)
        {
            if (ViewErrorInformationFlyout.IsOpen)
            {
                ViewErrorInformationFlyout.Hide();
            }

            if (!string.IsNullOrEmpty(InstallFailedInformation))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(InstallFailedInformation);

                await TeachingTipHelper.ShowAsync(new DataCopyTip(copyResult, false));
            }
        }

        /// <summary>
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            if ((sender as Button).Tag is string tag)
            {
                if (tag.Equals("AddDependencyFlyout", StringComparison.OrdinalIgnoreCase) && AddDependencyFlyout.IsOpen)
                {
                    AddDependencyFlyout.Hide();
                }
                else if (tag.Equals("ViewErrorInformationFlyout", StringComparison.OrdinalIgnoreCase) && ViewErrorInformationFlyout.IsOpen)
                {
                    ViewErrorInformationFlyout.Hide();
                }
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallAppClicked(object sender, RoutedEventArgs args)
        {
            if (File.Exists(fileName))
            {
                CanDragFile = false;
                IsInstalling = true;
                IsInstallFailed = false;
                InstallProgressValue = 0;
                InstallStateString = ResourceService.GetLocalized("Installer/PrepareInstall");

                await Task.Run(() =>
                {
                    string extensionName = Path.GetExtension(fileName);

                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            List<Uri> dependencyUriList = [];

                            foreach (InstallDependencyModel installDependencyItem in InstallDependencyCollection)
                            {
                                dependencyUriList.Add(new Uri(installDependencyItem.DependencyPath));
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
        }

        /// <summary>
        /// 取消安装
        /// </summary>
        private async void OnCancelInstallClicked(object sender, RoutedEventArgs args)
        {
            if (installPackageWithProgress is not null)
            {
                IsCancelInstall = false;
                await Task.Run(installPackageWithProgress.Cancel);
                IsCancelInstall = true;
            }
        }

        #endregion 第四部分：应用安装器主页面——挂载的事件

        #region 第五部分：自定义事件

        /// <summary>
        /// 应用安装状态发生改变时触发的事件
        /// </summary>
        private async void OnInstallPackageProgressing(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, DeploymentProgress progress)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (progress.state is DeploymentProgressState.Queued)
                {
                    IsInstalling = true;
                    IsInstallWaiting = true;
                    InstallStateString = ResourceService.GetLocalized("Installer/WaitInstall");
                }
                else if (progress.state is DeploymentProgressState.Processing)
                {
                    IsInstalling = true;
                    IsInstallWaiting = false;
                    InstallProgressValue = progress.percentage;
                    InstallStateString = string.Format(ResourceService.GetLocalized("Installer/InstallProgress"), progress.percentage);
                }
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
                // 显示安装成功通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallSuccessfully"), PackageName));
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());

                // 更新应用安装状态
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanDragFile = true;
                    IsInstalling = false;
                    InstallProgressValue = 0;
                    IsInstallWaiting = false;
                    IsInstallFailed = false;
                    InstallFailedInformation = string.Empty;
                });
            }
            // 安装错误
            else if (status is AsyncStatus.Error)
            {
                string errorMessage = result.ErrorCode.Message;

                // 显示安装失败通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallFailed"), PackageName));
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallFailedReason"), errorMessage));
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());

                // 更新应用安装状态
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanDragFile = true;
                    IsInstalling = false;
                    InstallProgressValue = 0;
                    IsInstallWaiting = false;
                    IsInstallFailed = true;
                    InstallFailedInformation = errorMessage;
                });
            }
            else if (status is AsyncStatus.Canceled)
            {
                // 更新应用安装状态
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CanDragFile = true;
                    IsInstalling = false;
                    InstallProgressValue = 0;
                    IsInstallWaiting = false;
                    IsInstallFailed = false;
                    InstallFailedInformation = string.Empty;
                });
            }

            result.Close();
        }

        #endregion 第五部分：自定义事件

        #region 第六部分：解析应用包信息

        /// <summary>
        /// 解析应用包
        /// </summary>
        private async Task<Tuple<bool, Dictionary<string, object>>> ParsePackagedAppAsync(string filePath)
        {
            bool parseResult = false;
            Dictionary<string, object> parseDict = [];

            try
            {
                if (File.Exists(filePath))
                {
                    // 获取文件的扩展文件名称
                    string extensionName = Path.GetExtension(filePath);

                    // 第一部分：解析以 appx 或 msix 格式结尾的单个应用包
                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase))
                    {
                        IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                        if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                        {
                            if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out IntPtr appxFactoryPtr) is 0)
                            {
                                IAppxFactory3 appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.Unwrap);

                                if (appxFactory is not null && appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                {
                                    parseResult = true;

                                    // 解析安装包所有文件
                                    Dictionary<string, IAppxFile> appxFileDict = ParsePackagePayloadFiles(appxPackageReader);

                                    // 获取并解析本地资源文件
                                    IStream resourceFileStream = GetPackageResourceFileStream(appxFileDict);
                                    Dictionary<string, Dictionary<string, string>> resourceDict = null;

                                    if (resourceFileStream is not null)
                                    {
                                        resourceDict = ParsePackageResources(resourceFileStream);
                                        Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(resourceFileStream, CreateComInterfaceFlags.None));
                                        resourceFileStream = null;
                                    }

                                    // 从资源文件中查找符合的语言
                                    Dictionary<string, string> specifiedLanguageResourceDict = GetSpecifiedLanguageResource(resourceDict);

                                    // 解析资源包清单文件
                                    Dictionary<string, object> parseManifestDict = ParsePackageManifest(appxPackageReader, false, specifiedLanguageResourceDict);

                                    foreach (KeyValuePair<string, object> parseManifsetItem in parseManifestDict)
                                    {
                                        parseDict.TryAdd(parseManifsetItem.Key, parseManifsetItem.Value);
                                    }

                                    // 获取应用包图标
                                    if (parseDict.TryGetValue("Logo", out object logoObj) && logoObj is string logo)
                                    {
                                        IStream imageFileStream = GetPackageLogo(logo, appxFileDict);
                                        parseDict.TryAdd("ImageLogo", imageFileStream);
                                    }
                                }
                            }

                            randomAccessStream?.Dispose();
                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                            randomAccessStream = null;
                            fileStream = null;
                        }
                    }

                    // 解析以 appxbundle 或 msixbundle 格式结尾的应用包
                    else if (extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Ole32Library.CoCreateInstance(CLSID_AppxBundleFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxBundleFactory2).GUID, out IntPtr appxBundleFactoryPtr) is 0)
                        {
                            IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                            if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                            {
                                IAppxBundleFactory2 appxBundleFactory = (IAppxBundleFactory2)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxBundleFactoryPtr, CreateObjectFlags.Unwrap);

                                if (appxBundleFactory is not null && appxBundleFactory.CreateBundleReader2(fileStream, null, out IAppxBundleReader appxBundleReader) is 0 && appxBundleReader.GetManifest(out IAppxBundleManifestReader appxBundleManifestReader) is 0)
                                {
                                    Dictionary<string, object> packageBundleManifestDict = ParsePackageBundleManifestInfo(appxBundleManifestReader);
                                    Dictionary<string, Dictionary<string, string>> resourceDict = [];

                                    if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out IntPtr appxFactoryPtr) is 0)
                                    {
                                        IAppxFactory3 appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.Unwrap);

                                        if (appxFactory is not null)
                                        {
                                            parseResult = true;

                                            // 从资源文件中查找符合的语言
                                            if (packageBundleManifestDict.TryGetValue("QualifiedResource", out object qualifiedResourceObj) && qualifiedResourceObj is Dictionary<string, string> qualifiedResourceDict)
                                            {
                                                List<string> qualifiedResourceList = [];

                                                foreach (KeyValuePair<string, string> qualifiedResourceItem in qualifiedResourceDict)
                                                {
                                                    // 获取安装包支持的语言
                                                    qualifiedResourceList.Add(qualifiedResourceItem.Key);
                                                }

                                                // 获取特定语言的资源文件
                                                List<KeyValuePair<string, string>> specifiedLanguageResourceList = GetPackageBundleSpecifiedLanguageResource(qualifiedResourceDict);

                                                foreach (KeyValuePair<string, string> specifiedLanguageResourceItem in specifiedLanguageResourceList)
                                                {
                                                    // 解析应用捆绑包对应符合的资源包
                                                    appxBundleReader.GetPayloadPackage(specifiedLanguageResourceItem.Value, out IAppxFile specifiedLanguageResourceFile);

                                                    if (specifiedLanguageResourceFile.GetStream(out IStream specifiedLanguageResourceFileStream) is 0 && appxFactory.CreatePackageReader2(specifiedLanguageResourceFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                                    {
                                                        // 解析资源包所有文件
                                                        Dictionary<string, IAppxFile> fileDict = ParsePackagePayloadFiles(appxPackageReader);

                                                        // 获取并解析本地资源文件
                                                        IStream resourceFileStream = GetPackageResourceFileStream(fileDict);

                                                        if (resourceFileStream is not null)
                                                        {
                                                            Dictionary<string, Dictionary<string, string>> parseResourceDict = ParsePackageResources(resourceFileStream);
                                                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(resourceFileStream, CreateComInterfaceFlags.None));

                                                            foreach (KeyValuePair<string, Dictionary<string, string>> parseResourceItem in parseResourceDict)
                                                            {
                                                                if (parseResourceItem.Key.Equals(specifiedLanguageResourceItem.Key, StringComparison.OrdinalIgnoreCase))
                                                                {
                                                                    resourceDict.TryAdd(specifiedLanguageResourceItem.Key, parseResourceItem.Value);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                qualifiedResourceList.Sort();
                                                parseDict.TryAdd("QualifiedResource", qualifiedResourceList);
                                            }

                                            // 从资源文件中查找符合的语言
                                            Dictionary<string, string> specifiedLanguageResourceDict = GetSpecifiedLanguageResource(resourceDict);

                                            // 解析应用
                                            if (packageBundleManifestDict.TryGetValue("Application", out object applicationObj) && applicationObj is Dictionary<ProcessorArchitecture, string> applicationDict)
                                            {
                                                string applicationFileName = ParsePacakgeBundleCompatibleFile(applicationDict);
                                                string architecture = ParsePackageBundleArchitecture(applicationDict);
                                                parseDict.TryAdd("ProcessorArchitecture", architecture);

                                                appxBundleReader.GetPayloadPackage(applicationFileName, out IAppxFile applicationFile);

                                                if (applicationFile.GetStream(out IStream applicationFileStream) is 0)
                                                {
                                                    if (appxFactory.CreatePackageReader2(applicationFileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                                    {
                                                        Dictionary<string, IAppxFile> fileDict = ParsePackagePayloadFiles(appxPackageReader);
                                                        Dictionary<string, object> parseManifestDict = ParsePackageManifest(appxPackageReader, true, specifiedLanguageResourceDict);

                                                        foreach (KeyValuePair<string, object> parseManifsetItem in parseManifestDict)
                                                        {
                                                            parseDict.TryAdd(parseManifsetItem.Key, parseManifsetItem.Value);
                                                        }

                                                        // 获取应用包图标
                                                        if (parseDict.TryGetValue("Logo", out object logoObj) && logoObj is string logo)
                                                        {
                                                            IStream imageFileStream = GetPackageLogo(logo, fileDict);
                                                            parseDict.TryAdd("ImageLogo", imageFileStream);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                randomAccessStream?.Dispose();
                                Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                                randomAccessStream = null;
                                fileStream = null;
                            }
                        }
                    }

                    // 解析以 appinstaller 格式结尾的应用安装文件
                    else if (extensionName.Equals(".appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        // 解析应用安装文件
                        XmlDocument xmlDocument = await XmlDocument.LoadFromFileAsync(await StorageFile.GetFileFromPathAsync(filePath));

                        if (xmlDocument is not null)
                        {
                            parseResult = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, string.Format("Parse package {0} failed", filePath), e);
            }

            return Tuple.Create(parseResult, parseDict);
        }

        /// <summary>
        /// 解析依赖应用包
        /// </summary>
        private async Task<Tuple<bool, Dictionary<string, object>>> ParseDependencyAppAsync(string filePath)
        {
            bool parseResult = false;
            Dictionary<string, object> parseDict = [];

            try
            {
                if (File.Exists(filePath))
                {
                    // 获取文件的扩展文件名称
                    string extensionName = Path.GetExtension(filePath);

                    // 第一部分：解析以 appx 或 msix 格式结尾的单个应用包
                    if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase))
                    {
                        IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                        if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                        {
                            if (Ole32Library.CoCreateInstance(CLSID_AppxFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxFactory3).GUID, out IntPtr appxFactoryPtr) is 0)
                            {
                                IAppxFactory3 appxFactory = (IAppxFactory3)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxFactoryPtr, CreateObjectFlags.Unwrap);

                                if (appxFactory is not null && appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                {
                                    parseResult = true;

                                    // 解析资源包清单文件
                                    Dictionary<string, object> parseManifsetDict = ParseDependencyPackageManifest(appxPackageReader);

                                    foreach (KeyValuePair<string, object> parseManifsetItem in parseManifsetDict)
                                    {
                                        parseDict.TryAdd(parseManifsetItem.Key, parseManifsetItem.Value);
                                    }
                                }
                            }

                            randomAccessStream?.Dispose();
                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                            randomAccessStream = null;
                            fileStream = null;
                        }
                    }

                    // 解析以 appxbundle 或 msixbundle 格式结尾的应用包
                    else if (extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Ole32Library.CoCreateInstance(CLSID_AppxBundleFactory, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IAppxBundleFactory2).GUID, out IntPtr appxBundleFactoryPtr) is 0)
                        {
                            IRandomAccessStream randomAccessStream = await FileRandomAccessStream.OpenAsync(filePath, FileAccessMode.Read);
                            if (randomAccessStream is not null && ShCoreLibrary.CreateStreamOverRandomAccessStream((randomAccessStream as IWinRTObject).NativeObject.ThisPtr, typeof(IStream).GUID, out IStream fileStream) is 0)
                            {
                                IAppxBundleFactory2 appxBundleFactory = (IAppxBundleFactory2)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(appxBundleFactoryPtr, CreateObjectFlags.Unwrap);

                                if (appxBundleFactory is not null && appxBundleFactory.CreateBundleReader2(fileStream, null, out IAppxBundleReader appxBundleReader) is 0)
                                {
                                    parseResult = true;

                                    // 解析资源包清单文件
                                    Dictionary<string, object> parseManifsetDict = ParseDependencyPackageBundleManifest(appxBundleReader);

                                    foreach (KeyValuePair<string, object> parseManifsetItem in parseManifsetDict)
                                    {
                                        parseDict.TryAdd(parseManifsetItem.Key, parseManifsetItem.Value);
                                    }
                                }

                                randomAccessStream?.Dispose();
                                Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(fileStream, CreateComInterfaceFlags.None));
                                randomAccessStream = null;
                                fileStream = null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, string.Format("Parse package {0} failed", filePath), e);
            }

            return Tuple.Create(parseResult, parseDict);
        }

        /// <summary>
        /// 获取应用程序入口信息
        /// </summary>
        private List<ApplicationModel> ParsePackageApplication(IAppxManifestReader3 appxManifestReader, Dictionary<string, string> specifiedLanguageResourceDict)
        {
            List<ApplicationModel> applicationList = [];

            if (appxManifestReader.GetApplications(out IAppxManifestApplicationsEnumerator appxManifestApplicationsEnumerator) is 0)
            {
                while (appxManifestApplicationsEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                {
                    appxManifestApplicationsEnumerator.GetCurrent(out IAppxManifestApplication appxManifestApplication);
                    appxManifestApplication.GetStringValue("Description", out string description);
                    appxManifestApplication.GetStringValue("EntryPoint", out string entryPoint);
                    appxManifestApplication.GetStringValue("Executable", out string executable);
                    appxManifestApplication.GetStringValue("ID", out string id);

                    description = GetLocalizedString(description, specifiedLanguageResourceDict);

                    ApplicationModel applicationItem = new()
                    {
                        AppDescription = description,
                        EntryPoint = entryPoint,
                        Executable = executable,
                        AppID = id
                    };

                    applicationList.Add(applicationItem);
                    appxManifestApplicationsEnumerator.MoveNext(out _);
                }
            }

            applicationList.Sort((item1, item2) => item1.AppID.CompareTo(item2.AppID));
            return applicationList;
        }

        private List<Dictionary<string, object>> ParsePackageDependencies(IAppxManifestReader3 appxManifestReader)
        {
            List<Dictionary<string, object>> dependencyList = [];

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

                        Dictionary<string, object> dependencyDict = [];

                        PackageVersion dependencyMinPackageVersion = new(dependencyMinVersion);
                        dependencyDict.TryAdd("DependencyMinVersion", new Version(dependencyMinPackageVersion.Major, dependencyMinPackageVersion.Minor, dependencyMinPackageVersion.Build, dependencyMinPackageVersion.Revision));
                        dependencyDict.TryAdd("DependencyName", dependencyName);
                        dependencyDict.TryAdd("DependencyPublisher", dependencyPublisher);

                        PackageVersion dependencyMaxPackageMajorVersionTested = new(dependencyMaxMajorVersionTested);
                        dependencyDict.TryAdd("DependencyMaxMajorVersionTested", new Version(dependencyMaxPackageMajorVersionTested.Major, dependencyMaxPackageMajorVersionTested.Minor, dependencyMaxPackageMajorVersionTested.Build, dependencyMaxPackageMajorVersionTested.Revision));
                        dependencyList.Add(dependencyDict);
                    }

                    dependenciesEnumerator.MoveNext(out _);
                }
            }

            return dependencyList;
        }

        /// <summary>
        /// 解析应用包清单
        /// </summary>
        private Dictionary<string, object> ParsePackageManifest(IAppxPackageReader appxPackageReader, bool isBundle, Dictionary<string, string> specifiedLanguageResourceDict)
        {
            Dictionary<string, object> parseDict = [];

            try
            {
                // 分段 4：读取应用包清单
                if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0)
                {
                    // 获取应用包定义的功能列表
                    appxManifestReader.GetCapabilities(out APPX_CAPABILITIES capabilities);
                    parseDict.TryAdd("Capabilities", capabilities);

                    // 获取应用包定义的静态依赖项列表
                    List<Dictionary<string, object>> dependencyList = ParsePackageDependencies(appxManifestReader);
                    parseDict.TryAdd("Dependency", dependencyList);

                    // 获取应用包定义的包标识符
                    if (appxManifestReader.GetPackageId(out IAppxManifestPackageId2 packageId) is 0)
                    {
                        packageId.GetArchitecture2(out ProcessorArchitecture architecture);
                        packageId.GetPackageFamilyName(out string packageFamilyName);
                        packageId.GetPackageFullName(out string packageFullName);
                        packageId.GetVersion(out ulong version);

                        if (!isBundle)
                        {
                            parseDict.TryAdd("ProcessorArchitecture", architecture.ToString());
                        }

                        parseDict.TryAdd("PackageFamilyName", packageFamilyName);
                        parseDict.TryAdd("PackageFullName", packageFullName);

                        PackageVersion packageVersion = new(version);
                        parseDict.TryAdd("Version", new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
                    }

                    // 获取包的目标设备系列
                    List<TargetDeviceFamilyModel> targetDeviceFamilyList = ParsePackageTargetDeviceFamily(appxManifestReader);
                    parseDict.TryAdd("TargetDeviceFamily", targetDeviceFamilyList);

                    // 获取应用入口信息
                    List<ApplicationModel> applicationList = ParsePackageApplication(appxManifestReader, specifiedLanguageResourceDict);
                    parseDict.TryAdd("Application", applicationList);

                    // 获取应用包的属性
                    if (appxManifestReader.GetProperties(out IAppxManifestProperties packageProperties) is 0)
                    {
                        packageProperties.GetBoolValue("Framework", out bool isFramework);

                        packageProperties.GetStringValue("Description", out string description);
                        packageProperties.GetStringValue("DisplayName", out string displayName);
                        packageProperties.GetStringValue("Logo", out string logo);
                        packageProperties.GetStringValue("PublisherDisplayName", out string publisherDisplayName);

                        description = GetLocalizedString(description, specifiedLanguageResourceDict);
                        displayName = GetLocalizedString(displayName, specifiedLanguageResourceDict);
                        publisherDisplayName = GetLocalizedString(publisherDisplayName, specifiedLanguageResourceDict);

                        parseDict.TryAdd("IsFramework", isFramework);
                        parseDict.TryAdd("Description", description);
                        parseDict.TryAdd("DisplayName", displayName);
                        parseDict.TryAdd("Logo", logo);
                        parseDict.TryAdd("PublisherDisplayName", publisherDisplayName);
                    }

                    if (!isBundle)
                    {
                        // 获取应用包定义的限定资源
                        List<string> qualifiedResourceList = ParsePackageQualifiedResources(appxManifestReader);
                        parseDict.TryAdd("QualifiedResource", qualifiedResourceList);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Parse package manifest failed", e);
            }

            return parseDict;
        }

        /// <summary>
        /// 解析应用包的所有文件
        /// </summary>
        private Dictionary<string, IAppxFile> ParsePackagePayloadFiles(IAppxPackageReader appxPackageReader)
        {
            Dictionary<string, IAppxFile> fileDict = [];

            if (appxPackageReader.GetPayloadFiles(out IAppxFilesEnumerator appxFilesEnumerator) is 0)
            {
                while (appxFilesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                {
                    appxFilesEnumerator.GetCurrent(out IAppxFile appxFile);
                    appxFile.GetName(out string packageFileName);
                    fileDict.Add(packageFileName, appxFile);

                    appxFilesEnumerator.MoveNext(out _);
                }
            }

            return fileDict;
        }

        /// <summary>
        /// 解析应用包定义的限定资源
        /// </summary>
        private List<string> ParsePackageQualifiedResources(IAppxManifestReader3 appxManifestReader)
        {
            List<string> qualifiedResourceList = [];

            if (appxManifestReader.GetQualifiedResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
            {
                while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                {
                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                    {
                        qualifiedResourceList.Add(language);
                    }

                    appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                }
            }

            qualifiedResourceList.Sort();
            return qualifiedResourceList;
        }

        /// <summary>
        /// 解析应用包的所有资源
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> ParsePackageResources(IStream resourceFileStream)
        {
            Dictionary<string, Dictionary<string, string>> resourceDict = [];

            try
            {
                // 分段 3 ：解析资源文件
                if (resourceFileStream is not null)
                {
                    ShCoreLibrary.CreateRandomAccessStreamOverStream(resourceFileStream, BSOS_OPTIONS.BSOS_DEFAULT, typeof(IRandomAccessStream).GUID, out IntPtr ppv);
                    RandomAccessStreamOverStream randomAccessStreamOverStream = RandomAccessStreamOverStream.FromAbi(ppv);
                    Stream resourceStream = randomAccessStreamOverStream.AsStream();

                    if (resourceStream is not null)
                    {
                        // 读取并检查文件类型
                        BinaryReader priBinaryReader = new(resourceStream, Encoding.ASCII, true);
                        long fileStartOffset = priBinaryReader.BaseStream.Position;
                        string priType = new(priBinaryReader.ReadChars(8));

                        if (priType is "mrm_pri0" || priType is "mrm_pri1" || priType is "mrm_pri2" || priType is "mrm_prif")
                        {
                            priBinaryReader.ExpectUInt16(0);
                            priBinaryReader.ExpectUInt16(1);
                            uint totalFileSize = priBinaryReader.ReadUInt32();
                            uint tocOffset = priBinaryReader.ReadUInt32();
                            uint sectionStartOffset = priBinaryReader.ReadUInt32();
                            uint numSections = priBinaryReader.ReadUInt16();
                            priBinaryReader.ExpectUInt16(0xFFFF);
                            priBinaryReader.ExpectUInt32(0);
                            priBinaryReader.BaseStream.Seek(fileStartOffset + totalFileSize - 16, SeekOrigin.Begin);
                            priBinaryReader.ExpectUInt32(0xDEFFFADE);
                            priBinaryReader.ExpectUInt32(totalFileSize);
                            priBinaryReader.ExpectString(priType);
                            priBinaryReader.BaseStream.Seek(tocOffset, SeekOrigin.Begin);

                            // 读取内容列表
                            List<TocEntry> tocList = new((int)numSections);

                            for (int index = 0; index < numSections; index++)
                            {
                                tocList.Add(new TocEntry()
                                {
                                    SectionIdentifier = new(priBinaryReader.ReadChars(16)),
                                    Flags = priBinaryReader.ReadUInt16(),
                                    SectionFlags = priBinaryReader.ReadUInt16(),
                                    SectionQualifier = priBinaryReader.ReadUInt32(),
                                    SectionOffset = priBinaryReader.ReadUInt32(),
                                    SectionLength = priBinaryReader.ReadUInt32(),
                                });
                            }

                            // 读取分段列表
                            object[] sectionArray = new object[numSections];

                            for (int index = 0; index < sectionArray.Length; index++)
                            {
                                if (sectionArray[index] is null)
                                {
                                    priBinaryReader.BaseStream.Seek(sectionStartOffset + tocList[index].SectionOffset, SeekOrigin.Begin);

                                    switch (tocList[index].SectionIdentifier)
                                    {
                                        case "[mrm_pridescex]\0":
                                            {
                                                PriDescriptorSection section = new("[mrm_pridescex]\0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_hschema]  \0":
                                            {
                                                HierarchicalSchemaSection section = new("[mrm_hschema]  \0", priBinaryReader, false);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_hschemaex] ":
                                            {
                                                HierarchicalSchemaSection section = new("[mrm_hschemaex] ", priBinaryReader, true);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_decn_info]\0":
                                            {
                                                DecisionInfoSection section = new("[mrm_decn_info]\0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_res_map__]\0":
                                            {
                                                ResourceMapSection section = new("[mrm_res_map__]\0", priBinaryReader, false, ref sectionArray);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_res_map2_]\0":
                                            {
                                                ResourceMapSection section = new("[mrm_res_map2_]\0", priBinaryReader, true, ref sectionArray);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_dataitem] \0":
                                            {
                                                DataItemSection section = new("[mrm_dataitem] \0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[mrm_rev_map]  \0":
                                            {
                                                ReverseMapSection section = new("[mrm_rev_map]  \0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        case "[def_file_list]\0":
                                            {
                                                ReferencedFileSection section = new("[def_file_list]\0", priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                        default:
                                            {
                                                UnknownSection section = new(null, priBinaryReader);
                                                sectionArray[index] = section;
                                                break;
                                            }
                                    }
                                }
                            }

                            // 根据分段列表获取相应的内容
                            List<PriDescriptorSection> priDescriptorSectionList = [];

                            foreach (object section in sectionArray)
                            {
                                if (section is PriDescriptorSection priDescriptorSection)
                                {
                                    priDescriptorSectionList.Add(priDescriptorSection);
                                }
                            }

                            foreach (PriDescriptorSection priDescriptorSection in priDescriptorSectionList)
                            {
                                foreach (int resourceMapIndex in priDescriptorSection.ResourceMapSectionsList)
                                {
                                    if (sectionArray[resourceMapIndex] is ResourceMapSection resourceMapSection)
                                    {
                                        if (resourceMapSection.HierarchicalSchemaReference is not null)
                                        {
                                            continue;
                                        }

                                        DecisionInfoSection decisionInfoSection = sectionArray[resourceMapSection.DecisionInfoSectionIndex] as DecisionInfoSection;

                                        foreach (CandidateSet candidateSet in resourceMapSection.CandidateSetsDict.Values)
                                        {
                                            if (sectionArray[candidateSet.ResourceMapSectionAndIndex.Item1] is HierarchicalSchemaSection hierarchicalSchemaSection)
                                            {
                                                ResourceMapScopeAndItem resourceMapScopeAndItem = hierarchicalSchemaSection.ItemsList[candidateSet.ResourceMapSectionAndIndex.Item2];

                                                string key = string.Empty;

                                                if (resourceMapScopeAndItem.Name is not null && resourceMapScopeAndItem.Parent is not null)
                                                {
                                                    key = Path.Combine(resourceMapScopeAndItem.Parent.Name, resourceMapScopeAndItem.Name);
                                                }
                                                else if (resourceMapScopeAndItem.Name is not null)
                                                {
                                                    key = resourceMapScopeAndItem.Name;
                                                }

                                                if (string.IsNullOrEmpty(key))
                                                {
                                                    continue;
                                                }

                                                foreach (Candidate candidate in candidateSet.CandidatesList)
                                                {
                                                    string value = string.Empty;

                                                    if (candidate.SourceFileIndex is null)
                                                    {
                                                        ByteSpan byteSpan = null;

                                                        if (candidate.DataItemSectionAndIndex is not null)
                                                        {
                                                            DataItemSection dataItemSection = sectionArray[candidate.DataItemSectionAndIndex.Item1] as DataItemSection;
                                                            byteSpan = dataItemSection is not null ? dataItemSection.DataItemsList[candidate.DataItemSectionAndIndex.Item2] : candidate.Data;
                                                        }

                                                        if (byteSpan is not null)
                                                        {
                                                            resourceStream.Seek(byteSpan.Offset, SeekOrigin.Begin);
                                                            using BinaryReader binaryReader = new(resourceStream, Encoding.Default, true);
                                                            byte[] data = binaryReader.ReadBytes((int)byteSpan.Length);
                                                            binaryReader.Dispose();

                                                            switch (candidate.Type)
                                                            {
                                                                // ASCII 格式字符串内容
                                                                case ResourceValueType.AsciiString:
                                                                    {
                                                                        string content = Encoding.ASCII.GetString(data).TrimEnd('\0');
                                                                        string language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : "Neutral";

                                                                        if (resourceDict.TryGetValue(language, out Dictionary<string, string> stringContentDict))
                                                                        {
                                                                            stringContentDict.TryAdd(key, content);
                                                                        }
                                                                        else
                                                                        {
                                                                            Dictionary<string, string> stringDict = [];
                                                                            stringDict.TryAdd(key, content);
                                                                            resourceDict.TryAdd(language, stringDict);
                                                                        }

                                                                        break;
                                                                    }
                                                                // UTF8 格式字符串内容
                                                                case ResourceValueType.Utf8String:
                                                                    {
                                                                        string content = Encoding.UTF8.GetString(data).TrimEnd('\0');
                                                                        string language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : "Neutral";

                                                                        if (resourceDict.TryGetValue(language, out Dictionary<string, string> stringContentDict))
                                                                        {
                                                                            stringContentDict.TryAdd(key, content);
                                                                        }
                                                                        else
                                                                        {
                                                                            Dictionary<string, string> stringDict = [];
                                                                            stringDict.TryAdd(key, content);
                                                                            resourceDict.TryAdd(language, stringDict);
                                                                        }
                                                                        break;
                                                                    }
                                                                // Unicode 格式字符串内容
                                                                case ResourceValueType.UnicodeString:
                                                                    {
                                                                        string content = Encoding.Unicode.GetString(data).TrimEnd('\0');
                                                                        string language = decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList.Count > 0 ? decisionInfoSection.QualifierSetsList[candidate.QualifierSet].QualifiersList[0].Value : "Neutral";

                                                                        if (resourceDict.TryGetValue(language, out Dictionary<string, string> stringContentDict))
                                                                        {
                                                                            stringContentDict.TryAdd(key, content);
                                                                        }
                                                                        else
                                                                        {
                                                                            Dictionary<string, string> stringDict = [];
                                                                            stringDict.TryAdd(key, content);
                                                                            resourceDict.TryAdd(language, stringDict);
                                                                        }
                                                                        break;
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

                        priBinaryReader.Dispose();
                    }

                    randomAccessStreamOverStream?.Dispose();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Parse package resources failed", e);
            }

            return resourceDict;
        }

        /// <summary>
        /// 获取程序包面向的设备系列信息
        /// </summary>
        private List<TargetDeviceFamilyModel> ParsePackageTargetDeviceFamily(IAppxManifestReader3 appxManifestReader)
        {
            List<TargetDeviceFamilyModel> targetDeviceFamilyList = [];

            if (appxManifestReader.GetTargetDeviceFamilies(out IAppxManifestTargetDeviceFamiliesEnumerator appxManifestTargetDeviceFamiliesEnumerator) is 0)
            {
                while (appxManifestTargetDeviceFamiliesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                {
                    appxManifestTargetDeviceFamiliesEnumerator.GetCurrent(out IAppxManifestTargetDeviceFamily appxManifestTargetDeviceFamily);
                    TargetDeviceFamilyModel targetDeviceFamilyItem = new();
                    appxManifestTargetDeviceFamily.GetName(out string targetDeviceName);
                    appxManifestTargetDeviceFamily.GetMinVersion(out ulong minVersion);
                    appxManifestTargetDeviceFamily.GetMaxVersionTested(out ulong maxVersionTested);

                    targetDeviceFamilyItem.TargetDeviceName = targetDeviceName;
                    PackageVersion minPackageVersion = new(minVersion);
                    targetDeviceFamilyItem.MinVersion = new Version(minPackageVersion.Major, minPackageVersion.Minor, minPackageVersion.Build, minPackageVersion.Revision);
                    PackageVersion maxPackageVersionTested = new(maxVersionTested);
                    targetDeviceFamilyItem.MaxVersionTested = new Version(maxPackageVersionTested.Major, maxPackageVersionTested.Minor, maxPackageVersionTested.Build, maxPackageVersionTested.Revision);

                    targetDeviceFamilyList.Add(targetDeviceFamilyItem);
                    appxManifestTargetDeviceFamiliesEnumerator.MoveNext(out _);
                }
            }

            targetDeviceFamilyList.Sort((item1, item2) => item1.TargetDeviceName.CompareTo(item2.TargetDeviceName));
            return targetDeviceFamilyList;
        }

        /// <summary>
        /// 解析依赖包应用信息
        /// </summary>
        private Dictionary<string, object> ParseDependencyPackageManifest(IAppxPackageReader appxPackageReader)
        {
            Dictionary<string, object> parseDict = [];

            try
            {
                // 分段 4：读取应用包清单
                if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0 && appxManifestReader.GetPackageId(out IAppxManifestPackageId2 appxManifestPackageId) is 0)
                {
                    appxManifestPackageId.GetPackageFullName(out string packageFullName);
                    appxManifestPackageId.GetVersion(out ulong version);
                    appxManifestPackageId.GetPublisher(out string publisher);

                    parseDict.TryAdd("PackageFullName", packageFullName);
                    parseDict.TryAdd("PublisherDisplayName", publisher);

                    PackageVersion packageVersion = new(version);
                    parseDict.TryAdd("Version", new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Parse package manifest failed", e);
            }

            return parseDict;
        }

        /// <summary>
        /// 解析依赖捆绑包应用信息
        /// </summary>
        private Dictionary<string, object> ParseDependencyPackageBundleManifest(IAppxBundleReader appxBundleReader)
        {
            Dictionary<string, object> parseDict = [];

            try
            {
                // 分段 4：读取应用包清单
                if (appxBundleReader.GetManifest(out IAppxBundleManifestReader appxBundleManifestReader) is 0 && appxBundleManifestReader.GetPackageId(out IAppxManifestPackageId2 appxManifestPackageId) is 0)
                {
                    appxManifestPackageId.GetPackageFullName(out string packageFullName);
                    appxManifestPackageId.GetVersion(out ulong version);
                    appxManifestPackageId.GetPublisher(out string publisher);

                    parseDict.TryAdd("PackageFullName", packageFullName);
                    parseDict.TryAdd("PublisherDisplayName", publisher);

                    PackageVersion packageVersion = new(version);
                    parseDict.TryAdd("Version", new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Parse package manifest failed", e);
            }

            return parseDict;
        }

        /// <summary>
        /// 解析应用包支持的处理器架构
        /// </summary>
        private string ParsePackageBundleArchitecture(Dictionary<ProcessorArchitecture, string> applicationDict)
        {
            string architecture = string.Empty;

            if (applicationDict.ContainsKey(ProcessorArchitecture.X86))
            {
                architecture = ProcessorArchitecture.X86.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.X64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.X64.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Arm))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.Arm.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Arm64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.Arm64.ToString();
            }

            if (applicationDict.ContainsKey(ProcessorArchitecture.Neutral))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += ProcessorArchitecture.Neutral.ToString();
            }

            if (string.IsNullOrEmpty(architecture))
            {
                architecture = ResourceService.GetLocalized("Installer/Unknown");
            }

            return string.Format(ResourceService.GetLocalized("Installer/BundleHeader"), architecture);
        }

        /// <summary>
        /// 解析应用捆绑包适合主机系统的文件
        /// </summary>
        private string ParsePacakgeBundleCompatibleFile(Dictionary<ProcessorArchitecture, string> applicationDict)
        {
            Architecture osArchitecture = RuntimeInformation.ProcessArchitecture;
            string appxFile = null;

            if (osArchitecture is Architecture.X86)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.X86, out appxFile);
            }
            else if (osArchitecture is Architecture.X64)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.X64, out appxFile);
            }
            else if (osArchitecture is Architecture.Arm64)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.Arm64, out appxFile);
            }
            else if (osArchitecture is Architecture.Arm)
            {
                applicationDict.TryGetValue(ProcessorArchitecture.Arm, out appxFile);
            }
            else
            {
                foreach (KeyValuePair<ProcessorArchitecture, string> bundleFileItem in applicationDict)
                {
                    appxFile = bundleFileItem.Value;
                    break;
                }
            }

            return appxFile;
        }

        /// <summary>
        /// 解析捆绑包每个包信息
        /// </summary>
        private Dictionary<string, object> ParsePackageBundleManifestInfo(IAppxBundleManifestReader appxBundleManifestReader)
        {
            Dictionary<string, object> packageManifestInfoDict = [];
            Dictionary<ProcessorArchitecture, string> applicationDict = [];
            Dictionary<string, string> qualifiedResourceDict = [];

            if (appxBundleManifestReader.GetPackageInfoItems(out IAppxBundleManifestPackageInfoEnumerator appxBundleManifestPackageInfoEnumerator) is 0)
            {
                while (appxBundleManifestPackageInfoEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                {
                    appxBundleManifestPackageInfoEnumerator.GetCurrent(out IAppxBundleManifestPackageInfo appxBundleManifestPackageInfo);

                    appxBundleManifestPackageInfo.GetFileName(out string fileName);
                    appxBundleManifestPackageInfo.GetPackageType(out APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE packageType);

                    // 获取应用捆绑包中的所有资源包
                    if (packageType is APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE.APPX_BUNDLE_PAYLOAD_PACKAGE_TYPE_APPLICATION)
                    {
                        appxBundleManifestPackageInfo.GetPackageId(out IAppxManifestPackageId appxManifestPackageId);
                        appxManifestPackageId.GetArchitecture(out ProcessorArchitecture architecture);
                        applicationDict.TryAdd(architecture, fileName);
                    }

                    List<string> qualifiedResourceList = GetPackageBundleQualifiedResources(appxBundleManifestPackageInfo);

                    foreach (string qualifiedResourceItem in qualifiedResourceList)
                    {
                        qualifiedResourceDict.TryAdd(qualifiedResourceItem, fileName);
                    }

                    appxBundleManifestPackageInfoEnumerator.MoveNext(out _);
                }
            }

            packageManifestInfoDict.TryAdd("Application", applicationDict);
            packageManifestInfoDict.TryAdd("QualifiedResource", qualifiedResourceDict);
            return packageManifestInfoDict;
        }

        /// <summary>
        /// 获取本地化字符串
        /// </summary>
        private string GetLocalizedString(string resource, Dictionary<string, string> resourceDict)
        {
            // TODO:未完成

            if (!string.IsNullOrEmpty(resource) && resourceDict is not null)
            {
                if (resource.StartsWith("ms-resource:") && resourceDict.TryGetValue(resource.Replace("ms-resource:", @"resources\", StringComparison.OrdinalIgnoreCase), out string localizedDescription) || resource.StartsWith("ms-resource:") && resourceDict.TryGetValue(resource.Replace("ms-resource:", @"Resources\", StringComparison.OrdinalIgnoreCase), out localizedDescription))
                {
                    resource = localizedDescription;
                }
            }
            else
            {
                resource = string.Empty;
            }

            return resource;
        }

        /// <summary>
        /// 应用包图标
        /// </summary>
        private IStream GetPackageLogo(string logo, Dictionary<string, IAppxFile> fileDict)
        {
            List<KeyValuePair<string, IAppxFile>> logoList = [];
            IStream imageFileStream = null;
            string logoFileName = Path.GetFileNameWithoutExtension(logo);

            // 读取应用包图标
            foreach (KeyValuePair<string, IAppxFile> fileItem in fileDict)
            {
                if (fileItem.Value.GetName(out string packageFileName) is 0 && packageFileName.Contains(logoFileName))
                {
                    logoList.Add(KeyValuePair.Create(packageFileName, fileItem.Value));
                }
            }

            // TODO:更改应用包图标的识别方法
            if (logoList.Count > 0)
            {
                logoList[0].Value.GetStream(out imageFileStream);
            }

            return imageFileStream;
        }

        /// <summary>
        /// 获取应用包资源文件
        /// </summary>
        private IStream GetPackageResourceFileStream(Dictionary<string, IAppxFile> fileDict)
        {
            IStream resourceFileStream = null;

            foreach (KeyValuePair<string, IAppxFile> fileItem in fileDict)
            {
                if (fileItem.Key.Equals("resources.pri", StringComparison.OrdinalIgnoreCase))
                {
                    fileItem.Value.GetStream(out resourceFileStream);
                    break;
                }
            }

            return resourceFileStream;
        }

        /// <summary>
        /// 获取特定语言的资源
        /// </summary>
        private Dictionary<string, string> GetSpecifiedLanguageResource(Dictionary<string, Dictionary<string, string>> resourceDict)
        {
            if (resourceDict is null || resourceDict.Count is 0)
            {
                return null;
            }

            CultureInfo appCultureInfo = CultureInfo.GetCultureInfo(LanguageService.AppLanguage);
            CultureInfo systemCultureInfo = CultureInfo.CurrentCulture;
            CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(LanguageService.DefaultAppLanguage);

            // 查找符合当前应用显示界面的语言资源信息
            while (!string.IsNullOrEmpty(appCultureInfo.Name))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(appCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return resourceItem.Value;
                    }
                }

                appCultureInfo = appCultureInfo.Parent;
            }

            // 查找符合当前系统显示界面的语言资源信息
            while (!string.IsNullOrEmpty(systemCultureInfo.Name))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(systemCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return resourceItem.Value;
                    }
                }

                systemCultureInfo = systemCultureInfo.Parent;
            }

            // 查找默认语言的资源信息
            while (!string.IsNullOrEmpty(defaultCultureInfo.Name))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(defaultCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return resourceItem.Value;
                    }
                }

                defaultCultureInfo = systemCultureInfo.Parent;
            }

            // 不符合，如果存在未指定的语言，返回未指定的语言资源信息
            foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
            {
                if (resourceItem.Key.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
                {
                    return resourceItem.Value;
                }
            }

            // 不符合，如果存在资源信息，则返回第一个
            foreach (KeyValuePair<string, Dictionary<string, string>> resourceItem in resourceDict)
            {
                return resourceItem.Value;
            }

            return null;
        }

        /// <summary>
        /// 获取特定语言的资源文件
        /// </summary>
        private List<KeyValuePair<string, string>> GetPackageBundleSpecifiedLanguageResource(Dictionary<string, string> resourceDict)
        {
            List<KeyValuePair<string, string>> specifiedLanguageResourceList = [];

            if (resourceDict is null || resourceDict.Count is 0)
            {
                return null;
            }

            CultureInfo appCultureInfo = CultureInfo.GetCultureInfo(LanguageService.AppLanguage);
            CultureInfo systemCultureInfo = CultureInfo.CurrentCulture;
            CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(LanguageService.DefaultAppLanguage);

            // 查找符合当前应用显示界面的语言资源信息
            while (!string.IsNullOrEmpty(appCultureInfo.Name))
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(appCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        specifiedLanguageResourceList.Add(resourceItem);
                        break;
                    }
                }

                appCultureInfo = appCultureInfo.Parent;
            }

            // 查找符合当前系统显示界面的语言资源信息
            while (!string.IsNullOrEmpty(systemCultureInfo.Name))
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(systemCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        specifiedLanguageResourceList.Add(resourceItem);
                        break;
                    }
                }

                systemCultureInfo = systemCultureInfo.Parent;
            }

            // 查找默认语言的资源信息
            while (!string.IsNullOrEmpty(defaultCultureInfo.Name))
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    if (resourceItem.Key.Equals(defaultCultureInfo.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        specifiedLanguageResourceList.Add(resourceItem);
                        break;
                    }
                }

                defaultCultureInfo = systemCultureInfo.Parent;
            }

            // 不符合，如果存在未指定的语言，返回未指定的语言资源信息
            foreach (KeyValuePair<string, string> resourceItem in resourceDict)
            {
                if (resourceItem.Key.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
                {
                    specifiedLanguageResourceList.Add(resourceItem);
                    break;
                }
            }

            // 不符合，如果存在资源信息，则返回第一个
            if (specifiedLanguageResourceList.Count is 0)
            {
                foreach (KeyValuePair<string, string> resourceItem in resourceDict)
                {
                    specifiedLanguageResourceList.Add(resourceItem);
                    break;
                }
            }

            return specifiedLanguageResourceList;
        }

        /// <summary>
        /// 获取应用捆绑包限定的资源
        /// </summary>
        private List<string> GetPackageBundleQualifiedResources(IAppxBundleManifestPackageInfo appxBundleManifestPackageInfo)
        {
            List<string> qualifiedResourceList = [];

            // 获取应用包定义的限定资源
            if (appxBundleManifestPackageInfo.GetResources(out IAppxManifestQualifiedResourcesEnumerator appxManifestQualifiedResourcesEnumerator) is 0)
            {
                while (appxManifestQualifiedResourcesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
                {
                    if (appxManifestQualifiedResourcesEnumerator.GetCurrent(out IAppxManifestQualifiedResource appxManifestQualifiedResource) is 0 && appxManifestQualifiedResource.GetLanguage(out string language) is 0 && !string.IsNullOrEmpty(language))
                    {
                        qualifiedResourceList.Add(language);
                    }

                    appxManifestQualifiedResourcesEnumerator.MoveNext(out _);
                }
            }

            qualifiedResourceList.Sort();
            return qualifiedResourceList;
        }

        #endregion 第六部分：解析应用包信息

        #region 第七部分：更新应用包信息

        /// <summary>
        /// 恢复默认内容
        /// </summary>
        private void ResetResult()
        {
            CanDragFile = false;
            PackageDescription = string.Empty;
            IsLoadCompleted = false;
            IsParseSuccessfully = false;
            IsInstalling = false;
            InstallProgressValue = 0;
            IsInstallWaiting = false;
            IsInstallFailed = false;
            InstallStateString = string.Empty;
            InstallFailedInformation = string.Empty;
            IsCancelInstall = true;
            PackageIconImage = null;
            PackageName = string.Empty;
            PublisherDisplayName = string.Empty;
            Version = new Version();
            PackageFamilyName = string.Empty;
            PackageFullName = string.Empty;
            SupportedArchitecture = string.Empty;
            IsFramework = string.Empty;
            DependencyCollection.Clear();
            CapabilitiesCollection.Clear();
            ApplicationCollection.Clear();
            TargetDeviceFamilyCollection.Clear();
            LanguageCollection.Clear();
            InstallDependencyCollection.Clear();
        }

        /// <summary>
        /// 更新结果
        /// </summary>
        private async Task UpdateResultAsync(Tuple<bool, Dictionary<string, object>> resultDict)
        {
            IsParseSuccessfully = resultDict.Item1;

            if (IsParseSuccessfully)
            {
                Dictionary<string, object> parseDict = resultDict.Item2;

                if (parseDict.TryGetValue("Capabilities", out object appcapabilitiesObj))
                {
                    APPX_CAPABILITIES appcapabilities = (APPX_CAPABILITIES)appcapabilitiesObj;

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_INTERNET_CLIENT))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityInternetClient"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_INTERNET_CLIENT_SERVER))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityInternetClientServer"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_PRIVATE_NETWORK_CLIENT_SERVER))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityPrivateNetworkClientServer"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_DOCUMENTS_LIBRARY))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityDocumentsLibrary"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_PICTURES_LIBRARY))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityPicturesLibrary"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_VIDEOS_LIBRARY))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityVideosLibrary"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_MUSIC_LIBRARY))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityMusicLibrary"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_ENTERPRISE_AUTHENTICATION))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityEnterpriseAuthentication"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_SHARED_USER_CERTIFICATES))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilitySharedUserCertificates"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_REMOVABLE_STORAGE))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityRemoveStorage"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_APPOINTMENTS))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityAppointments"));
                    }

                    if (appcapabilities.HasFlag(APPX_CAPABILITIES.APPX_CAPABILITY_CONTACTS))
                    {
                        CapabilitiesCollection.Add(ResourceService.GetLocalized("Installer/CapabilityContacts"));
                    }
                }

                if (parseDict.TryGetValue("Dependency", out object dependencyObj))
                {
                    List<Dictionary<string, object>> dependencyList = dependencyObj as List<Dictionary<string, object>>;

                    foreach (Dictionary<string, object> dependencyItem in dependencyList)
                    {
                        DependencyCollection.Add(new DependencyModel()
                        {
                            DependencyName = dependencyItem["DependencyName"] as string,
                            DependencyPublisher = dependencyItem["DependencyPublisher"] as string,
                            DependencyMinVersion = dependencyItem["DependencyMinVersion"] as Version,
                            DependencySatisfied = ResourceService.GetLocalized("Installer/Yes")
                        });
                    }
                }

                if (parseDict.TryGetValue("TargetDeviceFamily", out object targetDeviceFamilyObj) && targetDeviceFamilyObj is List<TargetDeviceFamilyModel> targetDeviceFamilyList)
                {
                    foreach (TargetDeviceFamilyModel targetDeviceFamilyItem in targetDeviceFamilyList)
                    {
                        TargetDeviceFamilyCollection.Add(targetDeviceFamilyItem);
                    }
                }

                if (parseDict.TryGetValue("Application", out object applicationObj) && applicationObj is List<ApplicationModel> applicationList)
                {
                    foreach (ApplicationModel applicationItem in applicationList)
                    {
                        ApplicationCollection.Add(applicationItem);
                    }
                }

                SupportedArchitecture = parseDict.TryGetValue("ProcessorArchitecture", out object architectureObj) && architectureObj is string architecture ? architecture : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PackageName = parseDict.TryGetValue("DisplayName", out object displayNameObj) && displayNameObj is string displayName ? displayName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PackageFamilyName = parseDict.TryGetValue("PackageFamilyName", out object packageFamilyNameObj) && packageFamilyNameObj is string packageFamilyName ? packageFamilyName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PackageFullName = parseDict.TryGetValue("PackageFullName", out object packageFullNameObj) && packageFullNameObj is string packageFullName ? packageFullName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PublisherDisplayName = parseDict.TryGetValue("PublisherDisplayName", out object publisherDisplayNameObj) && publisherDisplayNameObj is string publisherDisplayName ? publisherDisplayName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                IsFramework = parseDict.TryGetValue("IsFramework", out object isFrameworkObj) && isFrameworkObj is bool isFramework ? isFramework ? ResourceService.GetLocalized("Installer/Yes") : ResourceService.GetLocalized("Installer/No") : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                Version = parseDict.TryGetValue("Version", out object versionObj) && versionObj is Version version ? version : new Version();
                PackageDescription = parseDict.TryGetValue("Description", out object descriptionObj) && descriptionObj is string description ? string.IsNullOrEmpty(description) ? ResourceService.GetLocalized("Installer/None") : description : ResourceService.GetLocalized("Installer/None");

                if (parseDict.TryGetValue("QualifiedResource", out object qualifiedResourceListObj) && qualifiedResourceListObj is List<string> qualifiedResourceList)
                {
                    foreach (string qualifiedResource in qualifiedResourceList)
                    {
                        LanguageCollection.Add(new Language(qualifiedResource));
                    }
                }

                try
                {
                    if (parseDict.TryGetValue("ImageLogo", out object imageLogoStreamObj) && imageLogoStreamObj is IStream imageLogoStream && imageLogoStream is not null)
                    {
                        ShCoreLibrary.CreateRandomAccessStreamOverStream(imageLogoStream, BSOS_OPTIONS.BSOS_DEFAULT, typeof(IRandomAccessStream).GUID, out IntPtr ppv);
                        RandomAccessStreamOverStream randomAccessStreamOverStream = RandomAccessStreamOverStream.FromAbi(ppv);
                        BitmapImage bitmapImage = new();
                        await bitmapImage.SetSourceAsync(randomAccessStreamOverStream);
                        PackageIconImage = bitmapImage;
                        randomAccessStreamOverStream?.Dispose();

                        if (imageLogoStream is not null)
                        {
                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(imageLogoStream, CreateComInterfaceFlags.None));
                        }

                        randomAccessStreamOverStream = null;
                        imageLogoStream = null;
                    }
                    else
                    {
                        PackageIconImage = null;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Display package app icon failed", e);
                }
            }

            CanDragFile = true;
            IsLoadCompleted = true;
        }

        #endregion 第七部分：更新应用包信息

        /// <summary>
        /// 处理提权模式下的文件拖拽
        /// </summary>
        public async Task DealElevatedDragDropAsync(string filePath)
        {
            string extensionName = Path.GetExtension(filePath);

            if (extensionName.Equals(".appx", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msix", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appxbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase) || extensionName.Equals(".appinstaller", StringComparison.OrdinalIgnoreCase))
            {
                fileName = filePath;

                if (!string.IsNullOrEmpty(fileName))
                {
                    IsParseEmpty = false;
                    ResetResult();

                    Tuple<bool, Dictionary<string, object>> parseResult = await Task.Run(async () =>
                    {
                        return await ParsePackagedAppAsync(fileName);
                    });

                    await UpdateResultAsync(parseResult);
                }
            }
        }
    }
}

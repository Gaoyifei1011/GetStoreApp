using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.UI.Backdrop;
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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
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

        private ObservableCollection<TargetDeviceFamilyModel> TargetDeviceFamilyCollection { get; } = [];

        private ObservableCollection<InstallDependencyModel> InstallDependencyCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPage(AppActivationArguments activationArguments)
        {
            InitializeComponent();
            Background = new MicaBrush(MicaKind.BaseAlt, true);
            appActivationArguments = activationArguments;
            Program.SetTitleBarTheme(ActualTheme);
            Program.SetClassicMenuTheme(ActualTheme);
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

                await Task.Delay(500);
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
            string dependencyFullName = args.Parameter as string;

            if (!string.IsNullOrEmpty(dependencyFullName))
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

                        await Task.Delay(500);
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

                        await Task.Delay(500);
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

                            await Task.Delay(500);
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
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add(".appx");
                    openFileDialog.FileTypeFilter.Add(".msix");
                    openFileDialog.FileTypeFilter.Add(".appxbundle");
                    openFileDialog.FileTypeFilter.Add(".msixbundle");
                    openFileDialog.FileTypeFilter.Add(".appinstaller");

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

                await Task.Delay(500);
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
                    };

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add(".appx");
                    openFileDialog.FileTypeFilter.Add(".msix");
                    openFileDialog.FileTypeFilter.Add(".appxbundle");
                    openFileDialog.FileTypeFilter.Add(".msixbundle");
                    openFileDialog.FileTypeFilter.Add(".appinstaller");

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

                await Task.Delay(500);
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
                        InstallDependencyCollection.Add(new InstallDependencyModel()
                        {
                            DependencyName = file.Name,
                            DependencyVersion = new Version(),
                            DependencyPublisher = string.Empty,
                            DependencyFullName = Guid.NewGuid().ToString(),
                            DependencyPath = file.Path
                        });
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

                    openFileDialog.FileTypeFilter.Clear();
                    openFileDialog.FileTypeFilter.Add(".appx");
                    openFileDialog.FileTypeFilter.Add(".msix");
                    openFileDialog.FileTypeFilter.Add(".appxbundle");
                    openFileDialog.FileTypeFilter.Add(".msixbundle");

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
        /// 关闭浮出控件
        /// </summary>
        private void OnCloseFlyoutClicked(object sender, RoutedEventArgs args)
        {
            if (AddDependencyFlyout.IsOpen)
            {
                AddDependencyFlyout.Hide();
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallAppClicked(object sender, RoutedEventArgs args)
        {
            if (File.Exists(fileName))
            {
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
                // TODO:
                await Task.Run(() =>
                {
                    installPackageWithProgress.Cancel();
                    installPackageWithProgress.Close();
                    installPackageWithProgress = null;
                });

                // 更新应用安装状态
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                });
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
                // TODO:
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
                // 显示安装成功通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/AppInstallSuccessfully"));
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
            }
            // 安装错误
            else if (status is AsyncStatus.Error)
            {
                // 显示安装失败通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/AppInstallFailed"));
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/AppInstallFailedReason"), result.ErrorCode.Message));
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
            }

            // 更新应用安装状态
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // TODO:
            });

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

                                if (appxFactory is not null)
                                {
                                    parseResult = true;

                                    // 读取应用包内容
                                    if (appxFactory.CreatePackageReader2(fileStream, null, out IAppxPackageReader appxPackageReader) is 0)
                                    {
                                        // 解析资源包所有文件
                                        Dictionary<string, IAppxFile> fileDict = ParsePackagePayloadFiles(appxPackageReader);

                                        // 获取并解析本地资源文件
                                        IStream resourceFileStream = GetPackageResourceFileStream(fileDict);

                                        if (resourceFileStream is not null)
                                        {
                                            ParsePackageResources(resourceFileStream);
                                            Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(resourceFileStream, CreateComInterfaceFlags.None));
                                            resourceFileStream = null;
                                        }

                                        // 解析资源包清单文件
                                        Dictionary<string, object> parseManifsetDict = ParsePackageManifest(appxPackageReader);

                                        foreach (KeyValuePair<string, object> parseManifsetItem in parseManifsetDict)
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

                                if (appxBundleFactory is not null)
                                {
                                    // 读取捆绑包的二进制文件内容
                                    if (appxBundleFactory.CreateBundleReader2(fileStream, null, out IAppxBundleReader appxBundleReader) is 0)
                                    {
                                        // 读取捆绑包的二进制文件
                                        if (appxBundleReader.GetPayloadPackages(out IAppxFilesEnumerator appxFilesEnumerator) is 0)
                                        {
                                            Dictionary<string, IAppxFile> bundleFileDict = ParsePacakgeBundleFiles(appxFilesEnumerator);

                                            // 获取并解析本地资源文件
                                            IStream resourceFileStream = GetPackageBundleResourceFileStream(bundleFileDict);

                                            // 获取应用捆绑包安装文件
                                            IStream parseFileStream = ParsePacakgeBundleFileStream(bundleFileDict);

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
                                                            // 解析资源包所有文件
                                                            Dictionary<string, IAppxFile> fileDict = ParsePackagePayloadFiles(appxPackageReader);

                                                            if (resourceFileStream is not null)
                                                            {
                                                                ParsePackageResources(resourceFileStream);
                                                                Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(resourceFileStream, CreateComInterfaceFlags.None));
                                                                resourceFileStream = null;
                                                            }

                                                            // 解析资源包清单文件
                                                            Dictionary<string, object> parseManifsetDict = ParsePackageManifest(appxPackageReader);

                                                            foreach (KeyValuePair<string, object> parseManifsetItem in parseManifsetDict)
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

                                                Marshal.Release(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(parseFileStream, CreateComInterfaceFlags.None));
                                            }

                                            string processorArchitecture = ParsePackageBundleArchitecture(bundleFileDict);

                                            if (!string.IsNullOrEmpty(processorArchitecture))
                                            {
                                                if (parseDict.ContainsKey("ProcessorArchitecture"))
                                                {
                                                    parseDict["ProcessorArchitecture"] = processorArchitecture;
                                                }
                                                else
                                                {
                                                    parseDict.TryAdd("ProcessorArchitecture", processorArchitecture);
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
        /// 解析应用包的所有资源
        /// </summary>
        private void ParsePackageResources(IStream resourceFileStream)
        {
            // 分段 3 ：解析资源文件
            if (resourceFileStream is not null)
            {
                ShCoreLibrary.CreateRandomAccessStreamOverStream(resourceFileStream, BSOS_OPTIONS.BSOS_DEFAULT, typeof(IRandomAccessStream).GUID, out IntPtr ppv);
                RandomAccessStreamOverStream randomAccessStreamOverStream = RandomAccessStreamOverStream.FromAbi(ppv);
                Stream resourceStream = randomAccessStreamOverStream.AsStream();

                if (resourceStream is not null)
                {
                    BinaryReader binaryReader = new(resourceStream, Encoding.ASCII, true);
                }

                randomAccessStreamOverStream?.Dispose();
                resourceStream?.Dispose();
            }
        }

        /// <summary>
        /// 解析应用包清单
        /// </summary>
        private Dictionary<string, object> ParsePackageManifest(IAppxPackageReader appxPackageReader)
        {
            Dictionary<string, object> parseDict = [];

            // 分段 4：读取应用包清单
            if (appxPackageReader.GetManifest(out IAppxManifestReader3 appxManifestReader) is 0)
            {
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
                    packageId.GetPackageFamilyName(out string packageFamilyName);
                    packageId.GetPackageFullName(out string packageFullName);
                    packageId.GetVersion(out ulong version);

                    parseDict.TryAdd("ProcessorArchitecture", architecture.ToString());
                    parseDict.TryAdd("PackageFamilyName", packageFamilyName);
                    parseDict.TryAdd("PackageFullName", packageFullName);

                    PackageVersion packageVersion = new(version);
                    parseDict.TryAdd("Version", new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));
                }

                // 获取包的目标设备系列
                if (appxManifestReader.GetTargetDeviceFamilies(out IAppxManifestTargetDeviceFamiliesEnumerator appxManifestTargetDeviceFamiliesEnumerator) is 0)
                {
                    List<TargetDeviceFamilyModel> targetDeviceFamilyList = ParsePackageTargetDeviceFamily(appxManifestTargetDeviceFamiliesEnumerator);
                    parseDict.TryAdd("TargetDeviceFamily", targetDeviceFamilyList);
                }

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
                    parseDict.TryAdd("DisplayName", displayName);
                    parseDict.TryAdd("Logo", logo);
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

            return parseDict;
        }

        private string ParsePackageBundleArchitecture(Dictionary<string, IAppxFile> bundleFileDict)
        {
            ProcessorArchitecture processorArchitecture = 0;
            string x86 = ProcessorArchitecture.X86.ToString();
            string x64 = ProcessorArchitecture.X64.ToString();
            string arm = ProcessorArchitecture.Arm.ToString();
            string arm64 = ProcessorArchitecture.Arm64.ToString();
            string neutral = ProcessorArchitecture.Neutral.ToString();

            foreach (string bundleFileName in bundleFileDict.Keys)
            {
                if (bundleFileName.Contains(x86, StringComparison.OrdinalIgnoreCase))
                {
                    processorArchitecture |= ProcessorArchitecture.X86;
                }
                else if (bundleFileName.Contains("x64", StringComparison.OrdinalIgnoreCase))
                {
                    processorArchitecture |= ProcessorArchitecture.X64;
                }
                else if (bundleFileName.Contains("arm", StringComparison.OrdinalIgnoreCase))
                {
                    processorArchitecture |= ProcessorArchitecture.Arm;
                }
                else if (bundleFileName.Contains("arm64", StringComparison.OrdinalIgnoreCase))
                {
                    processorArchitecture |= ProcessorArchitecture.Arm64;
                }
                else if (bundleFileName.Contains("neutral", StringComparison.OrdinalIgnoreCase))
                {
                    processorArchitecture |= ProcessorArchitecture.Neutral;
                }
            }

            string architecture = string.Empty;

            if (processorArchitecture.HasFlag(ProcessorArchitecture.X86))
            {
                architecture = x86;
            }

            if (processorArchitecture.HasFlag(ProcessorArchitecture.X64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += x64;
            }

            if (processorArchitecture.HasFlag(ProcessorArchitecture.Arm))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += arm;
            }

            if (processorArchitecture.HasFlag(ProcessorArchitecture.Arm64))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += arm64;
            }

            if (processorArchitecture.HasFlag(ProcessorArchitecture.Neutral))
            {
                if (!string.IsNullOrEmpty(architecture))
                {
                    architecture += " | ";
                }

                architecture += neutral;
            }

            if (string.IsNullOrEmpty(architecture))
            {
                architecture = ResourceService.GetLocalized("Installer/Unknown");
            }

            return string.Format(ResourceService.GetLocalized("Installer/BundleHeader"), architecture);
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

            // TODO:更改应用包的识别方法
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
        /// 解析应用捆绑包的所有文件
        /// </summary>
        private Dictionary<string, IAppxFile> ParsePacakgeBundleFiles(IAppxFilesEnumerator appxFilesEnumerator)
        {
            Dictionary<string, IAppxFile> bundleFileDict = [];

            while (appxFilesEnumerator.GetHasCurrent(out bool hasCurrent) is 0 && (hasCurrent is true))
            {
                appxFilesEnumerator.GetCurrent(out IAppxFile appxFile);
                appxFile.GetName(out string packageFileName);
                bundleFileDict.Add(packageFileName, appxFile);
                appxFilesEnumerator.MoveNext(out _);
            }

            return bundleFileDict;
        }

        /// <summary>
        /// 获取程序包面向的设备系列信息
        /// </summary>
        private List<TargetDeviceFamilyModel> ParsePackageTargetDeviceFamily(IAppxManifestTargetDeviceFamiliesEnumerator appxManifestTargetDeviceFamiliesEnumerator)
        {
            List<TargetDeviceFamilyModel> targetDeviceFamilyList = [];

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

            targetDeviceFamilyList.Sort((item1, item2) => item1.TargetDeviceName.CompareTo(item2.TargetDeviceName));
            return targetDeviceFamilyList;
        }

        /// <summary>
        /// 获取应用捆绑包资源文件
        /// </summary>
        private IStream GetPackageBundleResourceFileStream(Dictionary<string, IAppxFile> fileDict)
        {
            IStream resourceFileStream = null;

            // TODO:添加应用捆绑包资源文件解析
            return resourceFileStream;
        }

        /// <summary>
        /// 解析捆绑包适合主机系统的文件
        /// </summary>
        private IStream ParsePacakgeBundleFileStream(Dictionary<string, IAppxFile> bundleFileDict)
        {
            Architecture osArchitecture = RuntimeInformation.ProcessArchitecture;
            IStream parseFileStream = null;

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

            // TODO:添加资源包文件过滤（该类型包为空包）
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

            return parseFileStream;
        }

        #endregion 第六部分：解析应用包信息

        #region 第七部分：更新应用包信息

        /// <summary>
        /// 恢复默认内容
        /// </summary>
        private void ResetResult()
        {
            CanDragFile = false;
            IsLoadCompleted = false;
            IsParseSuccessfully = false;
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
            InstallDependencyCollection.Clear();
            TargetDeviceFamilyCollection.Clear();
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

                SupportedArchitecture = parseDict.TryGetValue("ProcessorArchitecture", out object architectureObj) && architectureObj is string architecture ? architecture : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PackageName = parseDict.TryGetValue("DisplayName", out object displayNameObj) && displayNameObj is string displayName ? displayName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PackageFamilyName = parseDict.TryGetValue("PackageFamilyName", out object packageFamilyNameObj) && packageFamilyNameObj is string packageFamilyName ? packageFamilyName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PackageFullName = parseDict.TryGetValue("PackageFullName", out object packageFullNameObj) && packageFullNameObj is string packageFullName ? packageFullName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                PublisherDisplayName = parseDict.TryGetValue("PublisherDisplayName", out object publisherDisplayNameObj) && publisherDisplayNameObj is string publisherDisplayName ? publisherDisplayName : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));
                IsFramework = parseDict.TryGetValue("IsFramework", out object isFrameworkObj) && isFrameworkObj is bool isFramework ? isFramework ? ResourceService.GetLocalized("Installer/Yes") : ResourceService.GetLocalized("Installer/No") : string.Format("[{0}]", ResourceService.GetLocalized("Installer/Unknown"));

                if (parseDict.TryGetValue("Version", out object versionObj) && versionObj is Version version)
                {
                    Version = version;
                }

                if (parseDict.TryGetValue("Description", out object descriptionObj) && descriptionObj is string description)
                {
                    PackageDescription = string.IsNullOrEmpty(description) ? ResourceService.GetLocalized("Installer/None") : description;
                }

                if (parseDict.TryGetValue("Resource", out object resourceListObj) && resourceListObj is List<string> resourceList)
                {
                }

                if (parseDict.TryGetValue("QualifiedResource", out object qualifiedResourceListObj) && qualifiedResourceListObj is List<string> qualifiedResourceList)
                {
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
    }
}

using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.WinGet;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Diagnostics;
using Windows.System;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 应用版本信息操作选项页面
    /// </summary>
    internal sealed partial class WinGetAppsVersionOptionsPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string PackageInstallModeDefaultString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageInstallModeDefault");
        private readonly string PackageInstallModeInteractiveString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageInstallModeInteractive");
        private readonly string PackageInstallModeSilentString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageInstallModeSilent");
        private readonly string PackageInstallScopeAnyString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageInstallScopeAny");
        private readonly string PackageInstallScopeUserString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageInstallScopeUser");
        private readonly string PackageInstallScopeSystemString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageInstallScopeSystem");
        private readonly string PackageRepairModeDefaultString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageRepairModeDefault");
        private readonly string PackageRepairModeInteractiveString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageRepairModeInteractive");
        private readonly string PackageRepairModeSilentString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageRepairModeSilent");
        private readonly string PackageRepairScopeAnyString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageRepairScopeAny");
        private readonly string PackageRepairScopeUserString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageRepairScopeUser");
        private readonly string PackageRepairScopeSystemString = ResourceService.GetLocalized("WinGetAppsVersionOptions/PackageRepairScopeSystem");
        private readonly string ProcessorArchitectureArm64String = ResourceService.GetLocalized("WinGetAppsVersionOptions/ProcessorArchitectureArm64");
        private readonly string ProcessorArchitectureDefaultString = ResourceService.GetLocalized("WinGetAppsVersionOptions/ProcessorArchitectureDefault");
        private readonly string ProcessorArchitectureX64String = ResourceService.GetLocalized("WinGetAppsVersionOptions/ProcessorArchitectureX64");
        private readonly string ProcessorArchitectureX86String = ResourceService.GetLocalized("WinGetAppsVersionOptions/ProcessorArchitectureX86");
        private readonly ProcessorArchitecture currentProcessorArchitecture = Package.Current.Id.Architecture;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private WinGetPage WinGetPage { get; set; }

        private WinGetAppsVersionDialog WinGetAppsVersionDialog { get; set; }

        private PackageOperationModel PackageOperation { get; set; }

        private string _winGetAppsOptionsTitle;

        private string WinGetAppsOptionsTitle
        {
            get { return _winGetAppsOptionsTitle; }

            set
            {
                if (!string.Equals(_winGetAppsOptionsTitle, value))
                {
                    _winGetAppsOptionsTitle = value;
                    PropertyChanged?.Invoke(this, new(nameof(WinGetAppsOptionsTitle)));
                }
            }
        }

        private PackageOperationKind _packageOperationKind;

        private PackageOperationKind PackageOperationKind
        {
            get { return _packageOperationKind; }

            set
            {
                if (!Equals(_packageOperationKind, value))
                {
                    _packageOperationKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageOperationKind)));
                }
            }
        }

        private bool _allowHashMismatch;

        private bool AllowHashMismatch
        {
            get { return _allowHashMismatch; }

            set
            {
                if (!Equals(_allowHashMismatch, value))
                {
                    _allowHashMismatch = value;
                    PropertyChanged?.Invoke(this, new(nameof(AllowHashMismatch)));
                }
            }
        }

        private string _packageDownloadPath;

        private string PackageDownloadPath
        {
            get { return _packageDownloadPath; }

            set
            {
                if (!string.Equals(_packageDownloadPath, value))
                {
                    _packageDownloadPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageDownloadPath)));
                }
            }
        }

        private ComboBoxItemModel _packageInstallScope;

        private ComboBoxItemModel PackageInstallScope
        {
            get { return _packageInstallScope; }

            set
            {
                if (!Equals(_packageInstallScope, value))
                {
                    _packageInstallScope = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageInstallScope)));
                }
            }
        }

        private ComboBoxItemModel _packageArchitecture;

        private ComboBoxItemModel PackageArchitecture
        {
            get { return _packageArchitecture; }

            set
            {
                if (!Equals(_packageArchitecture, value))
                {
                    _packageArchitecture = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageArchitecture)));
                }
            }
        }

        private bool _bypassIsStoreClientBlockedPolicyCheck;

        private bool BypassIsStoreClientBlockedPolicyCheck
        {
            get { return _bypassIsStoreClientBlockedPolicyCheck; }

            set
            {
                if (!Equals(_bypassIsStoreClientBlockedPolicyCheck, value))
                {
                    _bypassIsStoreClientBlockedPolicyCheck = value;
                    PropertyChanged?.Invoke(this, new(nameof(BypassIsStoreClientBlockedPolicyCheck)));
                }
            }
        }

        private bool _AllowUpgradeToUnknownVersion;

        private bool AllowUpgradeToUnknownVersion
        {
            get { return _AllowUpgradeToUnknownVersion; }

            set
            {
                if (!Equals(_AllowUpgradeToUnknownVersion, value))
                {
                    _AllowUpgradeToUnknownVersion = value;
                    PropertyChanged?.Invoke(this, new(nameof(AllowUpgradeToUnknownVersion)));
                }
            }
        }

        private bool _forceInstall;

        private bool ForceInstall
        {
            get { return _forceInstall; }

            set
            {
                if (!Equals(_forceInstall, value))
                {
                    _forceInstall = value;
                    PropertyChanged?.Invoke(this, new(nameof(ForceInstall)));
                }
            }
        }

        private ComboBoxItemModel _packageInstallMode;

        private ComboBoxItemModel PackageInstallMode
        {
            get { return _packageInstallMode; }

            set
            {
                if (!Equals(_packageInstallMode, value))
                {
                    _packageInstallMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageInstallMode)));
                }
            }
        }

        private string _packageInstallPath;

        private string PackageInstallPath
        {
            get { return _packageInstallPath; }

            set
            {
                if (!string.Equals(_packageInstallPath, value))
                {
                    _packageInstallPath = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageInstallPath)));
                }
            }
        }

        private bool _isX86ProcessorArchitecture;

        private bool IsX86ProcessorArchitecture
        {
            get { return _isX86ProcessorArchitecture; }

            set
            {
                if (!Equals(_isX86ProcessorArchitecture, value))
                {
                    _isX86ProcessorArchitecture = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsX86ProcessorArchitecture)));
                }
            }
        }

        private bool _isX64ProcessorArchitecture;

        private bool IsX64ProcessorArchitecture
        {
            get { return _isX64ProcessorArchitecture; }

            set
            {
                if (!Equals(_isX64ProcessorArchitecture, value))
                {
                    _isX64ProcessorArchitecture = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsX64ProcessorArchitecture)));
                }
            }
        }

        private bool _isArm64ProcessorArchitecture;

        private bool IsArm64ProcessorArchitecture
        {
            get { return _isArm64ProcessorArchitecture; }

            set
            {
                if (!Equals(_isArm64ProcessorArchitecture, value))
                {
                    _isArm64ProcessorArchitecture = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsArm64ProcessorArchitecture)));
                }
            }
        }

        private string _additionalInstallerArguments;

        private string AdditionalInstallerArguments
        {
            get { return _additionalInstallerArguments; }

            set
            {
                if (!string.Equals(_additionalInstallerArguments, value))
                {
                    _additionalInstallerArguments = value;
                    PropertyChanged?.Invoke(this, new(nameof(AdditionalInstallerArguments)));
                }
            }
        }

        private ComboBoxItemModel _packageRepairScope;

        private ComboBoxItemModel PackageRepairScope
        {
            get { return _packageRepairScope; }

            set
            {
                if (!Equals(_packageRepairScope, value))
                {
                    _packageRepairScope = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageRepairScope)));
                }
            }
        }

        private ComboBoxItemModel _packageRepairMode;

        private ComboBoxItemModel PackageRepairMode
        {
            get { return _packageRepairMode; }

            set
            {
                if (!Equals(_packageRepairMode, value))
                {
                    _packageRepairMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageRepairMode)));
                }
            }
        }

        private List<ComboBoxItemModel> PackageArchitectureList { get; } = [];

        private List<ComboBoxItemModel> PackageInstallScopeList { get; } = [];

        private List<ComboBoxItemModel> PackageInstallModeList { get; } = [];

        private List<ComboBoxItemModel> PackageRepairScopeList { get; } = [];

        private List<ComboBoxItemModel> PackageRepairModeList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal WinGetAppsVersionOptionsPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is List<object> argsList && argsList.Count is 3 && argsList[0] is WinGetPage winGetPage && argsList[1] is WinGetAppsVersionDialog winGetAppsVersionDialog && argsList[2] is PackageOperationModel packageOperation)
            {
                WinGetPage = winGetPage;
                WinGetAppsVersionDialog = winGetAppsVersionDialog;
                PackageOperation = packageOperation;
                PackageOperationKind = packageOperation.PackageOperationKind;

                switch (PackageOperationKind)
                {
                    case PackageOperationKind.Download:
                        {
                            WinGetAppsOptionsTitle = string.Format("{0} - {1}", packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            PackageArchitecture = PackageArchitectureList[0];
                            PackageInstallScope = PackageInstallScopeList[0];
                            PackageDownloadPath = WinGetConfigService.DefaultDownloadFolder;
                            break;
                        }
                    case PackageOperationKind.Install:
                        {
                            WinGetAppsOptionsTitle = string.Format("{0} - {1}", packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            BypassIsStoreClientBlockedPolicyCheck = false;
                            AllowUpgradeToUnknownVersion = false;
                            ForceInstall = false;
                            PackageInstallScope = PackageInstallScopeList[0];
                            PackageInstallMode = PackageInstallModeList[0];
                            PackageInstallPath = string.Empty;
                            IsX86ProcessorArchitecture = currentProcessorArchitecture is ProcessorArchitecture.X86;
                            IsX64ProcessorArchitecture = currentProcessorArchitecture is ProcessorArchitecture.X64;
                            IsArm64ProcessorArchitecture = currentProcessorArchitecture is ProcessorArchitecture.Arm64;
                            AdditionalInstallerArguments = string.Empty;
                            break;
                        }
                    case PackageOperationKind.Repair:
                        {
                            WinGetAppsOptionsTitle = string.Format("{0} - {1}", packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            BypassIsStoreClientBlockedPolicyCheck = false;
                            ForceInstall = false;
                            PackageRepairScope = PackageRepairScopeList[0];
                            PackageRepairMode = PackageRepairModeList[0];
                            break;
                        }
                    case PackageOperationKind.Upgrade:
                        {
                            WinGetAppsOptionsTitle = string.Format("{0} - {1}", packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            BypassIsStoreClientBlockedPolicyCheck = false;
                            AllowUpgradeToUnknownVersion = false;
                            ForceInstall = false;
                            PackageInstallScope = PackageInstallScopeList[0];
                            PackageInstallMode = PackageInstallModeList[0];
                            PackageInstallPath = string.Empty;
                            IsX86ProcessorArchitecture = currentProcessorArchitecture is ProcessorArchitecture.X86;
                            IsX64ProcessorArchitecture = currentProcessorArchitecture is ProcessorArchitecture.X64;
                            IsArm64ProcessorArchitecture = currentProcessorArchitecture is ProcessorArchitecture.Arm64;
                            AdditionalInstallerArguments = string.Empty;
                            break;
                        }
                }
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 下载应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageInstallScope)), DynamicWindowsRuntimeCast(typeof(ProcessorArchitecture))]
        private async void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.DownloadOptions = await CreateDownloadOptionsAsync(AllowHashMismatch, (ProcessorArchitecture)PackageArchitecture.SelectedValue, PackageDownloadPath, PackageOperation.PackageVersionId, (PackageInstallScope)PackageInstallScope.SelectedValue);
            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制下载命令信息
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ProcessorArchitecture)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnCopyDownloadTextClicked(object sender, RoutedEventArgs args)
        {
            string downloadArguments = await GenerateDownloadArgumentsAsync(true, PackageOperation.AppID, (ProcessorArchitecture)PackageArchitecture.SelectedValue, PackageDownloadPath, AllowHashMismatch, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.AppVersion);
            if (!string.IsNullOrEmpty(downloadArguments))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(downloadArguments);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 使用命令下载当前版本应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ProcessorArchitecture)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnDownloadWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string downloadArguments = await GenerateDownloadArgumentsAsync(false, PackageOperation.AppID, (ProcessorArchitecture)PackageArchitecture.SelectedValue, PackageDownloadPath, AllowHashMismatch, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.AppVersion);
            if (!string.IsNullOrEmpty(downloadArguments))
            {
                RunWinGetCommand(downloadArguments);
            }
        }

        /// <summary>
        /// 是否跳过哈希检验选择发生改变时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnAllowHashMismatchToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(AllowHashMismatch, toggleSwitch.IsOn))
            {
                AllowHashMismatch = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用包架构发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnPackageArchitectureSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(PackageArchitecture, comboBox.SelectedItem))
            {
                PackageArchitecture = comboBox.SelectedItem is ComboBoxItemModel packageArchitecture ? packageArchitecture : null;
            }
        }

        /// <summary>
        /// 应用安装包安装范围发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnPackageInstallScopeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(PackageInstallScope, comboBox.SelectedItem))
            {
                PackageInstallScope = comboBox.SelectedItem is ComboBoxItemModel packageInstallScope ? packageInstallScope : null;
            }
        }

        /// <summary>
        /// 打开应用下载路径
        /// </summary>
        private void OnPackageDownloadPathClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            OpenPackageDownloadPath(PackageDownloadPath);
        }

        /// <summary>
        /// 修改应用下载路径
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private async void OnChangePackageDownloadPathClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            PackageDownloadPath = WinGetConfigService.DefaultDownloadFolder;
                            break;
                        }
                    case "Download":
                        {
                            PackageDownloadPath = InfoHelper.UserDataPath.Downloads;
                            break;
                        }
                    case "Desktop":
                        {
                            PackageDownloadPath = InfoHelper.UserDataPath.Desktop;
                            break;
                        }
                    case "Custom":
                        {
                            try
                            {
                                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                                {
                                    SuggestedFolder = PackageDownloadPath
                                };

                                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                                {
                                    PackageDownloadPath = pickFolderResult.Path;
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetAppsVersionOptionsPage), nameof(OnChangePackageDownloadPathClicked), 1, e);
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FolderPicker));
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 安装当前版本应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageInstallMode)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnInstallClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.InstallOptions = await CreateInstallOptionsAsync(AdditionalInstallerArguments, AllowHashMismatch, AllowUpgradeToUnknownVersion, BypassIsStoreClientBlockedPolicyCheck, ForceInstall, (PackageInstallMode)PackageInstallMode.SelectedValue, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.PackageVersionId, PackageInstallPath, IsX86ProcessorArchitecture, IsX64ProcessorArchitecture, IsArm64ProcessorArchitecture);
            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制安装命令信息
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageInstallMode)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnCopyInstallTextClicked(object sender, RoutedEventArgs args)
        {
            string installArguments = await GenerateInstallArgumentsAsync(true, PackageOperation.AppID, AdditionalInstallerArguments, ForceInstall, AllowHashMismatch, (PackageInstallMode)PackageInstallMode.SelectedValue, PackageInstallPath, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.AppVersion);
            if (!string.IsNullOrEmpty(installArguments))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(installArguments);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 使用命令安装当前版本应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageInstallMode)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnInstallWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string installArguments = await GenerateInstallArgumentsAsync(false, PackageOperation.AppID, AdditionalInstallerArguments, ForceInstall, AllowHashMismatch, (PackageInstallMode)PackageInstallMode.SelectedValue, PackageInstallPath, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.AppVersion);
            if (!string.IsNullOrEmpty(installArguments))
            {
                RunWinGetCommand(installArguments);
            }
        }

        /// <summary>
        /// 跳过商店策略检查选项
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnBypassIsStoreClientBlockedPolicyCheckToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(BypassIsStoreClientBlockedPolicyCheck, toggleSwitch.IsOn))
            {
                BypassIsStoreClientBlockedPolicyCheck = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否允许升级到某一个未知版本
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnAllowUpgradeToUnknownVersionToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(AllowUpgradeToUnknownVersion, toggleSwitch.IsOn))
            {
                AllowUpgradeToUnknownVersion = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否强制安装 / 修复
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnForceInstallToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(ForceInstall, toggleSwitch.IsOn))
            {
                ForceInstall = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用包安装模式
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnPackageInstallModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(PackageInstallMode, comboBox.SelectedItem))
            {
                PackageInstallMode = comboBox.SelectedItem is ComboBoxItemModel packageInstallMode ? packageInstallMode : null;
            }
        }

        /// <summary>
        /// 额外安装参数文本框内容发生变化时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(TextBox))]
        private void OnAdditionalInstallerArgumentsTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is TextBox textBox)
            {
                AdditionalInstallerArguments = textBox.Text;
            }
        }

        /// <summary>
        /// 打开应用安装路径
        /// </summary>
        private void OnPackageInstallPathClicked(object sender, RoutedEventArgs args)
        {
            OpenPackageInstallPath(PackageInstallPath);
        }

        /// <summary>
        /// 修改应用安装路径
        /// </summary>
        private async void OnChangePackageInstallPathClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                {
                    SuggestedFolder = PackageDownloadPath
                };

                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                {
                    PackageInstallPath = pickFolderResult.Path;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetAppsVersionOptionsPage), nameof(OnChangePackageInstallPathClicked), 1, e);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FolderPicker));
            }
        }

        /// <summary>
        /// 允许的处理器架构发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ProcessorArchitecture)), DynamicWindowsRuntimeCast(typeof(ToggleButton))]
        private void OnProcessorArchitectureClicked(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleButton toggleButton && toggleButton.Tag is ProcessorArchitecture processorArchitecture)
            {
                switch (processorArchitecture)
                {
                    case ProcessorArchitecture.X86:
                        {
                            IsX86ProcessorArchitecture = !IsX86ProcessorArchitecture;
                            break;
                        }
                    case ProcessorArchitecture.X64:
                        {
                            IsX64ProcessorArchitecture = !IsX64ProcessorArchitecture;
                            break;
                        }
                    case ProcessorArchitecture.Arm64:
                        {
                            IsArm64ProcessorArchitecture = !IsArm64ProcessorArchitecture;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 修复当前版本应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageRepairMode)), DynamicWindowsRuntimeCast(typeof(PackageRepairScope))]
        private async void OnRepairClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.RepairOptions = await CreateRepairOptionsAsync(AllowHashMismatch, BypassIsStoreClientBlockedPolicyCheck, ForceInstall, (PackageRepairMode)PackageRepairMode.SelectedValue, (PackageRepairScope)PackageRepairScope.SelectedValue, PackageOperation.PackageVersionId);
            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制修复命令信息
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageRepairMode)), DynamicWindowsRuntimeCast(typeof(PackageRepairScope))]
        private async void OnCopyRepairTextClicked(object sender, RoutedEventArgs args)
        {
            string repairArguments = await GenerateRepairArgumentsAsync(true, PackageOperation.AppID, ForceInstall, AllowHashMismatch, (PackageRepairMode)PackageRepairMode.SelectedValue, PackageInstallPath, (PackageRepairScope)PackageRepairMode.SelectedValue, PackageOperation.AppVersion);
            if (!string.IsNullOrEmpty(repairArguments))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(repairArguments);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 使用命令修复当前版本应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageRepairMode)), DynamicWindowsRuntimeCast(typeof(PackageRepairScope))]
        private async void OnRepairWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string repairArguments = await GenerateRepairArgumentsAsync(false, PackageOperation.AppID, ForceInstall, AllowHashMismatch, (PackageRepairMode)PackageRepairMode.SelectedValue, PackageInstallPath, (PackageRepairScope)PackageRepairMode.SelectedValue, PackageOperation.AppVersion);
            if (!string.IsNullOrEmpty(repairArguments))
            {
                RunWinGetCommand(repairArguments);
            }
        }

        /// <summary>
        /// 应用安装包修复范围发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnPackageRepairScopeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(PackageRepairScope, comboBox.SelectedItem))
            {
                PackageRepairScope = comboBox.SelectedItem is ComboBoxItemModel packageRepairScope ? packageRepairScope : null;
            }
        }

        /// <summary>
        /// 应用包修复模式
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ComboBox))]
        private void OnPackageRepairModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is ComboBox comboBox && !Equals(PackageRepairMode, comboBox.SelectedItem))
            {
                PackageRepairMode = comboBox.SelectedItem is ComboBoxItemModel packageRepairMode ? packageRepairMode : null;
            }
        }

        /// <summary>
        /// 更新当前版本应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageInstallMode)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnUpgradeClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.InstallOptions = await CreateInstallOptionsAsync(AdditionalInstallerArguments, AllowHashMismatch, AllowUpgradeToUnknownVersion, BypassIsStoreClientBlockedPolicyCheck, ForceInstall, (PackageInstallMode)PackageInstallMode.SelectedValue, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.PackageVersionId, PackageInstallPath, IsX86ProcessorArchitecture, IsX64ProcessorArchitecture, IsArm64ProcessorArchitecture);
            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制更新命令信息
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageInstallMode)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnCopyUpgradeTextClicked(object sender, RoutedEventArgs args)
        {
            string upgradeArguments = await GenerateUpgradeArgumentsAsync(true, PackageOperation.AppID, AdditionalInstallerArguments, ForceInstall, AllowHashMismatch, (PackageInstallMode)PackageInstallMode.SelectedValue, PackageInstallPath, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.AppVersion);
            if (!string.IsNullOrEmpty(upgradeArguments))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(upgradeArguments);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 使用更新修复当前版本应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(PackageInstallMode)), DynamicWindowsRuntimeCast(typeof(PackageInstallScope))]
        private async void OnUpgradeWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string upgradeArguments = await GenerateUpgradeArgumentsAsync(false, PackageOperation.AppID, AdditionalInstallerArguments, ForceInstall, AllowHashMismatch, (PackageInstallMode)PackageInstallMode.SelectedValue, PackageInstallPath, (PackageInstallScope)PackageInstallScope.SelectedValue, PackageOperation.AppVersion);

            if (!string.IsNullOrEmpty(upgradeArguments))
            {
                RunWinGetCommand(upgradeArguments);
            }
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            PackageArchitectureList.Add(new() { SelectedValue = ProcessorArchitecture.Unknown, DisplayMember = ProcessorArchitectureDefaultString });
            PackageArchitectureList.Add(new() { SelectedValue = ProcessorArchitecture.X86, DisplayMember = ProcessorArchitectureX86String });
            PackageArchitectureList.Add(new() { SelectedValue = ProcessorArchitecture.X64, DisplayMember = ProcessorArchitectureX64String });
            PackageArchitectureList.Add(new() { SelectedValue = ProcessorArchitecture.Arm64, DisplayMember = ProcessorArchitectureArm64String });

            PackageInstallScopeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageInstallScope.Any, DisplayMember = PackageInstallScopeAnyString });
            PackageInstallScopeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageInstallScope.User, DisplayMember = PackageInstallScopeUserString });
            PackageInstallScopeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageInstallScope.System, DisplayMember = PackageInstallScopeSystemString });

            PackageInstallModeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageInstallMode.Default, DisplayMember = PackageInstallModeDefaultString });
            PackageInstallModeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageInstallMode.Interactive, DisplayMember = PackageInstallModeInteractiveString });
            PackageInstallModeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageInstallMode.Silent, DisplayMember = PackageInstallModeSilentString });

            PackageRepairScopeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageRepairScope.Any, DisplayMember = PackageRepairScopeAnyString });
            PackageRepairScopeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageRepairScope.User, DisplayMember = PackageRepairScopeUserString });
            PackageRepairScopeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageRepairScope.System, DisplayMember = PackageRepairScopeSystemString });

            PackageRepairModeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageRepairMode.Default, DisplayMember = PackageRepairModeDefaultString });
            PackageRepairModeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageRepairMode.Interactive, DisplayMember = PackageRepairModeInteractiveString });
            PackageRepairModeList.Add(new() { SelectedValue = Microsoft.Management.Deployment.PackageRepairMode.Silent, DisplayMember = PackageRepairModeSilentString });
        }

        /// <summary>
        /// 创建下载选项
        /// </summary>
        private async Task<DownloadOptions> CreateDownloadOptionsAsync(bool allowHashMismatch, ProcessorArchitecture packageArchitecture, string packageDownloadPath, PackageVersionId packageVersionId, PackageInstallScope packageInstallScope)
        {
            return await Task.Run(() =>
            {
                DownloadOptions downloadOptions = WinGetFactoryHelper.CreateDownloadOptions();
                downloadOptions.AcceptPackageAgreements = true;
                downloadOptions.AllowHashMismatch = allowHashMismatch;
                downloadOptions.Architecture = packageArchitecture;
                downloadOptions.DownloadDirectory = string.IsNullOrEmpty(packageDownloadPath) ? string.Empty : packageDownloadPath;
                downloadOptions.PackageVersionId = packageVersionId;
                downloadOptions.Scope = packageInstallScope;
                downloadOptions.SkipDependencies = false;
                return downloadOptions;
            });
        }

        /// <summary>
        /// 创建安装选项
        /// </summary>
        private async Task<InstallOptions> CreateInstallOptionsAsync(string additionalInstallerArguments, bool allowHashMismatch, bool allowUpgradeToUnknownVersion, bool bypassIsStoreClientBlockedPolicyCheck, bool forceInstall, PackageInstallMode packageInstallMode, PackageInstallScope packageInstallScope, PackageVersionId packageVersionId, string packageInstallPath, bool isX86ProcessorArchitecture, bool isX64ProcessorArchitecture, bool isArm64ProcessorArchitecture)
        {
            return await Task.Run(() =>
            {
                InstallOptions installOptions = WinGetFactoryHelper.CreateInstallOptions();
                installOptions.AcceptPackageAgreements = true;
                installOptions.AdditionalInstallerArguments = string.IsNullOrEmpty(additionalInstallerArguments) ? string.Empty : additionalInstallerArguments;
                installOptions.AllowHashMismatch = allowHashMismatch;
                installOptions.AllowUpgradeToUnknownVersion = allowUpgradeToUnknownVersion;
                installOptions.BypassIsStoreClientBlockedPolicyCheck = bypassIsStoreClientBlockedPolicyCheck;
                installOptions.Force = forceInstall;
                installOptions.LogOutputPath = LogService.WinGetFolderPath;
                installOptions.PackageInstallMode = packageInstallMode;
                installOptions.PackageInstallScope = packageInstallScope;
                installOptions.PackageVersionId = packageVersionId;
                installOptions.PreferredInstallLocation = string.IsNullOrEmpty(packageInstallPath) ? string.Empty : packageInstallPath;
                installOptions.SkipDependencies = false;
                installOptions.AllowedArchitectures.Clear();

                if (isX86ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.X86);
                }
                if (isX64ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.X64);
                }
                if (isArm64ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.Arm64);
                }

                return installOptions;
            });
        }

        /// <summary>
        /// 创建修复选项
        /// </summary>
        private async Task<RepairOptions> CreateRepairOptionsAsync(bool allowHashMismatch, bool bypassIsStoreClientBlockedPolicyCheck, bool forceInstall, PackageRepairMode packageRepairMode, PackageRepairScope packageRepairScope, PackageVersionId packageVersionId)
        {
            return await Task.Run(() =>
            {
                RepairOptions repairOptions = WinGetFactoryHelper.CreateRepairOptions();
                repairOptions.AcceptPackageAgreements = true;
                repairOptions.AllowHashMismatch = allowHashMismatch;
                repairOptions.BypassIsStoreClientBlockedPolicyCheck = bypassIsStoreClientBlockedPolicyCheck;
                repairOptions.Force = forceInstall;
                repairOptions.LogOutputPath = LogService.WinGetFolderPath;
                repairOptions.PackageRepairMode = packageRepairMode;
                repairOptions.PackageRepairScope = packageRepairScope;
                repairOptions.PackageVersionId = packageVersionId;
                return repairOptions;
            });
        }

        /// <summary>
        /// 生成下载参数
        /// </summary>
        private async Task<string> GenerateDownloadArgumentsAsync(bool hasProcessName, string appId, ProcessorArchitecture packageArchitecture, string packageDownloadPath, bool allowHashMismatch, PackageInstallScope packageInstallScope, string version)
        {
            return await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = [];
                if (hasProcessName)
                {
                    argsList.Add("winget.exe");
                }
                argsList.AddRange((string[])["download", "--id", string.Format(@"""{0}""", appId)]);
                argsList.Add("--accept-package-agreements");

                if (packageArchitecture is not ProcessorArchitecture.Unknown)
                {
                    argsList.Add("--architecture");
                    argsList.Add(string.Format(@"""{0}""", Convert.ToString(packageArchitecture)));
                }

                if (!string.IsNullOrEmpty(packageDownloadPath))
                {
                    argsList.Add("--download-directory");
                    argsList.Add(string.Format(@"""{0}""", packageDownloadPath));
                }

                if (allowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (packageInstallScope is Microsoft.Management.Deployment.PackageInstallScope.User)
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (packageInstallScope is Microsoft.Management.Deployment.PackageInstallScope.System)
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                argsList.Add("--skip-dependencies");

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                if (!string.IsNullOrEmpty(version))
                {
                    argsList.Add("--version");
                    argsList.Add(string.Format(@"""{0}""", version));
                }

                return string.Join(' ', argsList);
            });
        }

        /// <summary>
        /// 生成下载参数
        /// </summary>
        private async Task<string> GenerateInstallArgumentsAsync(bool hasProcessName, string appId, string additionalInstallerArguments, bool forceInstall, bool allowHashMismatch, PackageInstallMode packageInstallMode, string packageInstallPath, PackageInstallScope packageInstallScope, string version)
        {
            return await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = [];
                if (hasProcessName)
                {
                    argsList.Add("winget.exe");
                }
                argsList.AddRange((string[])["install", "--id", string.Format(@"""{0}""", appId)]);
                argsList.Add("--accept-package-agreements");

                if (!string.IsNullOrEmpty(additionalInstallerArguments))
                {
                    argsList.Add("--custom");
                    argsList.Add(string.Format(@"""{0}""", additionalInstallerArguments));
                }

                if (forceInstall)
                {
                    argsList.Add("--force");
                }

                if (allowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (packageInstallMode is Microsoft.Management.Deployment.PackageInstallMode.Interactive)
                {
                    argsList.Add("--interactive");
                }

                if (!string.IsNullOrEmpty(packageInstallPath))
                {
                    argsList.Add("--location");
                    argsList.Add(string.Format(@"""{0}""", packageInstallPath));
                }

                if (packageInstallScope is Microsoft.Management.Deployment.PackageInstallScope.User)
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (packageInstallScope is Microsoft.Management.Deployment.PackageInstallScope.System)
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (packageInstallMode is Microsoft.Management.Deployment.PackageInstallMode.Silent)
                {
                    argsList.Add("--silent");
                }

                argsList.Add("--skip-dependencies");

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                if (!string.IsNullOrEmpty(version))
                {
                    argsList.Add("--version");
                    argsList.Add(string.Format(@"""{0}""", version));
                }

                return string.Join(' ', argsList);
            });
        }

        /// <summary>
        /// 生成修复参数
        /// </summary>
        private async Task<string> GenerateRepairArgumentsAsync(bool hasProcessName, string appId, bool forceInstall, bool allowHashMismatch, PackageRepairMode packageRepairMode, string packageInstallPath, PackageRepairScope packageRepairScope, string version)
        {
            return await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = [];
                if (hasProcessName)
                {
                    argsList.Add("winget.exe");
                }
                argsList.AddRange((string[])["repair", "--id", string.Format(@"""{0}""", appId)]);
                argsList.Add("--accept-package-agreements");

                if (forceInstall)
                {
                    argsList.Add("--force");
                }

                if (allowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (packageRepairMode is Microsoft.Management.Deployment.PackageRepairMode.Interactive)
                {
                    argsList.Add("--interactive");
                }

                if (packageRepairScope is Microsoft.Management.Deployment.PackageRepairScope.User)
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (packageRepairScope is Microsoft.Management.Deployment.PackageRepairScope.System)
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (packageRepairMode is Microsoft.Management.Deployment.PackageRepairMode.Silent)
                {
                    argsList.Add("--silent");
                }

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                if (!string.IsNullOrEmpty(version))
                {
                    argsList.Add("--version");
                    argsList.Add(string.Format(@"""{0}""", version));
                }

                return string.Join(' ', argsList);
            });
        }

        /// <summary>
        /// 生成更新参数
        /// </summary>
        private async Task<string> GenerateUpgradeArgumentsAsync(bool hasProcessName, string appId, string additionalInstallerArguments, bool forceInstall, bool allowHashMismatch, PackageInstallMode packageInstallMode, string packageInstallPath, PackageInstallScope packageInstallScope, string version)
        {
            return await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = [];
                if (hasProcessName)
                {
                    argsList.Add("winget.exe");
                }
                argsList.AddRange((string[])["upgrade", "--id", string.Format(@"""{0}""", appId)]);

                argsList.Add("--accept-package-agreements");

                if (!string.IsNullOrEmpty(additionalInstallerArguments))
                {
                    argsList.Add("--custom");
                    argsList.Add(string.Format(@"""{0}""", additionalInstallerArguments));
                }

                if (forceInstall)
                {
                    argsList.Add("--force");
                }

                if (allowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (packageInstallMode is Microsoft.Management.Deployment.PackageInstallMode.Interactive)
                {
                    argsList.Add("--interactive");
                }

                if (!string.IsNullOrEmpty(packageInstallPath))
                {
                    argsList.Add("--location");
                    argsList.Add(string.Format(@"""{0}""", packageInstallPath));
                }

                if (packageInstallScope is Microsoft.Management.Deployment.PackageInstallScope.User)
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (packageInstallScope is Microsoft.Management.Deployment.PackageInstallScope.System)
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (packageInstallMode is Microsoft.Management.Deployment.PackageInstallMode.Silent)
                {
                    argsList.Add("--silent");
                }

                argsList.Add("--skip-dependencies");

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                if (!string.IsNullOrEmpty(version))
                {
                    argsList.Add("--version");
                    argsList.Add(string.Format(@"""{0}""", version));
                }

                return string.Join(' ', argsList);
            });
        }

        /// <summary>
        /// 运行 WinGet 命令
        /// </summary>
        private void RunWinGetCommand(string arguments)
        {
            if (!string.IsNullOrEmpty(arguments))
            {
                Task.Run(() =>
                {
                    Shell32Library.ShellExecute(nint.Zero, "open", "winget.exe", arguments, null, WindowShowStyle.SW_SHOWNORMAL);
                });
            }
        }

        /// <summary>
        /// 打开应用下载路径
        /// </summary>
        private void OpenPackageDownloadPath(string packageDownloadPath)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchFolderPathAsync(packageDownloadPath);
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开应用安装路径
        /// </summary>
        private void OpenPackageInstallPath(string packageInstallPath)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchFolderPathAsync(packageInstallPath);
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        private Visibility CheckPackagePathVisibility(string packagePath)
        {
            return string.IsNullOrEmpty(packagePath) ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}

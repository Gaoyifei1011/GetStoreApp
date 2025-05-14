using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Dialogs;
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
using Windows.Storage;
using Windows.System;

// 抑制 CA1822，CS8305，IDE0060 警告
#pragma warning disable CA1822,CS8305,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 应用版本信息操作选项页面
    /// </summary>
    public sealed partial class WinGetAppsVersionOptionsPage : Page, INotifyPropertyChanged
    {
        private readonly ProcessorArchitecture currentProcessorArchitecture = Package.Current.Id.Architecture;

        private WinGetPage WinGetPage { get; set; }

        private WinGetAppsVersionDialog WinGetAppsVersionDialog { get; set; }

        private PackageOperationModel PackageOperation { get; set; }

        private string _winGetAppsOptionsTitle;

        public string WinGetAppsOptionsTitle
        {
            get { return _winGetAppsOptionsTitle; }

            set
            {
                if (!Equals(_winGetAppsOptionsTitle, value))
                {
                    _winGetAppsOptionsTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinGetAppsOptionsTitle)));
                }
            }
        }

        private PackageOperationKind _packageOperationKind;

        public PackageOperationKind PackageOperationKind
        {
            get { return _packageOperationKind; }

            set
            {
                if (!Equals(_packageOperationKind, value))
                {
                    _packageOperationKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageOperationKind)));
                }
            }
        }

        private bool _allowHashMismatch;

        public bool AllowHashMismatch
        {
            get { return _allowHashMismatch; }

            set
            {
                if (!Equals(_allowHashMismatch, value))
                {
                    _allowHashMismatch = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowHashMismatch)));
                }
            }
        }

        private string _packageDownloadPath;

        public string PackageDownloadPath
        {
            get { return _packageDownloadPath; }

            set
            {
                if (!Equals(_packageDownloadPath, value))
                {
                    _packageDownloadPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageDownloadPath)));
                }
            }
        }

        private KeyValuePair<PackageInstallScope, string> _packageInstallScope;

        public KeyValuePair<PackageInstallScope, string> PackageInstallScope
        {
            get { return _packageInstallScope; }

            set
            {
                if (!Equals(_packageInstallScope, value))
                {
                    _packageInstallScope = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageInstallScope)));
                }
            }
        }

        private KeyValuePair<ProcessorArchitecture, string> _packageArchitecture;

        public KeyValuePair<ProcessorArchitecture, string> PackageArchitecture
        {
            get { return _packageArchitecture; }

            set
            {
                if (!Equals(_packageArchitecture, value))
                {
                    _packageArchitecture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageArchitecture)));
                }
            }
        }

        private bool _bypassIsStoreClientBlockedPolicyCheck;

        public bool BypassIsStoreClientBlockedPolicyCheck
        {
            get { return _bypassIsStoreClientBlockedPolicyCheck; }

            set
            {
                if (!Equals(_bypassIsStoreClientBlockedPolicyCheck, value))
                {
                    _bypassIsStoreClientBlockedPolicyCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BypassIsStoreClientBlockedPolicyCheck)));
                }
            }
        }

        private bool _AllowUpgradeToUnknownVersion;

        public bool AllowUpgradeToUnknownVersion
        {
            get { return _AllowUpgradeToUnknownVersion; }

            set
            {
                if (!Equals(_AllowUpgradeToUnknownVersion, value))
                {
                    _AllowUpgradeToUnknownVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowUpgradeToUnknownVersion)));
                }
            }
        }

        private bool _force;

        public bool Force
        {
            get { return _force; }

            set
            {
                if (!Equals(_force, value))
                {
                    _force = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Force)));
                }
            }
        }

        private KeyValuePair<PackageInstallMode, string> _packageInstallMode;

        public KeyValuePair<PackageInstallMode, string> PackageInstallMode
        {
            get { return _packageInstallMode; }

            set
            {
                if (!Equals(_packageInstallMode, value))
                {
                    _packageInstallMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageInstallMode)));
                }
            }
        }

        private string _packageInstallPath;

        public string PackageInstallPath
        {
            get { return _packageInstallPath; }

            set
            {
                if (!Equals(_packageInstallPath, value))
                {
                    _packageInstallPath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageInstallPath)));
                }
            }
        }

        private bool _isX86ProcessorArchitecture;

        public bool IsX86ProcessorArchitecture
        {
            get { return _isX86ProcessorArchitecture; }

            set
            {
                if (!Equals(_isX86ProcessorArchitecture, value))
                {
                    _isX86ProcessorArchitecture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsX86ProcessorArchitecture)));
                }
            }
        }

        private bool _isX64ProcessorArchitecture;

        public bool IsX64ProcessorArchitecture
        {
            get { return _isX64ProcessorArchitecture; }

            set
            {
                if (!Equals(_isX64ProcessorArchitecture, value))
                {
                    _isX64ProcessorArchitecture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsX64ProcessorArchitecture)));
                }
            }
        }

        private bool _isArm64ProcessorArchitecture;

        public bool IsArm64ProcessorArchitecture
        {
            get { return _isArm64ProcessorArchitecture; }

            set
            {
                if (!Equals(_isArm64ProcessorArchitecture, value))
                {
                    _isArm64ProcessorArchitecture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsArm64ProcessorArchitecture)));
                }
            }
        }

        private string _additionalInstallerArguments;

        public string AdditionalInstallerArguments
        {
            get { return _additionalInstallerArguments; }

            set
            {
                if (!Equals(_additionalInstallerArguments, value))
                {
                    _additionalInstallerArguments = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdditionalInstallerArguments)));
                }
            }
        }

        private KeyValuePair<PackageRepairScope, string> _packageRepairScope;

        public KeyValuePair<PackageRepairScope, string> PackageRepairScope
        {
            get { return _packageRepairScope; }

            set
            {
                if (!Equals(_packageRepairScope, value))
                {
                    _packageRepairScope = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageRepairScope)));
                }
            }
        }

        private KeyValuePair<PackageRepairMode, string> _packageRepairMode;

        public KeyValuePair<PackageRepairMode, string> PackageRepairMode
        {
            get { return _packageRepairMode; }

            set
            {
                if (!Equals(_packageRepairMode, value))
                {
                    _packageRepairMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageRepairMode)));
                }
            }
        }

        private List<KeyValuePair<ProcessorArchitecture, string>> PackageArchitectureList { get; } =
        [
            KeyValuePair.Create(ProcessorArchitecture.Unknown, ResourceService.GetLocalized("WinGet/ProcessorArchitectureDefault")),
            KeyValuePair.Create(ProcessorArchitecture.X86, ResourceService.GetLocalized("WinGet/ProcessorArchitectureX86")),
            KeyValuePair.Create(ProcessorArchitecture.X64, ResourceService.GetLocalized("WinGet/ProcessorArchitectureX64")),
            KeyValuePair.Create(ProcessorArchitecture.Arm64, ResourceService.GetLocalized("WinGet/ProcessorArchitectureArm64")),
        ];

        private List<KeyValuePair<PackageInstallScope, string>> PackageInstallScopeList { get; } =
        [
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageInstallScope.Any, ResourceService.GetLocalized("WinGet/PackageInstallScopeAny")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageInstallScope.User, ResourceService.GetLocalized("WinGet/PackageInstallScopeUser")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageInstallScope.System, ResourceService.GetLocalized("WinGet/PackageInstallScopeSystem")),
        ];

        private List<KeyValuePair<PackageInstallMode, string>> PackageInstallModeList { get; } =
        [
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageInstallMode.Default, ResourceService.GetLocalized("WinGet/PackageInstallModeDefault")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageInstallMode.Interactive, ResourceService.GetLocalized("WinGet/PackageInstallModeInteractive")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageInstallMode.Silent, ResourceService.GetLocalized("WinGet/PackageInstallModeSilent")),
        ];

        private List<KeyValuePair<PackageRepairScope, string>> PackageRepairScopeList { get; } =
        [
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageRepairScope.Any, ResourceService.GetLocalized("WinGet/PackageRepairScopeAny")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageRepairScope.User, ResourceService.GetLocalized("WinGet/PackageRepairScopeUser")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageRepairScope.System, ResourceService.GetLocalized("WinGet/PackageRepairScopeSystem")),
        ];

        private List<KeyValuePair<PackageRepairMode, string>> PackageRepairModeList { get; } =
        [
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageRepairMode.Default, ResourceService.GetLocalized("WinGet/PackageRepairModeDefault")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageRepairMode.Interactive, ResourceService.GetLocalized("WinGet/PackageRepairModeInteractive")),
            KeyValuePair.Create(Microsoft.Management.Deployment.PackageRepairMode.Silent, ResourceService.GetLocalized("WinGet/PackageRepairModeSilent")),
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetAppsVersionOptionsPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

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
                            PackageDownloadPath = WinGetConfigService.DefaultDownloadFolder.Path;
                            break;
                        }
                    case PackageOperationKind.Install:
                        {
                            WinGetAppsOptionsTitle = string.Format("{0} - {1}", packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            BypassIsStoreClientBlockedPolicyCheck = false;
                            AllowUpgradeToUnknownVersion = false;
                            Force = false;
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
                            Force = false;
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
                            Force = false;
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

        #endregion 第一部分：重写父类事件

        #region 第二部分：WinGet 应用版本信息操作选项页面——挂载的事件

        /// <summary>
        /// 下载应用
        /// </summary>
        private async void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.DownloadOptions = await Task.Run(() =>
            {
                return new DownloadOptions()
                {
                    AcceptPackageAgreements = true,
                    AllowHashMismatch = AllowHashMismatch,
                    Architecture = PackageArchitecture.Key,
                    DownloadDirectory = PackageDownloadPath,
                    PackageVersionId = PackageOperation.PackageVersionId,
                    Scope = PackageInstallScope.Key,
                    SkipDependencies = false,
                };
            });

            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制下载命令信息
        /// </summary>
        private async void OnCopyDownloadTextClicked(object sender, RoutedEventArgs args)
        {
            string downloadCommand = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["winget.exe", "download", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (!Equals(PackageArchitecture, PackageArchitectureList[0]))
                {
                    argsList.Add("--architecture");
                    argsList.Add(string.Format(@"""{0}""", Convert.ToString(PackageArchitecture.Key)));
                }

                if (!string.IsNullOrEmpty(PackageDownloadPath))
                {
                    argsList.Add("--download-directory");
                    argsList.Add(string.Format(@"""{0}""", PackageDownloadPath));
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageInstallScope, PackageInstallScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageInstallScope, PackageInstallScopeList[2]))
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

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(downloadCommand);
            await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetSearchDownload, copyResult));
        }

        /// <summary>
        /// 使用命令下载当前版本应用
        /// </summary>
        private async void OnDownloadWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string downloadParameter = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["download", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (!Equals(PackageArchitecture, PackageArchitectureList[0]))
                {
                    argsList.Add("--architecture");
                    argsList.Add(string.Format(@"""{0}""", Convert.ToString(PackageArchitecture.Key)));
                }

                if (!string.IsNullOrEmpty(PackageDownloadPath))
                {
                    argsList.Add("--download-directory");
                    argsList.Add(string.Format(@"""{0}""", PackageDownloadPath));
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageInstallScope, PackageInstallScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageInstallScope, PackageInstallScopeList[2]))
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

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            await Task.Run(() =>
            {
                Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", downloadParameter, null, WindowShowStyle.SW_SHOWNORMAL);
            });
        }

        /// <summary>
        /// 是否跳过哈希检验选择发生改变时触发的事件
        /// </summary>
        private void OnAllowHashMismatchToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AllowHashMismatch = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用包架构发生更改时触发的事件
        /// </summary>
        private void OnPackageArchitectureSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                PackageArchitecture = PackageArchitectureList[Convert.ToInt32(tag)];
            }
        }

        /// <summary>
        /// 应用安装包安装范围发生更改时触发的事件
        /// </summary>
        private void OnPackageInstallScopeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                PackageInstallScope = PackageInstallScopeList[Convert.ToInt32(tag)];
            }
        }

        /// <summary>
        /// 打开应用下载路径
        /// </summary>
        private void OnPackageDownloadPathClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchFolderPathAsync(PackageDownloadPath);
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 修改应用下载路径
        /// </summary>
        private async void OnChangePackageDownloadPathClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            PackageDownloadPath = WinGetConfigService.DefaultDownloadFolder.Path;
                            break;
                        }
                    case "Download":
                        {
                            PackageDownloadPath = (await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Downloads)).Path;
                            break;
                        }
                    case "Desktop":
                        {
                            PackageDownloadPath = (await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Desktop)).Path;
                            break;
                        }
                    case "Custom":
                        {
                            try
                            {
                                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                                {
                                    SuggestedStartLocation = PickerLocationId.Downloads
                                };

                                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                                {
                                    PackageDownloadPath = pickFolderResult.Path;
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Open folderPicker failed", e);
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FolderPicker));
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 安装当前版本应用
        /// </summary>
        private async void OnInstallClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.InstallOptions = await Task.Run(() =>
            {
                InstallOptions installOptions = new InstallOptions()
                {
                    AcceptPackageAgreements = true,
                    AdditionalInstallerArguments = AdditionalInstallerArguments,
                    AllowHashMismatch = AllowHashMismatch,
                    AllowUpgradeToUnknownVersion = AllowUpgradeToUnknownVersion,
                    BypassIsStoreClientBlockedPolicyCheck = BypassIsStoreClientBlockedPolicyCheck,
                    Force = Force,
                    LogOutputPath = LogService.WinGetFolderPath,
                    PackageInstallMode = PackageInstallMode.Key,
                    PackageInstallScope = PackageInstallScope.Key,
                    PackageVersionId = PackageOperation.PackageVersionId,
                    PreferredInstallLocation = PackageInstallPath,
                    SkipDependencies = false,
                };

                installOptions.AllowedArchitectures.Clear();
                if (IsX86ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.X86);
                }
                if (IsX64ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.X64);
                }
                if (IsArm64ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.Arm64);
                }

                return installOptions;
            });

            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制安装命令信息
        /// </summary>
        private async void OnCopyInstallTextClicked(object sender, RoutedEventArgs args)
        {
            string installCommand = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["winget.exe", "install", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (!string.IsNullOrEmpty(AdditionalInstallerArguments))
                {
                    argsList.Add("--custom");
                    argsList.Add(string.Format(@"""{0}""", AdditionalInstallerArguments));
                }

                if (Force)
                {
                    argsList.Add("--force");
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[1]))
                {
                    argsList.Add("--interactive");
                }

                if (!string.IsNullOrEmpty(PackageInstallPath))
                {
                    argsList.Add("--location");
                    argsList.Add(string.Format(@"""{0}""", PackageInstallPath));
                }

                if (Equals(PackageInstallScope, PackageInstallScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageInstallScope, PackageInstallScopeList[2]))
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[2]))
                {
                    argsList.Add("--silent");
                }

                argsList.Add("--skip-dependencies");

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(installCommand);
            await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetSearchInstall, copyResult));
        }

        /// <summary>
        /// 使用命令安装当前版本应用
        /// </summary>
        private async void OnInstallWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string installParameter = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["install", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (!string.IsNullOrEmpty(AdditionalInstallerArguments))
                {
                    argsList.Add("--custom");
                    argsList.Add(string.Format(@"""{0}""", AdditionalInstallerArguments));
                }

                if (Force)
                {
                    argsList.Add("--force");
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[1]))
                {
                    argsList.Add("--interactive");
                }

                if (!string.IsNullOrEmpty(PackageInstallPath))
                {
                    argsList.Add("--location");
                    argsList.Add(string.Format(@"""{0}""", PackageInstallPath));
                }

                if (Equals(PackageInstallScope, PackageInstallScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageInstallScope, PackageInstallScopeList[2]))
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[2]))
                {
                    argsList.Add("--silent");
                }

                argsList.Add("--skip-dependencies");

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            await Task.Run(() =>
            {
                Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", installParameter, null, WindowShowStyle.SW_SHOWNORMAL);
            });
        }

        /// <summary>
        /// 跳过商店策略检查选项
        /// </summary>
        private void OnBypassIsStoreClientBlockedPolicyCheckToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                BypassIsStoreClientBlockedPolicyCheck = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否允许升级到某一个未知版本
        /// </summary>
        private void OnAllowUpgradeToUnknownVersionToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AllowUpgradeToUnknownVersion = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否强制安装 / 修复
        /// </summary>
        private void OnForceToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                Force = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用包安装模式
        /// </summary>
        private void OnPackageInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                PackageInstallMode = PackageInstallModeList[Convert.ToInt32(tag)];
            }
        }

        /// <summary>
        /// 额外安装参数文本框内容发生变化时触发的事件
        /// </summary>
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
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchFolderPathAsync(PackageInstallPath);
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
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
                    SuggestedStartLocation = PickerLocationId.ComputerFolder
                };

                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                {
                    PackageInstallPath = pickFolderResult.Path;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Open folderPicker failed", e);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FolderPicker));
            }
        }

        /// <summary>
        /// 允许的处理器架构发生更改时触发的事件
        /// </summary>
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
        private async void OnRepairClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.RepairOptions = await Task.Run(() =>
            {
                return new RepairOptions()
                {
                    AcceptPackageAgreements = true,
                    AllowHashMismatch = AllowHashMismatch,
                    BypassIsStoreClientBlockedPolicyCheck = BypassIsStoreClientBlockedPolicyCheck,
                    Force = Force,
                    LogOutputPath = LogService.WinGetFolderPath,
                    PackageRepairMode = PackageRepairMode.Key,
                    PackageRepairScope = PackageRepairScope.Key,
                    PackageVersionId = PackageOperation.PackageVersionId,
                };
            });

            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制修复命令信息
        /// </summary>
        private async void OnCopyRepairTextClicked(object sender, RoutedEventArgs args)
        {
            string repairCommand = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["winget.exe", "repair", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (Force)
                {
                    argsList.Add("--force");
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageRepairMode, PackageRepairModeList[1]))
                {
                    argsList.Add("--interactive");
                }

                if (Equals(PackageRepairScope, PackageRepairScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageRepairScope, PackageRepairScopeList[2]))
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (Equals(PackageRepairMode, PackageRepairModeList[2]))
                {
                    argsList.Add("--silent");
                }

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(repairCommand);
            await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetSearchRepair, copyResult));
        }

        /// <summary>
        /// 使用命令修复当前版本应用
        /// </summary>
        private async void OnRepairWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string repairParameter = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["repair", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (Force)
                {
                    argsList.Add("--force");
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageRepairMode, PackageRepairModeList[1]))
                {
                    argsList.Add("--interactive");
                }

                if (Equals(PackageRepairScope, PackageRepairScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageRepairScope, PackageRepairScopeList[2]))
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (Equals(PackageRepairMode, PackageRepairModeList[2]))
                {
                    argsList.Add("--silent");
                }

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            await Task.Run(() =>
            {
                Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", repairParameter, null, WindowShowStyle.SW_SHOWNORMAL);
            });
        }

        /// <summary>
        /// 应用安装包修复范围发生更改时触发的事件
        /// </summary>
        private void OnPackageRepairScopeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                PackageRepairScope = PackageRepairScopeList[Convert.ToInt32(tag)];
            }
        }

        /// <summary>
        /// 应用包修复模式
        /// </summary>
        private void OnPackageRepairModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                PackageRepairMode = PackageRepairModeList[Convert.ToInt32(tag)];
            }
        }

        /// <summary>
        /// 更新当前版本应用
        /// </summary>
        private async void OnUpgradeClicked(object sender, RoutedEventArgs args)
        {
            PackageOperation.InstallOptions = await Task.Run(() =>
            {
                InstallOptions installOptions = new InstallOptions()
                {
                    AcceptPackageAgreements = true,
                    AdditionalInstallerArguments = AdditionalInstallerArguments,
                    AllowHashMismatch = AllowHashMismatch,
                    AllowUpgradeToUnknownVersion = AllowUpgradeToUnknownVersion,
                    BypassIsStoreClientBlockedPolicyCheck = BypassIsStoreClientBlockedPolicyCheck,
                    Force = Force,
                    LogOutputPath = LogService.WinGetFolderPath,
                    PackageInstallMode = PackageInstallMode.Key,
                    PackageInstallScope = PackageInstallScope.Key,
                    PackageVersionId = PackageOperation.PackageVersionId,
                    PreferredInstallLocation = PackageInstallPath,
                    SkipDependencies = false,
                };

                installOptions.AllowedArchitectures.Clear();
                if (IsX86ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.X86);
                }
                if (IsX64ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.X64);
                }
                if (IsArm64ProcessorArchitecture)
                {
                    installOptions.AllowedArchitectures.Add(ProcessorArchitecture.Arm64);
                }

                return installOptions;
            });

            WinGetAppsVersionDialog.Hide();
            await WinGetPage.AddTaskAsync(PackageOperation);
        }

        /// <summary>
        /// 复制更新命令信息
        /// </summary>
        private async void OnCopyUpgradeTextClicked(object sender, RoutedEventArgs args)
        {
            string upgradeCommand = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["winget.exe", "upgrade", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (!string.IsNullOrEmpty(AdditionalInstallerArguments))
                {
                    argsList.Add("--custom");
                    argsList.Add(string.Format(@"""{0}""", AdditionalInstallerArguments));
                }

                if (Force)
                {
                    argsList.Add("--force");
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[1]))
                {
                    argsList.Add("--interactive");
                }

                if (!string.IsNullOrEmpty(PackageInstallPath))
                {
                    argsList.Add("--location");
                    argsList.Add(string.Format(@"""{0}""", PackageInstallPath));
                }

                if (Equals(PackageInstallScope, PackageInstallScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageInstallScope, PackageInstallScopeList[2]))
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[2]))
                {
                    argsList.Add("--silent");
                }

                argsList.Add("--skip-dependencies");

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(upgradeCommand);
            await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetUpgradeInstall, copyResult));
        }

        /// <summary>
        /// 使用更新修复当前版本应用
        /// </summary>
        private async void OnUpgradeWithCmdClicked(object sender, RoutedEventArgs args)
        {
            string upgradeParameter = await Task.Run(() =>
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
                List<string> argsList = ["upgrade", "--id", string.Format(@"""{0}""", PackageOperation.AppID)];

                argsList.Add("--accept-package-agreements");

                if (!string.IsNullOrEmpty(AdditionalInstallerArguments))
                {
                    argsList.Add("--custom");
                    argsList.Add(string.Format(@"""{0}""", AdditionalInstallerArguments));
                }

                if (Force)
                {
                    argsList.Add("--force");
                }

                if (AllowHashMismatch)
                {
                    argsList.Add("--ignore-security-hash");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[1]))
                {
                    argsList.Add("--interactive");
                }

                if (!string.IsNullOrEmpty(PackageInstallPath))
                {
                    argsList.Add("--location");
                    argsList.Add(string.Format(@"""{0}""", PackageInstallPath));
                }

                if (Equals(PackageInstallScope, PackageInstallScopeList[1]))
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Equals(PackageInstallScope, PackageInstallScopeList[2]))
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                if (Equals(PackageInstallMode, PackageInstallModeList[2]))
                {
                    argsList.Add("--silent");
                }

                argsList.Add("--skip-dependencies");

                if (!Equals(winGetDataSourceName, default))
                {
                    argsList.Add("--source");
                    argsList.Add(string.Format(@"""{0}""", winGetDataSourceName.Key));
                }

                argsList.Add("--version");
                argsList.Add(string.Format(@"""{0}""", PackageOperation.AppVersion));

                return string.Join(' ', argsList);
            });

            await Task.Run(() =>
            {
                Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", upgradeParameter, null, WindowShowStyle.SW_SHOWNORMAL);
            });
        }

        #endregion 第二部分：WinGet 应用版本信息操作选项页面——挂载的事件

        private Visibility CheckPackagePath(string packagePath)
        {
            return string.IsNullOrEmpty(packagePath) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}

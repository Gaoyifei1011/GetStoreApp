using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
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
        private readonly string WinGetAppsDownloadOption = ResourceService.GetLocalized("WinGet/WinGetAppsDownloadOption");
        private readonly string WinGetAppsInstallOption = ResourceService.GetLocalized("WinGet/WinGetAppsInstallOption");
        private readonly string WinGetAppsRepairOption = ResourceService.GetLocalized("WinGet/WinGetAppsRepairOption");
        private readonly string WinGetAppsUpgradeOption = ResourceService.GetLocalized("WinGet/WinGetAppsUpgradeOption");

        private WinGetPage WinGetPage { get; set; }

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

        private PackageRepairScope _packageRepairScope;

        public PackageRepairScope PackageRepairScope
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

        private PackageRepairMode _packageRepairMode;

        public PackageRepairMode PackageRepairMode
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

            if (args.Parameter is List<object> argsList && argsList.Count is 2 && argsList[0] is WinGetPage winGetPage && argsList[1] is PackageOperationModel packageOperation)
            {
                WinGetPage = winGetPage;
                PackageOperation = packageOperation;
                PackageOperationKind = packageOperation.PackageOperationKind;

                switch (PackageOperationKind)
                {
                    case PackageOperationKind.Download:
                        {
                            WinGetAppsOptionsTitle = string.Format(WinGetAppsDownloadOption, packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            PackageArchitecture = PackageArchitectureList[0];
                            PackageInstallScope = PackageInstallScopeList[0];
                            PackageDownloadPath = WinGetConfigService.DefaultDownloadFolder.Path;
                            break;
                        }
                    case PackageOperationKind.Install:
                        {
                            WinGetAppsOptionsTitle = string.Format(WinGetAppsInstallOption, packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            BypassIsStoreClientBlockedPolicyCheck = false;
                            AllowUpgradeToUnknownVersion = false;
                            Force = false;
                            PackageInstallScope = PackageInstallScopeList[0];
                            PackageInstallMode = PackageInstallModeList[0];
                            PackageInstallPath = null;
                            AdditionalInstallerArguments = null;
                            break;
                        }
                    case PackageOperationKind.Repair:
                        {
                            WinGetAppsOptionsTitle = string.Format(WinGetAppsRepairOption, packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            BypassIsStoreClientBlockedPolicyCheck = false;
                            break;
                        }
                    case PackageOperationKind.Upgrade:
                        {
                            WinGetAppsOptionsTitle = string.Format(WinGetAppsUpgradeOption, packageOperation.AppName, packageOperation.AppVersion);
                            AllowHashMismatch = false;
                            BypassIsStoreClientBlockedPolicyCheck = false;
                            AllowUpgradeToUnknownVersion = false;
                            Force = false;
                            PackageInstallScope = PackageInstallScopeList[0];
                            PackageInstallMode = PackageInstallModeList[0];
                            PackageInstallPath = null;
                            AdditionalInstallerArguments = null;
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
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 复制下载命令信息
        /// </summary>
        private void OnCopyDownloadTextClicked(object sender, RoutedEventArgs args)
        {
            //KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
            //string copyContent = Equals(winGetDataSourceName, default) ? string.Format(@"winget download {0} -d ""{1}"" -v ""{2}""", SearchApps.AppID, WinGetConfigService.DownloadFolder.Path, SelectedItem.Version) : string.Format(@"winget download {0} -s ""{1}"" -d ""{2}"" -v ""{3}""", SearchApps.AppID, winGetDataSourceName.Key, WinGetConfigService.DownloadFolder.Path, SelectedItem.Version);
            //bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

            //await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetSearchDownload, copyResult));
        }

        /// <summary>
        /// 使用命令下载当前版本应用
        /// </summary>
        private void OnDownloadWithCmdClicked(object sender, RoutedEventArgs args)
        {
            //Task.Run(() =>
            //{
            //    KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

            //    if (Equals(winGetDataSourceName, default))
            //    {
            //        Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"download {0} -d ""{1}"" -v ""{2}""", SearchApps.AppID, WinGetConfigService.DownloadFolder.Path, SelectedItem.Version), null, WindowShowStyle.SW_SHOWNORMAL);
            //    }
            //    else
            //    {
            //        Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"download {0} -s ""{1}"" -d ""{2}"" -v ""{3}""", SearchApps.AppID, winGetDataSourceName.Key, WinGetConfigService.DownloadFolder.Path, SelectedItem.Version), null, WindowShowStyle.SW_SHOWNORMAL);
            //    }
            //});
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
        private void OnInstallClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 使用命令安装当前版本应用
        /// </summary>
        private void OnInstallWithCmdClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                //KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                //if (Equals(winGetDataSourceName, default))
                //{
                //    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"install {0} -v ""{1}""", SearchApps.AppID, SelectedItem.Version), null, WindowShowStyle.SW_SHOWNORMAL);
                //}
                //else
                //{
                //    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"install {0} -d ""{1}"" -v ""{2}""", SearchApps.AppID, winGetDataSourceName.Key, SelectedItem.Version), null, WindowShowStyle.SW_SHOWNORMAL);
                //}
            });
        }

        /// <summary>
        /// 复制安装命令信息
        /// </summary>
        private void OnCopyInstallTextClicked(object sender, RoutedEventArgs args)
        {
            //KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();
            //string copyContent = Equals(winGetDataSourceName, default) ? string.Format(@"winget install {0} -v ""{1}""", SearchApps.AppID, SelectedItem.Version) : string.Format(@"winget install {0} -s ""{1}"" -v ""{2}""", SearchApps.AppID, winGetDataSourceName.Key, SelectedItem.Version);
            //bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

            //await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetSearchInstall, copyResult));
        }

        /// <summary>
        ///
        /// </summary>
        private void OnBypassIsStoreClientBlockedPolicyCheckToggled(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 是否允许升级到某一个未知版本
        /// </summary>
        private void OnAllowUpgradeToUnknownVersionToggled(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 是否强制安装 / 修复
        /// </summary>
        private void OnForceToggled(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 应用包安装模式
        /// </summary>
        private void OnPackageInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 额外安装参数文本框内容发生变化时触发的事件
        /// </summary>
        private void OnAdditionalInstallerArgumentsTextChanged(object sender, TextChangedEventArgs args)
        {
        }

        /// <summary>
        /// 打开应用安装路径
        /// </summary>
        private void OnPackageInstallPathClicked(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// 修改应用安装路径
        /// </summary>
        private async void OnChangePackageInstallPathClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            PackageInstallPath = WinGetConfigService.DefaultDownloadFolder.Path;
                            break;
                        }
                    case "Download":
                        {
                            PackageInstallPath = (await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Downloads)).Path;
                            break;
                        }
                    case "Desktop":
                        {
                            PackageInstallPath = (await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Desktop)).Path;
                            break;
                        }
                    case "Custom":
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

                            break;
                        }
                }
            }
        }

        #endregion 第二部分：WinGet 应用版本信息操作选项页面——挂载的事件

        /// <summary>
        /// 检查安装包是安装还是更新
        /// </summary>
        private bool GetAppInstallOrUpgrade(PackageOperationKind packageOperationKind)
        {
            return packageOperationKind is PackageOperationKind.Install || packageOperationKind is PackageOperationKind.Upgrade;
        }
    }
}

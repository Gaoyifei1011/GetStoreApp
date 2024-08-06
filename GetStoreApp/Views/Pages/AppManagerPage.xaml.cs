using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.AppManager;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Management.Core;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;
using Windows.UI.StartScreen;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理页面
    /// </summary>
    public sealed partial class AppManagerPage : Page, INotifyPropertyChanged
    {
        private bool needToRefreshData = false;
        private AutoResetEvent autoResetEvent;
        private readonly PackageManager packageManager = new();

        private string Unknown { get; } = ResourceService.GetLocalized("AppManager/Unknown");

        private string Yes { get; } = ResourceService.GetLocalized("AppManager/Yes");

        private string No { get; } = ResourceService.GetLocalized("AppManager/No");

        private string PackageCountInfo { get; } = ResourceService.GetLocalized("AppManager/PackageCountInfo");

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }

            set
            {
                if (!Equals(_selectedIndex, value))
                {
                    _selectedIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

        private bool _isLoadedCompleted = false;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                if (!Equals(_isLoadedCompleted, value))
                {
                    _isLoadedCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadedCompleted)));
                }
            }
        }

        private bool _isPackageEmpty = true;

        public bool IsPackageEmpty
        {
            get { return _isPackageEmpty; }

            set
            {
                if (!Equals(_isPackageEmpty, value))
                {
                    _isPackageEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPackageEmpty)));
                }
            }
        }

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                if (!Equals(_isIncrease, value))
                {
                    _isIncrease = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncrease)));
                }
            }
        }

        private bool _isAppFramework = false;

        public bool IsAppFramework
        {
            get { return _isAppFramework; }

            set
            {
                if (!Equals(_isAppFramework, value))
                {
                    _isAppFramework = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAppFramework)));
                }
            }
        }

        private AppSortRuleKind _selectedRule = AppSortRuleKind.DisplayName;

        public AppSortRuleKind SelectedRule
        {
            get { return _selectedRule; }

            set
            {
                if (!Equals(_selectedRule, value))
                {
                    _selectedRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRule)));
                }
            }
        }

        private PackageSignKind _signType = PackageSignKind.Store;

        public PackageSignKind SignType
        {
            get { return _signType; }

            set
            {
                if (!Equals(_signType, value))
                {
                    _signType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignType)));
                }
            }
        }

        private string _displayName = string.Empty;

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

        private string _familyName = string.Empty;

        public string FamilyName
        {
            get { return _familyName; }

            set
            {
                if (!Equals(_familyName, value))
                {
                    _familyName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FamilyName)));
                }
            }
        }

        private string _fullName = string.Empty;

        public string FullName
        {
            get { return _fullName; }

            set
            {
                if (!Equals(_fullName, value))
                {
                    _fullName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
                }
            }
        }

        private string _description = string.Empty;

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

        private string _publisherName = string.Empty;

        public string PublisherName
        {
            get { return _publisherName; }

            set
            {
                if (!Equals(_publisherName, value))
                {
                    _publisherName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublisherName)));
                }
            }
        }

        private string _publisherId = string.Empty;

        public string PublisherId
        {
            get { return _publisherId; }

            set
            {
                if (!Equals(_publisherId, value))
                {
                    _publisherId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublisherId)));
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

        private string _installedDate;

        public string InstalledDate
        {
            get { return _installedDate; }

            set
            {
                if (!Equals(_installedDate, value))
                {
                    _installedDate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstalledDate)));
                }
            }
        }

        private string _architecture;

        public string Architecture
        {
            get { return _architecture; }

            set
            {
                if (!Equals(_architecture, value))
                {
                    _architecture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Architecture)));
                }
            }
        }

        private string _signatureKind;

        private string SignatureKind
        {
            get { return _signatureKind; }

            set
            {
                if (!Equals(_signatureKind, value))
                {
                    _signatureKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignatureKind)));
                }
            }
        }

        private string _resourceId;

        public string ResourceId
        {
            get { return _resourceId; }

            set
            {
                if (!Equals(_resourceId, value))
                {
                    _resourceId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResourceId)));
                }
            }
        }

        private string _isBundle;

        public string IsBundle
        {
            get { return _isBundle; }

            set
            {
                if (!Equals(_isBundle, value))
                {
                    _isBundle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBundle)));
                }
            }
        }

        private string _isDevelopmentMode;

        public string IsDevelopmentMode
        {
            get { return _isDevelopmentMode; }

            set
            {
                if (!Equals(_isDevelopmentMode, value))
                {
                    _isDevelopmentMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDevelopmentMode)));
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

        private string _isOptional;

        public string IsOptional
        {
            get { return _isOptional; }

            set
            {
                if (!Equals(_isOptional, value))
                {
                    _isOptional = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOptional)));
                }
            }
        }

        private string _isResourcePackage;

        public string IsResourcePackage
        {
            get { return _isResourcePackage; }

            set
            {
                if (!Equals(_isResourcePackage, value))
                {
                    _isResourcePackage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsResourcePackage)));
                }
            }
        }

        private string _isStub;

        public string IsStub
        {
            get { return _isStub; }

            set
            {
                if (!Equals(_isStub, value))
                {
                    _isStub = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStub)));
                }
            }
        }

        private string _vertifyIsOK;

        public string VertifyIsOK
        {
            get { return _vertifyIsOK; }

            set
            {
                if (!Equals(_vertifyIsOK, value))
                {
                    _vertifyIsOK = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VertifyIsOK)));
                }
            }
        }

        private readonly List<Package> MatchResultList = [];

        private ObservableCollection<PackageModel> AppManagerDataCollection { get; } = [];

        private ObservableCollection<AppListEntryModel> AppListEntryCollection { get; } = [];

        private ObservableCollection<PackageModel> DependenciesCollection { get; } = [];

        public ObservableCollection<DictionaryEntry> BreadCollection { get; } =
        [
            new DictionaryEntry(ResourceService.GetLocalized("AppManager/AppList"), "AppList")
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppManagerPage()
        {
            InitializeComponent();

            GetInstalledApps();
            InitializeData();
        }

        #region 第一部分：应用管理页面——XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制应用入口的应用程序用户模型 ID
        /// </summary>
        private void OnCopyAUMIDExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string aumid = args.Parameter as string;

            if (!string.IsNullOrEmpty(aumid))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(aumid);
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.AppUserModelId, copyResult));
            }
        }

        /// <summary>
        /// 复制依赖包信息
        /// </summary>
        private void OnCopyDependencyInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;

            if (package is not null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        StringBuilder copyBuilder = new();
                        copyBuilder.AppendLine(package.DisplayName);
                        copyBuilder.AppendLine(package.Id.FamilyName);
                        copyBuilder.AppendLine(package.Id.FullName);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyBuilder.ToString());
                            TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.DependencyInformation, copyResult));
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App information copy failed", e);
                    }
                });
            }
        }

        /// <summary>
        /// 复制依赖包名称
        /// </summary>
        private void OnCopyDependencyNameExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string displayName = args.Parameter as string;
            if (displayName is not null)
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(displayName);
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.DependencyName, copyResult));
            }
        }

        /// <summary>
        /// 启动对应入口的应用
        /// </summary>
        private void OnLaunchExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            AppListEntryModel appListEntryItem = args.Parameter as AppListEntryModel;

            Task.Run(async () =>
            {
                try
                {
                    await appListEntryItem.AppListEntry.LaunchAsync();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, string.Format("Open app {0} failed", appListEntryItem.DisplayName), e);
                }
            });
        }

        /// <summary>
        /// 打开应用
        /// </summary>
        private void OnOpenAppExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;

            if (package is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await package.GetAppListEntries()[0].LaunchAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Open app {0} failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用缓存目录
        /// </summary>
        private void OnOpenCacheFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;

            if (package is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        ApplicationData applicationData = ApplicationDataManager.CreateForPackageFamily(package.Id.FamilyName);
                        await Launcher.LaunchFolderAsync(applicationData.LocalFolder);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "Open app cache folder failed.", e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开安装目录
        /// </summary>
        private void OnOpenFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;

            if (package is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("{0} app installed folder open failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用安装目录
        /// </summary>
        private void OnOpenInstalledFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;

            if (package is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("{0} app installed folder open failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用清单文件
        /// </summary>
        private void OnOpenManifestExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;
            if (package is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(Path.Combine(package.InstalledPath, "AppxManifest.xml"));
                        if (file is not null)
                        {
                            await Launcher.LaunchFileAsync(file);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, string.Format("{0}'s AppxManifest.xml file open failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        private void OnOpenStoreExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;

            if (package is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={package.Id.FamilyName}"));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Open microsoft store {0} failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private void OnPinToDesktopExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Task.Run(() =>
            {
                bool isPinnedSuccessfully = false;

                try
                {
                    if (StoreConfiguration.IsPinToDesktopSupported())
                    {
                        StoreConfiguration.PinToDesktop(FamilyName);
                        isPinnedSuccessfully = true;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Create desktop shortcut failed.", e);
                }
                finally
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Desktop, isPinnedSuccessfully));
                    });
                }
            });
        }

        /// <summary>
        /// 固定应用入口到开始“屏幕”
        /// </summary>
        private void OnPinToStartScreenExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            AppListEntryModel appListEntryItem = args.Parameter as AppListEntryModel;

            if (appListEntryItem is not null)
            {
                Task.Run(async () =>
                {
                    bool isPinnedSuccessfully = false;

                    try
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        isPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(appListEntryItem.AppListEntry);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pin app to startscreen failed.", e);
                    }
                    finally
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.StartScreen, isPinnedSuccessfully));
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 固定应用入口到任务栏
        /// </summary>
        private void OnPinToTaskbarExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            AppListEntryModel appListEntryItem = args.Parameter as AppListEntryModel;

            Task.Run(async () =>
            {
                if (appListEntryItem is not null)
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri("taskbarpinner:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                        {
                            { "AppUserModelId", appListEntryItem.AppUserModelId },
                            { "PackageFullName", appListEntryItem.PackageFullName },
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pin app to taskbar failed.", e);
                    }
                }
            });
        }

        /// <summary>
        /// 更多按钮点击时显示菜单
        /// </summary>
        private void OnShowMoreExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            HyperlinkButton hyperlinkButton = args.Parameter as HyperlinkButton;
            if (hyperlinkButton is not null)
            {
                FlyoutBase.ShowAttachedFlyout(hyperlinkButton);
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        private void OnUnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            Package package = args.Parameter as Package;

            if (package is not null)
            {
                foreach (PackageModel packageItem in AppManagerDataCollection)
                {
                    if (packageItem.Package.Id.FullName == package.Id.FullName)
                    {
                        packageItem.IsUnInstalling = true;
                        break;
                    }
                }

                try
                {
                    Task.Run(() =>
                    {
                        IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> uninstallOperation = packageManager.RemovePackageAsync(package.Id.FullName);

                        AutoResetEvent uninstallCompletedEvent = new(false);

                        uninstallOperation.Completed = (result, progress) =>
                        {
                            // 卸载成功
                            if (result.Status is AsyncStatus.Completed)
                            {
                                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                                {
                                    foreach (PackageModel pacakgeItem in AppManagerDataCollection)
                                    {
                                        if (pacakgeItem.Package.Id.FullName == package.Id.FullName)
                                        {
                                            ToastNotificationService.Show(NotificationKind.UWPUnInstallSuccessfully, pacakgeItem.Package.DisplayName);

                                            AppManagerDataCollection.Remove(pacakgeItem);
                                            break;
                                        }
                                    }
                                });
                            }

                            // 卸载失败
                            else if (result.Status is AsyncStatus.Error)
                            {
                                DeploymentResult uninstallResult = uninstallOperation.GetResults();

                                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                                {
                                    foreach (PackageModel pacakgeItem in AppManagerDataCollection)
                                    {
                                        if (pacakgeItem.Package.Id.FullName == package.Id.FullName)
                                        {
                                            ToastNotificationService.Show(NotificationKind.UWPUnInstallFailed,
                                                pacakgeItem.Package.DisplayName,
                                                uninstallResult.ExtendedErrorCode.HResult.ToString(),
                                                uninstallResult.ErrorText
                                                );

                                            LogService.WriteLog(LoggingLevel.Information, string.Format("UnInstall app {0} failed", pacakgeItem.Package.DisplayName), uninstallResult.ExtendedErrorCode);

                                            pacakgeItem.IsUnInstalling = false;
                                            break;
                                        }
                                    }
                                });
                            }

                            uninstallCompletedEvent.Set();
                        };

                        uninstallCompletedEvent.WaitOne();
                        uninstallCompletedEvent.Dispose();
                    });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Information, string.Format("UnInstall app {0} failed", package.Id.FullName), e);
                }
            }
        }

        /// <summary>
        /// 查看应用信息
        /// </summary>
        private void OnViewInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            PackageModel packageItem = args.Parameter as PackageModel;

            if (packageItem is not null)
            {
                Task.Run(() =>
                {
                    Dictionary<string, object> packageDict = new()
                    {
                        ["DisplayName"] = packageItem.DisplayName
                    };

                    try
                    {
                        packageDict["FamilyName"] = string.IsNullOrEmpty(packageItem.Package.Id.FamilyName) ? Unknown : packageItem.Package.Id.FamilyName;
                    }
                    catch
                    {
                        packageDict["DisplayName"] = Unknown;
                    }

                    try
                    {
                        packageDict["FullName"] = string.IsNullOrEmpty(packageItem.Package.Id.FullName) ? Unknown : packageItem.Package.Id.FullName;
                    }
                    catch
                    {
                        packageDict["FullName"] = Unknown;
                    }

                    try
                    {
                        packageDict["Description"] = string.IsNullOrEmpty(packageItem.Package.Description) ? Unknown : packageItem.Package.Description;
                    }
                    catch
                    {
                        packageDict["FullName"] = Unknown;
                    }

                    packageDict["PublisherName"] = packageItem.PublisherName;

                    try
                    {
                        packageDict["PublisherId"] = string.IsNullOrEmpty(packageItem.Package.Id.PublisherId) ? Unknown : packageItem.Package.Id.PublisherId;
                    }
                    catch
                    {
                        packageDict["PublisherId"] = Unknown;
                    }

                    packageDict["Version"] = packageItem.Version;
                    packageDict["InstalledDate"] = packageItem.InstallDate;

                    try
                    {
                        packageDict["Architecture"] = string.IsNullOrEmpty(packageItem.Package.Id.Architecture.ToString()) ? Unknown : packageItem.Package.Id.Architecture.ToString();
                    }
                    catch
                    {
                        packageDict["Architecture"] = Unknown;
                    }

                    packageDict["SignatureKind"] = ResourceService.GetLocalized(string.Format("AppManager/Signature{0}", packageItem.SignatureKind.ToString()));

                    try
                    {
                        packageDict["ResourceId"] = string.IsNullOrEmpty(packageItem.Package.Id.ResourceId) ? Unknown : packageItem.Package.Id.ResourceId;
                    }
                    catch
                    {
                        packageDict["ResourceId"] = Unknown;
                    }

                    try
                    {
                        packageDict["IsBundle"] = packageItem.Package.IsBundle ? Yes : No;
                    }
                    catch
                    {
                        packageDict["IsBundle"] = Unknown;
                    }

                    try
                    {
                        packageDict["IsDevelopmentMode"] = packageItem.Package.IsDevelopmentMode ? Yes : No;
                    }
                    catch
                    {
                        packageDict["IsDevelopmentMode"] = Unknown;
                    }

                    packageDict["IsFramework"] = packageItem.IsFramework ? Yes : No;

                    try
                    {
                        packageDict["IsOptional"] = packageItem.Package.IsOptional ? Yes : No;
                    }
                    catch
                    {
                        packageDict["IsOptional"] = Unknown;
                    }

                    try
                    {
                        packageDict["IsResourcePackage"] = packageItem.Package.IsResourcePackage ? Yes : No;
                    }
                    catch
                    {
                        packageDict["IsResourcePackage"] = Unknown;
                    }

                    try
                    {
                        packageDict["IsStub"] = packageItem.Package.IsStub ? Yes : No;
                    }
                    catch
                    {
                        packageDict["IsStub"] = Unknown;
                    }

                    try
                    {
                        packageDict["VertifyIsOK"] = packageItem.Package.Status.VerifyIsOK() ? Yes : No;
                    }
                    catch
                    {
                        packageDict["VertifyIsOK"] = Unknown;
                    }

                    try
                    {
                        List<AppListEntryModel> appListEntryList = [];
                        IReadOnlyList<AppListEntry> appListEntriesList = packageItem.Package.GetAppListEntries();
                        for (int index = 0; index < appListEntriesList.Count; index++)
                        {
                            appListEntryList.Add(new AppListEntryModel()
                            {
                                DisplayName = appListEntriesList[index].DisplayInfo.DisplayName,
                                Description = appListEntriesList[index].DisplayInfo.Description,
                                AppUserModelId = appListEntriesList[index].AppUserModelId,
                                AppListEntry = appListEntriesList[index],
                                PackageFullName = packageItem.Package.Id.FullName
                            });
                        }
                        packageDict["AppListEntryCollection"] = appListEntryList;
                    }
                    catch
                    {
                        packageDict["AppListEntryCollection"] = new List<AppListEntry>();
                    }

                    try
                    {
                        List<PackageModel> dependenciesList = [];
                        IReadOnlyList<Package> dependcies = packageItem.Package.Dependencies;

                        if (dependcies.Count > 0)
                        {
                            for (int index = 0; index < dependcies.Count; index++)
                            {
                                try
                                {
                                    dependenciesList.Add(new PackageModel()
                                    {
                                        DisplayName = dependcies[index].DisplayName,
                                        PublisherName = dependcies[index].PublisherDisplayName,
                                        Version = new Version(dependcies[index].Id.Version.Major, dependcies[index].Id.Version.Minor, dependcies[index].Id.Version.Build, dependcies[index].Id.Version.Revision).ToString(),
                                        Package = dependcies[index]
                                    });
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }

                        dependenciesList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                        packageDict["DependenciesCollection"] = dependenciesList;
                    }
                    catch
                    {
                        packageDict["DependenciesCollection"] = new List<PackageModel>();
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        if (packageDict is not null)
                        {
                            InitializeAppInfo(packageDict);
                            BreadCollection.Add(new DictionaryEntry(ResourceService.GetLocalized("AppManager/AppInformation"), "AppInformation"));
                        }
                    });
                });
            }
        }

        #endregion 第一部分：应用管理页面——XamlUICommand 命令调用时挂载的事件

        #region 第二部分：应用管理页面——挂载的事件

        /// <summary>
        /// 打开设置中的安装的应用
        /// </summary>
        private async void OnInstalledAppsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        }

        /// <summary>
        /// 单击痕迹栏条目时发生的事件
        /// </summary>
        private void OnItemClicked(object sender, BreadcrumbBarItemClickedEventArgs args)
        {
            DictionaryEntry breadItem = (DictionaryEntry)args.Item;
            if (BreadCollection.Count is 2)
            {
                if (breadItem.Value.Equals(BreadCollection[0].Value))
                {
                    BackToAppList();
                }
            }
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                InitializeData(true);
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            if (autoSuggestBox is not null)
            {
                SearchText = autoSuggestBox.Text;
                if (SearchText.Equals(string.Empty))
                {
                    SearchText = string.Empty;
                    InitializeData();
                }
            }
        }

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem is not null)
            {
                IsIncrease = Convert.ToBoolean(toggleMenuFlyoutItem.Tag);
                InitializeData();
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem is not null)
            {
                SelectedRule = (AppSortRuleKind)toggleMenuFlyoutItem.Tag;
                InitializeData();
            }
        }

        /// <summary>
        /// 根据过滤方式对列表进行过滤
        /// </summary>
        private void OnFilterWayClicked(object sender, RoutedEventArgs args)
        {
            IsAppFramework = !IsAppFramework;
            needToRefreshData = true;
        }

        /// <summary>
        /// 根据签名规则进行过滤
        /// </summary>
        private void OnSignatureRuleClicked(object sender, RoutedEventArgs args)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if (toggleButton is not null)
            {
                if (SignType.HasFlag((PackageSignKind)toggleButton.Tag))
                {
                    SignType &= ~(PackageSignKind)toggleButton.Tag;
                }
                else
                {
                    SignType |= (PackageSignKind)toggleButton.Tag;
                }

                needToRefreshData = true;
            }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList.Clear();
            IsLoadedCompleted = false;
            SearchText = string.Empty;
            GetInstalledApps();
            InitializeData();
        }

        /// <summary>
        /// 浮出菜单关闭后更新数据
        /// </summary>
        private void OnClosed(object sender, object args)
        {
            if (needToRefreshData)
            {
                InitializeData();
            }

            needToRefreshData = false;
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private void OnCopyClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                StringBuilder copyBuilder = new();
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/DisplayName"), DisplayName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/FamilyName"), FamilyName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/FullName"), FullName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/Description"), Description));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/PublisherName"), PublisherName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/PublisherId"), PublisherId));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/Version"), Version));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/InstalledDate"), InstalledDate));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/Architecture"), Architecture));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/SignatureKind"), SignatureKind));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/ResourceId"), ResourceId));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsBundle"), IsBundle));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsDevelopmentMode"), IsDevelopmentMode));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsFramework"), IsFramework));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsOptional"), IsOptional));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsResourcePackage"), IsResourcePackage));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsStub"), IsStub));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/VertifyIsOK"), VertifyIsOK));

                DispatcherQueue.TryEnqueue(() =>
                {
                    bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyBuilder.ToString());
                    TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.PackageInformation, copyResult));
                });
            });
        }

        #endregion 第二部分：应用管理页面——挂载的事件

        /// <summary>
        /// 返回到应用信息页面
        /// </summary>
        public void BackToAppList()
        {
            if (BreadCollection.Count is 2)
            {
                BreadCollection.RemoveAt(1);
            }
        }

        /// <summary>
        /// 加载系统已安装的应用信息
        /// </summary>
        private void GetInstalledApps()
        {
            autoResetEvent ??= new AutoResetEvent(false);
            Task.Run(() =>
            {
                IEnumerable<Package> findResultList = packageManager.FindPackagesForUser(string.Empty);
                foreach (Package packageItem in findResultList)
                {
                    MatchResultList.Add(packageItem);
                }

                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                {
                    IsPackageEmpty = MatchResultList.Count is 0;
                });

                autoResetEvent?.Set();
            });
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private void InitializeData(bool hasSearchText = false)
        {
            IsLoadedCompleted = false;
            AppManagerDataCollection.Clear();

            Task.Run(() =>
            {
                autoResetEvent?.WaitOne();
                autoResetEvent?.Dispose();
                autoResetEvent = null;

                if (MatchResultList is not null)
                {
                    // 备份数据
                    List<Package> backupList = MatchResultList;
                    List<Package> appTypesList = [];

                    // 根据选项是否筛选包含框架包的数据
                    if (IsAppFramework)
                    {
                        foreach (Package packageItem in backupList)
                        {
                            if (packageItem.IsFramework == IsAppFramework)
                            {
                                appTypesList.Add(packageItem);
                            }
                        }
                    }
                    else
                    {
                        appTypesList = backupList;
                    }

                    List<Package> filteredList = [];
                    foreach (Package packageItem in appTypesList)
                    {
                        if (packageItem.SignatureKind.Equals(PackageSignatureKind.Store) && SignType.HasFlag(PackageSignKind.Store))
                        {
                            filteredList.Add(packageItem);
                        }
                        else if (packageItem.SignatureKind.Equals(PackageSignatureKind.System) && SignType.HasFlag(PackageSignKind.System))
                        {
                            filteredList.Add(packageItem);
                        }
                        else if (packageItem.SignatureKind.Equals(PackageSignatureKind.Enterprise) && SignType.HasFlag(PackageSignKind.Enterprise))
                        {
                            filteredList.Add(packageItem);
                        }
                        else if (packageItem.SignatureKind.Equals(PackageSignatureKind.Developer) && SignType.HasFlag(PackageSignKind.Developer))
                        {
                            filteredList.Add(packageItem);
                        }
                        else if (packageItem.SignatureKind.Equals(PackageSignatureKind.None) && SignType.HasFlag(PackageSignKind.None))
                        {
                            filteredList.Add(packageItem);
                        }
                    }

                    // 对过滤后的列表数据进行排序
                    switch (SelectedRule)
                    {
                        case AppSortRuleKind.DisplayName:
                            {
                                if (IsIncrease)
                                {
                                    filteredList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                }
                                else
                                {
                                    filteredList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                }
                                break;
                            }
                        case AppSortRuleKind.PublisherName:
                            {
                                if (IsIncrease)
                                {
                                    filteredList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                }
                                else
                                {
                                    filteredList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                }
                                break;
                            }
                        case AppSortRuleKind.InstallDate:
                            {
                                if (IsIncrease)
                                {
                                    filteredList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                }
                                else
                                {
                                    filteredList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                }
                                break;
                            }
                    }

                    List<PackageModel> packageList = [];

                    // 根据搜索条件对搜索符合要求的数据
                    if (hasSearchText)
                    {
                        for (int index = filteredList.Count - 1; index >= 0; index--)
                        {
                            if (!(filteredList[index].DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || filteredList[index].Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || filteredList[index].PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                            {
                                filteredList.RemoveAt(index);
                            }
                        }
                    }

                    foreach (Package packageItem in filteredList)
                    {
                        packageList.Add(new PackageModel()
                        {
                            IsFramework = GetIsFramework(packageItem),
                            AppListEntryCount = GetAppListEntriesCount(packageItem),
                            DisplayName = GetDisplayName(packageItem),
                            InstallDate = GetInstallDate(packageItem),
                            PublisherName = GetPublisherName(packageItem),
                            Version = GetVersion(packageItem),
                            SignatureKind = GetSignatureKind(packageItem),
                            InstalledDate = GetInstalledDate(packageItem),
                            Package = packageItem,
                            IsUnInstalling = false
                        });
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        foreach (PackageModel packageItem in packageList)
                        {
                            AppManagerDataCollection.Add(packageItem);
                        }

                        IsLoadedCompleted = true;
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsLoadedCompleted = true;
                    });
                }
            });
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        private void InitializeAppInfo(Dictionary<string, object> appInfoDict)
        {
            DisplayName = appInfoDict[nameof(DisplayName)].ToString();
            FamilyName = appInfoDict[nameof(FamilyName)].ToString();
            FullName = appInfoDict[nameof(FullName)].ToString();
            Description = appInfoDict[nameof(Description)].ToString();
            PublisherName = appInfoDict[nameof(PublisherName)].ToString();
            PublisherId = appInfoDict[nameof(PublisherId)].ToString();
            Version = appInfoDict[nameof(Version)].ToString();
            InstalledDate = appInfoDict[nameof(InstalledDate)].ToString();
            Architecture = appInfoDict[nameof(Architecture)].ToString();
            SignatureKind = appInfoDict[nameof(SignatureKind)].ToString();
            ResourceId = appInfoDict[nameof(ResourceId)].ToString();
            IsBundle = appInfoDict[nameof(IsBundle)].ToString();
            IsDevelopmentMode = appInfoDict[nameof(IsDevelopmentMode)].ToString();
            IsFramework = appInfoDict[nameof(IsFramework)].ToString();
            IsOptional = appInfoDict[nameof(IsOptional)].ToString();
            IsResourcePackage = appInfoDict[nameof(IsResourcePackage)].ToString();
            IsStub = appInfoDict[nameof(IsStub)].ToString();
            VertifyIsOK = appInfoDict[nameof(VertifyIsOK)].ToString();

            AppListEntryCollection.Clear();
            foreach (AppListEntryModel appListEntry in appInfoDict[nameof(AppListEntryCollection)] as List<AppListEntryModel>)
            {
                AppListEntryCollection.Add(appListEntry);
            }

            DependenciesCollection.Clear();
            foreach (PackageModel packageItem in appInfoDict[nameof(DependenciesCollection)] as List<PackageModel>)
            {
                DependenciesCollection.Add(packageItem);
            }
        }

        /// <summary>
        /// 获取应用包是否为框架包
        /// </summary>
        private static bool GetIsFramework(Package package)
        {
            try
            {
                return package.IsFramework;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取应用包的入口数
        /// </summary>
        private static int GetAppListEntriesCount(Package package)
        {
            try
            {
                return package.GetAppListEntries().Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取应用的显示名称
        /// </summary>
        private string GetDisplayName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.DisplayName) ? Unknown : package.DisplayName;
            }
            catch
            {
                return Unknown;
            }
        }

        /// <summary>
        /// 获取应用的发布者显示名称
        /// </summary>
        private string GetPublisherName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.PublisherDisplayName) ? Unknown : package.PublisherDisplayName;
            }
            catch
            {
                return Unknown;
            }
        }

        /// <summary>
        /// 获取应用的版本信息
        /// </summary>
        private static string GetVersion(Package package)
        {
            try
            {
                return new Version(package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision).ToString();
            }
            catch
            {
                return "0.0.0.0";
            }
        }

        /// <summary>
        /// 获取应用的安装日期
        /// </summary>
        private static string GetInstallDate(Package package)
        {
            try
            {
                return package.InstalledDate.ToString("yyyy/MM/dd HH:mm");
            }
            catch
            {
                return "1970/01/01 00:00";
            }
        }

        /// <summary>
        /// 获取应用包签名方式
        /// </summary>
        private static PackageSignatureKind GetSignatureKind(Package package)
        {
            try
            {
                return package.SignatureKind;
            }
            catch
            {
                return PackageSignatureKind.None;
            }
        }

        /// <summary>
        /// 获取应用包安装日期
        /// </summary>
        private static DateTimeOffset GetInstalledDate(Package package)
        {
            try
            {
                return package.InstalledDate;
            }
            catch
            {
                return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            }
        }
    }
}

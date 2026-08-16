using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.System;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理信息页
    /// </summary>
    internal sealed partial class AppInformationPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AppDescriptionString = ResourceService.GetLocalized("AppInformation/AppDescription");
        private readonly string AppDisplayNameString = ResourceService.GetLocalized("AppInformation/AppDisplayName");
        private readonly string ArchitectureString = ResourceService.GetLocalized("AppInformation/Architecture");
        private readonly string InstalledDateString = ResourceService.GetLocalized("AppInformation/InstalledDate");
        private readonly string IsBundleString = ResourceService.GetLocalized("AppInformation/IsBundle");
        private readonly string IsDevelopmentModeString = ResourceService.GetLocalized("AppInformation/IsDevelopmentMode");
        private readonly string IsFrameworkString = ResourceService.GetLocalized("AppInformation/IsFramework");
        private readonly string IsOptionalString = ResourceService.GetLocalized("AppInformation/IsOptional");
        private readonly string IsResourcePackageString = ResourceService.GetLocalized("AppInformation/IsResourcePackage");
        private readonly string IsStubString = ResourceService.GetLocalized("AppInformation/IsStub");
        private readonly string PackageFamilyNameString = ResourceService.GetLocalized("AppInformation/PackageFamilyName");
        private readonly string PackageFullNameString = ResourceService.GetLocalized("AppInformation/PackageFullName");
        private readonly string PublisherDisplayNameString = ResourceService.GetLocalized("AppInformation/PublisherDisplayName");
        private readonly string PublisherIdString = ResourceService.GetLocalized("AppInformation/PublisherId");
        private readonly string ResourceIdString = ResourceService.GetLocalized("AppInformation/ResourceId");
        private readonly string SignatureKindString = ResourceService.GetLocalized("AppInformation/SignatureKind");
        private readonly string VerifyIsOKString = ResourceService.GetLocalized("AppInformation/VerifyIsOK");
        private readonly string VersionString = ResourceService.GetLocalized("AppInformation/Version");
        private AppInformation appInformation;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private string _displayName;

        private string DisplayName
        {
            get { return _displayName; }

            set
            {
                if (!string.Equals(_displayName, value))
                {
                    _displayName = value;
                    PropertyChanged?.Invoke(this, new(nameof(DisplayName)));
                }
            }
        }

        private string _packageFamilyName;

        private string PackageFamilyName
        {
            get { return _packageFamilyName; }

            set
            {
                if (!string.Equals(_packageFamilyName, value))
                {
                    _packageFamilyName = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageFamilyName)));
                }
            }
        }

        private string _packageFullName;

        private string PackageFullName
        {
            get { return _packageFullName; }

            set
            {
                if (!string.Equals(_packageFullName, value))
                {
                    _packageFullName = value;
                    PropertyChanged?.Invoke(this, new(nameof(PackageFullName)));
                }
            }
        }

        private string _description;

        private string Description
        {
            get { return _description; }

            set
            {
                if (!string.Equals(_description, value))
                {
                    _description = value;
                    PropertyChanged?.Invoke(this, new(nameof(Description)));
                }
            }
        }

        private string _publisherDisplayName;

        private string PublisherDisplayName
        {
            get { return _publisherDisplayName; }

            set
            {
                if (!string.Equals(_publisherDisplayName, value))
                {
                    _publisherDisplayName = value;
                    PropertyChanged?.Invoke(this, new(nameof(PublisherDisplayName)));
                }
            }
        }

        private string _publisherId;

        private string PublisherId
        {
            get { return _publisherId; }

            set
            {
                if (!string.Equals(_publisherId, value))
                {
                    _publisherId = value;
                    PropertyChanged?.Invoke(this, new(nameof(PublisherId)));
                }
            }
        }

        private string _version;

        private string Version
        {
            get { return _version; }

            set
            {
                if (!string.Equals(_version, value))
                {
                    _version = value;
                    PropertyChanged?.Invoke(this, new(nameof(Version)));
                }
            }
        }

        private string _installedDate;

        private string InstalledDate
        {
            get { return _installedDate; }

            set
            {
                if (!string.Equals(_installedDate, value))
                {
                    _installedDate = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstalledDate)));
                }
            }
        }

        private string _architecture;

        private string Architecture
        {
            get { return _architecture; }

            set
            {
                if (!string.Equals(_architecture, value))
                {
                    _architecture = value;
                    PropertyChanged?.Invoke(this, new(nameof(Architecture)));
                }
            }
        }

        private string _signatureKind;

        private string SignatureKind
        {
            get { return _signatureKind; }

            set
            {
                if (!string.Equals(_signatureKind, value))
                {
                    _signatureKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(SignatureKind)));
                }
            }
        }

        private string _resourceId;

        private string ResourceId
        {
            get { return _resourceId; }

            set
            {
                if (!string.Equals(_resourceId, value))
                {
                    _resourceId = value;
                    PropertyChanged?.Invoke(this, new(nameof(ResourceId)));
                }
            }
        }

        private string _isBundle;

        private string IsBundle
        {
            get { return _isBundle; }

            set
            {
                if (!string.Equals(_isBundle, value))
                {
                    _isBundle = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsBundle)));
                }
            }
        }

        private string _isDevelopmentMode;

        private string IsDevelopmentMode
        {
            get { return _isDevelopmentMode; }

            set
            {
                if (!string.Equals(_isDevelopmentMode, value))
                {
                    _isDevelopmentMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsDevelopmentMode)));
                }
            }
        }

        private string _isFramework;

        private string IsFramework
        {
            get { return _isFramework; }

            set
            {
                if (!string.Equals(_isFramework, value))
                {
                    _isFramework = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsFramework)));
                }
            }
        }

        private string _isOptional;

        private string IsOptional
        {
            get { return _isOptional; }

            set
            {
                if (!string.Equals(_isOptional, value))
                {
                    _isOptional = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsOptional)));
                }
            }
        }

        private string _isResourcePackage;

        private string IsResourcePackage
        {
            get { return _isResourcePackage; }

            set
            {
                if (!string.Equals(_isResourcePackage, value))
                {
                    _isResourcePackage = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsResourcePackage)));
                }
            }
        }

        private string _isStub;

        private string IsStub
        {
            get { return _isStub; }

            set
            {
                if (!string.Equals(_isStub, value))
                {
                    _isStub = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsStub)));
                }
            }
        }

        private string _verifyIsOK;

        private string VerifyIsOK
        {
            get { return _verifyIsOK; }

            set
            {
                if (!string.Equals(_verifyIsOK, value))
                {
                    _verifyIsOK = value;
                    PropertyChanged?.Invoke(this, new(nameof(VerifyIsOK)));
                }
            }
        }

        private ObservableCollection<AppListEntryModel> AppListEntryCollection { get; } = [];

        private ObservableCollection<PackageModel> DependenciesCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal AppInformationPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is AppInformation appInformation)
            {
                InitializeData(appInformation);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 启动对应入口的应用
        /// </summary>
        private void OnLaunchExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is AppListEntryModel appListEntry)
            {
                LaunchApp(appListEntry);
            }
        }

        /// <summary>
        /// 打开安装目录
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Package))]
        private void OnOpenFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                OpenInstalledPath(package);
            }
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Package))]
        private void OnOpenStoreExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                OpenStore(package);
            }
        }

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private async void OnPinToDesktopExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            bool isPinnedSuccessfully = await PinToDesktopAsync(PackageFamilyName);
            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Desktop, isPinnedSuccessfully));
        }

        /// <summary>
        /// 固定应用入口到开始“屏幕”
        /// </summary>
        private async void OnPinToStartScreenExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is AppListEntryModel appListEntry)
            {
                bool isPinnedSuccessfully = await PinToStartScreenAsync(appListEntry);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.StartScreen, isPinnedSuccessfully));
            }
        }

        /// <summary>
        /// 固定应用入口到任务栏
        /// </summary>
        private void OnPinToTaskbarExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is AppListEntryModel appListEntry)
            {
                PinToTaskbar(appListEntry);
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyClicked(object sender, RoutedEventArgs args)
        {
            List<string> appInformationCopyStringList = await GetAppInformationCopyListAsync(appInformation);

            if (appInformationCopyStringList is not null)
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, appInformationCopyStringList));
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData(AppInformation appInformationItem)
        {
            if (appInformationItem is not null)
            {
                appInformation = appInformationItem;
                DisplayName = appInformation.DisplayName;
                PackageFamilyName = appInformation.PackageFamilyName;
                PackageFullName = appInformation.PackageFullName;
                Description = appInformation.Description;
                PublisherDisplayName = appInformation.PublisherDisplayName;
                PublisherId = appInformation.PublisherId;
                Version = appInformation.Version;
                InstalledDate = appInformation.InstallDate;
                Architecture = appInformation.Architecture;
                SignatureKind = appInformation.SignatureKind;
                ResourceId = appInformation.ResourceId;
                IsBundle = appInformation.IsBundle;
                IsDevelopmentMode = appInformation.IsDevelopmentMode;
                IsFramework = appInformation.IsFramework;
                IsOptional = appInformation.IsOptional;
                IsResourcePackage = appInformation.IsResourcePackage;
                IsStub = appInformation.IsStub;
                VerifyIsOK = appInformation.VerifyIsOK;

                AppListEntryCollection.Clear();
                foreach (AppListEntryModel appListEntry in appInformation.AppListEntryList)
                {
                    AppListEntryCollection.Add(appListEntry);
                }

                DependenciesCollection.Clear();
                foreach (PackageModel packageItem in appInformation.DependenciesList)
                {
                    DependenciesCollection.Add(packageItem);
                }
            }
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        private void LaunchApp(AppListEntryModel appListEntry)
        {
            if (appListEntry is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await appListEntry.AppListEntry.LaunchAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(LaunchApp), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开安装目录
        /// </summary>
        private void OpenInstalledPath(Package package)
        {
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
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OpenInstalledPath), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        private void OpenStore(Package package)
        {
            if (package is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new($"ms-windows-store://pdp/?PFN={package.Id.FamilyName}"));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OpenStore), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private async Task<bool> PinToDesktopAsync(string packageFamilyName)
        {
            if (!string.IsNullOrEmpty(packageFamilyName))
            {
                return await Task.Run(() =>
                {
                    bool isPinnedSuccessfully = false;

                    try
                    {
                        if (StoreConfiguration.IsPinToDesktopSupported())
                        {
                            StoreConfiguration.PinToDesktop(packageFamilyName);
                            isPinnedSuccessfully = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(PinToDesktopAsync), 1, e);
                    }
                    return isPinnedSuccessfully;
                });
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 固定应用到开始屏幕
        /// </summary>
        private async Task<bool> PinToStartScreenAsync(AppListEntryModel appListEntry)
        {
            if (appListEntry is not null)
            {
                return await Task.Run(async () =>
                {
                    bool isPinnedSuccessfully = false;

                    try
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        isPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(appListEntry.AppListEntry);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(PinToStartScreenAsync), 1, e);
                    }
                    return isPinnedSuccessfully;
                });
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 固定应用到任务栏
        /// </summary>
        private void PinToTaskbar(AppListEntryModel appListEntry)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("getstoreapppinner:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                        {
                            {"Type", nameof(TaskbarManager) },
                            { "AppUserModelId", appListEntry.AppUserModelId },
                            { "PackageFullName", appListEntry.PackageFullName },
                        });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(PinToTaskbar), 1, e);
                }
            });
        }

        /// <summary>
        /// 获取应用信息要准备复制的字符串内容
        /// </summary>
        private async Task<List<string>> GetAppInformationCopyListAsync(AppInformation appInformation)
        {
            if (appInformation is not null)
            {
                return await Task.Run(() =>
                {
                    List<string> copyStringList = [];

                    copyStringList.Add(string.Format("{0}:\t{1}", AppDisplayNameString, appInformation.DisplayName));
                    copyStringList.Add(string.Format("{0}:\t{1}", PackageFamilyNameString, appInformation.PackageFamilyName));
                    copyStringList.Add(string.Format("{0}:\t{1}", PackageFullNameString, appInformation.PackageFullName));
                    copyStringList.Add(string.Format("{0}:\t{1}", AppDescriptionString, appInformation.Description));
                    copyStringList.Add(string.Format("{0}:\t{1}", PublisherDisplayNameString, appInformation.PublisherDisplayName));
                    copyStringList.Add(string.Format("{0}:\t{1}", PublisherIdString, appInformation.PublisherId));
                    copyStringList.Add(string.Format("{0}:\t{1}", VersionString, appInformation.Version));
                    copyStringList.Add(string.Format("{0}:\t{1}", InstalledDateString, appInformation.InstallDate));
                    copyStringList.Add(string.Format("{0}:\t{1}", ArchitectureString, appInformation.Architecture));
                    copyStringList.Add(string.Format("{0}:\t{1}", SignatureKindString, appInformation.SignatureKind));
                    copyStringList.Add(string.Format("{0}:\t{1}", ResourceIdString, appInformation.ResourceId));
                    copyStringList.Add(string.Format("{0}:\t{1}", IsBundleString, appInformation.IsBundle));
                    copyStringList.Add(string.Format("{0}:\t{1}", IsDevelopmentModeString, appInformation.IsDevelopmentMode));
                    copyStringList.Add(string.Format("{0}:\t{1}", IsFrameworkString, appInformation.IsFramework));
                    copyStringList.Add(string.Format("{0}:\t{1}", IsOptionalString, appInformation.IsOptional));
                    copyStringList.Add(string.Format("{0}:\t{1}", IsResourcePackageString, appInformation.IsResourcePackage));
                    copyStringList.Add(string.Format("{0}:\t{1}", IsStubString, appInformation.IsStub));
                    copyStringList.Add(string.Format("{0}:\t{1}", VerifyIsOKString, appInformation.VerifyIsOK));
                    return copyStringList;
                });
            }
            else
            {
                return default;
            }
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}

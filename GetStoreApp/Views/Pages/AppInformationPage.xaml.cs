using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
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

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理信息页
    /// </summary>
    public sealed partial class AppInformationPage : Page, INotifyPropertyChanged
    {
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
        private readonly string VersionString = ResourceService.GetLocalized("AppInformation/Version");
        private readonly string VertifyIsOKString = ResourceService.GetLocalized("AppInformation/VertifyIsOK");

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

        private string _packageFamilyName = string.Empty;

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

        private string _packageFullName = string.Empty;

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

        private string _publisherDisplayName = string.Empty;

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

        private ObservableCollection<AppListEntryModel> AppListEntryCollection { get; } = [];

        private ObservableCollection<PackageModel> DependenciesCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInformationPage()
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

            if (args.Parameter is AppInformation appInformation)
            {
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
                VertifyIsOK = appInformation.VertifyIsOK;

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

        #endregion 第一部分：重写父类事件

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制应用入口的应用程序用户模型 ID
        /// </summary>
        private async void OnCopyAUMIDExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string aumid && !string.IsNullOrEmpty(aumid))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(aumid);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制依赖包信息
        /// </summary>
        private async void OnCopyDependencyInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                List<string> copyDependencyInformationCopyStringList = [];

                await Task.Run(() =>
                {
                    try
                    {
                        copyDependencyInformationCopyStringList.Add(package.DisplayName);
                        copyDependencyInformationCopyStringList.Add(package.Id.FamilyName);
                        copyDependencyInformationCopyStringList.Add(package.Id.FullName);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OnCopyDependencyInformationExecuteRequested), 1, e);
                    }
                });

                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyDependencyInformationCopyStringList));
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 复制依赖包名称
        /// </summary>
        private async void OnCopyDependencyNameExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string displayName && !string.IsNullOrEmpty(displayName))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(displayName);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 启动对应入口的应用
        /// </summary>
        private void OnLaunchExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is AppListEntryModel appListEntry)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await appListEntry.AppListEntry.LaunchAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OnLaunchExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开安装目录
        /// </summary>
        private void OnOpenFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OnOpenFolderExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        private void OnOpenStoreExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={package.Id.FamilyName}"));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OnOpenStoreExecuteRequested), 1, e);
                    }
                });
            }
        }

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private async void OnPinToDesktopExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            await Task.Run(() =>
            {
                try
                {
                    if (StoreConfiguration.IsPinToDesktopSupported())
                    {
                        StoreConfiguration.PinToDesktop(PackageFamilyName);
                        isPinnedSuccessfully = true;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OnPinToDesktopExecuteRequested), 1, e);
                }
            });

            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.Desktop, isPinnedSuccessfully));
        }

        /// <summary>
        /// 固定应用入口到开始“屏幕”
        /// </summary>
        private async void OnPinToStartScreenExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is AppListEntryModel appListEntry)
            {
                bool isPinnedSuccessfully = false;

                await Task.Run(async () =>
                {
                    try
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        isPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(appListEntry.AppListEntry);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OnPinToStartScreenExecuteRequested), 1, e);
                    }
                });

                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.StartScreen, isPinnedSuccessfully));
            }
        }

        /// <summary>
        /// 固定应用入口到任务栏
        /// </summary>
        private void OnPinToTaskbarExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is AppListEntryModel appListEntry)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri("getstoreapppinner:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                        {
                            {"Type", nameof(TaskbarManager) },
                            { "AppUserModelId", appListEntry.AppUserModelId },
                            { "PackageFullName", appListEntry.PackageFullName },
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(OnPinToTaskbarExecuteRequested), 1, e);
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：应用信息页面——挂载的事件

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private async void OnCopyClicked(object sender, RoutedEventArgs args)
        {
            List<string> copyStringList = await Task.Run(() =>
            {
                List<string> copyStringList = [];

                copyStringList.Add(string.Format("{0}:\t{1}", AppDisplayNameString, DisplayName));
                copyStringList.Add(string.Format("{0}:\t{1}", PackageFamilyNameString, PackageFamilyName));
                copyStringList.Add(string.Format("{0}:\t{1}", PackageFullNameString, PackageFullName));
                copyStringList.Add(string.Format("{0}:\t{1}", AppDescriptionString, Description));
                copyStringList.Add(string.Format("{0}:\t{1}", PublisherDisplayNameString, PublisherDisplayName));
                copyStringList.Add(string.Format("{0}:\t{1}", PublisherIdString, PublisherId));
                copyStringList.Add(string.Format("{0}:\t{1}", VersionString, Version));
                copyStringList.Add(string.Format("{0}:\t{1}", InstalledDateString, InstalledDate));
                copyStringList.Add(string.Format("{0}:\t{1}", ArchitectureString, Architecture));
                copyStringList.Add(string.Format("{0}:\t{1}", SignatureKindString, SignatureKind));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceIdString, ResourceId));
                copyStringList.Add(string.Format("{0}:\t{1}", IsBundleString, IsBundle));
                copyStringList.Add(string.Format("{0}:\t{1}", IsDevelopmentModeString, IsDevelopmentMode));
                copyStringList.Add(string.Format("{0}:\t{1}", IsFrameworkString, IsFramework));
                copyStringList.Add(string.Format("{0}:\t{1}", IsOptionalString, IsOptional));
                copyStringList.Add(string.Format("{0}:\t{1}", IsResourcePackageString, IsResourcePackage));
                copyStringList.Add(string.Format("{0}:\t{1}", IsStubString, IsStub));
                copyStringList.Add(string.Format("{0}:\t{1}", VertifyIsOKString, VertifyIsOK));
                return copyStringList;
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyStringList));
            await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
        }

        #endregion 第二部分：应用信息页面——挂载的事件
    }
}

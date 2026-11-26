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
    internal sealed partial class AppInformationPage : Page
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

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private ObservableCollection<AppDetailInfoModel> BasicInformationCollection { get; } = [];

        private ObservableCollection<AppDetailInfoModel> PublisherInformationCollection { get; } = [];

        private ObservableCollection<AppDetailInfoModel> PackageInformationCollection { get; } = [];

        private ObservableCollection<AppListEntryModel> AppListEntryCollection { get; } = [];

        private ObservableCollection<PackageModel> DependencyCollection { get; } = [];

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
            if (args.Parameter is AppListEntryModel appListEntry && !string.IsNullOrEmpty(appListEntry.PackageFamilyName))
            {
                bool isPinnedSuccessfully = await PinToDesktopAsync(appListEntry.PackageFamilyName);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Desktop, isPinnedSuccessfully));
            }
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
            List<AppDetailInfoModel> appDetailInfoList = [];
            foreach (AppDetailInfoModel basicInformationItem in BasicInformationCollection)
            {
                appDetailInfoList.Add(basicInformationItem);
            }
            foreach (AppDetailInfoModel publisherInformationItem in PublisherInformationCollection)
            {
                appDetailInfoList.Add(publisherInformationItem);
            }
            foreach (AppDetailInfoModel packageInformation in PackageInformationCollection)
            {
                appDetailInfoList.Add(packageInformation);
            }
            if (await GetAppInformationCopyStringAsync(appDetailInfoList.AsReadOnly()) is string appInformation && !string.IsNullOrEmpty(appInformation))
            {
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(appInformation);
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
            if (appInformationItem is null)
            {
                return;
            }

            BasicInformationCollection.Clear();
            if (!string.IsNullOrEmpty(appInformationItem.DisplayName))
            {
                BasicInformationCollection.Add(new() { ClassName = AppDisplayNameString, Content = appInformationItem.DisplayName });
            }
            if (!string.IsNullOrEmpty(appInformationItem.PackageFamilyName))
            {
                BasicInformationCollection.Add(new() { ClassName = PackageFamilyNameString, Content = appInformationItem.PackageFamilyName });
            }
            if (!string.IsNullOrEmpty(appInformationItem.PackageFullName))
            {
                BasicInformationCollection.Add(new() { ClassName = PackageFullNameString, Content = appInformationItem.PackageFullName });
            }
            if (!string.IsNullOrEmpty(appInformationItem.Description))
            {
                BasicInformationCollection.Add(new() { ClassName = AppDescriptionString, Content = appInformationItem.Description });
            }

            PublisherInformationCollection.Clear();
            if (!string.IsNullOrEmpty(appInformationItem.PublisherDisplayName))
            {
                PublisherInformationCollection.Add(new() { ClassName = PublisherDisplayNameString, Content = appInformationItem.PublisherDisplayName });
            }
            if (!string.IsNullOrEmpty(appInformationItem.PublisherId))
            {
                PublisherInformationCollection.Add(new() { ClassName = PublisherIdString, Content = appInformationItem.PublisherId });
            }

            PackageInformationCollection.Clear();
            if (!string.IsNullOrEmpty(appInformationItem.Version))
            {
                PackageInformationCollection.Add(new() { ClassName = VersionString, Content = appInformationItem.Version });
            }
            if (!string.IsNullOrEmpty(appInformationItem.InstallDate))
            {
                PackageInformationCollection.Add(new() { ClassName = InstalledDateString, Content = appInformationItem.InstallDate });
            }
            if (!string.IsNullOrEmpty(appInformationItem.Architecture))
            {
                PackageInformationCollection.Add(new() { ClassName = ArchitectureString, Content = appInformationItem.Architecture });
            }
            if (!string.IsNullOrEmpty(appInformationItem.SignatureKind))
            {
                PackageInformationCollection.Add(new() { ClassName = SignatureKindString, Content = appInformationItem.SignatureKind });
            }
            if (!string.IsNullOrEmpty(appInformationItem.ResourceId))
            {
                PackageInformationCollection.Add(new() { ClassName = ResourceIdString, Content = appInformationItem.ResourceId });
            }
            if (!string.IsNullOrEmpty(appInformationItem.IsBundle))
            {
                PackageInformationCollection.Add(new() { ClassName = IsBundleString, Content = appInformationItem.IsBundle });
            }
            if (!string.IsNullOrEmpty(appInformationItem.IsDevelopmentMode))
            {
                PackageInformationCollection.Add(new() { ClassName = IsDevelopmentModeString, Content = appInformationItem.IsDevelopmentMode });
            }
            if (!string.IsNullOrEmpty(appInformationItem.IsFramework))
            {
                PackageInformationCollection.Add(new() { ClassName = IsFrameworkString, Content = appInformationItem.IsFramework });
            }
            if (!string.IsNullOrEmpty(appInformationItem.IsOptional))
            {
                PackageInformationCollection.Add(new() { ClassName = IsOptionalString, Content = appInformationItem.IsOptional });
            }
            if (!string.IsNullOrEmpty(appInformationItem.IsResourcePackage))
            {
                PackageInformationCollection.Add(new() { ClassName = IsResourcePackageString, Content = appInformationItem.IsResourcePackage });
            }
            if (!string.IsNullOrEmpty(appInformationItem.IsStub))
            {
                PackageInformationCollection.Add(new() { ClassName = IsStubString, Content = appInformationItem.IsStub });
            }
            if (!string.IsNullOrEmpty(appInformationItem.VerifyIsOK))
            {
                PackageInformationCollection.Add(new() { ClassName = VerifyIsOKString, Content = appInformationItem.VerifyIsOK });
            }

            AppListEntryCollection.Clear();
            foreach (AppListEntryModel appListEntry in appInformationItem.AppListEntryList)
            {
                AppListEntryCollection.Add(appListEntry);
            }

            DependencyCollection.Clear();
            foreach (PackageModel packageItem in appInformationItem.DependenciesList)
            {
                DependencyCollection.Add(packageItem);
            }
        }

        /// <summary>
        /// 启动应用
        /// </summary>
        private void LaunchApp(AppListEntryModel appListEntry)
        {
            if (appListEntry is null)
            {
                return;
            }

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

        /// <summary>
        /// 打开安装目录
        /// </summary>
        private void OpenInstalledPath(Package package)
        {
            if (package is null)
            {
                return;
            }

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

        /// <summary>
        /// 打开商店
        /// </summary>
        private void OpenStore(Package package)
        {
            if (package is null)
            {
                return;
            }

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

        /// <summary>
        /// 固定应用到桌面
        /// </summary>
        private async Task<bool> PinToDesktopAsync(string packageFamilyName)
        {
            if (string.IsNullOrEmpty(packageFamilyName))
            {
                return default;
            }

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

        /// <summary>
        /// 固定应用到开始屏幕
        /// </summary>
        private async Task<bool> PinToStartScreenAsync(AppListEntryModel appListEntry)
        {
            if (appListEntry is null)
            {
                return default;
            }

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

        /// <summary>
        /// 固定应用到任务栏
        /// </summary>
        private void PinToTaskbar(AppListEntryModel appListEntry)
        {
            if (appListEntry is null)
            {
                return;
            }

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
        private async Task<string> GetAppInformationCopyStringAsync(ReadOnlyCollection<AppDetailInfoModel> appDetailInfoCollection)
        {
            return await Task.Run(() =>
            {
                try
                {
                    List<string> copyStringList = [];
                    foreach (AppDetailInfoModel appDetailInfoItem in appDetailInfoCollection)
                    {
                        copyStringList.Add(string.Format("{0}:\t{1}", appDetailInfoItem.ClassName, appDetailInfoItem.Content));
                    }
                    return string.Join(Environment.NewLine, copyStringList);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(AppInformationPage), nameof(GetAppInformationCopyStringAsync), 1, e);
                    return default;
                }
            });
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}

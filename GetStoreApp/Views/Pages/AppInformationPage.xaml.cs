﻿using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
                FamilyName = appInformation.PackageFamilyName;
                FullName = appInformation.PackageFullName;
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
                        LogService.WriteLog(LoggingLevel.Error, "App information copy failed", e);
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
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Open app {0} failed", appListEntry.DisplayName), e);
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
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("{0} app installed folder open failed", package.DisplayName), e);
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
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Open microsoft store {0} failed", package.DisplayName), e);
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
                        StoreConfiguration.PinToDesktop(FamilyName);
                        isPinnedSuccessfully = true;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Create desktop shortcut failed.", e);
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
                        LogService.WriteLog(LoggingLevel.Error, "Pin app to startscreen failed.", e);
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
                        LogService.WriteLog(LoggingLevel.Error, "Use TaskbarManager api to pin app to taskbar failed.", e);
                    }
                });
            }
        }

        /// <summary>
        /// 更多按钮点击时显示菜单
        /// </summary>
        private void OnShowMoreExecuteRequested(object sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is HyperlinkButton hyperlinkButton)
            {
                FlyoutBase.ShowAttachedFlyout(hyperlinkButton);
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

                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/DisplayName"), DisplayName));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/FamilyName"), FamilyName));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/FullName"), FullName));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/Description"), Description));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/PublisherDisplayName"), PublisherDisplayName));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/PublisherId"), PublisherId));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/Version"), Version));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/InstalledDate"), InstalledDate));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/Architecture"), Architecture));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/SignatureKind"), SignatureKind));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/ResourceId"), ResourceId));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsBundle"), IsBundle));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsDevelopmentMode"), IsDevelopmentMode));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsFramework"), IsFramework));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsOptional"), IsOptional));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsResourcePackage"), IsResourcePackage));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/IsStub"), IsStub));
                copyStringList.Add(string.Format("{0}:\t{1}", ResourceService.GetLocalized("AppManager/VertifyIsOK"), VertifyIsOK));
                return copyStringList;
            });

            bool copyResult = CopyPasteHelper.CopyTextToClipBoard(string.Join(Environment.NewLine, copyStringList));
            await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
        }

        #endregion 第二部分：应用信息页面——挂载的事件
    }
}

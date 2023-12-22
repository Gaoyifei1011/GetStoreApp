using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.UWPApp;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation.Diagnostics;
using Windows.System;
using Windows.UI.StartScreen;

namespace GetStoreApp.UI.Controls.UWPApp
{
    /// <summary>
    /// 应用信息控件
    /// </summary>
    public sealed partial class AppInfoControl : Grid, INotifyPropertyChanged
    {
        private string _displayName = string.Empty;

        public string DisplayName
        {
            get { return _displayName; }

            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        private string _familyName = string.Empty;

        public string FamilyName
        {
            get { return _familyName; }

            set
            {
                _familyName = value;
                OnPropertyChanged();
            }
        }

        private string _fullName = string.Empty;

        public string FullName
        {
            get { return _fullName; }

            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        private string _description = string.Empty;

        public string Description
        {
            get { return _description; }

            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _publisherName = string.Empty;

        public string PublisherName
        {
            get { return _publisherName; }

            set
            {
                _publisherName = value;
                OnPropertyChanged();
            }
        }

        private string _publisherId = string.Empty;

        public string PublisherId
        {
            get { return _publisherId; }

            set
            {
                _publisherId = value;
                OnPropertyChanged();
            }
        }

        private string _version;

        public string Version
        {
            get { return _version; }

            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        private string _installedDate;

        public string InstalledDate
        {
            get { return _installedDate; }

            set
            {
                _installedDate = value;
                OnPropertyChanged();
            }
        }

        private string _architecture;

        public string Architecture
        {
            get { return _architecture; }

            set
            {
                _architecture = value;
                OnPropertyChanged();
            }
        }

        private string _signatureKind;

        private string SignatureKind
        {
            get { return _signatureKind; }

            set
            {
                _signatureKind = value;
                OnPropertyChanged();
            }
        }

        private string _resourceId;

        public string ResourceId
        {
            get { return _resourceId; }

            set
            {
                _resourceId = value;
                OnPropertyChanged();
            }
        }

        private string _isBundle;

        public string IsBundle
        {
            get { return _isBundle; }

            set
            {
                _isBundle = value;
                OnPropertyChanged();
            }
        }

        private string _isDevelopmentMode;

        public string IsDevelopmentMode
        {
            get { return _isDevelopmentMode; }

            set
            {
                _isDevelopmentMode = value;
                OnPropertyChanged();
            }
        }

        private string _isFramework;

        public string IsFramework
        {
            get { return _isFramework; }

            set
            {
                _isFramework = value;
                OnPropertyChanged();
            }
        }

        private string _isOptional;

        public string IsOptional
        {
            get { return _isOptional; }

            set
            {
                _isOptional = value;
                OnPropertyChanged();
            }
        }

        private string _isResourcePackage;

        public string IsResourcePackage
        {
            get { return _isResourcePackage; }

            set
            {
                _isResourcePackage = value;
                OnPropertyChanged();
            }
        }

        private string _isStub;

        public string IsStub
        {
            get { return _isStub; }

            set
            {
                _isStub = value;
                OnPropertyChanged();
            }
        }

        private string _vertifyIsOK;

        public string VertifyIsOK
        {
            get { return _vertifyIsOK; }

            set
            {
                _vertifyIsOK = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AppListEntryModel> AppListEntryCollection { get; } = new ObservableCollection<AppListEntryModel>();

        private ObservableCollection<PackageModel> DependenciesCollection { get; } = new ObservableCollection<PackageModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInfoControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制应用入口的应用程序用户模型 ID
        /// </summary>
        private void OnCopyAUMIDExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string aumid = args.Parameter as string;

            if (aumid is not null)
            {
                CopyPasteHelper.CopyTextToClipBoard(aumid);
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.AppUserModelId));
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
                        StringBuilder copyBuilder = new StringBuilder();
                        copyBuilder.AppendLine(package.DisplayName);
                        copyBuilder.AppendLine(package.Id.FamilyName);
                        copyBuilder.AppendLine(package.Id.FullName);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            CopyPasteHelper.CopyTextToClipBoard(copyBuilder.ToString());
                            TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.DependencyInformation));
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
                CopyPasteHelper.CopyTextToClipBoard(displayName);
                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.DependencyName));
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
                bool IsPinnedSuccessfully = false;

                try
                {
                    if (StoreConfiguration.IsPinToDesktopSupported())
                    {
                        StoreConfiguration.PinToDesktop(FamilyName);
                        IsPinnedSuccessfully = true;
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
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Desktop, IsPinnedSuccessfully));
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
                    bool IsPinnedSuccessfully = false;

                    try
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        IsPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(appListEntryItem.AppListEntry);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pin app to startscreen failed.", e);
                    }
                    finally
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.StartScreen, IsPinnedSuccessfully));
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
            string actualTheme = ActualTheme.ToString();

            Task.Run(async () =>
            {
                if (appListEntryItem is not null)
                {
                    try
                    {
                        ResultService.SaveResult(ConfigKey.TaskbarPinInfoKey, string.Format("{0} {1}", appListEntryItem.PackageFullName, appListEntryItem.AppUserModelId));
                        await Launcher.LaunchUriAsync(new Uri("taskbarpinner:"));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pin app to taskbar failed.", e);
                    }
                }
            });
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：应用信息控件——挂载的事件

        /// <summary>
        /// 复制应用信息
        /// </summary>
        private void OnCopyClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                StringBuilder copyBuilder = new StringBuilder();
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/DisplayName"), DisplayName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/FamilyName"), FamilyName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/FullName"), FullName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/Description"), Description));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/PublisherName"), PublisherName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/PublisherId"), PublisherId));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/Version"), Version));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/InstalledDate"), InstalledDate));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/Architecture"), Architecture));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/SignatureKind"), SignatureKind));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/ResourceId"), ResourceId));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsBundle"), IsBundle));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsDevelopmentMode"), IsDevelopmentMode));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsFramework"), IsFramework));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsOptional"), IsOptional));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsResourcePackage"), IsResourcePackage));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsStub"), IsStub));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/VertifyIsOK"), VertifyIsOK));

                DispatcherQueue.TryEnqueue(() =>
                {
                    CopyPasteHelper.CopyTextToClipBoard(copyBuilder.ToString());
                    TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.PackageInformation));
                });
            });
        }

        #endregion 第二部分：应用信息控件——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        public void InitializeAppInfo(Dictionary<string, object> appInfoDict)
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
    }
}

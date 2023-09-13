using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.About;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Shell;
using Windows.UI.StartScreen;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        private readonly int MajorVersion = InfoHelper.AppVersion.Major;

        private readonly int MinorVersion = InfoHelper.AppVersion.Minor;

        private readonly int BuildVersion = InfoHelper.AppVersion.Build;

        private readonly int RevisionVersion = InfoHelper.AppVersion.Revision;

        public string AppVersion => string.Format(ResourceService.GetLocalized("About/AppVersion"), MajorVersion, MinorVersion, BuildVersion, RevisionVersion);

        public Uri ReleaseNotes = new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases");

        //项目引用信息
        public List<KeyValuePairModel> ReferenceList { get; } = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel(){ Key = "Microsoft.Windows.CsWinRT",Value = "https://github.com/microsoft/cswinrt"},
            new KeyValuePairModel(){ Key = "Microsoft.WindowsAppSDK",Value = "https://github.com/microsoft/windowsappsdk"},
            new KeyValuePairModel(){ Key = "Microsoft.WindowsPackageManager.ComInterop",Value = "https://github.com/microsoft/winget-cli"},
            new KeyValuePairModel(){ Key = "Mile.Aria2",Value = "https://github.com/ProjectMile/Mile.Aria2"},
        };

        //项目感谢者信息
        public List<KeyValuePairModel> ThanksList { get; } = new List<KeyValuePairModel>()
        {
            new KeyValuePairModel(){ Key = "AndromedaMelody",Value = "https://github.com/AndromedaMelody" },
            new KeyValuePairModel(){ Key = "cnbluefire",Value = "https://github.com/cnbluefire" },
            new KeyValuePairModel(){ Key = "飞翔",Value = "https://fionlen.azurewebsites.net" },
            new KeyValuePairModel(){ Key = "MouriNaruto",Value = "https://github.com/MouriNaruto" },
            new KeyValuePairModel(){ Key = "TaylorShi",Value = "https://github.com/TaylorShi" },
            new KeyValuePairModel(){ Key = "wherewhere",Value = "https://github.com/wherewhere" },
        };

        private AppNaviagtionArgs AboutNavigationArgs { get; set; } = AppNaviagtionArgs.None;

        public AboutPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                AboutNavigationArgs = (AppNaviagtionArgs)Enum.Parse(typeof(AppNaviagtionArgs), Convert.ToString(args.Parameter));
            }
            else
            {
                AboutNavigationArgs = AppNaviagtionArgs.None;
            }
        }

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            double CurrentScrollPosition = AboutScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            switch (AboutNavigationArgs)
            {
                case AppNaviagtionArgs.Instructions:
                    {
                        Point TargetPosition = Instructions.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                        AboutScroll.ChangeView(null, TargetPosition.Y, null);
                        break;
                    }
                case AppNaviagtionArgs.SettingsHelp:
                    {
                        Point TargetPosition = SettingsHelp.TransformToVisual(AboutScroll).TransformPoint(CurrentPoint);
                        AboutScroll.ChangeView(null, TargetPosition.Y, null);
                        break;
                    }
                default:
                    {
                        AboutScroll.ChangeView(null, 0, null);
                        break;
                    }
            }
        }

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        public void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                bool IsCreatedSuccessfully = false;

                try
                {
                    if (StoreConfiguration.IsPinToDesktopSupported())
                    {
                        StoreConfiguration.PinToDesktop(Package.Current.Id.FamilyName);
                        IsCreatedSuccessfully = true;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogLevel.ERROR, "Create desktop shortcut failed.", e);
                }
                finally
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        new QuickOperationNotification(this, QuickOperationKind.Desktop, IsCreatedSuccessfully).Show();
                    });
                }
            });
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        public void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                bool IsPinnedSuccessfully = false;

                try
                {
                    IReadOnlyList<AppListEntry> AppEntries = await Package.Current.GetAppListEntriesAsync();

                    AppListEntry DefaultEntry = AppEntries[0];

                    if (DefaultEntry is not null)
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        IsPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(DefaultEntry);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogLevel.ERROR, "Pin app to startscreen failed.", e);
                }
                finally
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        new QuickOperationNotification(this, QuickOperationKind.StartScreen, IsPinnedSuccessfully).Show();
                    });
                }
            });
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        public void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                bool IsPinnedSuccessfully = false;

                try
                {
                    string featureId = "com.microsoft.windows.taskbar.pin";
                    string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                    string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                    LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                    if (accessResult.Status is LimitedAccessFeatureStatus.Available)
                    {
                        IsPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogLevel.ERROR, "Pin app to taskbar failed.", e);
                }
                finally
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        new QuickOperationNotification(this, QuickOperationKind.Taskbar, IsPinnedSuccessfully).Show();
                    });
                }
            });
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        public async void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        public async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new LicenseDialog(), this);
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        public async void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        public async void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/issues"));
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        }

        /// <summary>
        /// 桌面程序启动参数说明
        /// </summary>
        public async void OnDesktopLaunchClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new DesktopStartupArgsDialog(), this);
        }

        /// <summary>
        /// 控制台程序启动参数说明
        /// </summary>
        public async void OnConsoleLaunchClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new ConsoleStartupArgsDialog(), this);
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        public async void OnCheckNetWorkClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        public async void OnTroubleShootClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        public void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 区分传统桌面应用
        /// </summary>
        public async void OnRecognizeClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new DesktopAppsDialog(), this);
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        public async void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        public async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new AppInformationDialog(), this);
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        public async void OnAppSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        }
    }
}

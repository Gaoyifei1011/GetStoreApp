using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
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
        private AppNaviagtionArgs AboutNavigationArgs = AppNaviagtionArgs.None;

        private string AppVersion = string.Format(ResourceService.GetLocalized("About/AppVersion"), InfoHelper.AppVersion.ToString());

        //项目引用信息
        private Hashtable ReferenceDict { get; } = new Hashtable()
        {
            { "Microsoft.Windows.CsWinRT","https://github.com/microsoft/cswinrt"},
            { "Microsoft.WindowsAppSDK","https://github.com/microsoft/windowsappsdk"},
            { "Microsoft.WindowsPackageManager.ComInterop","https://github.com/microsoft/winget-cli"},
            { "Mile.Aria2", "https://github.com/ProjectMile/Mile.Aria2"},
        };

        //项目感谢者信息
        private Hashtable ThanksDict { get; } = new Hashtable()
        {
            { "AndromedaMelody","https://github.com/AndromedaMelody" },
            { "cnbluefire","https://github.com/cnbluefire" },
            { "飞翔","https://fionlen.azurewebsites.net" },
            { "MouriNaruto","https://github.com/MouriNaruto" },
            { "TaylorShi","https://github.com/TaylorShi" },
            { "wherewhere","https://github.com/wherewhere" },
        };

        public AboutPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

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

        #endregion 第一部分：重写父类事件

        #region 第二部分：关于页面——挂载的事件

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
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
        private void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
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
                    LogService.WriteLog(LoggingLevel.Error, "Create desktop shortcut failed.", e);
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
        private void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
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
                    LogService.WriteLog(LoggingLevel.Error, "Pin app to startscreen failed.", e);
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
        private void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
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
                    LogService.WriteLog(LoggingLevel.Error, "Pin app to taskbar failed.", e);
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
        private async void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        private async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new LicenseDialog(), this);
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        private async void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        private async void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/issues"));
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        }

        /// <summary>
        /// 桌面程序启动参数说明
        /// </summary>
        private async void OnDesktopLaunchClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new DesktopStartupArgsDialog(), this);
        }

        /// <summary>
        /// 控制台程序启动参数说明
        /// </summary>
        private async void OnConsoleLaunchClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new ConsoleStartupArgsDialog(), this);
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        private async void OnCheckNetWorkClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private async void OnTroubleShootClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        private void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 区分传统桌面应用
        /// </summary>
        private async void OnRecognizeClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new DesktopAppsDialog(), this);
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        private async void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        private async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new AppInformationDialog(), this);
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        private async void OnAppSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        }

        #endregion 第二部分：关于页面——挂载的事件
    }
}

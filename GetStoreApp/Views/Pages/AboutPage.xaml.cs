using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
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
        private AppNaviagtionArgs aboutNavigationArgs = AppNaviagtionArgs.None;

        //项目引用信息
        private Hashtable ReferenceDict { get; } = new Hashtable()
        {
            { "Microsoft.Windows.CsWinRT","https://github.com/microsoft/cswinrt"},
            { "Microsoft.Windows.SDK.Contracts","https://aka.ms/WinSDKProjectURL"},
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

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                aboutNavigationArgs = (AppNaviagtionArgs)Enum.Parse(typeof(AppNaviagtionArgs), Convert.ToString(args.Parameter));
            }
            else
            {
                aboutNavigationArgs = AppNaviagtionArgs.None;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：关于页面——挂载的事件

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            double currentScrollPosition = AboutScroll.VerticalOffset;
            Point currentPoint = new Point(0, (int)currentScrollPosition);

            switch (aboutNavigationArgs)
            {
                case AppNaviagtionArgs.Instructions:
                    {
                        Point targetPosition = Instructions.TransformToVisual(AboutScroll).TransformPoint(currentPoint);
                        AboutScroll.ChangeView(null, targetPosition.Y, null);
                        break;
                    }
                case AppNaviagtionArgs.SettingsHelp:
                    {
                        Point targetPosition = SettingsHelp.TransformToVisual(AboutScroll).TransformPoint(currentPoint);
                        AboutScroll.ChangeView(null, targetPosition.Y, null);
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
                bool isCreatedSuccessfully = false;

                try
                {
                    if (StoreConfiguration.IsPinToDesktopSupported())
                    {
                        StoreConfiguration.PinToDesktop(Package.Current.Id.FamilyName);
                        isCreatedSuccessfully = true;
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
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Desktop, isCreatedSuccessfully));
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
                bool isPinnedSuccessfully = false;

                try
                {
                    IReadOnlyList<AppListEntry> appEntries = await Package.Current.GetAppListEntriesAsync();

                    AppListEntry defaultEntry = appEntries[0];

                    if (defaultEntry is not null)
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        isPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(defaultEntry);
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
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.StartScreen, isPinnedSuccessfully));
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
                bool isPinnedSuccessfully = false;

                try
                {
                    string featureId = "com.microsoft.windows.taskbar.pin";
                    string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                    string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                    LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                    if (accessResult.Status is LimitedAccessFeatureStatus.Available)
                    {
                        isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                    }
                    else
                    {
                        string tempFilePath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "GetStoreAppTemp.txt");
                        File.WriteAllText(tempFilePath, string.Format("{0}{1}{2}", Package.Current.Id.FullName, Environment.NewLine, Package.Current.GetAppListEntries()[0].AppUserModelId));

                        await Launcher.LaunchUriAsync(new Uri("taskbarpinner:"));
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
                        TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Taskbar, isPinnedSuccessfully));
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
        /// 帮助翻译应用
        /// </summary>
        private async void OnHelpTranslateClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/pulls"));
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
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
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

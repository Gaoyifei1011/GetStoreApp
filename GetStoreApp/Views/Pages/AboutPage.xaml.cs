using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using Windows.UI.Text;
using Windows.Web.Http;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public sealed partial class AboutPage : Page, INotifyPropertyChanged
    {
        private AppNaviagtionArgs aboutNavigationArgs = AppNaviagtionArgs.None;

        private bool _isChecking;

        public bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                if (!Equals(_isChecking, value))
                {
                    _isChecking = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecking)));
                }
            }
        }

        //项目引用信息
        private List<ContentLinkInfo> ReferenceList { get; } =
        [
            new ContentLinkInfo() { DisplayText = "Microsoft.UI.Xaml",Uri = new Uri("https://github.com/microsoft/microsoft-ui-xaml") },
            new ContentLinkInfo() { DisplayText = "Microsoft.Web.WebView2",Uri = new Uri("https://aka.ms/webview") },
            new ContentLinkInfo() { DisplayText = "Microsoft.Windows.CsWinRT",Uri = new Uri("https://github.com/microsoft/cswinrt") },
            new ContentLinkInfo() { DisplayText = "Microsoft.Windows.SDK.BuildTools",Uri = new Uri("https://aka.ms/WinSDKProjectURL") },
            new ContentLinkInfo() { DisplayText = "Microsoft.Windows.SDK.BuildTools.MSIX",Uri = new Uri("https://aka.ms/WinSDKProjectURL") },
            new ContentLinkInfo() { DisplayText = "Microsoft.WindowsAppSDK",Uri = new Uri("https://github.com/microsoft/windowsappsdk") },
            new ContentLinkInfo() { DisplayText = "Microsoft.WindowsPackageManager.ComInterop",Uri = new Uri("https://github.com/microsoft/winget-cli") },
        ];

        //项目感谢者信息
        private List<ContentLinkInfo> ThanksList { get; } =
        [
            new ContentLinkInfo() { DisplayText = "AndromedaMelody",Uri = new Uri("https://github.com/AndromedaMelody") },
            new ContentLinkInfo() { DisplayText = "cnbluefire",Uri = new Uri("https://github.com/cnbluefire") },
            new ContentLinkInfo() { DisplayText = "driver1998",Uri = new Uri("https://github.com/driver1998") },
            new ContentLinkInfo() { DisplayText = "GreenShadeZhang",Uri = new Uri("https://github.com/GreenShadeZhang") },
            new ContentLinkInfo() { DisplayText = "hez2010",Uri = new Uri("https://github.com/hez2010") },
            new ContentLinkInfo() { DisplayText = "飞翔",Uri = new Uri("https://fionlen.azurewebsites.net") },
            new ContentLinkInfo() { DisplayText = "MouriNaruto",Uri = new Uri("https://github.com/MouriNaruto") },
            new ContentLinkInfo() { DisplayText = "muhammadbahaa2001",Uri = new Uri("https://github.com/muhammadbahaa2001") },
            new ContentLinkInfo() { DisplayText = "TaylorShi",Uri = new Uri("https://github.com/TaylorShi") },
            new ContentLinkInfo() { DisplayText = "wherewhere",Uri = new Uri("https://github.com/wherewhere") },
            new ContentLinkInfo() { DisplayText = "Y-PLONI",Uri = new Uri("https://github.com/Y-PLONI") },
        ];

        public event PropertyChangedEventHandler PropertyChanged;

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
            aboutNavigationArgs = args.Parameter is not null && Enum.TryParse(Convert.ToString(args.Parameter), out AppNaviagtionArgs appNaviagtionArgs) ? appNaviagtionArgs : AppNaviagtionArgs.None;
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：关于页面——挂载的事件

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            double currentScrollPosition = AboutScroll.VerticalOffset;
            Point currentPoint = new(0, (int)currentScrollPosition);

            switch (aboutNavigationArgs)
            {
                case AppNaviagtionArgs.Instructions:
                    {
                        Point targetPosition = Instructions.TransformToVisual(AboutScroll).TransformPoint(currentPoint);
                        AboutScroll.ScrollTo(0, targetPosition.Y, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
                        break;
                    }
                case AppNaviagtionArgs.SettingsHelp:
                    {
                        Point targetPosition = SettingsHelp.TransformToVisual(AboutScroll).TransformPoint(currentPoint);
                        AboutScroll.ScrollTo(0, targetPosition.Y, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
                        break;
                    }
                default:
                    {
                        AboutScroll.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
                        break;
                    }
            }
        }

        /// <summary>
        /// 创建应用的桌面快捷方式
        /// </summary>
        private async void OnPinToDesktopClicked(object sender, RoutedEventArgs args)
        {
            bool isCreatedSuccessfully = false;

            await Task.Run(() =>
             {
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
             });

            await TeachingTipHelper.ShowAsync(new QuickOperationTip(QuickOperationKind.Desktop, isCreatedSuccessfully));
        }

        /// <summary>
        /// 将应用固定到“开始”屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            await Task.Run(async () =>
            {
                try
                {
                    IReadOnlyList<AppListEntry> appEntries = await Package.Current.GetAppListEntriesAsync();

                    if (appEntries[0] is AppListEntry defaultEntry)
                    {
                        StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                        isPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(defaultEntry);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Pin app to startscreen failed.", e);
                }
            });

            await TeachingTipHelper.ShowAsync(new QuickOperationTip(QuickOperationKind.StartScreen, isPinnedSuccessfully));
        }

        /// <summary>
        /// 将应用固定到任务栏
        /// </summary>
        private async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            Tuple<LimitedAccessFeatureStatus, bool> pinnedRsult = await Task.Run(async () =>
            {
                LimitedAccessFeatureStatus limitedAccessFeatureStatus = LimitedAccessFeatureStatus.Unknown;
                bool isPinnedSuccessfully = false;

                if (!RuntimeHelper.IsElevated)
                {
                    try
                    {
                        if (ApiInformation.IsTypePresent("Windows.UI.Shell.ITaskbarManagerDesktopAppSupportStatics"))
                        {
                            string featureId = "com.microsoft.windows.taskbar.pin";
                            string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                            string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                            LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);
                            limitedAccessFeatureStatus = accessResult.Status;

                            if (limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available)
                            {
                                isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Use TaskbarManager api to pin app to taskbar failed.", e);
                    }
                }

                if ((limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unavailable || limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unknown) && !isPinnedSuccessfully)
                {
                    await Launcher.LaunchUriAsync(new Uri("getstoreapppinner:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                        {
                            {"Type", nameof(TaskbarManager) },
                            { "AppUserModelId", Package.Current.GetAppListEntries()[0].AppUserModelId },
                            { "PackageFullName", Package.Current.Id.FullName },
                        });
                }

                return Tuple.Create(limitedAccessFeatureStatus, isPinnedSuccessfully);
            });

            if (pinnedRsult.Item1 is LimitedAccessFeatureStatus.Available || pinnedRsult.Item1 is LimitedAccessFeatureStatus.AvailableWithoutToken)
            {
                await TeachingTipHelper.ShowAsync(new QuickOperationTip(QuickOperationKind.Taskbar, pinnedRsult.Item2));
            }
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
            if (!IsChecking)
            {
                IsChecking = true;

                bool? isNewest = await Task.Run<bool?>(async () =>
                {
                    try
                    {
                        // 默认超时时间是 20 秒
                        HttpClient httpClient = new();
                        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36");
                        HttpRequestResult httpRequestResult = await httpClient.TryGetAsync(new Uri("https://api.github.com/repos/Gaoyifei1011/GetStoreApp/releases/latest"));
                        httpClient.Dispose();

                        // 请求成功
                        if (httpRequestResult.Succeeded && httpRequestResult.ResponseMessage.IsSuccessStatusCode)
                        {
                            string responseString = await httpRequestResult.ResponseMessage.Content.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(responseString))
                            {
                                if (JsonObject.TryParse(responseString, out JsonObject responseStringObject) && new Version(responseStringObject.GetNamedString("tag_name")[1..]) is Version tagVersion)
                                {
                                    return InfoHelper.AppVersion >= tagVersion;
                                }
                            }
                        }
                        // 请求失败
                        else
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Check update request failed", httpRequestResult.ExtendedError);
                        }

                        httpRequestResult.Dispose();
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Check update request unknown exception", e);
                    }

                    return null;
                });

                IsChecking = false;
                if (isNewest.HasValue)
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.CheckUpdate, isNewest.Value));
                }
            }
        }

        /// <summary>
        /// 桌面程序启动参数说明
        /// </summary>
        private async void OnDesktopLaunchClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new DesktopStartupArgsDialog(), this);
        }

        /// <summary>
        /// 控制台程序启动参数说明
        /// </summary>
        private async void OnConsoleLaunchClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new ConsoleStartupArgsDialog(), this);
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        private async void OnCheckNetWorkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        private async void OnTroubleShootClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        private void OnDownloadSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        private async void OnSystemInformationClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        private async void OnAppInformationClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new AppInformationDialog(), this);
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        private async void OnAppSettingsClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        }

        #endregion 第二部分：关于页面——挂载的事件
    }
}

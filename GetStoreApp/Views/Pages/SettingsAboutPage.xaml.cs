using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation.Diagnostics;
using Windows.Networking.Connectivity;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Text;
using Windows.Web.Http;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置关于页面
    /// </summary>
    public sealed partial class SettingsAboutPage : Page, INotifyPropertyChanged
    {
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
            new ContentLinkInfo() { DisplayText = "Microsoft.Web.WebView2",Uri = new Uri("https://aka.ms/webview") },
            new ContentLinkInfo() { DisplayText = "Microsoft.Windows.CsWinRT",Uri = new Uri("https://github.com/microsoft/cswinrt") },
            new ContentLinkInfo() { DisplayText = "Microsoft.Windows.SDK.BuildTools",Uri = new Uri("https://aka.ms/WinSDKProjectURL") },
            new ContentLinkInfo() { DisplayText = "Microsoft.Windows.SDK.BuildTools.MSIX",Uri = new Uri("https://aka.ms/WinSDKProjectURL") },
            new ContentLinkInfo() { DisplayText = "Microsoft.WindowsAppSDK",Uri = new Uri("https://github.com/microsoft/windowsappsdk") },
            new ContentLinkInfo() { DisplayText = "Microsoft.WindowsPackageManager.ComInterop",Uri = new Uri("https://github.com/microsoft/winget-cli") },
            new ContentLinkInfo() { DisplayText = "Microsoft.WindowsPackageManager.InProcCom",Uri = new Uri("https://github.com/microsoft/winget-cli") },
            new ContentLinkInfo() { DisplayText = "Mile.Aria2",Uri = new Uri("https://github.com/ProjectMile/Mile.Aria2") },
        ];

        //项目感谢者信息
        private List<ContentLinkInfo> ThanksList { get; } =
        [
            new ContentLinkInfo() { DisplayText = "AndromedaMelody",Uri = new Uri("https://github.com/AndromedaMelody") },
            new ContentLinkInfo() { DisplayText = "cnbluefire",Uri = new Uri("https://github.com/cnbluefire") },
            new ContentLinkInfo() { DisplayText = "driver1998",Uri = new Uri("https://github.com/driver1998") },
            new ContentLinkInfo() { DisplayText = "Goo-aw233",Uri = new Uri("https://github.com/Goo-aw233") },
            new ContentLinkInfo() { DisplayText = "GreenShadeZhang",Uri = new Uri("https://github.com/GreenShadeZhang") },
            new ContentLinkInfo() { DisplayText = "hez2010",Uri = new Uri("https://github.com/hez2010") },
            new ContentLinkInfo() { DisplayText = "飞翔",Uri = new Uri("https://fionlen.azurewebsites.net") },
            new ContentLinkInfo() { DisplayText = "Mahantor",Uri = new Uri("https://github.com/Mahantor") },
            new ContentLinkInfo() { DisplayText = "MouriNaruto",Uri = new Uri("https://github.com/MouriNaruto") },
            new ContentLinkInfo() { DisplayText = "muhammadbahaa2001",Uri = new Uri("https://github.com/muhammadbahaa2001") },
            new ContentLinkInfo() { DisplayText = "TaylorShi",Uri = new Uri("https://github.com/TaylorShi") },
            new ContentLinkInfo() { DisplayText = "wherewhere",Uri = new Uri("https://github.com/wherewhere") },
            new ContentLinkInfo() { DisplayText = "Y-PLONI",Uri = new Uri("https://github.com/Y-PLONI") },
        ];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsAboutPage()
        {
            InitializeComponent();
        }

        #region 第二部分：设置关于页面——挂载的事件

        /// <summary>
        /// 查看更新日志
        /// </summary>
        private void OnShowReleaseNotesClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        private async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new AppInformationDialog());
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        private void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 查看许可证
        /// </summary>
        private async void OnShowLicenseClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new LicenseDialog());
        }

        /// <summary>
        /// 帮助翻译应用
        /// </summary>
        private void OnHelpTranslateClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/pulls"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 项目主页
        /// </summary>
        private void OnProjectDescriptionClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        private void OnSendFeedbackClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/issues"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            if (!IsChecking)
            {
                IsChecking = true;
                if (NetWorkHelper.IsNetWorkConnected())
                {
                    if (RuntimeHelper.IsStoreVersion)
                    {
                        bool isNewest = false;

                        try
                        {
                            IsChecking = true;
                            StoreContext storeContext = StoreContext.GetDefault();
                            IReadOnlyList<StorePackageUpdate> packageUpdateList = await storeContext.GetAppAndOptionalStorePackageUpdatesAsync();
                            isNewest = packageUpdateList.Count is 0;
                            IsChecking = false;
                            DispatcherQueue.TryEnqueue(async () =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, Convert.ToInt32(isNewest)));
                            });
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAboutPage), nameof(OnCheckUpdateClicked), 1, e);
                            IsChecking = false;
                            await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                        }

                        if (!isNewest)
                        {
                            await MainWindow.Current.ShowDialogAsync(new UpdateAppDialog());
                        }
                    }
                    else
                    {
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
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAboutPage), nameof(OnCheckUpdateClicked), 1, httpRequestResult.ExtendedError);
                                }

                                httpRequestResult.Dispose();
                            }
                            // 其他异常
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAboutPage), nameof(OnCheckUpdateClicked), 2, e);
                            }

                            return null;
                        });

                        IsChecking = false;
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, isNewest.HasValue ? Convert.ToInt32(isNewest.Value) : 2));
                    }
                }
                else
                {
                    IsChecking = false;
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                }
            }
        }

        #endregion 第二部分：设置关于页面——挂载的事件
    }
}

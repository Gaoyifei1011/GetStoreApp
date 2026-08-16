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
        private static readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/149.0.0.0 Safari/537.36 Edg/149.0.0.0";

        private bool _isChecking;

        public bool IsChecking
        {
            get { return _isChecking; }

            set
            {
                if (!Equals(_isChecking, value))
                {
                    _isChecking = value;
                    PropertyChanged.Invoke(this, new(nameof(IsChecking)));
                }
            }
        }

        //项目引用信息
        private List<ContentLinkInfo> ReferenceList { get; } =
        [
            new() { DisplayText = "Microsoft.Web.WebView2",Uri = new("https://aka.ms/webview") },
            new() { DisplayText = "Microsoft.Windows.CsWinRT",Uri = new("https://github.com/microsoft/cswinrt") },
            new() { DisplayText = "Microsoft.Windows.SDK.BuildTools",Uri = new("https://aka.ms/WinSDKProjectURL") },
            new() { DisplayText = "Microsoft.Windows.SDK.BuildTools.MSIX",Uri = new("https://aka.ms/WinSDKProjectURL") },
            new() { DisplayText = "Microsoft.WindowsAppSDK",Uri = new("https://github.com/microsoft/windowsappsdk") },
            new() { DisplayText = "Microsoft.WindowsPackageManager.ComInterop",Uri = new("https://github.com/microsoft/winget-cli") },
            new() { DisplayText = "Microsoft.WindowsPackageManager.InProcCom",Uri = new("https://github.com/microsoft/winget-cli") },
            new() { DisplayText = "Mile.Aria2",Uri = new("https://github.com/ProjectMile/Mile.Aria2") },
        ];

        //项目感谢者信息
        private List<ContentLinkInfo> ThanksList { get; } =
        [
            new() { DisplayText = "AndromedaMelody",Uri = new("https://github.com/AndromedaMelody") },
            new() { DisplayText = "cnbluefire",Uri = new("https://github.com/cnbluefire") },
            new() { DisplayText = "driver1998",Uri = new("https://github.com/driver1998") },
            new() { DisplayText = "Goo-aw233",Uri = new("https://github.com/Goo-aw233") },
            new() { DisplayText = "GreenShadeZhang",Uri = new("https://github.com/GreenShadeZhang") },
            new() { DisplayText = "hez2010",Uri = new("https://github.com/hez2010") },
            new() { DisplayText = "飞翔",Uri = new("https://fionlen.azurewebsites.net") },
            new() { DisplayText = "Mahantor",Uri = new("https://github.com/Mahantor") },
            new() { DisplayText = "MouriNaruto",Uri = new("https://github.com/MouriNaruto") },
            new() { DisplayText = "muhammadbahaa2001",Uri = new("https://github.com/muhammadbahaa2001") },
            new() { DisplayText = "TaylorShi",Uri = new("https://github.com/TaylorShi") },
            new() { DisplayText = "wherewhere",Uri = new("https://github.com/wherewhere") },
            new() { DisplayText = "Y-PLONI",Uri = new("https://github.com/Y-PLONI") },
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
                    await Launcher.LaunchUriAsync(new("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
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
                    await Launcher.LaunchUriAsync(new("ms-settings:about"));
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
                    await Launcher.LaunchUriAsync(new("https://github.com/Gaoyifei1011/GetStoreApp/pulls"));
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
                    await Launcher.LaunchUriAsync(new("https://github.com/Gaoyifei1011/GetStoreApp"));
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
                    await Launcher.LaunchUriAsync(new("https://github.com/Gaoyifei1011/GetStoreApp/issues"));
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
                            DispatcherQueue.TryEnqueue(async () =>
                            {
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                            });
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
                                Uri checkUpdateLinkUri = new("https://api.github.com/repos/Gaoyifei1011/GetStoreApp/releases/latest");

                                // 默认超时时间是 20 秒
                                HttpClient httpClient = new();
                                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
                                httpClient.DefaultRequestHeaders.Referer = checkUpdateLinkUri;
                                httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", checkUpdateLinkUri.AbsolutePath);

                                HttpRequestResult httpRequestResult = await httpClient.TryGetAsync(checkUpdateLinkUri);
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

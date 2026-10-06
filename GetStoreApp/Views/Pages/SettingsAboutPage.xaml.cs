using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    internal sealed partial class SettingsAboutPage : Page
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/149.0.0.0 Safari/537.36 Edg/149.0.0.0";
        private readonly string AppInformationString = ResourceService.GetLocalized("SettingsAbout/AppInformation");
        private readonly string HelpTranslateString = ResourceService.GetLocalized("SettingsAbout/HelpTranslate");
        private readonly string ProjectHomePageString = ResourceService.GetLocalized("SettingsAbout/ProjectHomePage");
        private readonly string SendFeedbackString = ResourceService.GetLocalized("SettingsAbout/SendFeedback");
        private readonly string ShowLicenseString = ResourceService.GetLocalized("SettingsAbout/ShowLicense");
        private readonly string ShowReleaseNotesString = ResourceService.GetLocalized("SettingsAbout/ShowReleaseNotes");
        private readonly string SystemInformationString = ResourceService.GetLocalized("SettingsAbout/SystemInformation");

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private List<OperationBarItemModel> OperationBarItemList { get; } = [];

        //项目引用信息
        private ReadOnlyCollection<ContentLinkInfo> ReferenceCollection { get; } =
        [
            new() { DisplayText = "Microsoft.Web.WebView2", Uri = new("https://aka.ms/webview") },
            new() { DisplayText = "Microsoft.Windows.CsWinRT", Uri = new("https://github.com/microsoft/cswinrt") },
            new() { DisplayText = "Microsoft.Windows.SDK.BuildTools", Uri = new("https://aka.ms/WinSDKProjectURL") },
            new() { DisplayText = "Microsoft.Windows.SDK.BuildTools.MSIX",Uri = new("https://aka.ms/WinSDKProjectURL") },
            new() { DisplayText = "Microsoft.WindowsAppSDK", Uri = new("https://github.com/microsoft/windowsappsdk") },
            new() { DisplayText = "Microsoft.WindowsPackageManager.ComInterop", Uri = new("https://github.com/microsoft/winget-cli") },
            new() { DisplayText = "Microsoft.WindowsPackageManager.InProcCom", Uri = new("https://github.com/microsoft/winget-cli") },
            new() { DisplayText = "Mile.Aria2", Uri = new("https://github.com/ProjectMile/Mile.Aria2") },
        ];

        //项目感谢者信息
        private ReadOnlyCollection<ContentLinkInfo> ThanksCollection { get; } =
        [
            new() { DisplayText = "AndromedaMelody", Uri = new("https://github.com/AndromedaMelody") },
            new() { DisplayText = "cnbluefire", Uri = new("https://github.com/cnbluefire") },
            new() { DisplayText = "driver1998", Uri = new("https://github.com/driver1998") },
            new() { DisplayText = "ghost1372", Uri = new("https://github.com/ghost1372") },
            new() { DisplayText = "Goo-aw233", Uri = new("https://github.com/Goo-aw233") },
            new() { DisplayText = "GreenShadeZhang", Uri = new("https://github.com/GreenShadeZhang") },
            new() { DisplayText = "hez2010", Uri = new("https://github.com/hez2010") },
            new() { DisplayText = "飞翔", Uri = new("https://fionlen.azurewebsites.net") },
            new() { DisplayText = "Mahantor", Uri = new("https://github.com/Mahantor") },
            new() { DisplayText = "MouriNaruto", Uri = new("https://github.com/MouriNaruto") },
            new() { DisplayText = "muhammadbahaa2001", Uri = new("https://github.com/muhammadbahaa2001") },
            new() { DisplayText = "TaylorShi", Uri = new("https://github.com/TaylorShi") },
            new() { DisplayText = "tinodin", Uri = new("https://github.com/tinodin") },
            new() { DisplayText = "wherewhere", Uri = new("https://github.com/wherewhere") },
            new() { DisplayText = "Y-PLONI", Uri = new("https://github.com/Y-PLONI") },
        ];

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal SettingsAboutPage()
        {
            InitializeComponent();
            InitializeData();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：挂载事件处理

        /// <summary>
        /// 点击操作栏项目时触发的事件
        /// </summary>
        private async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is OperationBarItemModel operationBarItem)
            {
                switch (operationBarItem.Tag)
                {
                    case "ProjectHomePage":
                        {
                            OpenProjectHomePage();
                            break;
                        }
                    case "SendFeedback":
                        {
                            SendFeedback();
                            break;
                        }
                    case "CheckUpdate":
                        {
                            if (!operationBarItem.IsCheckingUpdate)
                            {
                                operationBarItem.IsCheckingUpdate = true;
                                if (NetWorkHelper.IsNetWorkConnected())
                                {
                                    if (RuntimeHelper.IsStoreVersion)
                                    {
                                        bool isNewest = false;

                                        try
                                        {
                                            operationBarItem.IsCheckingUpdate = true;
                                            bool? checkResult = await CheckCurrentAppIsNewestVersionAsync(RuntimeHelper.IsStoreVersion);
                                            isNewest = checkResult is not null && checkResult.HasValue && checkResult.Value;
                                            operationBarItem.IsCheckingUpdate = false;
                                            DispatcherQueue.TryEnqueue(async () =>
                                            {
                                                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, Convert.ToInt32(isNewest)));
                                            });
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAboutPage), nameof(OnItemClicked), 1, e);
                                            operationBarItem.IsCheckingUpdate = false;
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
                                        bool? isNewest = await CheckCurrentAppIsNewestVersionAsync(RuntimeHelper.IsStoreVersion);
                                        operationBarItem.IsCheckingUpdate = false;
                                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, isNewest is not null && isNewest.HasValue ? Convert.ToInt32(isNewest.Value) : 2));
                                    }
                                }
                                else
                                {
                                    operationBarItem.IsCheckingUpdate = false;
                                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.CheckUpdate, 2));
                                }
                            }
                            break;
                        }
                    case "HelpTranslate":
                        {
                            HelpTranslate();
                            break;
                        }
                    case "ShowLicense":
                        {
                            await MainWindow.Current.ShowDialogAsync(new LicenseDialog());
                            break;
                        }
                    case "ShowReleaseNotes":
                        {
                            ShowReleaseNotes();
                            break;
                        }
                    case "AppInformation":
                        {
                            await MainWindow.Current.ShowDialogAsync(new AppInformationDialog());
                            break;
                        }
                    case "SystemInformation":
                        {
                            ShowSystemInformation();
                            break;
                        }
                }
            }
        }

        #endregion 第四部分：挂载事件处理

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitializeData()
        {
            OperationBarItemList.Add(new()
            {
                Title = ProjectHomePageString,
                Tag = "ProjectHomePage",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE80F"
            });
            OperationBarItemList.Add(new()
            {
                Title = SendFeedbackString,
                Tag = "SendFeedback",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uED15"
            });
            OperationBarItemList.Add(new()
            {
                Tag = "CheckUpdate",
                OperationBarKind = OperationBarKind.CheckUpdate,
                IconGlyph = "\uE895"
            });
            OperationBarItemList.Add(new()
            {
                Title = HelpTranslateString,
                Tag = "HelpTranslate",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uF2B7"
            });
            OperationBarItemList.Add(new()
            {
                Title = ShowLicenseString,
                Tag = "ShowLicense",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE779"
            });
            OperationBarItemList.Add(new()
            {
                Title = ShowReleaseNotesString,
                Tag = "ShowReleaseNotes",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE70B"
            });
            OperationBarItemList.Add(new()
            {
                Title = AppInformationString,
                Tag = "AppInformation",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uECAA"
            });
            OperationBarItemList.Add(new()
            {
                Title = SystemInformationString,
                Tag = "SystemInformation",
                OperationBarKind = OperationBarKind.Ordinary,
                IconGlyph = "\uE770"
            });
        }

        /// <summary>
        /// 查看更新日志
        /// </summary>
        private void ShowReleaseNotes()
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
        /// 查看系统信息
        /// </summary>
        private void ShowSystemInformation()
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
        /// 帮助翻译
        /// </summary>
        private void HelpTranslate()
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
        /// 打开项目主页
        /// </summary>
        private void OpenProjectHomePage()
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
        private void SendFeedback()
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
        /// 检查当前应用是否为最新版本
        /// </summary>
        private async Task<bool?> CheckCurrentAppIsNewestVersionAsync(bool isStoreVersion)
        {
            if (isStoreVersion)
            {
                StoreContext storeContext = StoreContext.GetDefault();
                IReadOnlyList<StorePackageUpdate> packageUpdateList = await storeContext.GetAppAndOptionalStorePackageUpdatesAsync();
                return packageUpdateList.Count is 0;
            }
            else
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
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAboutPage), nameof(CheckCurrentAppIsNewestVersionAsync), 1, httpRequestResult.ExtendedError);
                    }

                    httpRequestResult.Dispose();
                }
                // 其他异常
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsAboutPage), nameof(CheckCurrentAppIsNewestVersionAsync), 2, e);
                }

                return default;
            }
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}

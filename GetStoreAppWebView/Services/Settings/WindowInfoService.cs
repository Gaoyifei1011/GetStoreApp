using GetStoreAppWebView.Views.Windows;
using Microsoft.UI.Windowing;
using Microsoft.Windows.Storage;
using System;
using Windows.Graphics;

namespace GetStoreAppWebView.Services.Settings
{
    /// <summary>
    /// 窗口信息服务
    /// </summary>
    internal static class WindowInfoService
    {
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer windowInfoServiceContainer;
        private static ApplicationDataContainer webViewWindowContainer;

        /// <summary>
        /// 初始化窗口信息服务
        /// </summary>
        internal static void Initialize()
        {
            windowInfoServiceContainer = localSettingsContainer.CreateContainer("WindowInfoServiceContainer", ApplicationDataCreateDisposition.Always);
            webViewWindowContainer = windowInfoServiceContainer.CreateContainer(nameof(WebViewWindow), ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 检查窗口信息是否存在
        /// </summary>
        internal static bool IsWindowInformationExisted()
        {
            return webViewWindowContainer is not null && webViewWindowContainer.Values.ContainsKey("WindowInformationExisted");
        }

        /// <summary>
        /// 获取窗口信息
        /// </summary>
        internal static PointInt32 GetWindowInfo()
        {
            return webViewWindowContainer is not null && IsWindowInformationExisted() ? new PointInt32(Convert.ToInt32(webViewWindowContainer.Values["X"]), Convert.ToInt32(webViewWindowContainer.Values["Y"])) : default;
        }

        /// <summary>
        /// 保存窗口信息
        /// </summary>
        internal static void SaveWindowInfo(AppWindow appWindow)
        {
            if (appWindow is not null && webViewWindowContainer is not null)
            {
                webViewWindowContainer.Values["WindowInformationExisted"] = true;
                webViewWindowContainer.Values["X"] = appWindow.Position.X;
                webViewWindowContainer.Values["Y"] = appWindow.Position.Y;
            }
        }
    }
}

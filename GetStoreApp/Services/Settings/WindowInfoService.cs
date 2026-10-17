using GetStoreApp.Views.Windows;
using Microsoft.UI.Windowing;
using Microsoft.Windows.Storage;
using System;
using Windows.Graphics;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 窗口信息服务
    /// </summary>
    internal static class WindowInfoService
    {
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer windowInfoServiceContainer;
        private static ApplicationDataContainer mainWindowContainer;

        /// <summary>
        /// 初始化窗口信息服务
        /// </summary>
        internal static void Initialize()
        {
            windowInfoServiceContainer = localSettingsContainer.CreateContainer("WindowInfoServiceContainer", ApplicationDataCreateDisposition.Always);
            mainWindowContainer = windowInfoServiceContainer.CreateContainer(nameof(MainWindow), ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 检查窗口信息是否存在
        /// </summary>
        internal static bool IsWindowInformationExisted()
        {
            return mainWindowContainer is not null && mainWindowContainer.Values.ContainsKey("WindowInformationExisted");
        }

        /// <summary>
        /// 获取窗口信息
        /// </summary>
        internal static (bool, RectInt32) GetWindowInfo()
        {
            return mainWindowContainer is not null && IsWindowInformationExisted() ? (Convert.ToBoolean(mainWindowContainer.Values["IsWindowMaximized"]), new(Convert.ToInt32(mainWindowContainer.Values["X"]), Convert.ToInt32(mainWindowContainer.Values["Y"]), Convert.ToInt32(mainWindowContainer.Values["Width"]), Convert.ToInt32(mainWindowContainer.Values["Height"]))) : default;
        }

        /// <summary>
        /// 保存窗口信息
        /// </summary>
        internal static void SaveWindowInfo(AppWindow appWindow, bool isWindowMaximized)
        {
            if (appWindow is not null && mainWindowContainer is not null)
            {
                mainWindowContainer.Values["WindowInformationExisted"] = true;
                mainWindowContainer.Values["X"] = appWindow.Position.X;
                mainWindowContainer.Values["Y"] = appWindow.Position.Y;
                mainWindowContainer.Values["Width"] = appWindow.Size.Width;
                mainWindowContainer.Values["Height"] = appWindow.Size.Height;
            }
        }
    }
}

using Microsoft.UI.Windowing;
using Microsoft.Windows.Storage;
using System;
using Windows.Graphics;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 窗口信息服务
    /// </summary>
    internal static class WindowInfoService
    {
        private static bool isInitialized;
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer windowInfoServiceContainer;
        private static ApplicationDataContainer windowContainer;

        /// <summary>
        /// 初始化窗口信息服务
        /// </summary>
        internal static void Initialize(string windowName)
        {
            if (!isInitialized)
            {
                isInitialized = true;
                windowInfoServiceContainer = localSettingsContainer.CreateContainer("WindowInfoServiceContainer", ApplicationDataCreateDisposition.Always);
                windowContainer = windowInfoServiceContainer.CreateContainer(windowName, ApplicationDataCreateDisposition.Always);
            }
        }

        /// <summary>
        /// 检查窗口信息是否存在
        /// </summary>
        internal static bool IsWindowInformationExisted()
        {
            return windowContainer is not null && windowContainer.Values.ContainsKey("WindowInformationExisted");
        }

        /// <summary>
        /// 获取窗口信息
        /// </summary>
        internal static (bool, RectInt32) GetWindowInfo()
        {
            return windowContainer is not null && IsWindowInformationExisted() ? (Convert.ToBoolean(windowContainer.Values["IsWindowMaximized"]), new(Convert.ToInt32(windowContainer.Values["X"]), Convert.ToInt32(windowContainer.Values["Y"]), Convert.ToInt32(windowContainer.Values["Width"]), Convert.ToInt32(windowContainer.Values["Height"]))) : default;
        }

        /// <summary>
        /// 保存窗口信息
        /// </summary>
        internal static void SaveWindowInfo(AppWindow appWindow, bool isWindowMaximized)
        {
            if (appWindow is not null && windowContainer is not null)
            {
                windowContainer.Values["WindowInformationExisted"] = true;
                windowContainer.Values["X"] = appWindow.Position.X;
                windowContainer.Values["Y"] = appWindow.Position.Y;
                windowContainer.Values["Width"] = appWindow.Size.Width;
                windowContainer.Values["Height"] = appWindow.Size.Height;
                windowContainer.Values["IsWindowMaximized"] = isWindowMaximized;
            }
        }
    }
}

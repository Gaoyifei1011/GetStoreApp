using GetStoreAppInstaller.Views.Windows;
using Microsoft.UI.Windowing;
using Microsoft.Windows.Storage;
using System;
using Windows.Graphics;

namespace GetStoreAppInstaller.Services.Settings
{
    /// <summary>
    /// 窗口信息服务
    /// </summary>
    internal static class WindowInfoService
    {
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer windowInfoServiceContainer;
        private static ApplicationDataContainer installerWindowContainer;

        /// <summary>
        /// 初始化窗口信息服务
        /// </summary>
        internal static void Initialize()
        {
            windowInfoServiceContainer = localSettingsContainer.CreateContainer("WindowInfoServiceContainer", ApplicationDataCreateDisposition.Always);
            installerWindowContainer = windowInfoServiceContainer.CreateContainer(nameof(InstallerWindow), ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 检查窗口信息是否存在
        /// </summary>
        internal static bool IsWindowInformationExisted()
        {
            return installerWindowContainer is not null && installerWindowContainer.Values.ContainsKey("WindowInformationExisted");
        }

        /// <summary>
        /// 获取窗口信息
        /// </summary>
        internal static PointInt32 GetWindowInfo()
        {
            return installerWindowContainer is not null && IsWindowInformationExisted() ? new PointInt32(Convert.ToInt32(installerWindowContainer.Values["X"]), Convert.ToInt32(installerWindowContainer.Values["Y"])) : default;
        }

        /// <summary>
        /// 保存窗口信息
        /// </summary>
        internal static void SaveWindowInfo(AppWindow appWindow)
        {
            if (appWindow is not null && installerWindowContainer is not null)
            {
                installerWindowContainer.Values["WindowInformationExisted"] = true;
                installerWindowContainer.Values["X"] = appWindow.Position.X;
                installerWindowContainer.Values["Y"] = appWindow.Position.Y;
            }
        }
    }
}

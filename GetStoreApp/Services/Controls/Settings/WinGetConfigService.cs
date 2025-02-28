using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// WinGet 程序包配置服务
    /// </summary>
    public static class WinGetConfigService
    {
        private static readonly string winGetInstallModeSettingsKey = ConfigKey.WinGetInstallModeKey;

        public static bool IsWinGetInstalled { get; private set; }

        public static KeyValuePair<string, string> DefaultWinGetInstallMode { get; set; }

        public static KeyValuePair<string, string> WinGetInstallMode { get; set; }

        public static List<KeyValuePair<string, string>> WinGetInstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static void InitializeWinGetConfig()
        {
            WinGetInstallModeList = ResourceService.WinGetInstallModeList;

            DefaultWinGetInstallMode = WinGetInstallModeList.Find(item => item.Key.Equals(PackageInstallMode.Interactive.ToString(), StringComparison.OrdinalIgnoreCase));

            WinGetInstallMode = GetWinGetInstallMode();
            IsWinGetInstalled = GetWinGetInstalledState();
        }

        /// <summary>
        /// 获取设置存储的 WinGet 程序包安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetWinGetInstallMode()
        {
            string winGetInstallMode = LocalSettingsService.ReadSetting<string>(winGetInstallModeSettingsKey);

            if (string.IsNullOrEmpty(winGetInstallMode))
            {
                SetWinGetInstallMode(DefaultWinGetInstallMode);
                return WinGetInstallModeList.Find(item => item.Key.Equals(DefaultWinGetInstallMode.Key));
            }

            KeyValuePair<string, string> selectedWinGetInstallMode = WinGetInstallModeList.Find(item => item.Key.Equals(winGetInstallMode, StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrEmpty(selectedWinGetInstallMode.Key) ? DefaultWinGetInstallMode : selectedWinGetInstallMode;
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的 WinGet 程序包安装方式
        /// </summary>
        public static void SetWinGetInstallMode(KeyValuePair<string, string> winGetInstallMode)
        {
            WinGetInstallMode = winGetInstallMode;

            LocalSettingsService.SaveSetting(winGetInstallModeSettingsKey, winGetInstallMode.Key);
        }

        /// <summary>
        /// 获取 WinGet 的安装状态
        /// </summary>
        private static bool GetWinGetInstalledState()
        {
            Windows.Management.Deployment.PackageManager packageManager = new();
            foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
            {
                if (package.Id.FullName.Contains("Microsoft.DesktopAppInstaller") && File.Exists(Path.Combine(package.InstalledPath, "WinGet.exe")))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

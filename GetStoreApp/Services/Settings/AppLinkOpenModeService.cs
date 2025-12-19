using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用链接打开方式设置服务
    /// </summary>
    public static class AppLinkOpenModeService
    {
        private static readonly string appLinkOpenModeSettingsKey = ConfigKey.AppLinkOpenModeKey;

        private static string defaultAppLinkOpenMode;

        private static string _appLinkOpenMode;

        public static string AppLinkOpenMode
        {
            get { return _appLinkOpenMode; }

            private set
            {
                if (!string.Equals(_appLinkOpenMode, value))
                {
                    _appLinkOpenMode = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AppLinkOpenMode)));
                }
            }
        }

        public static List<string> AppLinkOpenModeList { get; } = ["BuiltInApp", "SystemBrowser"];

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的应用链接打开方式选择值
        /// </summary>
        public static void InitializeAppLinkOpenMode()
        {
            defaultAppLinkOpenMode = AppLinkOpenModeList.Find(item => item is "BuiltInApp");
            AppLinkOpenMode = GetAppLinkOpenMode();
        }

        /// <summary>
        /// 获取设置存储的应用链接打开方式选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetAppLinkOpenMode()
        {
            string appLinkOpenModeValue = LocalSettingsService.ReadSetting<string>(appLinkOpenModeSettingsKey);

            if (string.IsNullOrEmpty(appLinkOpenModeValue))
            {
                SetAppLinkOpenMode(defaultAppLinkOpenMode);
                return defaultAppLinkOpenMode;
            }

            string selectedAppLinkOpenMode = AppLinkOpenModeList.Find(item => string.Equals(item, appLinkOpenModeValue, StringComparison.OrdinalIgnoreCase));
            return selectedAppLinkOpenMode is null ? defaultAppLinkOpenMode : selectedAppLinkOpenMode;
        }

        /// <summary>
        /// 应用链接打开方式发生修改时修改设置存储的应用链接打开方式值
        /// </summary>
        public static void SetAppLinkOpenMode(string appLinkOpenMode)
        {
            AppLinkOpenMode = appLinkOpenMode;
            LocalSettingsService.SaveSetting(appLinkOpenModeSettingsKey, appLinkOpenMode);
        }
    }
}

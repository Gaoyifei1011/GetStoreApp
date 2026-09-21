using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用链接打开方式设置服务
    /// </summary>
    internal static class AppLinkOpenModeService
    {
        private static readonly string appLinkOpenModeSettingsKey = ConfigKey.AppLinkOpenModeKey;
        private static string defaultAppLinkOpenMode;

        private static string _appLinkOpenMode;

        internal static string AppLinkOpenMode
        {
            get { return _appLinkOpenMode; }

            private set
            {
                if (!string.Equals(_appLinkOpenMode, value))
                {
                    _appLinkOpenMode = value;
                    PropertyChanged?.Invoke(null, new(nameof(AppLinkOpenMode)));
                }
            }
        }

        internal static ReadOnlyCollection<string> AppLinkOpenModeCollection { get; } = ["BuiltInApp", "SystemBrowser"];

        internal static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的应用链接打开方式选择值
        /// </summary>
        internal static void InitializeAppLinkOpenMode()
        {
            foreach (string appLinkOpenMode in AppLinkOpenModeCollection)
            {
                if (appLinkOpenMode is "BuiltInApp")
                {
                    defaultAppLinkOpenMode = appLinkOpenMode;
                    break;
                }
            }
            AppLinkOpenMode = GetAppLinkOpenMode();
        }

        /// <summary>
        /// 获取设置存储的应用链接打开方式选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetAppLinkOpenMode()
        {
            string appLinkOpenMode = LocalSettingsService.ReadSetting<string>(appLinkOpenModeSettingsKey);

            if (string.IsNullOrEmpty(appLinkOpenMode))
            {
                SetAppLinkOpenMode(defaultAppLinkOpenMode);
                return defaultAppLinkOpenMode;
            }

            string selectedAppLinkOpenMode = null;
            foreach (string appLinkOpenModeItem in AppLinkOpenModeCollection)
            {
                if (string.Equals(appLinkOpenModeItem, appLinkOpenMode, StringComparison.OrdinalIgnoreCase))
                {
                    selectedAppLinkOpenMode = appLinkOpenModeItem;
                    break;
                }
            }

            return selectedAppLinkOpenMode is null ? defaultAppLinkOpenMode : selectedAppLinkOpenMode;
        }

        /// <summary>
        /// 应用链接打开方式发生修改时修改设置存储的应用链接打开方式值
        /// </summary>
        internal static void SetAppLinkOpenMode(string appLinkOpenMode)
        {
            AppLinkOpenMode = appLinkOpenMode;
            LocalSettingsService.SaveSetting(appLinkOpenModeSettingsKey, appLinkOpenMode);
        }
    }
}

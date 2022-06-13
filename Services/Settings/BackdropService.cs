using System;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class BackdropService
    {
        private const string SettingsKey = "ApplicationBackdrop";

        private static readonly string DefaultBackdrop = "Default";

        /// <summary>
        /// 应用使用的背景色
        /// </summary>
        public static string ApplicationBackdrop { get; set; }

        static BackdropService()
        {
            // 从设置存储中加载应用设定的背景色
            ApplicationBackdrop = GetBackdrop();
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// </summary>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 设置默认值
        /// </summary>
        private static void InitializeSettingsKey()
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = Convert.ToString(DefaultBackdrop);
        }

        /// <summary>
        /// 获取设置存储的应用背景色值
        /// </summary>
        private static string GetBackdrop()
        {
            if (IsSettingsKeyNullOrEmpty())
            {
                InitializeSettingsKey();
            }

            return Convert.ToString(ApplicationData.Current.LocalSettings.Values[SettingsKey]);
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        public static void SetBackdrop(string backdropMode)
        {
            ApplicationBackdrop = backdropMode;
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = backdropMode;
        }
    }
}

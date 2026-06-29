using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用窗口置顶设置服务
    /// </summary>
    public static class TopMostService
    {
        private static readonly string settingsKey = ConfigKey.TopMostKey;

        private static readonly bool defaultTopMost = false;

        private static bool _topMost;

        public static bool TopMost
        {
            get { return _topMost; }

            private set
            {
                if (!Equals(_topMost, value))
                {
                    _topMost = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(TopMost)));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的窗口置顶值
        /// </summary>
        public static void InitializeTopMost()
        {
            TopMost = GetTopMost();
        }

        /// <summary>
        /// 获取设置存储的窗口置顶值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetTopMost()
        {
            bool? topMost = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!topMost.HasValue)
            {
                SetTopMost(defaultTopMost);
                return defaultTopMost;
            }

            return Convert.ToBoolean(topMost);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的窗口置顶值
        /// </summary>
        public static void SetTopMost(bool topMost)
        {
            TopMost = topMost;

            LocalSettingsService.SaveSetting(settingsKey, topMost);
        }
    }
}

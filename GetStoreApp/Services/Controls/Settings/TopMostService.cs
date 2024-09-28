using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.ComponentModel;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用窗口置顶设置服务
    /// </summary>
    public static class TopMostService
    {
        private static readonly string settingsKey = ConfigKey.TopMostKey;

        private static readonly bool defaultTopMostValue;

        private static bool _topMostValue;

        public static bool TopMostValue
        {
            get { return _topMostValue; }

            private set
            {
                if (!Equals(_topMostValue, value))
                {
                    _topMostValue = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(TopMostValue)));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的窗口置顶值
        /// </summary>
        public static void InitializeTopMostValue()
        {
            TopMostValue = GetTopMostValue();
        }

        /// <summary>
        /// 获取设置存储的窗口置顶值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetTopMostValue()
        {
            bool? topMostValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!topMostValue.HasValue)
            {
                SetTopMostValue(defaultTopMostValue);
                return defaultTopMostValue;
            }

            return Convert.ToBoolean(topMostValue);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的使用说明按钮显示值
        /// </summary>
        public static void SetTopMostValue(bool topMostValue)
        {
            TopMostValue = topMostValue;

            LocalSettingsService.SaveSetting(settingsKey, topMostValue);
        }
    }
}

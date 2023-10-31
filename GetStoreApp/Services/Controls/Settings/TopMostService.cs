using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用窗口置顶设置服务
    /// </summary>
    public static class TopMostService
    {
        private static string SettingsKey { get; } = ConfigKey.TopMostKey;

        private static bool DefaultTopMostValue { get; } = false;

        public static bool TopMostValue { get; set; }

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
            bool? topMostValue = ConfigService.ReadSetting<bool?>(SettingsKey);

            if (!topMostValue.HasValue)
            {
                SetTopMostValue(DefaultTopMostValue);
                return DefaultTopMostValue;
            }

            return Convert.ToBoolean(topMostValue);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的使用说明按钮显示值
        /// </summary>
        public static void SetTopMostValue(bool topMostValue)
        {
            TopMostValue = topMostValue;

            ConfigService.SaveSetting(SettingsKey, topMostValue);
        }

        /// <summary>
        /// 设置应用的窗口置顶状态
        /// </summary>
        public static void SetAppTopMost()
        {
            Program.ApplicationRoot.MainWindow.SetTopMost(TopMostValue);
        }
    }
}

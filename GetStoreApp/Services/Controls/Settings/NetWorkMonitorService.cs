using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 实验性功能：下载文件时开启网络监控状态设置服务
    /// </summary>
    public static class NetWorkMonitorService
    {
        private static string SettingsKey { get; } = ConfigKey.NetWorkMonitorKey;

        private static bool DefaultNetWorkMonitorValue { get; } = true;

        public static bool NetWorkMonitorValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的网络监控开启值
        /// </summary>
        public static void InitializeNetWorkMonitorValue()
        {
            NetWorkMonitorValue = GetNetWorkMonitorValue();
        }

        /// <summary>
        /// 获取设置存储的网络监控开启值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetNetWorkMonitorValue()
        {
            bool? netWorkMonitorValue = ConfigService.ReadSetting<bool?>(SettingsKey);

            if (!netWorkMonitorValue.HasValue)
            {
                SetNetWorkMonitorValue(DefaultNetWorkMonitorValue);
                return DefaultNetWorkMonitorValue;
            }

            return Convert.ToBoolean(netWorkMonitorValue);
        }

        /// <summary>
        /// 网络监控开启值发生修改时修改设置存储的网络监控开启值
        /// </summary>
        public static void SetNetWorkMonitorValue(bool netWorkMonitorValue)
        {
            NetWorkMonitorValue = netWorkMonitorValue;

            ConfigService.SaveSetting(SettingsKey, netWorkMonitorValue);
        }

        /// <summary>
        /// 恢复默认网络监控开启值
        /// </summary>
        public static void RestoreDefaultValue()
        {
            NetWorkMonitorValue = DefaultNetWorkMonitorValue;

            ConfigService.SaveSetting(SettingsKey, DefaultNetWorkMonitorValue);
        }
    }
}

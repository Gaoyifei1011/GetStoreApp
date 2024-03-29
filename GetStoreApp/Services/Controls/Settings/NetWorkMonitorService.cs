﻿using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 实验性功能：下载文件时开启网络监控状态设置服务
    /// </summary>
    public static class NetWorkMonitorService
    {
        private static string settingsKey = ConfigKey.NetWorkMonitorKey;

        private static bool defaultNetWorkMonitorValue = true;

        public static bool NetWorkMonitorValue { get; private set; }

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
            bool? netWorkMonitorValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!netWorkMonitorValue.HasValue)
            {
                SetNetWorkMonitorValue(defaultNetWorkMonitorValue);
                return defaultNetWorkMonitorValue;
            }

            return Convert.ToBoolean(netWorkMonitorValue);
        }

        /// <summary>
        /// 网络监控开启值发生修改时修改设置存储的网络监控开启值
        /// </summary>
        public static void SetNetWorkMonitorValue(bool netWorkMonitorValue)
        {
            NetWorkMonitorValue = netWorkMonitorValue;

            LocalSettingsService.SaveSetting(settingsKey, netWorkMonitorValue);
        }

        /// <summary>
        /// 恢复默认网络监控开启值
        /// </summary>
        public static void RestoreDefaultValue()
        {
            NetWorkMonitorValue = defaultNetWorkMonitorValue;

            LocalSettingsService.SaveSetting(settingsKey, defaultNetWorkMonitorValue);
        }
    }
}

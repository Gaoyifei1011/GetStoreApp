using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Experiment
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
        public static async Task InitializeNetWorkMonitorValueAsync()
        {
            NetWorkMonitorValue = await GetNetWorkMonitorValueAsync();
        }

        /// <summary>
        /// 获取设置存储的网络监控开启值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetNetWorkMonitorValueAsync()
        {
            bool? netWorkMonitorValue = await ConfigService.ReadSettingAsync<bool?>(SettingsKey);

            if (!netWorkMonitorValue.HasValue)
            {
                return DefaultNetWorkMonitorValue;
            }

            return Convert.ToBoolean(netWorkMonitorValue);
        }

        /// <summary>
        /// 网络监控开启值发生修改时修改设置存储的网络监控开启值
        /// </summary>
        public static async Task SetNetWorkMonitorValueAsync(bool netWorkMonitorValue)
        {
            NetWorkMonitorValue = netWorkMonitorValue;

            await ConfigService.SaveSettingAsync(SettingsKey, netWorkMonitorValue);
        }

        /// <summary>
        /// 恢复默认网络监控开启值
        /// </summary>
        public static async Task RestoreDefaultValueAsync()
        {
            NetWorkMonitorValue = DefaultNetWorkMonitorValue;

            await ConfigService.SaveSettingAsync(SettingsKey, DefaultNetWorkMonitorValue);
        }
    }
}

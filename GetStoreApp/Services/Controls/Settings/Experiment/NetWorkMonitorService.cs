using GetStoreApp.Contracts.Services.Controls.Settings.Experiment;
using GetStoreAppCore.Settings;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Experiment
{
    /// <summary>
    /// 实验性功能：下载文件时开启网络监控状态
    /// </summary>
    public class NetWorkMonitorService : INetWorkMonitorService
    {
        private string SettingsKey { get; init; } = ConfigStorage.ConfigKey["NetWorkMonitorKey"];

        private bool DefaultNetWorkMonitorValue => true;

        public bool NetWorkMonitorValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的网络监控开启值
        /// </summary>
        public async Task InitializeNetWorkMonitorValueAsync()
        {
            NetWorkMonitorValue = await GetNetWorkMonitorValueAsync();
        }

        /// <summary>
        /// 获取设置存储的网络监控开启值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetNetWorkMonitorValueAsync()
        {
            bool? netWorkMonitorValue = await ConfigStorage.ReadSettingAsync<bool?>(SettingsKey);

            if (!netWorkMonitorValue.HasValue)
            {
                return DefaultNetWorkMonitorValue;
            }

            return Convert.ToBoolean(netWorkMonitorValue);
        }

        /// <summary>
        /// 网络监控开启值发生修改时修改设置存储的网络监控开启值
        /// </summary>
        public async Task SetNetWorkMonitorValueAsync(bool netWorkMonitorValue)
        {
            NetWorkMonitorValue = netWorkMonitorValue;

            await ConfigStorage.SaveSettingAsync(SettingsKey, netWorkMonitorValue);
        }

        /// <summary>
        /// 恢复默认网络监控开启值
        /// </summary>
        public async Task RestoreDefaultValueAsync()
        {
            NetWorkMonitorValue = DefaultNetWorkMonitorValue;

            await ConfigStorage.SaveSettingAsync(SettingsKey, DefaultNetWorkMonitorValue);
        }
    }
}

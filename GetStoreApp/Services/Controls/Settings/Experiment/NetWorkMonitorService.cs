using GetStoreApp.Contracts.Services.Controls.Settings.Experiment;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Experiment
{
    /// <summary>
    /// 实验性功能：下载文件时开启网络监控状态
    /// </summary>
    public class NetWorkMonitorService : INetWorkMonitorService
    {
        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private string SettingsKey { get; init; } = "NetWorkMonitorValue";

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
            bool? netWorkMonitorValue = await ConfigStorageService.ReadSettingAsync<bool?>(SettingsKey);

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

            await ConfigStorageService.SaveSettingAsync(SettingsKey, netWorkMonitorValue);
        }

        /// <summary>
        /// 恢复默认网络监控开启值
        /// </summary>
        public async Task RestoreDefaultValueAsync()
        {
            NetWorkMonitorValue = DefaultNetWorkMonitorValue;

            await ConfigStorageService.SaveSettingAsync(SettingsKey, DefaultNetWorkMonitorValue);
        }
    }
}

using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings.Appearance;
using GetStoreApp.Helpers;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings.Appearance
{
    /// <summary>
    /// 应用窗口置顶设置服务
    /// </summary>
    public class TopMostService : ITopMostService
    {
        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private string SettingsKey { get; init; } = "TopMostValue";

        private bool DefaultTopMostValue => false;

        public bool TopMostValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的窗口置顶值
        /// </summary>
        public async Task InitializeTopMostValueAsync()
        {
            TopMostValue = await GetTopMostValueAsync();
        }

        /// <summary>
        /// 获取设置存储的窗口置顶值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetTopMostValueAsync()
        {
            bool? topMostValue = await ConfigStorageService.ReadSettingAsync<bool?>(SettingsKey);

            if (!topMostValue.HasValue)
            {
                return DefaultTopMostValue;
            }

            return Convert.ToBoolean(topMostValue);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的使用说明按钮显示值
        /// </summary>
        public async Task SetTopMostValueAsync(bool topMostValue)
        {
            TopMostValue = topMostValue;

            await ConfigStorageService.SaveSettingAsync(SettingsKey, topMostValue);
        }

        /// <summary>
        /// 设置应用的窗口置顶
        /// </summary>
        public async Task SetAppTopMostAsync()
        {
            WindowHelper.SetAppTopMost(TopMostValue);

            await Task.CompletedTask;
        }
    }
}

using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Helpers.Root;
using GetStoreAppCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    public class AlwaysShowBackdropService : IAlwaysShowBackdropService
    {
        private string SettingsKey { get; init; } = ConfigStorage.ConfigKey["AlwaysShowBackdropKey"];

        private bool DefaultAlwaysShowBackdropValue = false;

        public bool AlwaysShowBackdropValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的始终显示背景色值
        /// </summary>
        public async Task InitializeAlwaysShowBackdropAsync()
        {
            AlwaysShowBackdropValue = await GetAlwaysShowBackdropValueAsync();
        }

        /// <summary>
        /// 获取设置存储的始终显示背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetAlwaysShowBackdropValueAsync()
        {
            bool? alwaysShowBackdropValue = await ConfigStorage.ReadSettingAsync<bool?>(SettingsKey);

            if (!alwaysShowBackdropValue.HasValue)
            {
                return DefaultAlwaysShowBackdropValue;
            }

            return alwaysShowBackdropValue.Value;
        }

        /// <summary>
        /// 始终显示背景色发生修改时修改设置存储的始终显示背景色值
        /// </summary>
        public async Task SetAlwaysShowBackdropAsync(bool alwaysShowBackdropValue)
        {
            AlwaysShowBackdropValue = alwaysShowBackdropValue;

            await ConfigStorage.SaveSettingAsync(SettingsKey, alwaysShowBackdropValue);
        }
    }
}

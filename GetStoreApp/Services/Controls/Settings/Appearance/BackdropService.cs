using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreAppCore.Settings;
using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using WinUIEx;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public class BackdropService : IBackdropService
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private string SettingsKey { get; init; } = ConfigStorage.ConfigKey["BackdropKey"];

        private BackdropModel DefaultAppBackdrop { get; set; }

        public BackdropModel AppBackdrop { get; set; }

        public List<BackdropModel> BackdropList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public async Task InitializeBackdropAsync()
        {
            BackdropList = ResourceService.BackdropList;

            DefaultAppBackdrop = BackdropList.Find(item => item.InternalName.Equals("Default", StringComparison.OrdinalIgnoreCase));

            AppBackdrop = await GetBackdropAsync();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<BackdropModel> GetBackdropAsync()
        {
            string backdrop = await ConfigStorage.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                return BackdropList.Find(item => item.InternalName.Equals(DefaultAppBackdrop.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return BackdropList.Find(item => item.InternalName.Equals(backdrop, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public async Task SetBackdropAsync(BackdropModel backdrop)
        {
            AppBackdrop = backdrop;

            await ConfigStorage.SaveSettingAsync(SettingsKey, backdrop.InternalName);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public async Task SetAppBackdropAsync()
        {
            App.MainWindow.Backdrop = AppBackdrop.InternalName switch
            {
                "Mica" => new MicaSystemBackdrop() { Kind = MicaKind.Base },
                "MicaAlt" => new MicaSystemBackdrop() { Kind = MicaKind.BaseAlt },
                "Acrylic" => new AcrylicSystemBackdrop()
                {
                    LightTintColor = Color.FromArgb(255, 243, 243, 243),
                    DarkTintColor = Color.FromArgb(255, 32, 32, 32),
                    LightFallbackColor = Color.FromArgb(255, 243, 243, 243),
                    DarkFallbackColor = Color.FromArgb(255, 32, 32, 32),
                },
                _ => new AcrylicSystemBackdrop()
                {
                    LightTintColor = Color.FromArgb(255, 240, 243, 249),
                    DarkTintColor = Color.FromArgb(255, 20, 20, 20),
                    LightFallbackColor = Color.FromArgb(255, 243, 243, 243),
                    DarkFallbackColor = Color.FromArgb(255, 32, 32, 32),
                    LightTintOpacity = 1,
                    DarkTintOpacity = 1,
                    LightLuminosityOpacity = 1,
                    DarkLuminosityOpacity = 1,
                }
            };

            await Task.CompletedTask;
        }
    }
}

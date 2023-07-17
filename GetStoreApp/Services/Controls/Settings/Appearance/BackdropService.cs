using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public static class BackdropService
    {
        private static string SettingsKey { get; } = ConfigKey.BackdropKey;

        private static BackdropModel DefaultAppBackdrop { get; set; }

        public static BackdropModel AppBackdrop { get; set; }

        public static List<BackdropModel> BackdropList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public static async Task InitializeBackdropAsync()
        {
            BackdropList = ResourceService.BackdropList;

            DefaultAppBackdrop = BackdropList.Find(item => item.InternalName.Equals("Default", StringComparison.OrdinalIgnoreCase));

            AppBackdrop = await GetBackdropAsync();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<BackdropModel> GetBackdropAsync()
        {
            string backdrop = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                return BackdropList.Find(item => item.InternalName.Equals(DefaultAppBackdrop.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return BackdropList.Find(item => item.InternalName.Equals(backdrop, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public static async Task SetBackdropAsync(BackdropModel backdrop)
        {
            AppBackdrop = backdrop;

            await ConfigService.SaveSettingAsync(SettingsKey, backdrop.InternalName);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public static void SetAppBackdrop()
        {
            Program.ApplicationRoot.MainWindow.SetSystemBackdrop(AppBackdrop.InternalName);
        }
    }
}

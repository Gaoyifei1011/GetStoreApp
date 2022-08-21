using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinUIEx;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public class BackdropService : IBackdropService
    {
        private string SettingsKey { get; init; } = "AppBackdrop";

        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

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
            string backdrop = await ConfigStorageService.GetSettingStringValueAsync(SettingsKey);

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

            await ConfigStorageService.SaveSettingStringValueAsync(SettingsKey, backdrop.InternalName);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public async Task SetAppBackdropAsync()
        {
            if (AppBackdrop.InternalName == "Mica") GetStoreApp.App.MainWindow.Backdrop = new MicaSystemBackdrop();
            else if (AppBackdrop.InternalName == "Acrylic") GetStoreApp.App.MainWindow.Backdrop = new AcrylicSystemBackdrop();
            else GetStoreApp.App.MainWindow.Backdrop = null;

            await Task.CompletedTask;
        }
    }
}

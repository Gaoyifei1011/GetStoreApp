using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
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
        private readonly IConfigStorageService ConfigStorageService;
        private readonly IResourceService ResourceService;

        private const string SettingsKey = "AppBackdrop";

        private string DefaultAppBackdrop { get; } = "Default";

        public string AppBackdrop { get; set; }

        public List<BackdropModel> BackdropList { get; set; }

        public BackdropService(IConfigStorageService configStorageService, IResourceService resourceService)
        {
            ConfigStorageService = configStorageService;
            ResourceService = resourceService;

            BackdropList = ResourceService.BackdropList;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public async Task InitializeBackdropAsync()
        {
            AppBackdrop = await GetBackdropAsync();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<string> GetBackdropAsync()
        {
            string backdrop = await ConfigStorageService.GetSettingStringValueAsync(SettingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                return BackdropList.Find(item => item.InternalName.Equals(DefaultAppBackdrop, StringComparison.OrdinalIgnoreCase)).InternalName;
            }

            return BackdropList.Find(item => item.InternalName.Equals(backdrop, StringComparison.OrdinalIgnoreCase)).InternalName;
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public async Task SetBackdropAsync(string backdrop)
        {
            AppBackdrop = backdrop;

            await ConfigStorageService.SaveSettingStringValueAsync(SettingsKey, backdrop);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public async Task SetAppBackdropAsync()
        {
            if (AppBackdrop == "Mica") GetStoreApp.App.MainWindow.Backdrop = new MicaSystemBackdrop();

            else if (AppBackdrop == "Acrylic") GetStoreApp.App.MainWindow.Backdrop = new AcrylicSystemBackdrop();

            else GetStoreApp.App.MainWindow.Backdrop = null;

            await Task.CompletedTask;
        }
    }
}

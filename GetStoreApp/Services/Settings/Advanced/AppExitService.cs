using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings.Advanced;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings.Advanced
{
    /// <summary>
    /// 应用关闭时行为设置服务
    /// </summary>
    public class AppExitService : IAppExitService
    {
        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private string SettingsKey { get; init; } = "AppExit";

        private AppExitModel DefaultAppExit { get; set; }

        public AppExitModel AppExit { get; set; }

        public List<AppExitModel> AppExitList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用关闭行为值
        /// </summary>
        public async Task InitializeAppExitAsync()
        {
            AppExitList = ResourceService.AppExitList;

            DefaultAppExit = AppExitList.Find(item => item.InternalName.Equals("CloseApp", StringComparison.OrdinalIgnoreCase));

            AppExit = await GetAppExitAsync();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<AppExitModel> GetAppExitAsync()
        {
            string appExit = await ConfigStorageService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(appExit))
            {
                return AppExitList.Find(item => item.InternalName.Equals(DefaultAppExit.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return AppExitList.Find(item => item.InternalName.Equals(appExit, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用关闭行为值发生修改时修改设置存储的应用关闭行为值
        /// </summary>
        public async Task SetAppExitAsync(AppExitModel appExit)
        {
            AppExit = appExit;

            await ConfigStorageService.SaveSettingAsync(SettingsKey, appExit.InternalName);
        }
    }
}

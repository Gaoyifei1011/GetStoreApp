using GetStoreApp.Contracts.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreAppCore.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
    /// <summary>
    /// 应用关闭时行为设置服务
    /// </summary>
    public class AppExitService : IAppExitService
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private string SettingsKey { get; init; } = ConfigStorage.ConfigKey["ExitKey"];

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
            string appExit = await ConfigStorage.ReadSettingAsync<string>(SettingsKey);

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

            await ConfigStorage.SaveSettingAsync(SettingsKey, appExit.InternalName);
        }
    }
}

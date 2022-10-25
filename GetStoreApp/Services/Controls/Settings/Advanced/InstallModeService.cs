using GetStoreApp.Contracts.Services.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Advanced;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
    /// <summary>
    /// 应用安装方式设置服务
    /// </summary>
    public class InstallModeService : IInstallModeService
    {
        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private string SettingsKey { get; init; } = "InstallMode";

        public InstallModeModel DefaultInstallMode { get; set; }

        public InstallModeModel InstallMode { get; set; }

        public List<InstallModeModel> InstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装方式值
        /// </summary>
        public async Task InitializeInstallModeAsync()
        {
            InstallModeList = ResourceService.InstallModeList;

            DefaultInstallMode = InstallModeList.Find(item => item.InternalName.Equals("AppInstall", StringComparison.OrdinalIgnoreCase));

            InstallMode = await GetInstallModeAsync();
        }

        /// <summary>
        /// 获取设置存储的应用安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<InstallModeModel> GetInstallModeAsync()
        {
            string installMode = await ConfigStorageService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(installMode))
            {
                return InstallModeList.Find(item => item.InternalName.Equals(DefaultInstallMode.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return InstallModeList.Find(item => item.InternalName.Equals(installMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的背景色值
        /// </summary>
        public async Task SetInstallModeAsync(InstallModeModel installMode)
        {
            InstallMode = installMode;

            await ConfigStorageService.SaveSettingAsync(SettingsKey, installMode.InternalName);
        }
    }
}

using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public class NotificatonService : INotificationService
    {
        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private string SettingsKey { get; init; } = "AppNotification";

        private bool DefaultAppNotification => true;

        public bool AppNotification { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用通知显示值
        /// </summary>
        public async Task InitializeNotificationAsync()
        {
            AppNotification = await GetNotificationAsync();
        }

        /// <summary>
        /// 获取设置存储的应用通知显示值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetNotificationAsync()
        {
            bool? appNotification = await ConfigStorageService.ReadSettingAsync<bool?>(SettingsKey);

            if (!appNotification.HasValue)
            {
                return DefaultAppNotification;
            }

            return Convert.ToBoolean(appNotification);
        }

        /// <summary>
        /// 应用通知显示发生修改时修改设置存储的使用说明按钮显示值
        /// </summary>
        public async Task SetNotificationAsync(bool appNotification)
        {
            AppNotification = appNotification;

            await ConfigStorageService.SaveSettingAsync(SettingsKey, appNotification);
        }
    }
}

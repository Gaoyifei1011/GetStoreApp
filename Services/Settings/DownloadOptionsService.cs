using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public class DownloadOptionsService : IDownloadOptionsService
    {
        private string FolderSettingsKey { get; init; } = "DownloadFolder";

        private string DownloadItemSettingsKey { get; init; } = "DownloadItemValue";

        private string NotificationSettingsKey { get; init; } = "DownloadNotification";

        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        public StorageFolder DefaultFolder { get; private set; }

        private int DefaultItem { get; } = 1;

        private bool DefaultNotification { get; } = true;

        public StorageFolder DownloadFolder { get; set; }

        public int DownloadItem { get; set; }

        public bool DownloadNotification { get; set; }

        public List<int> DownloadItemList { get; set; } = new List<int>() { 1, 2, 3 };

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public async Task InitializeAsync()
        {
            await CreateFolderAsync(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Downloads"));

            DefaultFolder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Downloads"));

            DownloadFolder = await GetFolderAsync();

            DownloadItem = await GetItemValueAsync();

            DownloadNotification = await GetNotificationAsync();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private async Task<StorageFolder> GetFolderAsync()
        {
            StorageFolder folder = await ConfigStorageService.GetSettingStorageFolderValueAsync(FolderSettingsKey);

            if (folder == null)
            {
                await SetFolderAsync(DefaultFolder);
                return DefaultFolder;
            }

            return folder;
        }

        /// <summary>
        /// 安全访问目录（当目录不存在的时候直接创建目录）
        /// </summary>
        public async Task OpenFolderAsync(StorageFolder folder)
        {
            if (!Directory.Exists(folder.Path)) await CreateFolderAsync(folder.Path);

            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        public async Task CreateFolderAsync(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 获取设置存储的同时下载任务数值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<int> GetItemValueAsync()
        {
            int? downloadItemValue = await ConfigStorageService.GetSettingIntValueAsync(DownloadItemSettingsKey);

            if (!downloadItemValue.HasValue) return DefaultItem;

            return Convert.ToInt32(downloadItemValue);
        }

        /// <summary>
        /// 获取设置存储的同时通知显示值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetNotificationAsync()
        {
            bool? downloadNotification = await ConfigStorageService.GetSettingBoolValueAsync(NotificationSettingsKey);

            if (!downloadNotification.HasValue) return DefaultNotification;

            return Convert.ToBoolean(downloadNotification);
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public async Task SetFolderAsync(StorageFolder downloadFolder)
        {
            DownloadFolder = downloadFolder;

            await ConfigStorageService.SaveSettingStorageFolderValueAsync(FolderSettingsKey, downloadFolder);
        }

        /// <summary>
        /// 同时下载任务数发生修改时修改设置存储的同时下载任务数值
        /// </summary>
        public async Task SetItemValueAsync(int downloadItem)
        {
            DownloadItem = downloadItem;

            await ConfigStorageService.SaveSettingIntValueAsync(DownloadItemSettingsKey, downloadItem);
        }

        /// <summary>
        /// 下载通知显示发生修改时修改设置存储的下载通知显示值
        /// </summary>
        public async Task SetNotificationAsync(bool downloadNotification)
        {
            DownloadNotification = downloadNotification;

            await ConfigStorageService.SaveSettingBoolValueAsync(NotificationSettingsKey, downloadNotification);
        }
    }
}

using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public static class DownloadOptionsService
    {
        private static string folderSettingsKey = ConfigKey.DownloadFolderKey;
        private static string downloadItemSettingsKey = ConfigKey.DownloadItemKey;
        private static int defaultItem = 1;

        public static StorageFolder DefaultDownloadFolder { get; private set; }

        public static StorageFolder DownloadFolder { get; private set; }

        public static int DownloadItem { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public static async Task InitializeAsync()
        {
            DefaultDownloadFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Downloads", CreationCollisionOption.OpenIfExists);

            DownloadFolder = await GetFolderAsync();
            DownloadItem = GetItem();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private static async Task<StorageFolder> GetFolderAsync()
        {
            string folder = LocalSettingsService.ReadSetting<string>(folderSettingsKey);

            try
            {
                if (string.IsNullOrEmpty(folder))
                {
                    SetFolder(DefaultDownloadFolder);
                    return DefaultDownloadFolder;
                }
                else
                {
                    return await StorageFolder.GetFolderFromPathAsync(folder);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get download saved folder failed.", e);
                SetFolder(DefaultDownloadFolder);
                return DefaultDownloadFolder;
            }
        }

        /// <summary>
        /// 获取设置存储的同时下载任务数值，如果设置没有存储，使用默认值
        /// </summary>
        private static int GetItem()
        {
            int? downloadItemValue = LocalSettingsService.ReadSetting<int?>(downloadItemSettingsKey);

            if (!downloadItemValue.HasValue)
            {
                SetItem(defaultItem);
                return defaultItem;
            }

            return Convert.ToInt32(downloadItemValue);
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public static void SetFolder(StorageFolder downloadFolder)
        {
            DownloadFolder = downloadFolder;

            LocalSettingsService.SaveSetting(folderSettingsKey, downloadFolder.Path);
        }

        /// <summary>
        /// 同时下载任务数发生修改时修改设置存储的同时下载任务数值
        /// </summary>
        public static void SetItem(int downloadItem)
        {
            DownloadItem = downloadItem;

            LocalSettingsService.SaveSetting(downloadItemSettingsKey, downloadItem);
        }

        /// <summary>
        /// 安全访问目录（当目录不存在的时候直接创建目录）
        /// </summary>
        public static async Task OpenFolderAsync(StorageFolder folder)
        {
            if (!Directory.Exists(folder.Path))
            {
                Directory.CreateDirectory(folder.Path);
            }

            await Launcher.LaunchFolderAsync(folder);
        }
    }
}

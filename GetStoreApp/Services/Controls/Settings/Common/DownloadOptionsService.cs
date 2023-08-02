using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public static class DownloadOptionsService
    {
        private static string FolderSettingsKey { get; } = ConfigKey.DownloadFolderKey;

        private static string DownloadItemSettingsKey { get; } = ConfigKey.DownloadItemKey;

        private static string DownloadModeSettingsKey { get; } = ConfigKey.DownloadModeKey;

        public static List<GroupOptionsModel> DownloadModeList { get; set; }

        public static StorageFolder AppCacheFolder { get; private set; }

        private static int DefaultItem { get; } = 1;

        private static GroupOptionsModel DefaultDownloadMode { get; set; }

        public static StorageFolder DownloadFolder { get; set; }

        public static int DownloadItem { get; set; }

        public static GroupOptionsModel DownloadMode { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public static async Task InitializeAsync()
        {
            DownloadModeList = ResourceService.DownloadModeList;

            AppCacheFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Downloads", CreationCollisionOption.OpenIfExists);

            DefaultDownloadMode = DownloadModeList.Find(item => item.SelectedValue.Equals("DownloadInApp", StringComparison.OrdinalIgnoreCase));

            DownloadFolder = await GetFolderAsync();

            DownloadItem = GetItem();

            DownloadMode = GetMode();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private static async Task<StorageFolder> GetFolderAsync()
        {
            string folder = ConfigService.ReadSetting<string>(FolderSettingsKey);

            try
            {
                if (string.IsNullOrEmpty(folder))
                {
                    return AppCacheFolder;
                }
                else
                {
                    return await StorageFolder.GetFolderFromPathAsync(folder);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.WARNING, "Get download saved folder failed.", e);
                SetFolder(AppCacheFolder);
                return AppCacheFolder;
            }
        }

        /// <summary>
        /// 获取设置存储的同时下载任务数值，如果设置没有存储，使用默认值
        /// </summary>
        private static int GetItem()
        {
            int? downloadItemValue = ConfigService.ReadSetting<int?>(DownloadItemSettingsKey);

            if (!downloadItemValue.HasValue)
            {
                return DefaultItem;
            }

            return Convert.ToInt32(downloadItemValue);
        }

        /// <summary>
        /// 获取设置存储的下载方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetMode()
        {
            string downloadMode = ConfigService.ReadSetting<string>(DownloadModeSettingsKey);

            if (string.IsNullOrEmpty(downloadMode))
            {
                return DownloadModeList.Find(item => item.SelectedValue.Equals(DefaultDownloadMode.SelectedValue, StringComparison.OrdinalIgnoreCase));
            }

            return DownloadModeList.Find(item => item.SelectedValue.Equals(downloadMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public static void SetFolder(StorageFolder downloadFolder)
        {
            DownloadFolder = downloadFolder;

            ConfigService.SaveSetting(FolderSettingsKey, downloadFolder.Path);
        }

        /// <summary>
        /// 同时下载任务数发生修改时修改设置存储的同时下载任务数值
        /// </summary>
        public static void SetItem(int downloadItem)
        {
            DownloadItem = downloadItem;

            ConfigService.SaveSetting(DownloadItemSettingsKey, downloadItem);
        }

        /// <summary>
        /// 下载方式设定值发送改变时修改涉嫌存储的下载方式设定值
        /// </summary>
        public static void SetMode(GroupOptionsModel downloadMode)
        {
            DownloadMode = downloadMode;

            ConfigService.SaveSetting(DownloadModeSettingsKey, downloadMode.SelectedValue);
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

using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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

        public static List<DownloadModeModel> DownloadModeList { get; set; }

        public static StorageFolder DefaultFolder { get; private set; }

        private static int DefaultItem { get; } = 1;

        private static DownloadModeModel DefaultDownloadMode { get; set; }

        public static StorageFolder DownloadFolder { get; set; }

        public static int DownloadItem { get; set; }

        public static DownloadModeModel DownloadMode { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public static async Task InitializeAsync()
        {
            DownloadModeList = ResourceService.DownloadModeList;

            DefaultFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Downloads", CreationCollisionOption.OpenIfExists);

            DefaultDownloadMode = DownloadModeList.Find(item => item.InternalName.Equals("DownloadInApp", StringComparison.OrdinalIgnoreCase));

            DownloadFolder = await GetFolderAsync();

            DownloadItem = await GetItemAsync();

            DownloadMode = await GetModeAsync();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private static async Task<StorageFolder> GetFolderAsync()
        {
            string folder = await ConfigService.ReadSettingAsync<string>(FolderSettingsKey);

            if (string.IsNullOrEmpty(folder))
            {
                await SetFolderAsync(DefaultFolder);
                return DefaultFolder;
            }

            return await StorageFolder.GetFolderFromPathAsync(folder);
        }

        /// <summary>
        /// 获取设置存储的同时下载任务数值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<int> GetItemAsync()
        {
            int? downloadItemValue = await ConfigService.ReadSettingAsync<int?>(DownloadItemSettingsKey);

            if (!downloadItemValue.HasValue)
            {
                return DefaultItem;
            }

            return Convert.ToInt32(downloadItemValue);
        }

        /// <summary>
        /// 获取设置存储的下载方式值，如果设置没有存储，使用默认值
        /// </summary>
        /// <returns></returns>
        private static async Task<DownloadModeModel> GetModeAsync()
        {
            string downloadMode = await ConfigService.ReadSettingAsync<string>(DownloadModeSettingsKey);

            if (string.IsNullOrEmpty(downloadMode))
            {
                return DownloadModeList.Find(item => item.InternalName.Equals(DefaultDownloadMode.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return DownloadModeList.Find(item => item.InternalName.Equals(downloadMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public static async Task SetFolderAsync(StorageFolder downloadFolder)
        {
            DownloadFolder = downloadFolder;

            await ConfigService.SaveSettingAsync(FolderSettingsKey, downloadFolder.Path);
        }

        /// <summary>
        /// 同时下载任务数发生修改时修改设置存储的同时下载任务数值
        /// </summary>
        public static async Task SetItemAsync(int downloadItem)
        {
            DownloadItem = downloadItem;

            await ConfigService.SaveSettingAsync(DownloadItemSettingsKey, downloadItem);
        }

        /// <summary>
        /// 下载方式设定值发送改变时修改涉嫌存储的下载方式设定值
        /// </summary>
        public static async Task SetModeAsync(DownloadModeModel downloadMode)
        {
            DownloadMode = downloadMode;

            await ConfigService.SaveSettingAsync(DownloadModeSettingsKey, downloadMode.InternalName);
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

        /// <summary>
        /// 在资源管理器中打开文件对应的目录，并选中该文件
        /// </summary>
        public static async Task OpenItemFolderAsync(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                IntPtr pidlList = Shell32Library.ILCreateFromPath(filePath);
                if (pidlList != IntPtr.Zero)
                {
                    try
                    {
                        Marshal.ThrowExceptionForHR(Shell32Library.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
                        await Task.CompletedTask;
                    }
                    finally
                    {
                        Shell32Library.ILFree(pidlList);
                    }
                }
            }
        }
    }
}

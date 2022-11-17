using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreAppCore.Settings;
using GetStoreAppWindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public class DownloadOptionsService : IDownloadOptionsService
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private string FolderSettingsKey { get; init; } = ConfigStorage.ConfigKey["DownloadFolderKey"];

        private string DownloadItemSettingsKey { get; init; } = ConfigStorage.ConfigKey["DownloadItemKey"];

        private string DownloadModeSettingsKey { get; init; } = ConfigStorage.ConfigKey["DownloadModeKey"];

        public List<DownloadModeModel> DownloadModeList { get; set; }

        public List<int> DownloadItemList => new List<int>() { 1, 2, 3 };

        public StorageFolder DefaultFolder { get; private set; }

        private int DefaultItem => 1;

        private DownloadModeModel DefaultDownloadMode;

        public StorageFolder DownloadFolder { get; set; }

        public int DownloadItem { get; set; }

        public DownloadModeModel DownloadMode { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public async Task InitializeAsync()
        {
            DownloadModeList = ResourceService.DownloadModeList;

            await CreateFolderAsync(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Downloads"));

            DefaultFolder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Downloads"));

            DefaultDownloadMode = DownloadModeList.Find(item => item.InternalName.Equals("DownloadInApp", StringComparison.OrdinalIgnoreCase));

            DownloadFolder = await GetFolderAsync();

            DownloadItem = await GetItemAsync();

            DownloadMode = await GetModeAsync();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private async Task<StorageFolder> GetFolderAsync()
        {
            string folder = await ConfigStorage.ReadSettingAsync<string>(FolderSettingsKey);

            if (string.IsNullOrEmpty(folder))
            {
                await SetFolderAsync(DefaultFolder);
                return DefaultFolder;
            }

            return await StorageFolder.GetFolderFromPathAsync(folder);
        }

        /// <summary>
        /// 安全访问目录（当目录不存在的时候直接创建目录）
        /// </summary>
        public async Task OpenFolderAsync(StorageFolder folder)
        {
            if (!Directory.Exists(folder.Path))
            {
                await CreateFolderAsync(folder.Path);
            }

            await Windows.System.Launcher.LaunchFolderAsync(folder);
        }

        /// <summary>
        /// 在资源管理器中打开文件对应的目录，并选中该文件
        /// </summary>
        public async Task OpenItemFolderAsync(string filePath)
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
        private async Task<int> GetItemAsync()
        {
            int? downloadItemValue = await ConfigStorage.ReadSettingAsync<int?>(DownloadItemSettingsKey);

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
        private async Task<DownloadModeModel> GetModeAsync()
        {
            string downloadMode = await ConfigStorage.ReadSettingAsync<string>(DownloadModeSettingsKey);

            if (string.IsNullOrEmpty(downloadMode))
            {
                return DownloadModeList.Find(item => item.InternalName.Equals(DefaultDownloadMode.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return DownloadModeList.Find(item => item.InternalName.Equals(downloadMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public async Task SetFolderAsync(StorageFolder downloadFolder)
        {
            DownloadFolder = downloadFolder;

            await ConfigStorage.SaveSettingAsync(FolderSettingsKey, downloadFolder.Path);
        }

        /// <summary>
        /// 同时下载任务数发生修改时修改设置存储的同时下载任务数值
        /// </summary>
        public async Task SetItemAsync(int downloadItem)
        {
            DownloadItem = downloadItem;

            await ConfigStorage.SaveSettingAsync(DownloadItemSettingsKey, downloadItem);
        }

        /// <summary>
        /// 下载方式设定值发送改变时修改涉嫌存储的下载方式设定值
        /// </summary>
        public async Task SetModeAsync(DownloadModeModel downloadMode)
        {
            DownloadMode = downloadMode;

            await ConfigStorage.SaveSettingAsync(DownloadModeSettingsKey, downloadMode.InternalName);
        }
    }
}

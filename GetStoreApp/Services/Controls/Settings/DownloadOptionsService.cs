using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
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
        private static readonly string downloadFolderKey = ConfigKey.DownloadFolderKey;
        private static readonly string doEngineModeKey = ConfigKey.DoEngineModeKey;

        private static KeyValuePair<string, string> defaultDoEngineMode;

        public static StorageFolder DefaultDownloadFolder { get; private set; }

        public static StorageFolder DownloadFolder { get; private set; }

        public static KeyValuePair<string, string> DoEngineMode { get; private set; }

        public static List<KeyValuePair<string, string>> DoEngineModeList { get; } = ResourceService.DoEngineModeList;

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public static async Task InitializeDownloadAsync()
        {
            DefaultDownloadFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Downloads", CreationCollisionOption.OpenIfExists);

            defaultDoEngineMode = InfoHelper.IsDeliveryOptimizationEnabled ? DoEngineModeList[0] : DoEngineModeList[1];

            DownloadFolder = await GetFolderAsync();

            DoEngineMode = GetDoEngineMode();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private static async Task<StorageFolder> GetFolderAsync()
        {
            string folder = LocalSettingsService.ReadSetting<string>(downloadFolderKey);

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
        /// 获取设置存储的下载引擎方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetDoEngineMode()
        {
            object doEngineMode = LocalSettingsService.ReadSetting<object>(doEngineModeKey);

            if (doEngineMode is null)
            {
                SetDoEngineMode(defaultDoEngineMode);
                return DoEngineModeList.Find(item => item.Key.Equals(defaultDoEngineMode.Key));
            }

            KeyValuePair<string, string> selectedDoEngine = DoEngineModeList.Find(item => item.Key.Equals(doEngineMode));

            return selectedDoEngine.Key is null ? defaultDoEngineMode : selectedDoEngine;
        }

        /// <summary>
        /// 下载位置发生修改时修改设置存储的下载位置值
        /// </summary>
        public static void SetFolder(StorageFolder downloadFolder)
        {
            DownloadFolder = downloadFolder;

            LocalSettingsService.SaveSetting(downloadFolderKey, downloadFolder.Path);
        }

        /// <summary>
        /// 应用下载引擎发生修改时修改设置存储的下载引擎方式值
        /// </summary>
        public static void SetDoEngineMode(KeyValuePair<string, string> doEngineMode)
        {
            DoEngineMode = doEngineMode;

            LocalSettingsService.SaveSetting(doEngineModeKey, doEngineMode.Key);
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

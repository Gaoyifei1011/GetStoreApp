using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public static class DownloadOptionsService
    {
        private static readonly string downloadFolderKey = ConfigKey.DownloadFolderKey;
        private static readonly string doEngineModeKey = ConfigKey.DoEngineModeKey;

        private static string defaultDoEngineMode;

        public static StorageFolder DefaultDownloadFolder { get; private set; }

        public static StorageFolder DownloadFolder { get; private set; }

        public static string DoEngineMode { get; private set; }

        public static List<string> DoEngineModeList { get; } = ["DeliveryOptimization", "Bits", "Aria2"];

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public static async Task InitializeDownloadOptionsAsync()
        {
            defaultDoEngineMode = InfoHelper.IsDeliveryOptimizationEnabled ? DoEngineModeList[0] : DoEngineModeList[1];
            DefaultDownloadFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Downloads", CreationCollisionOption.OpenIfExists);
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
        private static string GetDoEngineMode()
        {
            string doEngineMode = LocalSettingsService.ReadSetting<string>(doEngineModeKey);

            if (string.IsNullOrEmpty(doEngineMode))
            {
                SetDoEngineMode(defaultDoEngineMode);
                return DoEngineModeList.Find(item => string.Equals(item, defaultDoEngineMode, StringComparison.OrdinalIgnoreCase));
            }

            string selectedDoEngine = DoEngineModeList.Find(item => string.Equals(item, doEngineMode, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(selectedDoEngine) ? defaultDoEngineMode : selectedDoEngine;
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
        public static void SetDoEngineMode(string doEngineMode)
        {
            DoEngineMode = doEngineMode;
            LocalSettingsService.SaveSetting(doEngineModeKey, doEngineMode);
        }

        /// <summary>
        /// 安全访问目录（当目录不存在的时候直接创建目录）
        /// </summary>
        public static async Task OpenFolderAsync()
        {
            try
            {
                if (!Directory.Exists(DownloadFolder.Path))
                {
                    Directory.CreateDirectory(DownloadFolder.Path);
                }

                await Launcher.LaunchFolderAsync(DownloadFolder);
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }
    }
}

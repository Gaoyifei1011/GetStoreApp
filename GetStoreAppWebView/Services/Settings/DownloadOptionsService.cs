using GetStoreAppWebView.Extensions.DataType.Constant;
using GetStoreAppWebView.Services.Root;
using Microsoft.Windows.Storage;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppWebView.Services.Settings
{
    /// <summary>
    /// 应用下载设置服务
    /// </summary>
    public static class DownloadOptionsService
    {
        private static readonly string downloadFolderKey = ConfigKey.DownloadFolderKey;
        private static string defaultDownloadFolder;

        public static string DownloadFolder { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的下载相关内容设置值，并创建默认下载目录
        /// </summary>
        public static async Task InitializeDownloadOptionsAsync()
        {
            defaultDownloadFolder = (await ApplicationData.GetDefault().LocalCacheFolder.CreateFolderAsync("Downloads", Windows.Storage.CreationCollisionOption.OpenIfExists)).Path;
            DownloadFolder = GetFolder();
        }

        /// <summary>
        /// 获取设置存储的下载位置值，然后检查目录的读写权限。如果不能读取，使用默认的目录
        /// </summary>
        private static string GetFolder()
        {
            string folder = LocalSettingsService.ReadSetting<string>(downloadFolderKey);

            try
            {
                if (string.IsNullOrEmpty(folder))
                {
                    return defaultDownloadFolder;
                }
                else
                {
                    return folder;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(DownloadOptionsService), nameof(GetFolder), 1, e);
                return defaultDownloadFolder;
            }
        }
    }
}

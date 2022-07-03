using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Helpers
{
    public static class JsonHelper
    {
        private static readonly string SettingsFileName = "Settings.Json";

        public static readonly string SettingsFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Settings.Json");

        /// <summary>
        /// 设置文件不存在时，自动创建设置文件
        /// </summary>
        public static async Task InitializeJsonFileAsync()
        {
            // 创建设置文件
            await ApplicationData.Current.LocalFolder.CreateFileAsync(SettingsFileName, CreationCollisionOption.OpenIfExists);
        }

        //public static async Task ReadConfigurationAsync()
        //{
        //    using FileStream fileStream = new FileStream(SettingsFilePath, FileMode.Open, FileAccess.Read);
        //}
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// XML数据存储配置服务
    /// </summary>
    public static class XmlStorageService
    {
        private static string DownloadXmlFileName = "DownloadRecords.xml.dat";

        public static StorageFile DownloadXmlFile { get; private set; }

        public static async Task InitializeXmlFileAsync()
        {
            await InitializeDownloadXmlFileAsync();
        }

        /// <summary>
        /// 下载记录存储文件不存在时，自动创建下载记录存储文件
        /// </summary>
        public static async Task InitializeDownloadXmlFileAsync()
        {
            try
            {
                if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, DownloadXmlFileName)))
                {
                    DownloadXmlFile = await ApplicationData.Current.LocalFolder.GetFileAsync(DownloadXmlFileName);
                }
                else
                {
                    DownloadXmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(DownloadXmlFileName);
                    await InitializeDownloadDataAsync();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Critical, "Create download storage file failed.", e);
                Environment.Exit(82);
            }
        }

        /// <summary>
        /// 初始化历史记录存储文件基础内容
        /// </summary>
        private static async Task InitializeDownloadDataAsync()
        {
            XmlDocument DownloadXmlDocument = new XmlDocument();
            XmlElement DownloadElement = DownloadXmlDocument.CreateElement("DownloadRecords");
            DownloadElement.SetAttribute("Description", "Download Records Storage Data");
            DownloadElement.SetAttribute("Warning", "Please do not modify the storage file arbitrarily, otherwise the application will crash abnormally");
            DownloadXmlDocument.AppendChild(DownloadElement);
            await DownloadXmlDocument.SaveToFileAsync(DownloadXmlFile);
        }
    }
}

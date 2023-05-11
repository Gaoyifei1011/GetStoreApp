using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.WindowsAPI.PInvoke.Comctl32;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// XML数据存储配置服务
    /// </summary>
    public static class XmlStorageService
    {
        private static string HistoryXmlFileName { get; } = "HistoryRecords.xml.dat";

        private static string DownloadXmlFileName { get; } = "DownloadRecords.xml.dat";

        public static StorageFile HistoryXmlFile { get; set; }

        public static StorageFile DownloadXmlFile { get; set; }

        public static async Task InitializeXmlFileAsync()
        {
            await InitializeHistoryXmlFileAsync();
            await InitializeDownloadXmlFileAsync();
        }

        /// <summary>
        /// 历史记录存储文件不存在时，自动创建历史记录存储文件
        /// </summary>
        public static async Task InitializeHistoryXmlFileAsync()
        {
            try
            {
                if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, HistoryXmlFileName)))
                {
                    HistoryXmlFile = await ApplicationData.Current.LocalFolder.GetFileAsync(HistoryXmlFileName);
                }
                else
                {
                    HistoryXmlFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(HistoryXmlFileName);
                    await InitializeHistoryDataAsync();
                }
            }
            catch (Exception)
            {
                Comctl32Library.TaskDialog(
                    IntPtr.Zero,
                    IntPtr.Zero,
                    ResourceService.GetLocalized("Resources/AppDisplayName"),
                    ResourceService.GetLocalized("MessageInfo/CreateFileFailed"),
                    string.Empty,
                    TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON,
                    TASKDIALOGICON.TD_SHIELD_ERROR_RED_BAR,
                    out TaskDialogResult _
                    );
                Environment.Exit(Convert.ToInt32(AppExitCode.Failed));
            }
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
            catch (Exception)
            {
                Comctl32Library.TaskDialog(
                    IntPtr.Zero,
                    IntPtr.Zero,
                    ResourceService.GetLocalized("Resources/AppDisplayName"),
                    ResourceService.GetLocalized("MessageInfo/CreateFileFailed"),
                    string.Empty,
                    TASKDIALOG_COMMON_BUTTON_FLAGS.TDCBF_OK_BUTTON,
                    TASKDIALOGICON.TD_SHIELD_ERROR_RED_BAR,
                    out TaskDialogResult _
                    );
                Environment.Exit(Convert.ToInt32(AppExitCode.Failed));
            }
        }

        /// <summary>
        /// 初始化历史记录存储文件基础内容
        /// </summary>
        private static async Task InitializeHistoryDataAsync()
        {
            XmlDocument HistoryXmlDocument = new XmlDocument();
            XmlElement HistoryElement = HistoryXmlDocument.CreateElement("HistoryRecords");
            HistoryElement.SetAttribute("Description", "History Records Storage Data");
            HistoryElement.SetAttribute("Warning", "Please do not modify the storage file arbitrarily, otherwise the application will crash abnormally");
            HistoryXmlDocument.AppendChild(HistoryElement);
            await HistoryXmlDocument.SaveToFileAsync(HistoryXmlFile);
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

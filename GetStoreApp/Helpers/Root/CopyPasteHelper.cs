using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴辅助类
    /// </summary>
    public static class CopyPasteHelper
    {
        private static DataPackage DataPackage = new DataPackage();

        /// <summary>
        /// 复制字符串内容到剪贴板
        /// </summary>
        public static bool CopyTextToClipBoard(string content)
        {
            try
            {
                DataPackage.RequestedOperation = DataPackageOperation.Copy;
                DataPackage.SetText(content);
                Clipboard.SetContent(DataPackage);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Copy text to clipboard failed", e);
                return false;
            }
        }

        /// <summary>
        /// 复制字符串文件到剪贴板
        /// </summary>
        public static bool CopyFilesToClipBoard(IEnumerable<IStorageItem> files)
        {
            try
            {
                DataPackage.RequestedOperation = DataPackageOperation.Copy;
                DataPackage.SetStorageItems(files);
                Clipboard.SetContent(DataPackage);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Copy files to clipboard failed", e);
                return false;
            }
        }
    }
}

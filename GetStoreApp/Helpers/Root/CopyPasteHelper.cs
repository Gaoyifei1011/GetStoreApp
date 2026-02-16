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
        /// <summary>
        /// 复制字符串内容到剪贴板
        /// </summary>
        public static bool CopyTextToClipBoard(string content)
        {
            try
            {
                DataPackage dataPackage = new()
                {
                    RequestedOperation = DataPackageOperation.Copy,
                };
                dataPackage.SetText(content);
                Clipboard.SetContent(dataPackage);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CopyPasteHelper), nameof(CopyTextToClipBoard), 1, e);
                return false;
            }
        }

        /// <summary>
        /// 复制文件到剪贴板
        /// </summary>
        public static bool CopyFileToClipBoard(List<StorageFile> fileList)
        {
            try
            {
                DataPackage dataPackage = new()
                {
                    RequestedOperation = DataPackageOperation.Copy,
                };
                dataPackage.SetStorageItems(fileList);
                Clipboard.SetContent(dataPackage);
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CopyPasteHelper), nameof(CopyTextToClipBoard), 1, e);
                return false;
            }
        }
    }
}

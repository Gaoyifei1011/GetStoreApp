using GetStoreAppInstaller.Services.Root;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Diagnostics;

namespace GetStoreAppInstaller.Helpers.Root
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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(CopyPasteHelper), nameof(CopyTextToClipBoard), 1, e);
                return false;
            }
        }
    }
}

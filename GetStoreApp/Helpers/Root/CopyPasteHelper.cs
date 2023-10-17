using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴辅助类
    /// </summary>
    public static class CopyPasteHelper
    {
        private static DataPackage DataPackage { get; } = new DataPackage();

        /// <summary>
        /// 复制字符串内容到剪贴板
        /// </summary>
        public static void CopyTextToClipBoard(string content)
        {
            DataPackage.RequestedOperation = DataPackageOperation.Copy;
            DataPackage.SetText(content);
            Clipboard.SetContent(DataPackage);
        }

        /// <summary>
        /// 复制字符串文件到剪贴板
        /// </summary>
        public static void CopyFilesToClipBoard(IEnumerable<IStorageItem> files)
        {
            DataPackage.RequestedOperation = DataPackageOperation.Copy;
            DataPackage.SetStorageItems(files);
            Clipboard.SetContent(DataPackage);
        }
    }
}

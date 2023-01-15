using Windows.ApplicationModel.DataTransfer;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴辅助类
    /// </summary>
    public static class CopyPasteHelper
    {
        private static DataPackage DataPackage { get; } = new DataPackage();

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        /// <param name="content">复制到剪贴板的内容</param>
        public static void CopyToClipBoard(string content)
        {
            DataPackage.RequestedOperation = DataPackageOperation.Copy;

            DataPackage.SetText(content);

            Clipboard.SetContent(DataPackage);
        }
    }
}

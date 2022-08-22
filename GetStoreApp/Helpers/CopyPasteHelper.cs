using Windows.ApplicationModel.DataTransfer;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// 复制 / 粘贴剪贴板
    /// </summary>
    public static class CopyPasteHelper
    {
        private static DataPackage DataPackage { get; } = new DataPackage();

        public static void CopyToClipBoard(string content)
        {
            DataPackage.RequestedOperation = DataPackageOperation.Copy;

            DataPackage.SetText(content);

            Clipboard.SetContent(DataPackage);
        }
    }
}

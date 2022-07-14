using Windows.ApplicationModel.DataTransfer;

namespace GetStoreApp.Helpers
{
    public static class CopyPasteHelper
    {
        private static DataPackage DataPackage { get; } = new DataPackage();

        public static void CopyToClipBoard(string content)
        {
            // 复制
            DataPackage.RequestedOperation = DataPackageOperation.Copy;

            DataPackage.SetText(content);

            Clipboard.SetContent(DataPackage);
        }
    }
}

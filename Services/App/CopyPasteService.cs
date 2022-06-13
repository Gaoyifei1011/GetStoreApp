using Windows.ApplicationModel.DataTransfer;

namespace GetStoreApp.Services.App
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴
    /// </summary>
    public static class CopyPasteService
    {
        private static DataPackage DataPackage { get; } = new DataPackage();

        public static void CopyStringToClicpBoard(string content)
        {
            // 复制
            DataPackage.RequestedOperation = DataPackageOperation.Copy;

            DataPackage.SetText(content);

            Clipboard.SetContent(DataPackage);
        }
    }
}

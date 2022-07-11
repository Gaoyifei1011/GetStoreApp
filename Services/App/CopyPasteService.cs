using GetStoreApp.Contracts.Services.App;
using Windows.ApplicationModel.DataTransfer;

namespace GetStoreApp.Services.App
{
    /// <summary>
    /// 复制到剪贴板 / 从剪贴板中粘贴
    /// </summary>
    public class CopyPasteService : ICopyPasteService
    {
        private DataPackage DataPackage { get; } = new DataPackage();

        public void CopyStringToClipBoard(string content)
        {
            // 复制
            DataPackage.RequestedOperation = DataPackageOperation.Copy;

            DataPackage.SetText(content);

            Clipboard.SetContent(DataPackage);
        }
    }
}

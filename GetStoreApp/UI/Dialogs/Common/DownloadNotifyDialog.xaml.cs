using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs.Common
{
    /// <summary>
    /// 重复下载提示对话框
    /// </summary>
    public sealed partial class DownloadNotifyDialog : ContentDialog
    {
        public DownloadNotifyDialog(DuplicatedDataKind duplicatedDataInfo)
        {
            InitializeComponent();
            switch (duplicatedDataInfo)
            {
                case DuplicatedDataKind.Unfinished: Content = ResourceService.GetLocalized("Dialog/DownloadUnfinishedContent"); break;
                case DuplicatedDataKind.Completed: Content = ResourceService.GetLocalized("Dialog/DownloadCompletedContent"); break;
                case DuplicatedDataKind.MultiRecord: Content = ResourceService.GetLocalized("Dialog/DownloadMultiRecordContent"); break;
            }
        }
    }
}

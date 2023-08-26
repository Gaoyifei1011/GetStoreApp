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
        public string DownloadNotifyContent { get; set; }

        public DownloadNotifyDialog(DuplicatedDataKind duplicatedDataInfo)
        {
            switch (duplicatedDataInfo)
            {
                case DuplicatedDataKind.Unfinished: DownloadNotifyContent = ResourceService.GetLocalized("Dialog/DownloadUnfinishedContent"); break;
                case DuplicatedDataKind.Completed: DownloadNotifyContent = ResourceService.GetLocalized("Dialog/DownloadCompletedContent"); break;
                case DuplicatedDataKind.MultiRecord: DownloadNotifyContent = ResourceService.GetLocalized("Dialog/DownloadMultiRecordContent"); break;
            }

            InitializeComponent();
        }
    }
}

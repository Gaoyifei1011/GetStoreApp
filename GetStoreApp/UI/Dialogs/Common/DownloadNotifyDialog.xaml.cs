using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;

namespace GetStoreApp.UI.Dialogs.Common
{
    /// <summary>
    /// 重复下载提示对话框视图
    /// </summary>
    public sealed partial class DownloadNotifyDialog : ExtendedContentDialog
    {
        public string DownloadNotifyContent { get; set; }

        public DownloadNotifyDialog(DuplicatedDataInfoArgs duplicatedDataInfo)
        {
            switch (duplicatedDataInfo)
            {
                case DuplicatedDataInfoArgs.Unfinished: DownloadNotifyContent = ResourceService.GetLocalized("Dialog/DownloadUnfinishedContent"); break;
                case DuplicatedDataInfoArgs.Completed: DownloadNotifyContent = ResourceService.GetLocalized("Dialog/DownloadCompletedContent"); break;
                case DuplicatedDataInfoArgs.MultiRecord: DownloadNotifyContent = ResourceService.GetLocalized("Dialog/DownloadMultiRecordContent"); break;
            }

            InitializeComponent();
        }
    }
}

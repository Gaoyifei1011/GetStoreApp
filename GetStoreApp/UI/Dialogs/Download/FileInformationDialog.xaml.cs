using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;

namespace GetStoreApp.UI.Dialogs.Download
{
    /// <summary>
    /// 文件信息对话框视图
    /// </summary>
    public sealed partial class FileInformationDialog : ExtendedContentDialog
    {
        public FileInformationDialog(CompletedModel completedItem)
        {
            InitializeComponent();
            ViewModel.InitializeFileInformation(completedItem);
        }
    }
}

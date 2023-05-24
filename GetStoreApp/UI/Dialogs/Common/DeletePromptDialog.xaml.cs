using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;

namespace GetStoreApp.UI.Dialogs.Common
{
    /// <summary>
    /// 删除提示对话框视图
    /// </summary>
    public sealed partial class DeletePromptDialog : ExtendedContentDialog
    {
        public string DeleteContent { get; set; }

        public DeletePromptDialog(DeleteArgs deletePrompt)
        {
            switch (deletePrompt)
            {
                case DeleteArgs.History: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeleteArgs.Download: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeleteArgs.DownloadWithFile: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
            }

            InitializeComponent();
        }
    }
}

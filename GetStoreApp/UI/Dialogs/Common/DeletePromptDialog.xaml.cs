using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs.Common
{
    /// <summary>
    /// 删除提示对话框
    /// </summary>
    public sealed partial class DeletePromptDialog : ContentDialog
    {
        private string DeleteContent;

        public DeletePromptDialog(DeleteKind deletePrompt)
        {
            switch (deletePrompt)
            {
                case DeleteKind.History: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeleteKind.Download: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
                case DeleteKind.DownloadWithFile: DeleteContent = ResourceService.GetLocalized(string.Format("/Dialog/Delete{0}", deletePrompt.ToString())); break;
            }

            InitializeComponent();
        }
    }
}

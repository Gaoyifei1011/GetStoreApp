using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Dialogs
{
    /// <summary>
    /// 删除提示对话框
    /// </summary>
    public sealed partial class DeletePromptDialog : ContentDialog
    {
        public DeletePromptDialog(DeleteKind deletePrompt)
        {
            InitializeComponent();
            switch (deletePrompt)
            {
                case DeleteKind.Download:
                    {
                        Content = ResourceService.GetLocalized(string.Format("Dialog/Delete{0}", deletePrompt.ToString()));
                        break;
                    }
                case DeleteKind.DownloadWithFile:
                    {
                        Content = ResourceService.GetLocalized(string.Format("Dialog/Delete{0}", deletePrompt.ToString()));
                        break;
                    }
            }
        }
    }
}

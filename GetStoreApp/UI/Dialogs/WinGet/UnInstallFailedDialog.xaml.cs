using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;

namespace GetStoreApp.UI.Dialogs.WinGet
{
    /// <summary>
    /// 卸载失败提示对话框视图
    /// </summary>
    public sealed partial class UnInstallFailedDialog : ExtendedContentDialog
    {
        public UnInstallFailedDialog(string appName)
        {
            InitializeComponent();
            ViewModel.UnInstallFailedContent = string.Format(ResourceService.GetLocalized("Dialog/UnInstallFailedContent"), appName);
        }
    }
}

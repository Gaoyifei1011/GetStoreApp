using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.ViewModels.Dialogs.Common
{
    /// <summary>
    /// 屏幕缩放通知对话框视图模型
    /// </summary>
    public sealed class DPIChangedNotifyViewModel
    {
        /// <summary>
        /// 重启应用
        /// </summary>
        public void OnRestartClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Program.ApplicationRoot.Restart();
        }
    }
}

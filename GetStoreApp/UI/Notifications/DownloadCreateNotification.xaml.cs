using GetStoreApp.Views.CustomControls.Notifications;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 下载任务创建成功后应用内通知视图
    /// </summary>
    public sealed partial class DownloadCreateNotification : InAppNotification
    {
        public DownloadCreateNotification(bool isDownloadCreated = false)
        {
            InitializeComponent();
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            ViewModel.Initialize(isDownloadCreated);
        }
    }
}

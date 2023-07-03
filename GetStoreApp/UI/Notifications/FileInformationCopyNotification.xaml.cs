using GetStoreApp.Views.CustomControls.Notifications;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 文件信息复制成功后应用内通知视图
    /// </summary>
    public sealed partial class FileInformationCopyNotification : InAppNotification
    {
        public FileInformationCopyNotification(bool copyState = false)
        {
            InitializeComponent();
            ViewModel.Initialize(copyState);
        }
    }
}

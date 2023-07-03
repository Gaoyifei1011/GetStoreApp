using GetStoreApp.Views.CustomControls.Notifications;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 应用信息复制成功后应用内通知视图
    /// </summary>
    public sealed partial class AppInformationCopyNotification : InAppNotification
    {
        public AppInformationCopyNotification(bool copyState = false)
        {
            InitializeComponent();
            ViewModel.Initialize(copyState);
        }
    }
}

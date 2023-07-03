using GetStoreApp.Views.CustomControls.Notifications;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 网页缓存清理成功后应用内通知视图
    /// </summary>
    public sealed partial class WebCacheCleanNotification : InAppNotification
    {
        public WebCacheCleanNotification(bool setResult = false)
        {
            InitializeComponent();
            ViewModel.Initialize(setResult);
        }
    }
}

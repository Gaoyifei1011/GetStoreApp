using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 网页缓存清理应用内通知
    /// </summary>
    public sealed partial class WebCacheCleanNotification : InAppNotification
    {
        public WebCacheCleanNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}

using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 网络连接异常应用内通知
    /// </summary>
    public sealed partial class NetWorkErrorNotification : InAppNotification
    {
        public NetWorkErrorNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}

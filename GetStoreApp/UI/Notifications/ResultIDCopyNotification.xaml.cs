using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 请求结果 CategoryID 复制应用内通知
    /// </summary>
    public sealed partial class ResultIDCopyNotification : InAppNotification
    {
        public ResultIDCopyNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}

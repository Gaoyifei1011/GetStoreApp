using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 浏览器内核初始化失败错误信息复制应用内通知
    /// </summary>
    public sealed partial class ExceptionCopyNotification : InAppNotification
    {
        public ExceptionCopyNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}

using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 文件信息复制应用内通知
    /// </summary>
    public sealed partial class FileInformationCopyNotification : InAppNotification
    {
        public FileInformationCopyNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}

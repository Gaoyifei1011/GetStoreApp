using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 文件夹选取框打开失败应用内通知
    /// </summary>
    public sealed partial class FolderPickerNotification : InAppNotification
    {
        public FolderPickerNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}

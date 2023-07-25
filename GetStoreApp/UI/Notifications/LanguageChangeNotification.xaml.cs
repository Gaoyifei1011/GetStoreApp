using GetStoreApp.Views.CustomControls.Notifications;
using Microsoft.UI.Xaml;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 语言设置修改应用内通知
    /// </summary>
    public sealed partial class LanguageChangeNotification : InAppNotification
    {
        public LanguageChangeNotification(FrameworkElement element) : base(element)
        {
            InitializeComponent();
        }
    }
}

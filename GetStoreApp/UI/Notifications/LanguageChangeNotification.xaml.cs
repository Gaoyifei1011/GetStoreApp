using GetStoreApp.Views.CustomControls.Notifications;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 语言设置修改成功时应用内通知视图
    /// </summary>
    public sealed partial class LanguageChangeNotification : InAppNotification
    {
        public LanguageChangeNotification(bool setResult = false)
        {
            InitializeComponent();
            ViewModel.Initialize(setResult);
        }
    }
}

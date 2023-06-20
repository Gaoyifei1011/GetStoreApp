using GetStoreApp.Views.CustomControls.Notifications;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 请求结果CategoryID复制成功后应用内通知视图
    /// </summary>
    public sealed partial class ResultIDCopyNotification : InAppNotification
    {
        public ResultIDCopyNotification(bool copyState = false)
        {
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
            ViewModel.Initialize(copyState);
        }
    }
}

using GetStoreApp.Views.CustomControls.Notifications;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 日志记录清除通知视图
    /// </summary>
    public sealed partial class LogCleanNotification : InAppNotification
    {
        public LogCleanNotification(bool setResult = false)
        {
            InitializeComponent();
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            ViewModel.Initialize(setResult);
        }
    }
}

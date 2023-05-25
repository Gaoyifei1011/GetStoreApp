using GetStoreApp.Views.CustomControls.Notifications;
using System.Runtime.InteropServices;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 下载任务创建成功后应用内通知视图
    /// </summary>
    public sealed partial class DownloadCreateNotification : InAppNotification
    {
        public DownloadCreateNotification([Optional, DefaultParameterValue(false)] bool isDownloadCreated)
        {
            InitializeComponent();
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            ViewModel.Initialize(isDownloadCreated);
        }
    }
}

using GetStoreApp.Views.CustomControls.Notifications;
using System.Runtime.InteropServices;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 网页缓存清理成功后应用内通知视图
    /// </summary>
    public sealed partial class WebCacheCleanNotification : InAppNotification
    {
        public WebCacheCleanNotification([Optional, DefaultParameterValue(false)] bool setResult)
        {
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
            ViewModel.Initialize(setResult);
        }
    }
}

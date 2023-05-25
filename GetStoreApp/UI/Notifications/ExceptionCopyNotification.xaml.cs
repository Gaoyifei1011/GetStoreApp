using GetStoreApp.Views.CustomControls.Notifications;
using System.Runtime.InteropServices;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// 浏览器内核初始化失败错误信息复制成功后应用内通知视图
    /// </summary>
    public sealed partial class ExceptionCopyNotification : InAppNotification
    {
        public ExceptionCopyNotification([Optional, DefaultParameterValue(false)] bool copyState)
        {
            InitializeComponent();
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            ViewModel.Initialize(copyState);
        }
    }
}

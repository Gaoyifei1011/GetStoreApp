using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.Web.WebView2.Core;

namespace GetStoreApp.UI.Dialogs.Web
{
    /// <summary>
    /// 浏览器内核初始化失败错误信息对话框视图
    /// </summary>
    public sealed partial class CoreWebView2FailedDialog : ExtendedContentDialog
    {
        public CoreWebView2FailedDialog(CoreWebView2ProcessFailedEventArgs args)
        {
            InitializeComponent();
            ViewModel.InitializeFailedInformation(args);
        }
    }
}

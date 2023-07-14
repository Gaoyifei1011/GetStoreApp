using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text;

namespace GetStoreApp.UI.Dialogs.Web
{
    /// <summary>
    /// 浏览器内核初始化失败错误信息对话框
    /// </summary>
    public sealed partial class CoreWebView2FailedDialog : ExtendedContentDialog
    {
        public string ProcessFailedKind { get; set; }

        public string Reason { get; set; }

        public string ExitCode { get; set; }

        public string ProcessDescription { get; set; }

        public CoreWebView2FailedDialog(CoreWebView2ProcessFailedEventArgs args)
        {
            InitializeComponent();
            ProcessFailedKind = Convert.ToString(args.ProcessFailedKind);
            Reason = Convert.ToString(args.Reason);
            ExitCode = Convert.ToString(args.ExitCode);
            ProcessDescription = Convert.ToString(args.ProcessDescription);
        }

        /// <summary>
        /// 复制异常信息
        /// </summary>
        public void OnCopyExceptionInformationClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/ProcessFailedKind") + ProcessFailedKind);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/Reason") + Reason);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/ExitCode") + ExitCode);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/ProcessDescription") + ProcessDescription);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            sender.Hide();
            new ExceptionCopyNotification().Show();
        }
    }
}

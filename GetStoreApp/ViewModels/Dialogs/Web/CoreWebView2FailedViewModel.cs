using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text;

namespace GetStoreApp.ViewModels.Dialogs.Web
{
    public class CoreWebView2FailedViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public string ProcessFailedKind { get; set; }

        public string Reason { get; set; }

        public string ExitCode { get; set; }

        public string ProcessDescription { get; set; }

        public void InitializeFailedInformation(CoreWebView2ProcessFailedEventArgs args)
        {
            ProcessFailedKind = Convert.ToString(args.ProcessFailedKind);
            Reason = Convert.ToString(args.Reason);
            ExitCode = Convert.ToString(args.ExitCode);
            ProcessDescription = Convert.ToString(args.ProcessDescription);
        }

        // 复制异常信息
        public IRelayCommand CopyExceptionCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/ProcessFailedKind") + ProcessFailedKind);
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/Reason") + Reason);
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/ExitCode") + ExitCode);
            stringBuilder.AppendLine(ResourceService.GetLocalized("/Dialog/ProcessDescription") + ProcessDescription);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            dialog.Hide();

            new ExceptionCopyNotification(true).Show();
        });

        // 关闭窗口
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });
    }
}

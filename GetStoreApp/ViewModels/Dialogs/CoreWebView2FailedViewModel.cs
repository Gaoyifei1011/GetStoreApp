using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System.Text;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class CoreWebView2FailedViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public string ProcessFailedKind { get; set; }

        public string Reason { get; set; }

        public string ExitCode { get; set; }

        public string ProcessDescription { get; set; }

        public void InitializeFailedInformation(CoreWebView2ProcessFailedEventArgs args)
        {
            ProcessFailedKind = string.Format(ResourceService.GetLocalized("/Dialog/ProcessFailedKind"), args.ProcessFailedKind);
            Reason = string.Format(ResourceService.GetLocalized("/Dialog/Reason"), args.Reason);
            ExitCode = string.Format(ResourceService.GetLocalized("/Dialog/ExitCode"), args.ExitCode);
            ProcessDescription = string.Format(ResourceService.GetLocalized("/Dialog/ProcessDescription"), args.ProcessDescription);
        }

        // 复制异常信息
        public IRelayCommand CopyExceptionCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(ProcessFailedKind);
            stringBuilder.AppendLine(Reason);
            stringBuilder.AppendLine(ExitCode);
            stringBuilder.AppendLine(ProcessDescription);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            dialog.Hide();
        });

        // 关闭窗口
        public IRelayCommand CoreWebView2FailedCancelCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });
    }
}

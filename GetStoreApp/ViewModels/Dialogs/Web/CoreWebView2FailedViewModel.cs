using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text;

namespace GetStoreApp.ViewModels.Dialogs.Web
{
    public sealed class CoreWebView2FailedViewModel
    {
        public string ProcessFailedKind { get; set; }

        public string Reason { get; set; }

        public string ExitCode { get; set; }

        public string ProcessDescription { get; set; }

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

        public void InitializeFailedInformation(CoreWebView2ProcessFailedEventArgs args)
        {
            ProcessFailedKind = Convert.ToString(args.ProcessFailedKind);
            Reason = Convert.ToString(args.Reason);
            ExitCode = Convert.ToString(args.ExitCode);
            ProcessDescription = Convert.ToString(args.ProcessDescription);
        }

        /// <summary>
        /// 对话框加载完成后让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            ContentDialog dialog = sender as ContentDialog;

            if (dialog is not null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(dialog);

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject current = VisualTreeHelper.GetChild(parent, i);
                    if (current is Rectangle { Name: "SmokeLayerBackground" } background)
                    {
                        background.Margin = new Thickness(0);
                        background.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 重置内容对话框烟雾背景距离顶栏的间隔
        /// </summary>
        private void OnMarginChanged(DependencyObject sender, DependencyProperty property)
        {
            if (property == FrameworkElement.MarginProperty)
            {
                sender.ClearValue(property);
            }
        }
    }
}

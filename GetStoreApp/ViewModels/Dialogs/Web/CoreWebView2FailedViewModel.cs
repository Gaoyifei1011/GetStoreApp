﻿using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text;

namespace GetStoreApp.ViewModels.Dialogs.Web
{
    /// <summary>
    /// 浏览器内核初始化失败错误信息对话框视图模型
    /// </summary>
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
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/ProcessFailedKind") + ProcessFailedKind);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/Reason") + Reason);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/ExitCode") + ExitCode);
            stringBuilder.AppendLine(ResourceService.GetLocalized("Dialog/ProcessDescription") + ProcessDescription);

            CopyPasteHelper.CopyToClipBoard(stringBuilder.ToString());
            dialog.Hide();

            new ExceptionCopyNotification(true).Show();
        });

        // 关闭对话框
        public IRelayCommand CloseDialogCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });

        /// <summary>
        /// 初始化错误信息
        /// </summary>
        public void InitializeFailedInformation(CoreWebView2ProcessFailedEventArgs args)
        {
            ProcessFailedKind = Convert.ToString(args.ProcessFailedKind);
            Reason = Convert.ToString(args.Reason);
            ExitCode = Convert.ToString(args.ExitCode);
            ProcessDescription = Convert.ToString(args.ProcessDescription);
        }
    }
}

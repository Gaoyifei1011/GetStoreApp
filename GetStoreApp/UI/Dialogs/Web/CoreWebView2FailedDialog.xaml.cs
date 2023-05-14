using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;

namespace GetStoreApp.UI.Dialogs.Web
{
    /// <summary>
    /// 浏览器内核初始化失败错误信息对话框视图
    /// </summary>
    public sealed partial class CoreWebView2FailedDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public CoreWebView2FailedDialog(CoreWebView2ProcessFailedEventArgs args)
        {
            InitializeComponent();
            ViewModel.InitializeFailedInformation(args);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void OnCloseDialogClicked(object sender, RoutedEventArgs args)
        {
            Hide();
        }

        /// <summary>
        /// 复制异常信息
        /// </summary>
        public void OnCopyExceptionInformationClicked(object sender, RoutedEventArgs args)
        {
            ViewModel.CopyExceptionInformation();
            Hide();
        }
    }
}

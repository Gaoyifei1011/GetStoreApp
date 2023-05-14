using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.WinGet
{
    /// <summary>
    /// 卸载失败提示对话框视图
    /// </summary>
    public sealed partial class UnInstallFailedDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public UnInstallFailedDialog(string appName)
        {
            InitializeComponent();
            ViewModel.UnInstallFailedContent = string.Format(ResourceService.GetLocalized("Dialog/UnInstallFailedContent"), appName);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void OnCloseDialogClicked(object sender, RoutedEventArgs args)
        {
            Hide();
        }

        /// <summary>
        /// 打开应用和功能卸载应用
        /// </summary>
        public void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            ViewModel.OpenSettings();
            Hide();
        }
    }
}

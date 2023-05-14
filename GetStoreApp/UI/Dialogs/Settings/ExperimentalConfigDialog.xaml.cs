using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 实验性功能配置对话框视图
    /// </summary>
    public sealed partial class ExperimentalConfigDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public ExperimentalConfigDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void OnCloseDialogClicked(object sender, RoutedEventArgs args)
        {
            Hide();
        }

        /// <summary>
        /// 还原默认值
        /// </summary>
        public void OnRestoreDefaultClicked(object sender, RoutedEventArgs args)
        {
            NetWorkMonitor.ViewModel.NetWorkMonitorValue = true;
            ViewModel.RestoreDefault();
        }
    }
}

using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 如何识别传统桌面应用对话框视图
    /// </summary>
    public sealed partial class DesktopAppsDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public DesktopAppsDialog()
        {
            InitializeComponent();
        }
    }
}

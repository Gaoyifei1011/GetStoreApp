using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 应用重启对话框视图
    /// </summary>
    public sealed partial class RestartAppsDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public RestartAppsDialog()
        {
            InitializeComponent();
        }
    }
}

using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.Common
{
    /// <summary>
    /// 应用程序正在运行中对话框视图
    /// </summary>
    public sealed partial class AppRunningDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public AppRunningDialog()
        {
            InitializeComponent();
        }
    }
}

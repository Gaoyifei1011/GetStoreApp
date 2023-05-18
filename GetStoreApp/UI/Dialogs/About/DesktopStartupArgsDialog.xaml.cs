using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.About
{
    /// <summary>
    /// 桌面程序参数对话框视图
    /// </summary>
    public sealed partial class DesktopStartupArgsDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public DesktopStartupArgsDialog()
        {
            InitializeComponent();
        }
    }
}

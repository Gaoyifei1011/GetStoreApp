using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.Download
{
    /// <summary>
    /// 应用安装提示对话框视图
    /// </summary>
    public sealed partial class InstallingNotifyDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public InstallingNotifyDialog()
        {
            InitializeComponent();
        }
    }
}

using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.WinGet
{
    /// <summary>
    /// 重启设备对话框视图
    /// </summary>
    public sealed partial class RebootDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public RebootDialog(WinGetOptionArgs options, string appName)
        {
            InitializeComponent();
            ViewModel.InitializeRebootContent(options, appName);
        }
    }
}

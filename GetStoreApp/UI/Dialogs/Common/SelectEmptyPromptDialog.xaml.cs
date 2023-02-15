using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.Common
{
    /// <summary>
    /// 项目选择为空时提示对话框视图
    /// </summary>
    public sealed partial class SelectEmptyPromptDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public SelectEmptyPromptDialog()
        {
            InitializeComponent();
        }
    }
}

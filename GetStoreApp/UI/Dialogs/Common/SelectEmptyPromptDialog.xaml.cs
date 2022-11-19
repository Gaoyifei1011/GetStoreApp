using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.Common
{
    public sealed partial class SelectEmptyPromptDialog : ContentDialog
    {
        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public SelectEmptyPromptDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }
    }
}

using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.About
{
    public sealed partial class LicenseDialog : ContentDialog
    {
        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public LicenseDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }
    }
}

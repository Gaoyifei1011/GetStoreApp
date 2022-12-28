using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.Window;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.Download
{
    public sealed partial class InstallingNotifyDialog : ContentDialog
    {
        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public InstallingNotifyDialog()
        {
            XamlRoot = MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
        }
    }
}

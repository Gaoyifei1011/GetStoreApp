using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Window;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.Settings
{
    public sealed partial class TraceCleanupPromptDialog : ContentDialog
    {
        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string CleanFailed => ResourceService.GetLocalized("/Dialog/CleanFailed");

        public TraceCleanupPromptDialog()
        {
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
            ViewModel.InitializeTraceCleanupList();
        }
    }
}

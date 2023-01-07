using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;

namespace GetStoreApp.UI.Dialogs.Web
{
    public sealed partial class CoreWebView2FailedDialog : ContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public CoreWebView2FailedDialog(CoreWebView2ProcessFailedEventArgs args)
        {
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
            ViewModel.InitializeFailedInformation(args);
        }
    }
}

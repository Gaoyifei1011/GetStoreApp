using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings.Appearance;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class CoreWebView2FailedDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public CoreWebView2FailedViewModel ViewModel { get; } = IOCHelper.GetService<CoreWebView2FailedViewModel>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public CoreWebView2FailedDialog(CoreWebView2ProcessFailedEventArgs args)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            ViewModel.InitializeFailedInformation(args);
            InitializeComponent();
        }
    }
}

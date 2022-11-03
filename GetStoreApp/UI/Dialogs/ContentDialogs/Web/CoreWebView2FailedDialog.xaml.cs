using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Dialogs.Web;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;

namespace GetStoreApp.UI.Dialogs.ContentDialogs.Web
{
    public sealed partial class CoreWebView2FailedDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public CoreWebView2FailedViewModel ViewModel { get; } = ContainerHelper.GetInstance<CoreWebView2FailedViewModel>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public CoreWebView2FailedDialog(CoreWebView2ProcessFailedEventArgs args)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            ViewModel.InitializeFailedInformation(args);
            InitializeComponent();
        }
    }
}

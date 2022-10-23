using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings.Appearance;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class RestartAppsDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public RestartAppsViewModel ViewModel { get; } = IOCHelper.GetService<RestartAppsViewModel>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public RestartAppsDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }
    }
}

using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings.Appearance;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class ClosingWindowDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public ClosingWindowDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }
    }
}

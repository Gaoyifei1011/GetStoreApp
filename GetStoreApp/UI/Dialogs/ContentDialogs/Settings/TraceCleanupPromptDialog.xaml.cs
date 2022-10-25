using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Dialogs.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.ContentDialogs.Settings
{
    public sealed partial class TraceCleanupPromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public TraceCleanupPromptViewModel ViewModel { get; } = IOCHelper.GetService<TraceCleanupPromptViewModel>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string CleanFailed => ResourceService.GetLocalized("/Dialog/CleanFailed");

        public TraceCleanupPromptDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            ViewModel.InitializeTraceCleanupList();
            InitializeComponent();
        }
    }
}

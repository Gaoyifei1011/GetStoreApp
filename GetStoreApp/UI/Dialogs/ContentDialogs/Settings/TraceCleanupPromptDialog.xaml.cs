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
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public TraceCleanupPromptViewModel ViewModel { get; } = ContainerHelper.GetInstance<TraceCleanupPromptViewModel>();

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

using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class TraceCleanupPromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public TraceCleanupPromptViewModel ViewModel { get; } = IOCHelper.GetService<TraceCleanupPromptViewModel>();

        public TraceCleanupPromptDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
        }
    }
}

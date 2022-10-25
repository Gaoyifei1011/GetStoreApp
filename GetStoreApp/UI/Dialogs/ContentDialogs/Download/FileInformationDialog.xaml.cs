using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.ViewModels.Dialogs.Download;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.ContentDialogs.Download
{
    public sealed partial class FileInformationDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public FileInformationViewModel ViewModel { get; } = IOCHelper.GetService<FileInformationViewModel>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public FileInformationDialog(CompletedModel completedItem)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
            ViewModel.InitializeFileInformation(completedItem);
        }
    }
}

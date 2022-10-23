using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings.Appearance;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Download;
using GetStoreApp.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
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

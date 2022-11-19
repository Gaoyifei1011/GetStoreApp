using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.Download
{
    public sealed partial class FileInformationDialog : ContentDialog
    {
        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public FileInformationDialog(CompletedModel completedItem)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            InitializeComponent();
            ViewModel.InitializeFileInformation(completedItem);
        }
    }
}

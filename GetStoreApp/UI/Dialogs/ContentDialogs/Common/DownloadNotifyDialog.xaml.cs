using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.ContentDialogs.Common
{
    public sealed partial class DownloadNotifyDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string DownloadNotifyContent { get; set; }

        public DownloadNotifyDialog(DuplicatedDataInfoArgs duplicatedDataInfo)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

            switch (duplicatedDataInfo)
            {
                case DuplicatedDataInfoArgs.Unfinished: DownloadNotifyContent = ResourceService.GetLocalized("/Dialog/DownloadUnfinishedContent"); break;
                case DuplicatedDataInfoArgs.Completed: DownloadNotifyContent = ResourceService.GetLocalized("/Dialog/DownloadCompletedContent"); break;
                case DuplicatedDataInfoArgs.MultiRecord: DownloadNotifyContent = ResourceService.GetLocalized("/Dialog/DownloadMultiRecordContent"); break;
            }

            InitializeComponent();
        }
    }
}

using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class DownloadNotifyDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string DownloadNotifyContent { get; set; }

        public DownloadNotifyDialog(DuplicatedDataInfo duplicatedDataInfo)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

            switch (duplicatedDataInfo)
            {
                case DuplicatedDataInfo.Unfinished: DownloadNotifyContent = ResourceService.GetLocalized("/Dialog/DownloadUnfinishedContent"); break;
                case DuplicatedDataInfo.Completed: DownloadNotifyContent = ResourceService.GetLocalized("/Dialog/DownloadCompletedContent"); break;
                case DuplicatedDataInfo.MultiRecord: DownloadNotifyContent = ResourceService.GetLocalized("/Dialog/DownloadMultiRecordContent"); break;
            }

            InitializeComponent();
        }
    }
}

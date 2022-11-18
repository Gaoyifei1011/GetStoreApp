using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.ContentDialogs.Common
{
    public sealed partial class DownloadNotifyDialog : ContentDialog
    {
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

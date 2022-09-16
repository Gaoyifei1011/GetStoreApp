﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class DownloadNotificationDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string DownloadNotifyContent { get; set; }

        public DownloadNotificationDialog(int count)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;
            RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

            if (count == default(int))
            {
                DownloadNotifyContent = ResourceService.GetLocalized("/Dialog/DownloadNotifyContent");
            }
            else
            {
                DownloadNotifyContent = string.Format(ResourceService.GetLocalized("/Dialog/DownloadNotifyMultiContent"), count);
            }

            InitializeComponent();
        }
    }
}

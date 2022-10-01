﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class DeletePromptDialog : ContentDialog
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public string DeleteContent { get; set; }

        public DeletePromptDialog(params string[] parameter)
        {
            XamlRoot = App.MainWindow.Content.XamlRoot;

            if (parameter.Length == 0)
            {
                DeleteContent = ResourceService.GetLocalized("/Dialog/DeleteContent");
            }
            else if (parameter[0] == "DeleteWithFile")
            {
                DeleteContent = ResourceService.GetLocalized("/Dialog/DeleteWithFileContent");
            }

            InitializeComponent();
        }
    }
}
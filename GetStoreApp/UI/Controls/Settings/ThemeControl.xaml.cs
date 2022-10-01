﻿using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class ThemeControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ThemeViewModel ViewModel { get; } = IOCHelper.GetService<ThemeViewModel>();

        public ThemeControl()
        {
            InitializeComponent();
        }
    }
}
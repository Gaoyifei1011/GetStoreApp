using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class ThemeViewModel : ObservableRecipient
    {
        private readonly IThemeService ThemeService;

        private string _theme;

        public string Theme
        {
            get { return _theme; }

            set { SetProperty(ref _theme, value); }
        }

        public List<ThemeModel> ThemeList { get; set; }

        public IAsyncRelayCommand ThemeSelectCommand { get; set; }

        public IAsyncRelayCommand SettingsColorCommand { get; set; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        });

        public ThemeViewModel(IThemeService themeService)
        {
            ThemeService = themeService;

            ThemeList = ThemeService.ThemeList;
            Theme = ThemeService.AppTheme;

            ThemeSelectCommand = new AsyncRelayCommand(async () =>
            {
                await ThemeService.SetThemeAsync(Theme);
            });
        }
    }
}

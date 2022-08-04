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
        private IThemeService ThemeService { get; } = App.GetService<IThemeService>();

        private ThemeModel _theme;

        public ThemeModel Theme
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

        public ThemeViewModel()
        {
            ThemeList = ThemeService.ThemeList;
            Theme = ThemeService.AppTheme;

            ThemeSelectCommand = new AsyncRelayCommand(async () =>
            {
                await ThemeService.SetThemeAsync(Theme);
                await ThemeService.SetAppThemeAsync();
            });
        }
    }
}

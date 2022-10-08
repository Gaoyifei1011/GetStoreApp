using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class ThemeViewModel : ObservableRecipient
    {
        private IThemeService ThemeService { get; } = IOCHelper.GetService<IThemeService>();

        public List<ThemeModel> ThemeList => ThemeService.ThemeList;

        private ThemeModel _theme;

        public ThemeModel Theme
        {
            get { return _theme; }

            set { SetProperty(ref _theme, value); }
        }

        // 主题修改设置
        public IRelayCommand ThemeSelectCommand => new RelayCommand(async () =>
            {
                await ThemeService.SetThemeAsync(Theme);
                await ThemeService.SetAppThemeAsync();
            });

        // 打开系统主题设置
        public IRelayCommand SettingsColorCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        });

        public ThemeViewModel()
        {
            Theme = ThemeService.AppTheme;
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.Models.Controls.Settings.Appearance;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public class ThemeViewModel : ObservableRecipient
    {
        private IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

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

using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public sealed class ThemeViewModel : ViewModelBase
    {
        public List<ThemeModel> ThemeList { get; } = ThemeService.ThemeList;

        private ThemeModel _theme = ThemeService.AppTheme;

        public ThemeModel Theme
        {
            get { return _theme; }

            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        // 打开系统主题设置
        public IRelayCommand SettingsColorCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        });

        /// <summary>
        /// 主题修改设置
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                await ThemeService.SetThemeAsync(Theme);
                await ThemeService.SetAppThemeAsync();
            }
        }
    }
}

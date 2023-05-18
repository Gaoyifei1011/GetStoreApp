using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：应用主题设置用户控件视图模型
    /// </summary>
    public sealed class ThemeViewModel : ViewModelBase
    {
        public List<ThemeModel> ThemeList { get; } = ThemeService.ThemeList;

        public List<NotifyIconMenuThemeModel> NotifyIconMenuThemeList { get; } = ThemeService.NotifyIconMenuThemeList;

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

        private NotifyIconMenuThemeModel _notifyIconMenuTheme = ThemeService.NotifyIconMenuTheme;

        public NotifyIconMenuThemeModel NotifyIconMenuTheme
        {
            get { return _notifyIconMenuTheme; }

            set
            {
                _notifyIconMenuTheme = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        public async void OnSettingsColorClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        public async void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(item.Tag)];
                await ThemeService.SetThemeAsync(Theme);
                ThemeService.SetWindowTheme();
                ThemeService.SetTrayWindowTheme();
            }
        }

        /// <summary>
        /// 通知区域右键菜单主题设置
        /// </summary>
        public async void OnNotifyIconMenuThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                NotifyIconMenuTheme = NotifyIconMenuThemeList[Convert.ToInt32(item.Tag)];
                await ThemeService.SetNotifyIconMenuThemeAsync(NotifyIconMenuTheme);
                ThemeService.SetTrayWindowTheme();
            }
        }
    }
}

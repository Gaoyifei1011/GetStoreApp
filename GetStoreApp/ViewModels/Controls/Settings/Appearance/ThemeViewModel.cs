using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.ViewModels.Base;
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

        // 打开系统主题设置
        public IRelayCommand SettingsColorCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        });

        // 主题修改设置
        public IRelayCommand ThemeSelectCommand => new RelayCommand<string>(async (themeIndex) =>
        {
            Theme = ThemeList[Convert.ToInt32(themeIndex)];
            await ThemeService.SetThemeAsync(Theme);
            ThemeService.SetWindowTheme();
            ThemeService.SetTrayWindowTheme();
        });

        // 通知区域右键菜单主题设置
        public IRelayCommand NotifyIconMenuThemeSelectCommand => new RelayCommand<string>(async (notifyIconMenuThemeIndex) =>
        {
            NotifyIconMenuTheme = NotifyIconMenuThemeList[Convert.ToInt32(notifyIconMenuThemeIndex)];
            await ThemeService.SetNotifyIconMenuThemeAsync(NotifyIconMenuTheme);
            ThemeService.SetTrayWindowTheme();
        });
    }
}

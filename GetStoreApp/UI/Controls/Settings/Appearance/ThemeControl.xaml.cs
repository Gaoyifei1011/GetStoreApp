using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;
using WinRT;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：应用主题设置控件
    /// </summary>
    public sealed partial class ThemeControl : Expander, INotifyPropertyChanged
    {
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

        public List<ThemeModel> ThemeList { get; } = ThemeService.ThemeList;

        public List<NotifyIconMenuThemeModel> NotifyIconMenuThemeList { get; } = ThemeService.NotifyIconMenuThemeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public ThemeControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
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
            ToggleMenuFlyoutItem item = sender.As<ToggleMenuFlyoutItem>();
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
            ToggleMenuFlyoutItem item = sender.As<ToggleMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                NotifyIconMenuTheme = NotifyIconMenuThemeList[Convert.ToInt32(item.Tag)];
                await ThemeService.SetNotifyIconMenuThemeAsync(NotifyIconMenuTheme);
                ThemeService.SetTrayWindowTheme();
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

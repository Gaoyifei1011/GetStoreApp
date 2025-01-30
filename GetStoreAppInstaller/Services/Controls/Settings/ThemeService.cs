using GetStoreAppInstaller.Extensions.DataType.Constant;
using GetStoreAppInstaller.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace GetStoreAppInstaller.Services.Controls.Settings
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static readonly string themeSettingsKey = ConfigKey.ThemeKey;

        private static KeyValuePair<string, string> defaultAppTheme;

        private static KeyValuePair<string, string> _appTheme;

        public static KeyValuePair<string, string> AppTheme
        {
            get { return _appTheme; }

            private set
            {
                if (!Equals(_appTheme, value))
                {
                    _appTheme = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AppTheme)));
                }
            }
        }

        public static List<KeyValuePair<string, string>> ThemeList { get; private set; }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static void InitializeTheme()
        {
            ThemeList = ResourceService.ThemeList;

            defaultAppTheme = ThemeList.Find(item => item.Key.Equals(nameof(ElementTheme.Default), StringComparison.OrdinalIgnoreCase));

            AppTheme = GetTheme();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetTheme()
        {
            object theme = LocalSettingsService.ReadSetting<object>(themeSettingsKey);

            if (theme is null)
            {
                return defaultAppTheme;
            }

            KeyValuePair<string, string> selectedTheme = ThemeList.Find(item => item.Key.Equals(theme));

            return selectedTheme.Key is null ? defaultAppTheme : selectedTheme;
        }
    }
}

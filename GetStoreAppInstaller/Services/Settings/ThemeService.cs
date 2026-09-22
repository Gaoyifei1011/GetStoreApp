using GetStoreAppInstaller.Extensions.DataType.Constant;
using GetStoreAppInstaller.Services.Root;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;

namespace GetStoreAppInstaller.Services.Settings
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    internal static class ThemeService
    {
        private static readonly string settingsKey = ConfigKey.ThemeKey;

        private static string defaultAppTheme;

        internal static string AppTheme { get; set; }

        internal static ReadOnlyCollection<string> ThemeCollection { get; } = [nameof(ElementTheme.Default), nameof(ElementTheme.Light), nameof(ElementTheme.Dark)];

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        internal static void InitializeTheme()
        {
            foreach (string themeItem in ThemeCollection)
            {
                if (string.Equals(themeItem, nameof(ElementTheme.Default), StringComparison.OrdinalIgnoreCase))
                {
                    defaultAppTheme = themeItem;
                    break;
                }
            }
            AppTheme = GetTheme();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetTheme()
        {
            string theme = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                return defaultAppTheme;
            }

            string selectedTheme = default;
            foreach (string themeItem in ThemeCollection)
            {
                if (string.Equals(themeItem, theme, StringComparison.OrdinalIgnoreCase))
                {
                    selectedTheme = themeItem;
                    break;
                }
            }
            return string.IsNullOrEmpty(selectedTheme) ? defaultAppTheme : selectedTheme;
        }
    }
}

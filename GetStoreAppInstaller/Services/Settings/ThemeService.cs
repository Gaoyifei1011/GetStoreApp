using GetStoreAppInstaller.Extensions.DataType.Constant;
using GetStoreAppInstaller.Services.Root;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace GetStoreAppInstaller.Services.Settings
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static readonly string settingsKey = ConfigKey.ThemeKey;

        private static string defaultAppTheme;

        public static string AppTheme { get; set; }

        public static List<string> ThemeList { get; } = [];

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static void InitializeTheme()
        {
            ThemeList.Add(nameof(ElementTheme.Default));
            ThemeList.Add(nameof(ElementTheme.Light));
            ThemeList.Add(nameof(ElementTheme.Dark));
            defaultAppTheme = ThemeList.Find(item => string.Equals(item, nameof(ElementTheme.Default), StringComparison.OrdinalIgnoreCase));
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

            string selectedTheme = ThemeList.Find(item => string.Equals(item, theme, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(selectedTheme) ? defaultAppTheme : selectedTheme;
        }
    }
}

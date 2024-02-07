using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string themeSettingsKey = ConfigKey.ThemeKey;

        private static DictionaryEntry defaultAppTheme;

        public static DictionaryEntry AppTheme { get; private set; }

        public static List<DictionaryEntry> ThemeList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static void InitializeTheme()
        {
            ThemeList = ResourceService.ThemeList;

            defaultAppTheme = ThemeList.Find(item => item.Value.ToString().Equals(nameof(ElementTheme.Default), StringComparison.OrdinalIgnoreCase));

            AppTheme = GetTheme();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetTheme()
        {
            object theme = LocalSettingsService.ReadSetting<object>(themeSettingsKey);

            if (theme is null)
            {
                SetTheme(defaultAppTheme);
                return defaultAppTheme;
            }

            DictionaryEntry selectedTheme = ThemeList.Find(item => item.Value.Equals(theme));

            return selectedTheme.Key is null ? defaultAppTheme : ThemeList.Find(item => item.Value.Equals(theme));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static void SetTheme(DictionaryEntry theme)
        {
            AppTheme = theme;

            LocalSettingsService.SaveSetting(themeSettingsKey, theme.Value);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public static void SetWindowTheme()
        {
            if (AppTheme.Value.Equals(ThemeList[0].Value))
            {
                if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                {
                    MainWindow.Current.WindowTheme = ElementTheme.Light;
                }
                else
                {
                    MainWindow.Current.WindowTheme = ElementTheme.Dark;
                }
            }
            else
            {
                MainWindow.Current.WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.Value.ToString());
            }
        }
    }
}

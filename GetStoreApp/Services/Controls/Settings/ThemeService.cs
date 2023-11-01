using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
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
        private static string ThemeSettingsKey = ConfigKey.ThemeKey;

        private static DictionaryEntry DefaultAppTheme;

        public static DictionaryEntry AppTheme { get; private set; }

        public static List<DictionaryEntry> ThemeList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static void InitializeTheme()
        {
            ThemeList = ResourceService.ThemeList;

            DefaultAppTheme = ThemeList.Find(item => item.Value.ToString().Equals(nameof(ElementTheme.Default), StringComparison.OrdinalIgnoreCase));

            AppTheme = GetTheme();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetTheme()
        {
            object theme = ConfigService.ReadSetting<object>(ThemeSettingsKey);

            if (theme is null)
            {
                SetTheme(DefaultAppTheme);
                return DefaultAppTheme;
            }

            DictionaryEntry selectedTheme = ThemeList.Find(item => item.Value.Equals(theme));

            return selectedTheme.Key is null ? DefaultAppTheme : ThemeList.Find(item => item.Value.Equals(theme));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static void SetTheme(DictionaryEntry theme)
        {
            AppTheme = theme;

            ConfigService.SaveSetting(ThemeSettingsKey, theme.Value);
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
                    Program.ApplicationRoot.MainWindow.WindowTheme = ElementTheme.Light;
                }
                else
                {
                    Program.ApplicationRoot.MainWindow.WindowTheme = ElementTheme.Dark;
                }
            }
            else
            {
                Program.ApplicationRoot.MainWindow.WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.Value.ToString());
            }
        }
    }
}

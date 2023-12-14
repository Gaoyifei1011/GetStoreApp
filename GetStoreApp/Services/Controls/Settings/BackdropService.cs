using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public static class BackdropService
    {
        private static string SettingsKey = ConfigKey.BackdropKey;

        private static DictionaryEntry DefaultAppBackdrop;

        public static DictionaryEntry AppBackdrop { get; private set; }

        public static List<DictionaryEntry> BackdropList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public static void InitializeBackdrop()
        {
            BackdropList = ResourceService.BackdropList;

            DefaultAppBackdrop = BackdropList.Find(item => item.Value.ToString().Equals(nameof(SystemBackdropTheme.Default), StringComparison.OrdinalIgnoreCase));

            AppBackdrop = GetBackdrop();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetBackdrop()
        {
            object backdrop = LocalSettingsService.ReadSetting<object>(SettingsKey);

            if (backdrop is null)
            {
                SetBackdrop(DefaultAppBackdrop);
                return DefaultAppBackdrop;
            }

            DictionaryEntry selectedBackdrop = BackdropList.Find(item => item.Value.Equals(backdrop));

            return selectedBackdrop.Key is null ? DefaultAppBackdrop : selectedBackdrop;
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public static void SetBackdrop(DictionaryEntry backdrop)
        {
            AppBackdrop = backdrop;

            LocalSettingsService.SaveSetting(SettingsKey, backdrop.Value);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public static void SetAppBackdrop()
        {
            MainWindow.Current.SetSystemBackdrop(AppBackdrop);
        }
    }
}

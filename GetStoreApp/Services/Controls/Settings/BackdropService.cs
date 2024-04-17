using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public static class BackdropService
    {
        private static string settingsKey = ConfigKey.BackdropKey;
        private static DictionaryEntry defaultAppBackdrop;

        private static DictionaryEntry _appBackdrop;

        public static DictionaryEntry AppBackdrop
        {
            get { return _appBackdrop; }

            private set
            {
                if (!Equals(_appBackdrop, value))
                {
                    _appBackdrop = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(AppBackdrop)));
                }
            }
        }

        public static List<DictionaryEntry> BackdropList { get; private set; }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public static void InitializeBackdrop()
        {
            BackdropList = ResourceService.BackdropList;

            defaultAppBackdrop = BackdropList.Find(item => item.Value.ToString().Equals(nameof(SystemBackdropTheme.Default), StringComparison.OrdinalIgnoreCase));

            AppBackdrop = GetBackdrop();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetBackdrop()
        {
            object backdrop = LocalSettingsService.ReadSetting<object>(settingsKey);

            if (backdrop is null)
            {
                SetBackdrop(defaultAppBackdrop);
                return defaultAppBackdrop;
            }

            DictionaryEntry selectedBackdrop = BackdropList.Find(item => item.Value.Equals(backdrop));

            return selectedBackdrop.Key is null ? defaultAppBackdrop : selectedBackdrop;
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public static void SetBackdrop(DictionaryEntry backdrop)
        {
            AppBackdrop = backdrop;

            LocalSettingsService.SaveSetting(settingsKey, backdrop.Value);
        }
    }
}

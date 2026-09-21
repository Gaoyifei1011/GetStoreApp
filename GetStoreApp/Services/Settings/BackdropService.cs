using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    internal static class BackdropService
    {
        private static readonly string settingsKey = ConfigKey.BackdropKey;
        private static string defaultAppBackdrop;

        private static string _appBackdrop;

        internal static string AppBackdrop
        {
            get { return _appBackdrop; }

            private set
            {
                if (!string.Equals(_appBackdrop, value))
                {
                    _appBackdrop = value;
                    PropertyChanged?.Invoke(null, new(nameof(AppBackdrop)));
                }
            }
        }

        internal static ReadOnlyCollection<string> BackdropCollection { get; } = [nameof(SystemBackdropTheme.Default), nameof(MicaKind) + nameof(MicaKind.Base), nameof(MicaKind) + nameof(MicaKind.BaseAlt), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin)];

        internal static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        internal static void InitializeBackdrop()
        {
            foreach (string backdrop in BackdropCollection)
            {
                if (string.Equals(backdrop, nameof(SystemBackdropTheme.Default), StringComparison.OrdinalIgnoreCase))
                {
                    defaultAppBackdrop = backdrop;
                    break;
                }
            }
            AppBackdrop = GetBackdrop();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetBackdrop()
        {
            string backdrop = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                SetBackdrop(defaultAppBackdrop);
                return defaultAppBackdrop;
            }

            string selectedBackdrop = default;
            foreach (string backdropItem in BackdropCollection)
            {
                if (string.Equals(backdropItem, backdrop, StringComparison.OrdinalIgnoreCase))
                {
                    selectedBackdrop = backdropItem;
                    break;
                }
            }
            return string.IsNullOrEmpty(selectedBackdrop) ? defaultAppBackdrop : selectedBackdrop;
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        internal static void SetBackdrop(string backdrop)
        {
            AppBackdrop = backdrop;
            LocalSettingsService.SaveSetting(settingsKey, backdrop);
        }
    }
}

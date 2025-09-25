using GetStoreAppWebView.Extensions.DataType.Constant;
using GetStoreAppWebView.Services.Root;
using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections.Generic;

namespace GetStoreAppWebView.Services.Settings
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public static class BackdropService
    {
        private static readonly string settingsKey = ConfigKey.BackdropKey;
        private static string defaultAppBackdrop;

        public static string AppBackdrop { get; set; }

        public static List<string> BackdropList { get; } = [nameof(SystemBackdropTheme.Default), nameof(MicaKind) + nameof(MicaKind.Base), nameof(MicaKind) + nameof(MicaKind.BaseAlt), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Default), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Base), nameof(DesktopAcrylicKind) + nameof(DesktopAcrylicKind.Thin)];

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public static void InitializeBackdrop()
        {
            defaultAppBackdrop = BackdropList.Find(item => string.Equals(item, nameof(SystemBackdropTheme.Default), StringComparison.OrdinalIgnoreCase));
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
                return defaultAppBackdrop;
            }

            string selectedBackdrop = BackdropList.Find(item => string.Equals(item, backdrop, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(selectedBackdrop) ? defaultAppBackdrop : selectedBackdrop;
        }
    }
}

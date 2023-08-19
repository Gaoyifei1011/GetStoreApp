using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.UI.Composition.SystemBackdrops;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用背景色设置服务
    /// </summary>
    public static class BackdropService
    {
        private static string SettingsKey { get; } = ConfigKey.BackdropKey;

        private static GroupOptionsModel DefaultAppBackdrop { get; set; }

        public static GroupOptionsModel AppBackdrop { get; set; }

        public static List<GroupOptionsModel> BackdropList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public static void InitializeBackdrop()
        {
            BackdropList = ResourceService.BackdropList;

            DefaultAppBackdrop = BackdropList.Find(item => item.SelectedValue.Equals(nameof(SystemBackdropTheme.Default), StringComparison.OrdinalIgnoreCase));

            AppBackdrop = GetBackdrop();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetBackdrop()
        {
            string backdrop = ConfigService.ReadSetting<string>(SettingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                return DefaultAppBackdrop;
            }

            GroupOptionsModel selectedBackdrop = BackdropList.Find(item => item.SelectedValue.Equals(backdrop, StringComparison.OrdinalIgnoreCase));

            return selectedBackdrop is null ? DefaultAppBackdrop : selectedBackdrop;
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public static void SetBackdrop(GroupOptionsModel backdrop)
        {
            AppBackdrop = backdrop;

            ConfigService.SaveSetting(SettingsKey, backdrop.SelectedValue);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public static void SetAppBackdrop()
        {
            Program.ApplicationRoot.MainWindow.SetSystemBackdrop(AppBackdrop);
        }
    }
}

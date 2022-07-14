﻿using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    public class BackdropService : IBackdropService
    {
        private readonly IConfigService ConfigService;
        private readonly IResourceService ResourceService;

        private const string SettingsKey = "AppBackdrop";

        private static string DefaultAppBackdrop { get; } = "Default";

        public string AppBackdrop { get; set; }

        public List<BackdropModel> BackdropList { get; set; }

        public BackdropService(IConfigService configService, IResourceService resourceService)
        {
            ConfigService = configService;
            ResourceService = resourceService;

            BackdropList = ResourceService.BackdropList;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的背景色值
        /// </summary>
        public async Task InitializeBackdropAsync()
        {
            AppBackdrop = await GetBackdropAsync();
        }

        /// <summary>
        /// 获取设置存储的背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<string> GetBackdropAsync()
        {
            string backdrop = await ConfigService.GetSettingStringValueAsync(SettingsKey);

            if (string.IsNullOrEmpty(backdrop))
            {
                return BackdropList.Find(item => item.InternalName.Equals(DefaultAppBackdrop, StringComparison.OrdinalIgnoreCase)).InternalName;
            }

            return BackdropList.Find(item => item.InternalName.Equals(backdrop, StringComparison.OrdinalIgnoreCase)).InternalName;
        }

        /// <summary>
        /// 应用背景色发生修改时修改设置存储的背景色值
        /// </summary>
        public async Task SetBackdropAsync(string backdrop)
        {
            AppBackdrop = backdrop;

            await ConfigService.SaveSettingStringValueAsync(SettingsKey, backdrop);
        }

        /// <summary>
        /// 设置应用显示的背景色
        /// </summary>
        public async Task SetAppBackdropAsync(string appTheme, string appBackdrop)
        {
            BackdropHelper.SetBackdrop(appTheme, appBackdrop);

            await Task.CompletedTask;
        }
    }
}

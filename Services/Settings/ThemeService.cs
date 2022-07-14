using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    public class ThemeService : IThemeService
    {
        private readonly IConfigService ConfigService;
        private readonly IResourceService ResourceService;

        private const string SettingsKey = "AppTheme";

        private string DefaultAppTheme { get; } = Convert.ToString(ElementTheme.Default);

        public string AppTheme { get; set; }

        public List<ThemeModel> ThemeList { get; set; }

        public ThemeService(IConfigService configService, IResourceService resourceService)
        {
            ConfigService = configService;
            ResourceService = resourceService;

            ThemeList = ResourceService.ThemeList;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public async Task InitializeThemeAsync()
        {
            AppTheme = await GetThemeAsync();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<string> GetThemeAsync()
        {
            string theme = await ConfigService.GetSettingStringValueAsync(SettingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                return ThemeList.Find(item => item.InternalName.Equals(DefaultAppTheme, StringComparison.OrdinalIgnoreCase)).InternalName;
            }

            return ThemeList.Find(item => item.InternalName.Equals(theme, StringComparison.OrdinalIgnoreCase)).InternalName;
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public async Task SetThemeAsync(string theme)
        {
            AppTheme = theme;

            await ConfigService.SaveSettingStringValueAsync(SettingsKey, theme);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public async Task SetAppThemeAsync()
        {
            if (GetStoreApp.App.MainWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme);
            }
            await Task.CompletedTask;
        }
    }
}

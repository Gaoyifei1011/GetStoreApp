using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public class ThemeService : IThemeService
    {
        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private string SettingsKey { get; init; } = "AppTheme";

        private ThemeModel DefaultAppTheme { get; set; }

        public ThemeModel AppTheme { get; set; }

        public List<ThemeModel> ThemeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public async Task InitializeThemeAsync()
        {
            ThemeList = ResourceService.ThemeList;

            DefaultAppTheme = ThemeList.Find(item => item.InternalName == Convert.ToString(ElementTheme.Default));

            AppTheme = await GetThemeAsync();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<ThemeModel> GetThemeAsync()
        {
            string theme = await ConfigStorageService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                return ThemeList.Find(item => item.InternalName.Equals(DefaultAppTheme.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return ThemeList.Find(item => item.InternalName.Equals(theme, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public async Task SetThemeAsync(ThemeModel theme)
        {
            AppTheme = theme;

            await ConfigStorageService.SaveSettingAsync(SettingsKey, theme.InternalName);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public async Task SetAppThemeAsync()
        {
            if (App.MainWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.InternalName);
                TitleBarHelper.UpdateTitleBar((ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.InternalName));
            }
            await Task.CompletedTask;
        }
    }
}

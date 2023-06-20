using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string ThemeSettingsKey { get; } = ConfigKey.ThemeKey;

        private static string NotifyIconMenuThemeSettingsKey { get; } = ConfigKey.NotifyIconMenuThemeKey;

        private static ThemeModel DefaultAppTheme { get; set; }

        private static NotifyIconMenuThemeModel DefaultNotifyIconMenuTheme { get; set; }

        public static ThemeModel AppTheme { get; set; }

        public static NotifyIconMenuThemeModel NotifyIconMenuTheme { get; set; }

        public static List<ThemeModel> ThemeList { get; set; }

        public static List<NotifyIconMenuThemeModel> NotifyIconMenuThemeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static async Task InitializeAsync()
        {
            ThemeList = ResourceService.ThemeList;

            NotifyIconMenuThemeList = ResourceService.NotifyIconMenuThemeList;

            DefaultAppTheme = ThemeList.Find(item => item.InternalName == Convert.ToString(ElementTheme.Default));

            DefaultNotifyIconMenuTheme = NotifyIconMenuThemeList.Find(item => item.InternalName.Equals("NotifyIconMenuSystemTheme", StringComparison.OrdinalIgnoreCase));

            (bool, ThemeModel) ThemeResult = await GetThemeAsync();

            AppTheme = ThemeResult.Item2;

            if (ThemeResult.Item1)
            {
                await SetThemeAsync(AppTheme, false);
            }

            (bool, NotifyIconMenuThemeModel) NotifyIconMenuThemeResult = await GetNotifyIconMenuThemeAsync();

            NotifyIconMenuTheme = NotifyIconMenuThemeResult.Item2;

            if (NotifyIconMenuThemeResult.Item1)
            {
                await SetNotifyIconMenuThemeAsync(NotifyIconMenuTheme, false);
            }
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<(bool, ThemeModel)> GetThemeAsync()
        {
            string theme = await ConfigService.ReadSettingAsync<string>(ThemeSettingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                return (true, ThemeList.Find(item => item.InternalName.Equals(DefaultAppTheme.InternalName, StringComparison.OrdinalIgnoreCase)));
            }

            return (false, ThemeList.Find(item => item.InternalName.Equals(theme, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 获取设置存储的通知区域右键菜单主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<(bool, NotifyIconMenuThemeModel)> GetNotifyIconMenuThemeAsync()
        {
            string notifyIconMenuTheme = await ConfigService.ReadSettingAsync<string>(NotifyIconMenuThemeSettingsKey);

            if (string.IsNullOrEmpty(notifyIconMenuTheme))
            {
                return (true, NotifyIconMenuThemeList.Find(item => item.InternalName.Equals(DefaultNotifyIconMenuTheme.InternalName, StringComparison.OrdinalIgnoreCase)));
            }

            return (false, NotifyIconMenuThemeList.Find(item => item.InternalName.Equals(notifyIconMenuTheme, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static async Task SetThemeAsync(ThemeModel theme, bool isNotFirstSet = true)
        {
            if (isNotFirstSet)
            {
                AppTheme = theme;
            }

            await ConfigService.SaveSettingAsync(ThemeSettingsKey, theme.InternalName);
        }

        /// <summary>
        /// 通知区域右键菜单主题发生修改时修改设置存储的主题值
        /// </summary>
        public static async Task SetNotifyIconMenuThemeAsync(NotifyIconMenuThemeModel notifyIconMenuTheme, bool isNotFirstSet = true)
        {
            if (isNotFirstSet)
            {
                NotifyIconMenuTheme = notifyIconMenuTheme;
            }

            await ConfigService.SaveSettingAsync(NotifyIconMenuThemeSettingsKey, notifyIconMenuTheme.InternalName);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public static void SetWindowTheme()
        {
            Program.ApplicationRoot.MainWindow.ViewModel.WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.InternalName);
        }

        /// <summary>
        /// 设置任务栏菜单窗口显示的主题
        /// </summary>
        public static void SetTrayWindowTheme()
        {
            if (NotifyIconMenuTheme == NotifyIconMenuThemeList[0])
            {
                Program.ApplicationRoot.TrayMenuWindow.ViewModel.WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.InternalName);
            }
            else
            {
                Program.ApplicationRoot.TrayMenuWindow.ViewModel.WindowTheme = RegistryHelper.GetSystemUsesTheme();
            }
        }
    }
}

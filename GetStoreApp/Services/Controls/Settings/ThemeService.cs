using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string ThemeSettingsKey { get; } = ConfigKey.ThemeKey;

        private static GroupOptionsModel DefaultAppTheme { get; set; }

        public static GroupOptionsModel AppTheme { get; set; }

        public static List<GroupOptionsModel> ThemeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static void InitializeTheme()
        {
            ThemeList = ResourceService.ThemeList;

            DefaultAppTheme = ThemeList.Find(item => item.SelectedValue == nameof(ElementTheme.Default));

            (bool, GroupOptionsModel) ThemeResult = GetTheme();

            AppTheme = ThemeResult.Item2;

            if (ThemeResult.Item1)
            {
                SetTheme(AppTheme, false);
            }
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static (bool, GroupOptionsModel) GetTheme()
        {
            string theme = ConfigService.ReadSetting<string>(ThemeSettingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                return (true, DefaultAppTheme);
            }

            GroupOptionsModel selectedTheme = ThemeList.Find(item => item.SelectedValue.Equals(theme, StringComparison.OrdinalIgnoreCase));

            return selectedTheme is null ? (true, DefaultAppTheme) : (false, ThemeList.Find(item => item.SelectedValue.Equals(theme, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static void SetTheme(GroupOptionsModel theme, bool isNotFirstSet = true)
        {
            if (isNotFirstSet)
            {
                AppTheme = theme;
            }

            ConfigService.SaveSetting(ThemeSettingsKey, theme.SelectedValue);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public static void SetWindowTheme()
        {
            if (AppTheme.SelectedValue == ThemeList[0].SelectedValue)
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
                Program.ApplicationRoot.MainWindow.WindowTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.SelectedValue);
            }
        }
    }
}

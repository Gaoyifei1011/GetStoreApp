using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 文件右键菜单设置服务
    /// </summary>
    public static class ShellMenuService
    {
        private static readonly string settingsKey = ConfigKey.ShellMenuKey;

        private static readonly bool defaultShellMenu = true;

        public static bool ShellMenu { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的文件右键菜单显示值
        /// </summary>
        public static void InitializeShellMenu()
        {
            ShellMenu = GetShellMenu();
        }

        /// <summary>
        /// 获取设置存储的文件右键菜单显示值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetShellMenu()
        {
            bool? shellMenu = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!shellMenu.HasValue)
            {
                SetShellMenu(defaultShellMenu);
                return defaultShellMenu;
            }

            return shellMenu.Value;
        }

        /// <summary>
        /// 文件右键菜单显示值发生修改时修改设置存储的文件右键菜单显示值
        /// </summary>
        public static void SetShellMenu(bool shellMenu)
        {
            ShellMenu = shellMenu;
            LocalSettingsService.SaveSetting(settingsKey, shellMenu);
        }
    }
}

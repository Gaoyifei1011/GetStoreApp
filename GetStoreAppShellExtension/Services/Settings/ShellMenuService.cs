using GetStoreAppShellExtension.Extensions.DataType.Constant;
using GetStoreAppShellExtension.Services.Root;

namespace GetStoreAppShellExtension.Services.Settings
{
    /// <summary>
    /// 文件右键菜单设置服务
    /// </summary>
    public static class ShellMenuService
    {
        private static readonly string settingsKey = ConfigKey.ShellMenuKey;
        private static readonly bool defaultShellMenu = false;

        /// <summary>
        /// 获取设置存储的文件右键菜单显示值，如果设置没有存储，使用默认值
        /// </summary>
        public static bool GetShellMenu()
        {
            bool? shellMenu = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!shellMenu.HasValue)
            {
                return defaultShellMenu;
            }

            return shellMenu.Value;
        }
    }
}

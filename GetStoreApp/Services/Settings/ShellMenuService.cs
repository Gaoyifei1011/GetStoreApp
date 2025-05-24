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

        private static readonly bool defaultShellMenuValue = true;

        public static bool ShellMenuValue { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的文件右键菜单显示值
        /// </summary>
        public static void InitializeShellMenu()
        {
            ShellMenuValue = GetShellMenuValue();
        }

        /// <summary>
        /// 获取设置存储的文件右键菜单显示值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetShellMenuValue()
        {
            bool? shellMenuValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!shellMenuValue.HasValue)
            {
                SetShellMenuValue(defaultShellMenuValue);
                return defaultShellMenuValue;
            }

            return shellMenuValue.Value;
        }

        /// <summary>
        /// 文件右键菜单显示值发生修改时修改设置存储的文件右键菜单显示值
        /// </summary>
        public static void SetShellMenuValue(bool shellMenuValue)
        {
            ShellMenuValue = shellMenuValue;

            LocalSettingsService.SaveSetting(settingsKey, shellMenuValue);
        }
    }
}

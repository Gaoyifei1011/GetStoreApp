using GetStoreAppShellExtension.Extensions.DataType.Constant;
using GetStoreAppShellExtension.Services.Root;

namespace GetStoreAppShellExtension.Services.Controls.Settings
{
    /// <summary>
    /// 文件右键菜单设置服务
    /// </summary>
    public static class FileShellMenuService
    {
        private static readonly string settingsKey = ConfigKey.FileShellMenuKey;

        private static readonly bool defaultFileShellMenuValue = true;

        /// <summary>
        /// 获取设置存储的文件右键菜单显示值，如果设置没有存储，使用默认值
        /// </summary>
        public static bool GetFileShellMenuValue()
        {
            bool? fileShellMenuValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!fileShellMenuValue.HasValue)
            {
                return defaultFileShellMenuValue;
            }

            return fileShellMenuValue.Value;
        }
    }
}

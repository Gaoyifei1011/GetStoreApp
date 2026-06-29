using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用安装选项
    /// </summary>
    public static class AppInstallService
    {
        private static readonly string allowUnsignedPackageKey = ConfigKey.AllowUnsignedPackageKey;
        private static readonly string forceAppShutdownKey = ConfigKey.ForceAppShutdownKey;
        private static readonly string forceTargetAppShutdownKey = ConfigKey.ForceTargetAppShutdownKey;

        private static readonly bool defaultAllowUnsignedPackage = false;
        private static readonly bool defaultForceAppShutdown = false;
        private static readonly bool defaultForceTargetAppShutdown = false;

        public static bool AllowUnsignedPackage { get; private set; }

        public static bool ForceAppShutdown { get; private set; }

        public static bool ForceTargetAppShutdown { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装选项值
        /// </summary>
        public static void InitializeAppInstall()
        {
            AllowUnsignedPackage = GetAllowUnsignedPackage();
            ForceAppShutdown = GetForceAppShutdown();
            ForceTargetAppShutdown = GetForceTargetAppShutdown();
        }

        /// <summary>
        /// 获取设置存储的允许安装未签名的安装包值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAllowUnsignedPackage()
        {
            bool? allowUnsignedPackage = LocalSettingsService.ReadSetting<bool?>(allowUnsignedPackageKey);

            if (!allowUnsignedPackage.HasValue)
            {
                SetAllowUnsignedPackage(defaultAllowUnsignedPackage);
                return defaultAllowUnsignedPackage;
            }

            return allowUnsignedPackage.Value;
        }

        /// <summary>
        /// 获取设置存储的安装应用时强制关闭与包关联的进程的选项值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetForceAppShutdown()
        {
            bool? forceAppShutdown = LocalSettingsService.ReadSetting<bool?>(forceAppShutdownKey);

            if (!forceAppShutdown.HasValue)
            {
                SetForceAppShutdown(defaultForceAppShutdown);
                return defaultForceAppShutdown;
            }

            return forceAppShutdown.Value;
        }

        /// <summary>
        /// 获取设置存储的安装应用时强制关闭与包关联的进程的选项值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetForceTargetAppShutdown()
        {
            bool? forceTargetAppShutdown = LocalSettingsService.ReadSetting<bool?>(forceTargetAppShutdownKey);

            if (!forceTargetAppShutdown.HasValue)
            {
                SetForceTargetAppShutdown(defaultForceTargetAppShutdown);
                return defaultForceTargetAppShutdown;
            }

            return forceTargetAppShutdown.Value;
        }

        /// <summary>
        /// 允许安装未签名的安装包值发生修改时修改设置存储的允许安装未签名的安装包值
        /// </summary>
        public static void SetAllowUnsignedPackage(bool allowUnsignedPackage)
        {
            AllowUnsignedPackage = allowUnsignedPackage;
            LocalSettingsService.SaveSetting(allowUnsignedPackageKey, allowUnsignedPackage);
        }

        /// <summary>
        /// 安装应用时强制关闭与包关联的进程的选项值发生修改时修改设置存储的安装应用时强制关闭与包关联的进程的选项值
        /// </summary>
        public static void SetForceAppShutdown(bool forceAppShutdown)
        {
            ForceAppShutdown = forceAppShutdown;
            LocalSettingsService.SaveSetting(forceAppShutdownKey, forceAppShutdown);
        }

        /// <summary>
        /// 安装应用时强制关闭与包关联的进程的选项值发生修改时修改设置存储的安装应用时强制关闭与包关联的进程的选项值
        /// </summary>
        public static void SetForceTargetAppShutdown(bool forceTargetAppShutdown)
        {
            ForceTargetAppShutdown = forceTargetAppShutdown;

            LocalSettingsService.SaveSetting(forceTargetAppShutdownKey, forceTargetAppShutdown);
        }
    }
}

using GetStoreAppInstaller.Extensions.DataType.Constant;
using GetStoreAppInstaller.Services.Root;

namespace GetStoreAppInstaller.Services.Controls.Settings
{
    /// <summary>
    /// 应用安装选项
    /// </summary>
    public static class AppInstallService
    {
        private static readonly string allowUnsignedPackageKey = ConfigKey.AllowUnsignedPackageKey;
        private static readonly string forceAppShutdownKey = ConfigKey.ForceAppShutdownKey;
        private static readonly string forceTargetAppShutdownKey = ConfigKey.ForceTargetAppShutdownKey;

        private static readonly bool defaultAllowUnsignedPackageValue = false;
        private static readonly bool defaultForceAppShutdownValue = false;
        private static readonly bool defaultForceTargetAppShutdownValue = false;

        public static bool AllowUnsignedPackageValue { get; private set; }

        public static bool ForceAppShutdownValue { get; private set; }

        public static bool ForceTargetAppShutdownValue { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装选项值
        /// </summary>
        public static void InitializeAppInstall()
        {
            AllowUnsignedPackageValue = GetAllowUnsignedPackageValue();
            ForceAppShutdownValue = GetForceAppShutdownValue();
            ForceTargetAppShutdownValue = GetForceTargetAppShutdownValue();
        }

        /// <summary>
        /// 获取设置存储的允许安装未签名的安装包值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAllowUnsignedPackageValue()
        {
            bool? allowUnsignedPackageValue = LocalSettingsService.ReadSetting<bool?>(allowUnsignedPackageKey);

            if (!allowUnsignedPackageValue.HasValue)
            {
                return defaultAllowUnsignedPackageValue;
            }

            return allowUnsignedPackageValue.Value;
        }

        /// <summary>
        /// 获取设置存储的安装应用时强制关闭与包关联的进程的选项值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetForceAppShutdownValue()
        {
            bool? forceAppShutdownValue = LocalSettingsService.ReadSetting<bool?>(forceAppShutdownKey);

            if (!forceAppShutdownValue.HasValue)
            {
                return defaultForceAppShutdownValue;
            }

            return forceAppShutdownValue.Value;
        }

        /// <summary>
        /// 获取设置存储的安装应用时强制关闭与包关联的进程的选项值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetForceTargetAppShutdownValue()
        {
            bool? forceTargetAppShutdownValue = LocalSettingsService.ReadSetting<bool?>(forceTargetAppShutdownKey);

            if (!forceTargetAppShutdownValue.HasValue)
            {
                return defaultForceTargetAppShutdownValue;
            }

            return forceTargetAppShutdownValue.Value;
        }
    }
}

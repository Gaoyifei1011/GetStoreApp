using GetStoreApp.WindowsAPI.PInvoke.Version;
using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.System.Profile;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        private static Version AppVersion = new Version(
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Build,
            Package.Current.Id.Version.Revision
            );

        private static Version SystemVersion;

        /// <summary>
        /// 初始化系统版本信息
        /// </summary>
        public static void InitializeSystemVersion()
        {
            ulong VersionInfo = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);

            SystemVersion = new Version(
                Convert.ToInt32((VersionInfo & 0xFFFF000000000000L) >> 48),
                Convert.ToInt32((VersionInfo & 0xFFFF000000000000L) >> 32),
                Convert.ToInt32((VersionInfo & 0x00000000FFFF0000L) >> 16),
                Convert.ToInt32(VersionInfo & 0x000000000000FFFFL)
                );
        }

        /// <summary>
        /// 获取应用安装根目录
        /// </summary>
        public static string GetAppInstalledLocation()
        {
            return Package.Current.InstalledLocation.Path;
        }

        /// <summary>
        /// 获取应用版本信息
        /// </summary>
        public static Version GetAppVersion()
        {
            return AppVersion;
        }

        /// <summary>
        /// 获取表示应用程序正在运行的设备类型
        /// </summary>
        public static string GetDeviceFamily()
        {
            return AnalyticsInfo.VersionInfo.DeviceFamily;
        }

        /// <summary>
        /// 获取系统版本信息
        /// </summary>
        public static Version GetSystemVersion()
        {
            return SystemVersion;
        }

        /// <summary>
        /// 获取文件的信息
        /// </summary>
        public static VS_FIXEDFILEINFO GetFileInfo(string filePath)
        {
            int versionSize = VersionLibrary.GetFileVersionInfoSize(filePath, out _);
            byte[] versionData = new byte[versionSize];
            VersionLibrary.GetFileVersionInfo(filePath, 0, versionSize, versionData);

            IntPtr pFixedVersionInfo;
            VersionLibrary.VerQueryValue(versionData, "\\", out pFixedVersionInfo, out _);

            return (VS_FIXEDFILEINFO)Marshal.PtrToStructure(pFixedVersionInfo, typeof(VS_FIXEDFILEINFO));
        }
    }
}

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
        // 应用版本信息
        public static Version AppVersion { get; } = new Version(
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Build,
            Package.Current.Id.Version.Revision
            );

        // 系统版本信息
        public static Version SystemVersion { get; } = Environment.OSVersion.Version;

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
        /// 获取文件的信息
        /// </summary>
        public static VS_FIXEDFILEINFO GetFileInfo(string filePath)
        {
            int versionSize = VersionLibrary.GetFileVersionInfoSize(filePath, out _);
            byte[] versionData = new byte[versionSize];
            VersionLibrary.GetFileVersionInfo(filePath, 0, versionSize, versionData);
            VersionLibrary.VerQueryValue(versionData, "\\", out nint pFixedVersionInfo, out _);

            return (VS_FIXEDFILEINFO)Marshal.PtrToStructure(pFixedVersionInfo, typeof(VS_FIXEDFILEINFO));
        }
    }
}

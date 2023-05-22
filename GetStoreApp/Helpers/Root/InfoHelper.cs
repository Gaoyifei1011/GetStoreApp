using GetStoreApp.Extensions.DataType.Struct;
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
        private static AppVersion AppVersion;

        private static SystemVersion SystemVersion;

        /// <summary>
        /// 初始化应用版本信息
        /// </summary>
        public static void InitializeAppVersion()
        {
            AppVersion.MajorVersion = Package.Current.Id.Version.Major;
            AppVersion.MinorVersion = Package.Current.Id.Version.Minor;
            AppVersion.BuildVersion = Package.Current.Id.Version.Build;
            AppVersion.RevisionVersion = Package.Current.Id.Version.Revision;
        }

        /// <summary>
        /// 初始化系统版本信息
        /// </summary>
        public static void InitializeSystemVersion()
        {
            ulong VersionInfo = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);

            SystemVersion.MajorVersion = (VersionInfo & 0xFFFF000000000000L) >> 48;
            SystemVersion.MinorVersion = (VersionInfo & 0x0000FFFF00000000L) >> 32;
            SystemVersion.BuildNumber = (VersionInfo & 0x00000000FFFF0000L) >> 16;
            SystemVersion.BuildRevision = VersionInfo & 0x000000000000FFFFL;
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
        public static AppVersion GetAppVersion()
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
        public static SystemVersion GetSystemVersion()
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

using System;
using Windows.ApplicationModel;
using Windows.Storage;
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
        public static Version SystemVersion { get; }

        // 系统范围文件夹位置
        public static SystemDataPaths SystemDataPath { get; } = SystemDataPaths.GetDefault();

        // 常见用户数据文件夹的完整路径
        public static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();

        // 应用安装根目录
        public static string AppInstalledLocation { get; } = Package.Current.InstalledLocation.Path;

        static InfoHelper()
        {
            string systemVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(systemVersion);
            SystemVersion = new Version((int)((version & 0xFFFF000000000000L) >> 48), (int)((version & 0x0000FFFF00000000L) >> 32), (int)((version & 0x00000000FFFF0000L) >> 16), (int)(version & 0x000000000000FFFFL));
        }
    }
}

using System;
using Windows.ApplicationModel;
using Windows.Storage;

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

        // 系统范围文件夹位置
        public static SystemDataPaths SystemDataPath { get; } = SystemDataPaths.GetDefault();

        // 常见用户数据文件夹的完整路径
        public static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();

        // 应用安装根目录
        public static string AppInstalledLocation { get; } = Package.Current.InstalledLocation.Path;
    }
}

using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.System.Profile;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息
    /// </summary>
    public static class InfoHelper
    {
        private static readonly string SystemVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;

        private static Dictionary<string, ulong> SystemVersionDict { get; } = new Dictionary<string, ulong>();

        private static Dictionary<string, ushort> AppVersionDict { get; } = new Dictionary<string, ushort>();

        static InfoHelper()
        {
            InitializeSystemVersion();

            InitializeAppVersion();
        }

        private static void InitializeSystemVersion()
        {
            ulong VersionInfo = ulong.Parse(SystemVersion);

            ulong MajorVersion = (VersionInfo & 0xFFFF000000000000L) >> 48;
            ulong MinorVersion = (VersionInfo & 0x0000FFFF00000000L) >> 32;
            ulong BuildNumber = (VersionInfo & 0x00000000FFFF0000L) >> 16;
            ulong BuildRevision = VersionInfo & 0x000000000000FFFFL;

            SystemVersionDict.Add(nameof(MajorVersion), MajorVersion);
            SystemVersionDict.Add(nameof(MinorVersion), MinorVersion);
            SystemVersionDict.Add(nameof(BuildNumber), BuildNumber);
            SystemVersionDict.Add(nameof(BuildRevision), BuildRevision);
        }

        private static void InitializeAppVersion()
        {
            ushort MajorVersion = Package.Current.Id.Version.Major;
            ushort MinorVersion = Package.Current.Id.Version.Minor;
            ushort BuildVersion = Package.Current.Id.Version.Build;
            ushort RevisionVersion = Package.Current.Id.Version.Revision;

            AppVersionDict.Add(nameof(MajorVersion), MajorVersion);
            AppVersionDict.Add(nameof(MinorVersion), MinorVersion);
            AppVersionDict.Add(nameof(BuildVersion), BuildVersion);
            AppVersionDict.Add(nameof(RevisionVersion), RevisionVersion);
        }

        /// <summary>
        /// 获取系统版本
        /// </summary>
        public static Dictionary<string, ulong> GetSystemVersion()
        {
            return SystemVersionDict;
        }

        /// <summary>
        /// 获取应用版本
        /// </summary>
        public static Dictionary<string, ushort> GetAppVersion()
        {
            return AppVersionDict;
        }
    }
}

using GetStoreAppHelper.Extensions.DataType.Struct;
using Windows.System.Profile;

namespace GetStoreAppHelper.Helpers.Root
{
    /// <summary>
    /// 系统版本和应用版本信息辅助类
    /// </summary>
    public static class InfoHelper
    {
        private static SystemVersion SystemVersion;

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
        /// 获取系统版本信息
        /// </summary>
        public static SystemVersion GetSystemVersion()
        {
            return SystemVersion;
        }
    }
}

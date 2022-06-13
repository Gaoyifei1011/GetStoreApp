using System.Collections.Generic;
using Windows.System.Profile;

namespace GetStoreApp.Helpers
{
    public class SystemInfoHelper
    {
        private readonly string SystemVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;

        private Dictionary<string, ulong> SystemVersionDict { get; set; } = new Dictionary<string, ulong>();

        public Dictionary<string, ulong> GetSystemVersion()
        {
            ulong VersionInfo = ulong.Parse(SystemVersion);
            ulong MajorVersion = (VersionInfo & 0xFFFF000000000000L) >> 48;
            ulong MinorVersion = (VersionInfo & 0x0000FFFF00000000L) >> 32;
            ulong BuildNumber = (VersionInfo & 0x00000000FFFF0000L) >> 16;
            ulong BuildRevision = (VersionInfo & 0x000000000000FFFFL);

            SystemVersionDict.Add("MajorVersion", MajorVersion);
            SystemVersionDict.Add("MinorVersion", MinorVersion);
            SystemVersionDict.Add("BuildNumber", BuildNumber);
            SystemVersionDict.Add("BuildRevision", BuildRevision);

            return SystemVersionDict;
        }
    }
}

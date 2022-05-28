using System.Runtime.InteropServices;
using System.Text;

namespace GetStoreApp.Helpers
{
    public class SystemGeographicalLocation
    {
        public const int GEOCLASS_NATION = 0x10;

        private SystemGeographicalLocation()
        {
        }

        public delegate bool EnumGeoInfoProc(int GeoId);

        public enum SYSGEOTYPE
        {
            GEO_NATION = 0x0001,
            GEO_LATITUDE = 0x0002,
            GEO_LONGITUDE = 0x0003,
            GEO_ISO2 = 0x0004,
            GEO_ISO3 = 0x0005,
            GEO_RFC1766 = 0x0006,
            GEO_LCID = 0x0007,
            GEO_FRIENDLYNAME = 0x0008,
            GEO_OFFICIALNAME = 0x0009,
            GEO_TIMEZONES = 0x000A,
            GEO_OFFICIALLANGUAGES = 0x000B
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern int GetGeoInfo(int location, SYSGEOTYPE geoType, StringBuilder lpGeoData, int cchData, int langId);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern int EnumSystemGeoID(int geoClass, int parentGeoId, EnumGeoInfoProc lpGeoEnumProc);
    }
}
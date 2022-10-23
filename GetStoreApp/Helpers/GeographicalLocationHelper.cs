using GetStoreApp.Extensions.DataType.Delegate;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Models.Settings;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// Windows 支持的国家/地区列表
    /// </summary>
    public static class GeographicalLocationHelper
    {
        private static readonly List<RegionModel> _geographicalLocations;
        private static readonly List<int> _geoIds;
        private static readonly EnumGeoInfoProc _callback;
        private static readonly int _lcid;
        private static readonly int GEOCLASS_NATION = 0x10;

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int GetGeoInfo(int location, SYSGEOTYPE geoType, StringBuilder lpGeoData, int cchData, int langId);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int EnumSystemGeoID(int geoClass, int parentGeoId, EnumGeoInfoProc lpGeoEnumProc);

        static GeographicalLocationHelper()
        {
            _geographicalLocations = new List<RegionModel>();
            _geoIds = new List<int>();
            _callback = EnumGeoInfoCallback;
            _lcid = CultureInfo.CurrentCulture.LCID;
        }

        public static IEnumerable<RegionModel> GetGeographicalLocations()
        {
            if (_geographicalLocations.Count == 0)
            {
                EnumSystemGeoID(GEOCLASS_NATION, 0, _callback);

                foreach (int geoId in _geoIds)
                {
                    RegionModel location = new RegionModel
                    {
                        Nation = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_NATION, _lcid),
                        Latitude = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_LATITUDE, _lcid),
                        Longitude = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_LONGITUDE, _lcid),
                        ISO2 = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_ISO2, _lcid),
                        ISO3 = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_ISO3, _lcid),
                        Rfc1766 = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_RFC1766, _lcid),
                        Lcid = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_LCID, _lcid),
                        FriendlyName = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_FRIENDLYNAME, _lcid),
                        OfficialName = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_OFFICIALNAME, _lcid),
                        TimeZones = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_TIMEZONES, _lcid),
                        OfficialLanguages = GetGeoInfoA(geoId, SYSGEOTYPE.GEO_OFFICIALLANGUAGES, _lcid)
                    };

                    _geographicalLocations.Add(location);
                }
            }

            return _geographicalLocations;
        }

        private static string GetGeoInfoA(int location, SYSGEOTYPE geoType, int langId)
        {
            StringBuilder geoDataBuilder = new StringBuilder();

            int bufferSize = GetGeoInfo(location, geoType, geoDataBuilder, 0, langId);

            if (bufferSize > 0)
            {
                geoDataBuilder.Capacity = bufferSize;
                GetGeoInfo(location, geoType, geoDataBuilder, bufferSize, langId);
            }

            return geoDataBuilder.ToString();
        }

        private static bool EnumGeoInfoCallback(int geoId)
        {
            if (geoId != 0)
            {
                _geoIds.Add(geoId);
                return true;
            }

            return false;
        }
    }
}

using GetStoreApp.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// 获取 Windows 支持的国家/地区列表
    /// </summary>
    public static class GeographicalLocationHelper
    {
        private static readonly List<GeographicalLocation> _geographicalLocations;
        private static readonly List<int> _geoIds;
        private static readonly SystemGeographicalLocation.EnumGeoInfoProc _callback;
        private static readonly int _lcid;

        static GeographicalLocationHelper()
        {
            _geographicalLocations = new List<GeographicalLocation>();
            _geoIds = new List<int>();
            _callback = EnumGeoInfoCallback;
            _lcid = CultureInfo.CurrentCulture.LCID;
        }

        public static IEnumerable<GeographicalLocation> GetGeographicalLocations()
        {
            if (_geographicalLocations.Count == 0)
            {
                SystemGeographicalLocation.EnumSystemGeoID(SystemGeographicalLocation.GEOCLASS_NATION, 0, _callback);

                foreach (var geoId in _geoIds)
                {
                    var location = new GeographicalLocation
                    {
                        Nation = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_NATION, _lcid),
                        Latitude = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LATITUDE, _lcid),
                        Longitude = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LONGITUDE, _lcid),
                        ISO2 = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_ISO2, _lcid),
                        ISO3 = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_ISO3, _lcid),
                        Rfc1766 = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_RFC1766, _lcid),
                        Lcid = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LCID, _lcid),
                        FriendlyName = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_FRIENDLYNAME, _lcid),
                        OfficialName = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_OFFICIALNAME, _lcid),
                        TimeZones = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_TIMEZONES, _lcid),
                        OfficialLanguages = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_OFFICIALLANGUAGES, _lcid)
                    };

                    _geographicalLocations.Add(location);
                }
            }

            return _geographicalLocations;
        }

        private static string GetGeoInfoA(int location, SystemGeographicalLocation.SYSGEOTYPE geoType, int langId)
        {
            var geoDataBuilder = new StringBuilder();

            int bufferSize = SystemGeographicalLocation.GetGeoInfo(location, geoType, geoDataBuilder, 0, langId);

            if (bufferSize > 0)
            {
                geoDataBuilder.Capacity = bufferSize;
                SystemGeographicalLocation.GetGeoInfo(location, geoType, geoDataBuilder, bufferSize, langId);
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

using GetStoreApp.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// C# 获取 Windows 支持的国家/地区列表
    /// Get a list of Windows supported countries with C#
    /// </summary>
    public static class GeographicalLocationHelper
    {
        private static List<GeographicalLocationModel> _geographicalLocations;
        private static List<int> _geoIds;
        private static SystemGeographicalLocation.EnumGeoInfoProc _callback;
        private static int _lcid;

        static GeographicalLocationHelper()
        {
            _geographicalLocations = new List<GeographicalLocationModel>();
            _geoIds = new List<int>();
            _callback = EnumGeoInfoCallback;
            _lcid = CultureInfo.CurrentCulture.LCID;
        }

        public static IEnumerable<GeographicalLocationModel> GetGeographicalLocations()
        {
            if (_geographicalLocations.Count == 0)
            {
                SystemGeographicalLocation.EnumSystemGeoID(SystemGeographicalLocation.GEOCLASS_NATION, 0, _callback);

                foreach (var geoId in _geoIds)
                {
                    var location = new GeographicalLocationModel();

                    location.Nation = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_NATION, _lcid);
                    location.Latitude = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LATITUDE, _lcid);
                    location.Longitude = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LONGITUDE, _lcid);
                    location.ISO2 = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_ISO2, _lcid);
                    location.ISO3 = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_ISO3, _lcid);
                    location.Rfc1766 = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_RFC1766, _lcid);
                    location.Lcid = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LCID, _lcid);
                    location.FriendlyName = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_FRIENDLYNAME, _lcid);
                    location.OfficialName = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_OFFICIALNAME, _lcid);
                    location.TimeZones = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_TIMEZONES, _lcid);
                    location.OfficialLanguages = GetGeoInfoA(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_OFFICIALLANGUAGES, _lcid);

                    _geographicalLocations.Add(location);
                }
            }

            return _geographicalLocations;
        }

        private static string GetGeoInfoA(int location, SystemGeographicalLocation.SYSGEOTYPE geoType, int langId)
        {
            var geoDataBuilder = new StringBuilder();
            int bufferSize = 0;

            bufferSize = SystemGeographicalLocation.GetGeoInfo(location, geoType, geoDataBuilder, 0, langId);

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
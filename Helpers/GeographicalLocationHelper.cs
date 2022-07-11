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
        private static readonly List<RegionModel> _geographicalLocations;
        private static readonly List<int> _geoIds;
        private static readonly SystemGeographicalLocationHelper.EnumGeoInfoProc _callback;
        private static readonly int _lcid;

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
                SystemGeographicalLocationHelper.EnumSystemGeoID(SystemGeographicalLocationHelper.GEOCLASS_NATION, 0, _callback);

                foreach (var geoId in _geoIds)
                {
                    RegionModel location = new RegionModel
                    {
                        Nation = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_NATION, _lcid),
                        Latitude = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_LATITUDE, _lcid),
                        Longitude = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_LONGITUDE, _lcid),
                        ISO2 = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_ISO2, _lcid),
                        ISO3 = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_ISO3, _lcid),
                        Rfc1766 = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_RFC1766, _lcid),
                        Lcid = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_LCID, _lcid),
                        FriendlyName = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_FRIENDLYNAME, _lcid),
                        OfficialName = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_OFFICIALNAME, _lcid),
                        TimeZones = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_TIMEZONES, _lcid),
                        OfficialLanguages = GetGeoInfoA(geoId, SystemGeographicalLocationHelper.SYSGEOTYPE.GEO_OFFICIALLANGUAGES, _lcid)
                    };

                    _geographicalLocations.Add(location);
                }
            }

            return _geographicalLocations;
        }

        private static string GetGeoInfoA(int location, SystemGeographicalLocationHelper.SYSGEOTYPE geoType, int langId)
        {
            var geoDataBuilder = new StringBuilder();

            int bufferSize = SystemGeographicalLocationHelper.GetGeoInfo(location, geoType, geoDataBuilder, 0, langId);

            if (bufferSize > 0)
            {
                geoDataBuilder.Capacity = bufferSize;
                SystemGeographicalLocationHelper.GetGeoInfo(location, geoType, geoDataBuilder, bufferSize, langId);
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

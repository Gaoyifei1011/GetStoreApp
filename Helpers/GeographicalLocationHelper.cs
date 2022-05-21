using GetStoreApp.Core.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GetStoreApp.Helpers
{
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

                    location.Nation = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_NATION, _lcid);
                    location.Latitude = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LATITUDE, _lcid);
                    location.Longitude = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LONGITUDE, _lcid);
                    location.ISO2 = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_ISO2, _lcid);
                    location.ISO3 = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_ISO3, _lcid);
                    location.Rfc1766 = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_RFC1766, _lcid);
                    location.Lcid = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_LCID, _lcid);
                    location.FriendlyName = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_FRIENDLYNAME, _lcid);
                    location.OfficialName = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_OFFICIALNAME, _lcid);
                    location.TimeZones = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_TIMEZONES, _lcid);
                    location.OfficialLanguages = GetGeoInfo(geoId, SystemGeographicalLocation.SYSGEOTYPE.GEO_OFFICIALLANGUAGES, _lcid);

                    _geographicalLocations.Add(location);
                }
            }

            return _geographicalLocations;
        }

        private static string GetGeoInfo(int location, SystemGeographicalLocation.SYSGEOTYPE geoType, int langId)
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

using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// Windows 支持的国家/地区列表
    /// </summary>
    public static class GeographicalLocationHelper
    {
        private static readonly List<RegionModel> _geographicalLocations = new List<RegionModel>();
        private static readonly List<int> _geoIds = new List<int>();
        private static readonly EnumGeoInfoProc _callback;
        private static readonly int _lcid = CultureInfo.CurrentCulture.LCID;
        private static readonly int GEOCLASS_NATION = 0x10;

        static GeographicalLocationHelper()
        {
            _callback = EnumGeoInfoCallback;
        }

        public static IEnumerable<RegionModel> GetGeographicalLocations()
        {
            if (_geographicalLocations.Count == 0)
            {
                Kernel32Library.EnumSystemGeoID(GEOCLASS_NATION, 0, _callback);

                foreach (int geoId in _geoIds)
                {
                    _geographicalLocations.Add(new RegionModel
                    {
                        ISO2 = GetGeoInfo(geoId, SYSGEOTYPE.GEO_ISO2, _lcid),
                        FriendlyName = GetGeoInfo(geoId, SYSGEOTYPE.GEO_FRIENDLYNAME, _lcid)
                    });
                }
            }

            return _geographicalLocations;
        }

        private static string GetGeoInfo(int location, SYSGEOTYPE geoType, int langId)
        {
            StringBuilder geoDataBuilder = new StringBuilder();

            int bufferSize = Kernel32Library.GetGeoInfo(location, geoType, geoDataBuilder, 0, langId);

            if (bufferSize > 0)
            {
                geoDataBuilder.Capacity = bufferSize;
                Kernel32Library.GetGeoInfo(location, geoType, geoDataBuilder, bufferSize, langId);
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

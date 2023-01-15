using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GetStoreApp.Helpers.Root
{
    /// <summary>
    /// Windows 支持的国家 / 地区列表辅助类
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

        /// <summary>
        /// 获取系统存储的地理位置信息列表
        /// </summary>
        public static IEnumerable<RegionModel> GetGeographicalLocations()
        {
            if (_geographicalLocations.Count is 0)
            {
                Kernel32Library.EnumSystemGeoID(GEOCLASS_NATION, 0, _callback);
                _geographicalLocations.AddRange(_geoIds.Select(geoId => new RegionModel
                {
                    ISO2 = GetGeoInfo(geoId, SYSGEOTYPE.GEO_ISO2, _lcid),
                    FriendlyName = GetGeoInfo(geoId, SYSGEOTYPE.GEO_FRIENDLYNAME, _lcid)
                }));
            }

            return _geographicalLocations;
        }

        /// <summary>
        /// 获取系统存储的地理位置信息列表
        /// </summary>
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

        /// <summary>
        /// 回调函数
        /// </summary>
        private static bool EnumGeoInfoCallback(int geoId)
        {
            if (geoId is not 0)
            {
                _geoIds.Add(geoId);
                return true;
            }

            return false;
        }
    }
}

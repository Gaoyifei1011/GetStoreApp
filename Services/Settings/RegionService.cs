using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class RegionService
    {
        private const string SettingsKey = "AppSelectedRegion";

        private static string CurrSysRegionCodeName = RegionInfo.CurrentRegion.TwoLetterISORegionName;

        private static readonly string DefaultRegionCodeName;

        public static string RegionCodeName { get; set; }

        /// <summary>
        /// Windows 系统中包含的区域信息
        /// </summary>
        public static List<GeographicalLocation> AppGlobalLocations = GeographicalLocationHelper.GetGeographicalLocations().OrderBy(item => item.FriendlyName).ToList();

        static RegionService()
        {
            DefaultRegionCodeName = AppGlobalLocations.Find(item => item.ISO2 == CurrSysRegionCodeName).ISO2;

            // 获取设置中存储的区域编码
            RegionCodeName = GetRegion();
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// </summary>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 设置应用展示的区域值
        /// </summary>
        private static void InitializeSettingsKey(string regionCodeName)
        {
            SetRegion(regionCodeName);
        }

        /// <summary>
        /// 获取设置存储的区域编码
        /// </summary>
        private static string GetRegion()
        {
            // 检测存储的键值是否为空,如果不存在,设置默认值
            if (IsSettingsKeyNullOrEmpty())
            {
                InitializeSettingsKey(DefaultRegionCodeName);
            }

            return ApplicationData.Current.LocalSettings.Values[SettingsKey].ToString();
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        public static void SetRegion(string regionCodeName)
        {
            RegionCodeName = regionCodeName;
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = regionCodeName;
        }
    }
}

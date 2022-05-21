using GetStoreApp.Core.Models;
using GetStoreApp.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class RegionSelectorService
    {
        private const string SettingsKey = "AppSelectedRegion";

        private static string CurrSysRegionCodeName = RegionInfo.CurrentRegion.TwoLetterISORegionName;

        // 根据当前地理位置决定
        private static readonly string DefaultRegionCodeName;

        // 应用设定的区域值
        public static string RegionCodeName;

        // 系统中包含的区域信息
        public static List<GeographicalLocationModel> AppGlobalLocations = GeographicalLocationHelper.GetGeographicalLocations().OrderBy(item => item.FriendlyName).ToList();

        /// <summary>
        /// 静态资源初始化
        /// </summary>
        static RegionSelectorService()
        {
            // 默认区域编码值
            DefaultRegionCodeName = AppGlobalLocations.Find(item => item.ISO2 == CurrSysRegionCodeName).ISO2;

            RegionCodeName = GetRegion();
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// When the application is initialized, the system information about the key value storage is empty, so you need to determine whether the system stored key value is empty
        /// </summary>
        /// <returns>键值存储的信息是否为空</returns>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 设置应用展示的区域值
        /// Detects whether the stored key value is empty, and if it does not exist, sets the default value
        /// </summary>
        /// <param name="RegionCodeName">选定的区域编码值</param>
        private static void InitializeSettingsKey(string RegionCodeName)
        {
            SetRegion(RegionCodeName);
        }

        /// <summary>
        /// 获取设置存储的区域编码
        /// Gets the region encoding for the settings store
        /// </summary>
        /// <returns>返回存储的区域编码信息</returns>
        private static string GetRegion()
        {
            // 检测存储的键值是否为空,如果不存在,设置默认值
            if (IsSettingsKeyNullOrEmpty())
            {
                InitializeSettingsKey(DefaultRegionCodeName);
            }

            // 获取设置存储的区域编码
            return LoadRegionFromSettings();
        }

        /// <summary>
        /// 获取设置存储的区域编码
        /// Gets the region encoding for the settings store
        /// </summary>
        /// <returns>设置存储的区域编码</returns>
        private static string LoadRegionFromSettings()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey].ToString();
        }

        /// <summary>
        /// 设置存储的区域编码
        /// Sets the region that the current app displays and sets the region encoding stored
        /// </summary>
        /// <param name="regionCodeName">传入的区域编码值</param>
        public static void SetRegion(string regionCodeName)
        {
            RegionCodeName = regionCodeName;
            // 修改设置存储的值
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = regionCodeName;
        }
    }
}

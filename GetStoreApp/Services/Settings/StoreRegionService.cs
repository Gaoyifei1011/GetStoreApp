using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Globalization;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 商店区域设置服务
    /// </summary>
    internal static class StoreRegionService
    {
        private static readonly string useSystemRegionKey = ConfigKey.UseSystemRegionKey;
        private static readonly string storeRegionKey = ConfigKey.StoreRegionKey;
        private static GEO_ENUMNAMEPROC enumNameProc;
        private static readonly bool defaultUseSystemRegion = true;

        internal static bool UseSystemRegion { get; private set; }

        internal static GeographicRegion DefaultStoreRegion { get; private set; }

        internal static GeographicRegion StoreRegion { get; private set; }

        internal static List<GeographicRegion> StoreRegionList { get; } = [];

        internal static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的区域值，如果设置值为空，设定默认的应用区域值
        /// </summary>
        internal static void InitializeStoreRegion()
        {
            InitializeStoreRegionList();
            GeographicRegion systemRegion = new();
            DefaultStoreRegion = StoreRegionList.Find(item => string.Equals(item.CodeTwoLetter, systemRegion.CodeTwoLetter, StringComparison.OrdinalIgnoreCase));
            UseSystemRegion = GetUseSystemRegion();
            StoreRegion = GetRegion();
        }

        /// <summary>
        /// 当系统默认区域发生改变时，更新默认区域
        /// </summary>
        internal static void UpdateDefaultRegion()
        {
            GeographicRegion systemRegion = new();

            if (!string.Equals(systemRegion.CodeTwoLetter, DefaultStoreRegion.CodeTwoLetter))
            {
                DefaultStoreRegion = StoreRegionList.Find(item => string.Equals(item.CodeTwoLetter, systemRegion.CodeTwoLetter, StringComparison.OrdinalIgnoreCase));

                if (UseSystemRegion)
                {
                    StoreRegion = DefaultStoreRegion;
                }

                PropertyChanged?.Invoke(null, new(nameof(StoreRegion)));
            }
        }

        /// <summary>
        /// 初始化应用区域信息列表
        /// </summary>
        private static void InitializeStoreRegionList()
        {
            enumNameProc = new(EnumNameProc);
            Kernel32Library.EnumSystemGeoNames(SYSGEOCLASS.GEOCLASS_NATION, enumNameProc, nint.Zero);
            StoreRegionList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
        }

        /// <summary>
        /// 获取设置存储的使用系统区域值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetUseSystemRegion()
        {
            bool? useSystemRegion = LocalSettingsService.ReadSetting<bool?>(useSystemRegionKey);

            if (!useSystemRegion.HasValue)
            {
                SetUseSystemRegion(defaultUseSystemRegion);
                return defaultUseSystemRegion;
            }

            return useSystemRegion.Value;
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static GeographicRegion GetRegion()
        {
            string storeRegion = LocalSettingsService.ReadSetting<string>(storeRegionKey);

            if (string.IsNullOrEmpty(storeRegion))
            {
                SetRegion(DefaultStoreRegion);
                return DefaultStoreRegion;
            }

            GeographicRegion selectedRegion = StoreRegionList.Find(item => string.Equals(item.CodeTwoLetter, storeRegion, StringComparison.OrdinalIgnoreCase));

            if (UseSystemRegion)
            {
                SetRegion(DefaultStoreRegion);
                return DefaultStoreRegion;
            }
            else
            {
                return selectedRegion is null ? DefaultStoreRegion : selectedRegion;
            }
        }

        /// <summary>
        /// 使用系统区域值发生修改时修改设置存储的始终显示背景色值
        /// </summary>
        internal static void SetUseSystemRegion(bool useSystemRegion)
        {
            UseSystemRegion = useSystemRegion;

            LocalSettingsService.SaveSetting(useSystemRegionKey, useSystemRegion);
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的应用安装方式值
        /// </summary>
        internal static void SetRegion(GeographicRegion region)
        {
            StoreRegion = region;
            LocalSettingsService.SaveSetting(storeRegionKey, region.CodeTwoLetter);
        }

        /// <summary>
        /// 遍历所有的区域
        /// </summary>
        private static bool EnumNameProc(nint unnamedParam1, nint unnamedParam2)
        {
            string region = Marshal.PtrToStringUni(unnamedParam1);
            if (GeographicRegion.IsSupported(region))
            {
                StoreRegionList.Add(new(region));
            }

            return true;
        }
    }
}

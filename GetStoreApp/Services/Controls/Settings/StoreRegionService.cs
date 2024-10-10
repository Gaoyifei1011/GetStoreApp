using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using Windows.Globalization;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 商店区域设置服务
    /// </summary>
    public static class StoreRegionService
    {
        private static readonly string useSystemRegionKey = ConfigKey.UseSystemRegionKey;
        private static readonly string storeRegionKey = ConfigKey.StoreRegionKey;
        private static GEO_ENUMNAMEPROC enumNameProc;

        private static readonly bool defaultUseSystemRegionValue = true;

        public static bool UseSystemRegionValue { get; private set; }

        public static GeographicRegion DefaultStoreRegion { get; private set; }

        public static GeographicRegion StoreRegion { get; private set; }

        public static List<GeographicRegion> StoreRegionList { get; } = [];

        public static event EventHandler ServiceChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的区域值，如果设置值为空，设定默认的应用区域值
        /// </summary>
        public static void InitializeStoreRegion()
        {
            InitializeStoreRegionList();
            GeographicRegion systemRegion = new();
            DefaultStoreRegion = StoreRegionList.Find(item => item.CodeTwoLetter.Equals(systemRegion.CodeTwoLetter, StringComparison.OrdinalIgnoreCase));

            UseSystemRegionValue = GetUseSystemRegionValue();
            StoreRegion = GetRegion();
        }

        /// <summary>
        /// 当系统默认区域发生改变时，更新默认区域
        /// </summary>
        public static void UpdateDefaultRegion()
        {
            GeographicRegion systemRegion = new();

            if (!systemRegion.CodeTwoLetter.Equals(DefaultStoreRegion.CodeTwoLetter))
            {
                DefaultStoreRegion = StoreRegionList.Find(item => item.CodeTwoLetter.Equals(systemRegion.CodeTwoLetter, StringComparison.OrdinalIgnoreCase));

                if (UseSystemRegionValue)
                {
                    StoreRegion = DefaultStoreRegion;
                }

                ServiceChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 初始化应用区域信息列表
        /// </summary>
        private static void InitializeStoreRegionList()
        {
            enumNameProc = new GEO_ENUMNAMEPROC(EnumNameProc);
            Kernel32Library.EnumSystemGeoNames(SYSGEOCLASS.GEOCLASS_NATION, enumNameProc, IntPtr.Zero);
            StoreRegionList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
        }

        /// <summary>
        /// 获取设置存储的使用系统区域值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetUseSystemRegionValue()
        {
            bool? useSystemRegionValue = LocalSettingsService.ReadSetting<bool?>(useSystemRegionKey);

            if (!useSystemRegionValue.HasValue)
            {
                SetUseSystemRegion(defaultUseSystemRegionValue);
                return defaultUseSystemRegionValue;
            }

            return useSystemRegionValue.Value;
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static GeographicRegion GetRegion()
        {
            object storeRegion = LocalSettingsService.ReadSetting<object>(storeRegionKey);

            if (storeRegion is null)
            {
                SetRegion(DefaultStoreRegion);
                return DefaultStoreRegion;
            }

            GeographicRegion selectedRegion = StoreRegionList.Find(item => item.CodeTwoLetter.Equals(storeRegion));

            if (UseSystemRegionValue)
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
        public static void SetUseSystemRegion(bool useSystemRegionValue)
        {
            UseSystemRegionValue = useSystemRegionValue;

            LocalSettingsService.SaveSetting(useSystemRegionKey, useSystemRegionValue);
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的应用安装方式值
        /// </summary>
        public static void SetRegion(GeographicRegion region)
        {
            StoreRegion = region;

            LocalSettingsService.SaveSetting(storeRegionKey, region.CodeTwoLetter);
        }

        /// <summary>
        /// 遍历所有的区域
        /// </summary>
        private static unsafe bool EnumNameProc(IntPtr unmamedParam1, IntPtr unmamedParam2)
        {
            string region = Utf16StringMarshaller.ConvertToManaged((ushort*)unmamedParam1);
            if (GeographicRegion.IsSupported(region))
            {
                StoreRegionList.Add(new(region));
            }

            return true;
        }
    }
}

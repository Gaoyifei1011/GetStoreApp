namespace GetStoreAppWindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 定义 GetGeoInfo 函数中请求的地理位置信息的类型。
    /// </summary>
    public enum SYSGEOTYPE
    {
        /// <summary>
        /// 地理位置标识符 (一个国家的 GEOID) 。 此值存储在长整数中。
        /// </summary>
        GEO_NATION = 0x0001,

        /// <summary>
        /// 位置所在的纬度。 此值存储在浮点数中。
        /// </summary>
        GEO_LATITUDE = 0x0002,

        /// <summary>
        ///
        /// </summary>
        GEO_LONGITUDE = 0x0003,

        /// <summary>
        /// ISO 2 字母国家/地区代码。 此值存储在字符串中。
        /// </summary>
        GEO_ISO2 = 0x0004,

        /// <summary>
        /// ISO 3 字母国家/地区代码。 此值存储在字符串中。
        /// </summary>
        GEO_ISO3 = 0x0005,

        /// <summary>
        /// 字符串的名称，符合 RFC 4646 (从 Windows Vista) 开始，该名称派生自 GetGeoInfo 参数 语言 和 GeoId。
        /// </summary>
        GEO_RFC1766 = 0x0006,

        /// <summary>
        /// 使用 GetGeoInfo 派生的区域设置标识符。
        /// </summary>
        GEO_LCID = 0x0007,

        /// <summary>
        /// 国家或地区的友好名称。 此值存储在字符串中。
        /// </summary>
        GEO_FRIENDLYNAME = 0x0008,

        /// <summary>
        /// 国家或地区的官方名称。 此值存储在字符串中。
        /// </summary>
        GEO_OFFICIALNAME = 0x0009,

        /// <summary>
        /// 未实现。
        /// </summary>
        GEO_TIMEZONES = 0x000A,

        /// <summary>
        /// 未实现。
        /// </summary>
        GEO_OFFICIALLANGUAGES = 0x000B,

        /// <summary>
        /// 从 Windows 8 开始：ISO 3 位国家/地区代码。 此值存储在字符串中。
        /// </summary>
        GEO_ISO_UN_NUMBER = 0x000C,

        /// <summary>
        /// 从Windows 8开始：国家/地区父区域的地理位置标识符。 此值存储在字符串中。
        /// </summary>
        GEO_PARENT = 0x000D,

        /// <summary>
        /// 从Windows 10版本 1709 开始：用于地理位置电话号码的拨号代码。
        /// </summary>
        GEO_DIALINGCODE = 0x000E,

        /// <summary>
        /// 从 Windows 10 版本 1709 开始：地理位置使用的货币的三字母代码。
        /// </summary>
        GEO_CURRENCYCODE = 0x000F,

        /// <summary>
        /// 从Windows 10版本 1709 开始：地理位置使用的货币的符号。
        /// </summary>
        GEO_CURRENCYSYMBOL = 0x0010,

        /// <summary>
        /// 从Windows 10版本 1709 开始：国际标准化组织 (ISO) 3166-1 代码或数字联合国 (联合国) 系列 M、数字 49 (M.49) 代码。
        /// </summary>
        GEO_NAME = 0x0011,

        /// <summary>
        /// 从 Windows 10 版本 1709 开始：该区域的 Windows 地理位置标识符 (GEOID) 。 提供此值是为了向后兼容。
        /// 不要在新应用程序中使用此值，而是改用 GEO_NAME 。
        /// </summary>
        GEO_ID = 0x0012
    }
}

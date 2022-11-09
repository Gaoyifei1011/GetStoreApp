using System.Runtime.InteropServices;
using System.Text;

namespace GetStoreAppWindowsAPI.PInvoke.Kernel32
{
    public static class DllFunctions
    {
        private const string Kernel32 = "Kernel32.dll";

        /// <summary>枚举操作系统上可用的地理位置标识符 (类型 GEOID) 。</summary>
        /// <param name="geoClass">
        /// 要枚举标识符的地理位置类。 目前仅支持GEOCLASS_NATION。 此类型会导致函数枚举操作系统上国家/地区的所有地理标识符。
        /// </param>
        /// <param name="parentGeoId">保留。 此参数必须为 0。</param>
        /// <param name="lpGeoEnumProc">指向应用程序定义的回调函数 EnumGeoInfoProc 的指针。 EnumSystemGeoID 函数对此回调函数进行重复调用，直到返回 FALSE。</param>
        /// <returns>如果成功，则返回非零值，否则返回 0。</returns>
        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int EnumSystemGeoID(int geoClass, int parentGeoId, EnumGeoInfoProc lpGeoEnumProc);

        /// <summary>检索有关指定地理位置的信息。</summary>
        /// <param name="location">要获取信息的地理位置的标识符。可以通过调用 EnumSystemGeoID 来获取可用值。</param>
        /// <param name="geoType">
        /// 要检索的信息类型。 可能的值由 SYSGEOTYPE 枚举定义。 如果 GeoType 的值GEO_LCID，该函数将检索区域设置标识符。
        /// 如果 GeoType 的值GEO_RFC1766，该函数将检索符合 RFC 4646 (Windows Vista) 的字符串名称。
        /// </param>
        /// <param name="lpGeoData">指向此函数检索信息的缓冲区的指针。</param>
        /// <param name="cchData">
        /// 由 lpGeoData 指示的缓冲区的大小。 大小是函数 ANSI 版本的字节数，或 Unicode 版本的单词数。
        /// 如果函数返回所需的缓冲区大小，应用程序可以将此参数设置为 0。
        /// </param>
        /// <param name="langId">
        /// 语言的标识符，与 Location 值一起使用。 应用程序可以将此参数设置为 0，并为 GeoType 指定了GEO_RFC1766或GEO_LCID。
        /// 此设置会导致函数通过调用 GetUserDefaultLangID 检索语言标识符。
        /// </param>
        /// <returns>
        /// 返回 (ANSI) 或单词 (Unicode) 在输出缓冲区中检索的地理位置信息的字节数。
        /// 如果 cchData 设置为 0，则函数返回缓冲区所需的大小。如果函数不成功，则返回 0。
        /// </returns>
        [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetGeoInfo(int location, SYSGEOTYPE geoType, StringBuilder lpGeoData, int cchData, int langId);
    }
}

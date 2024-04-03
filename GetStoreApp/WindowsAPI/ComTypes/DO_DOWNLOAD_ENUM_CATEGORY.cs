using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDOManager：：EnumDownloads 使用 DO_DOWNLOAD_ENUM_CATEGORY 结构按特定属性的值筛选下载枚举。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public unsafe struct DO_DOWNLOAD_ENUM_CATEGORY
    {
        /// <summary>
        /// 要用于下载枚举的属性名称。 出于枚举目的，支持这些属性。
        /// DODownloadProperty_Id
        /// DODownloadProperty_Uri
        /// DODownloadProperty_ContentId
        /// DODownloadProperty_DisplayName
        /// DODownloadProperty_LocalPath
        /// </summary>
        public DODownloadProperty Property;

        /// <summary>
        /// 属性的值。
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Value;
    }
}

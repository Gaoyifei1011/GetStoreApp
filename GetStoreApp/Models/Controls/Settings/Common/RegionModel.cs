using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using System.Text;

namespace GetStoreApp.Models.Controls.Settings.Common
{
    /// <summary>
    /// 区域设置数据模型
    /// </summary>
    public class RegionModel
    {
        /// <summary>
        /// 地理位置标识符
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 位置所在的纬度
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// 位置所在的经度
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// ISO 2 字母国家/地区代码
        /// </summary>
        public string ISO2 { get; set; }

        /// <summary>
        /// ISO 3 字母国家/地区代码
        /// </summary>
        public string ISO3 { get; set; }

        /// <summary>
        /// 字符串的名称，符合 RFC 4646 (从 Windows Vista) 开始，该名称派生自 GetGeoInfo 参数 语言 和 GeoId。
        /// </summary>
        public string Rfc1766 { get; set; }

        /// <summary>
        /// 使用 <see cref="Kernel32Library.GetGeoInfo" /> 派生的区域设置标识符。
        /// </summary>
        public string Lcid { get; set; }

        /// <summary>
        /// 国家的友好名称
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// 国家的官方名称
        /// </summary>
        public string OfficialName { get; set; }

        /// <summary>
        /// 未实现
        /// </summary>
        public string TimeZones { get; set; }

        /// <summary>
        /// 未实现
        /// </summary>
        public string OfficialLanguages { get; set; }
    }
}

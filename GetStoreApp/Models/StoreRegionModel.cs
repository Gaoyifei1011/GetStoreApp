using Windows.Globalization;
using WinRT;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 区域模型
    /// </summary>
    [GeneratedBindableCustomProperty]
    public sealed partial class StoreRegionModel
    {
        public string DisplayMember { get; set; }

        public string CodeTwoLetter { get; set; }

        public GeographicRegion GeographicRegion { get; set; }
    }
}

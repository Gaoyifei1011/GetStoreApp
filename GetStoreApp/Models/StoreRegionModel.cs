using Windows.Globalization;
using WinRT;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 区域模型
    /// </summary>
    [GeneratedBindableCustomProperty]
    internal sealed partial class StoreRegionModel
    {
        internal string DisplayMember { get; set; }

        internal string CodeTwoLetter { get; set; }

        internal GeographicRegion GeographicRegion { get; set; }
    }
}

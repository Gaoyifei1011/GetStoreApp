using GetStoreApp.Models.Controls.Settings.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Common
{
    public interface IRegionService
    {
        RegionModel AppRegion { get; set; }

        List<RegionModel> RegionList { get; }

        Task InitializeRegionAsync();

        Task SetRegionAsync(RegionModel region);
    }
}

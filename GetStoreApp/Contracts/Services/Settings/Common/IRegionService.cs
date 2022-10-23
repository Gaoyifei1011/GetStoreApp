using GetStoreApp.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings.Common
{
    public interface IRegionService
    {
        RegionModel AppRegion { get; set; }

        List<RegionModel> RegionList { get; }

        Task InitializeRegionAsync();

        Task SetRegionAsync(RegionModel region);
    }
}

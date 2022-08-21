using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IRegionService
    {
        RegionModel AppRegion { get; set; }

        List<RegionModel> RegionList { get; set; }

        Task InitializeRegionAsync();

        Task SetRegionAsync(RegionModel region);
    }
}

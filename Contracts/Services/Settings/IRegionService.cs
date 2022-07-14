using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IRegionService
    {
        string AppRegion { get; set; }

        List<RegionModel> RegionList { get; set; }

        Task InitializeRegionAsync();

        Task SetRegionAsync(string region);
    }
}

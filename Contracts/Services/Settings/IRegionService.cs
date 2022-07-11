using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

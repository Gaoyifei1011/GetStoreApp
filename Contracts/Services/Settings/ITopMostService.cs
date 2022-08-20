using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface ITopMostService
    {
        bool TopMostValue { get; set; }

        Task InitializeTopMostValueAsync();

        Task SetTopMostValueAsync(bool topMostValue);

        Task SetAppTopMostAsync();
    }
}

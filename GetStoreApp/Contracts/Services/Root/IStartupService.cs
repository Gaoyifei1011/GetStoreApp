using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IStartupService
    {
        Dictionary<string, object> StartupArgs { get; }

        Task InitializeStartupAsync();
    }
}

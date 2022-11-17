using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Root
{
    public interface IStartupService
    {
        Dictionary<string, object> StartupArgs { get; }

        Task InitializeStartupAsync();
    }
}

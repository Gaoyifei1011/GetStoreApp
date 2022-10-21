using System.Collections;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IStartupArgsService
    {
        Hashtable StartupArgs { get; }

        Task InitializeStartupArgsAsync();
    }
}

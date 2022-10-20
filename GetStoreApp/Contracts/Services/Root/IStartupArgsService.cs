using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IStartupArgsService
    {
        Hashtable StartupArgs { get; }

        Task InitializeStartupArgsAsync();
    }
}

using GetStoreApp.Extensions.DataType.Enum;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings.Advanced
{
    public interface ITraceCleanupService
    {
        Task<bool> CleanAppTraceAsync(CleanArgs cleanupArgs);
    }
}

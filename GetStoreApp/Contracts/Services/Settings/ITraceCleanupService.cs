using GetStoreApp.Extensions.DataType.Enum;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface ITraceCleanupService
    {
        Task<bool> CleanAppTraceAsync(CleanArgs cleanupArgs);
    }
}

using GetStoreApp.Extensions.DataType.Enum;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Controls.Settings.Advanced
{
    public interface ITraceCleanupService
    {
        Task<bool> CleanAppTraceAsync(CleanArgs cleanupArgs);
    }
}

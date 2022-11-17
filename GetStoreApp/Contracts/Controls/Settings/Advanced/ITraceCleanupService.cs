using GetStoreApp.Extensions.DataType.Enums;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Advanced
{
    public interface ITraceCleanupService
    {
        Task<bool> CleanAppTraceAsync(CleanArgs cleanupArgs);
    }
}

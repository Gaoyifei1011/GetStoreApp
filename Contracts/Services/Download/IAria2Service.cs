using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IAria2Service
    {
        Task InitializeAria2Async();

        Task CloseAria2Async();

        Task<string> AddTaskAsync(string downloadLink,string folderPath ,string fileName);

        Task<bool> ContinueAllAsync();

        Task<bool> PauseAllAsync();

        Task<bool> DeleteSelectedAsync(List<string> GIDList);

        Task<bool> ContinueAsync(string GID);

        Task<bool> PauseAsync(string GID);

        Task<bool> DeleteAsync(string GID);

        Task TellStatusAsync();
    }
}

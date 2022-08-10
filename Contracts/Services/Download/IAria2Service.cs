using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IAria2Service
    {
        Task InitializeAria2Async();

        Task CloseAria2Async();

        Task<string> AddTaskAsync(string downloadLink,string folderPath ,string fileName);

        Task ContinueAllAsync(List<string> GIDList);

        Task PauseAllAsync(List<string> GIDList);

        Task DeleteAsync(List<string> GIDList);

        Task<bool> ContinueDownloadAsync(string GID);

        Task<bool> PauseDownloadAsync(string GID);

        Task<bool> DeleteTaskAsync(string GID);
    }
}

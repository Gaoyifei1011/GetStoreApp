using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IAria2Service
    {
        Task InitializeAria2Async();

        Task CloseAria2Async();

        Task<string> AddUriAsync(string downloadLink, string folderPath, string fileName);

        Task<List<string>> ContinueAllAsync(List<string> GIDList);

        Task<List<string>> PauseAllAsync(List<string> GIDList);

        Task<List<string>> DeleteSelectedAsync(List<string> GIDList);

        Task<string> ContinueAsync(string GID);

        Task<string> PauseAsync(string GID);

        Task<string> DeleteAsync(string GID);

        Task<Tuple<string, string, int, int, int>> TellStatusAsync(string GID);
    }
}

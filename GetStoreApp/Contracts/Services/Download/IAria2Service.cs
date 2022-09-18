using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IAria2Service
    {
        Task InitializeAria2Async();

        Task CloseAria2Async();

        Task<(bool, string)> AddUriAsync(string downloadLink, string folderPath);

        Task<(bool, string)> PauseAsync(string GID);

        Task<(bool, string)> DeleteAsync(string GID);

        Task<(bool, string, long, long, long)> TellStatusAsync(string GID);

        Task<(bool, long)> GetFileSizeAsync(string GID);
    }
}

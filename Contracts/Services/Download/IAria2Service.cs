using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IAria2Service
    {
        Task InitializeAria2Async();

        Task CloseAria2Async();
    }
}

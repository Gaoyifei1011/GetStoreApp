using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface ITopMostService
    {
        bool TopMostValue { get; set; }

        Task InitializeTopMostValueAsync();

        Task SetTopMostValueAsync(bool topMostValue);

        Task SetAppTopMostAsync();
    }
}

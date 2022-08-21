using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IUseInstructionService
    {
        bool UseInsVisValue { get; set; }

        Task InitializeUseInsVisValueAsync();

        Task SetUseInsVisValueAsync(bool useInsVisValue);
    }
}

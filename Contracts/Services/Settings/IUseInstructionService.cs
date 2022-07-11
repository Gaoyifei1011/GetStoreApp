using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IUseInstructionService
    {
        bool UseInsVisValue { get; set; }

        Task InitializeUseInsVIsValueAsync();

        Task SetUseInsVisValueAsync(bool useInsVisValue);
    }
}

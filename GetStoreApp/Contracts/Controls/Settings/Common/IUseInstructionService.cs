using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Common
{
    public interface IUseInstructionService
    {
        bool UseInsVisValue { get; set; }

        Task InitializeUseInsVisValueAsync();

        Task SetUseInsVisValueAsync(bool useInsVisValue);
    }
}

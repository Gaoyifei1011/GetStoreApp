using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}

using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
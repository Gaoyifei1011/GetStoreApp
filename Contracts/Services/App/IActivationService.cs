using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
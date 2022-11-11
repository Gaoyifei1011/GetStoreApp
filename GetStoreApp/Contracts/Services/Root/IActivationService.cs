using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IActivationService
    {
        Task ActivateAsync(LaunchActivatedEventArgs activationArgs);
    }
}

using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Root
{
    public interface IActivationService
    {
        Task ActivateAsync(LaunchActivatedEventArgs activationArgs);
    }
}

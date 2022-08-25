using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}

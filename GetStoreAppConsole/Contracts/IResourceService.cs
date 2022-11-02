using System.Threading.Tasks;

namespace GetStoreAppConsole.Contracts
{
    public interface IResourceService
    {
        Task InitializeResourceAsync(string defaultConsoleLanguage, string currentConsoleLanguage);

        string GetLocalized(string resource);
    }
}

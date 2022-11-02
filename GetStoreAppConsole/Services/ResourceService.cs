using GetStoreAppConsole.Contracts;
using GetStoreAppConsole.Strings;
using System;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;

namespace GetStoreAppConsole.Services
{
    public class ResourceService : IResourceService
    {
        private CultureInfo DefaultConsoleLanguage { get; set; }

        private CultureInfo CurrentConsoleLanguage { get; set; }

        private ResourceManager ResourceManager { get; } = new ResourceManager("GetStoreAppConsole.Strings.Resources", typeof(Resources).Assembly);

        public async Task InitializeResourceAsync(string defaultConsoleLanguage, string currentConsoleLanguage)
        {
            DefaultConsoleLanguage = CultureInfo.GetCultureInfo(defaultConsoleLanguage);
            CurrentConsoleLanguage = CultureInfo.GetCultureInfo(currentConsoleLanguage);

            await Task.CompletedTask;
        }

        /// <summary>
        /// UI字符串本地化
        /// </summary>
        public string GetLocalized(string resource)
        {
            try
            {
                return ResourceManager.GetString(resource, CurrentConsoleLanguage);
            }
            catch (Exception)
            {
                try
                {
                    return ResourceManager.GetString(ResourceManager.GetString(resource, DefaultConsoleLanguage));
                }
                catch (Exception)
                {
                    return resource;
                }
            }
        }
    }
}

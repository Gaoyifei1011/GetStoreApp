using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;

namespace GetStoreAppConsole.Services
{
    public static class ResourceService
    {
        private static string DefaultConsoleLanguage { get; set; }

        private static string CurrentConsoleLanguage { get; set; }

        private static ResourceContext DefaultResourceContext { get; set; } = new ResourceContext();

        private static ResourceContext CurrentResourceContext { get; set; } = new ResourceContext();

        private static ResourceMap ResourceMap { get; } = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

        public static async Task InitializeResourceAsync(string defaultConsoleLanguage, string currentConsoleLanguage)
        {
            DefaultConsoleLanguage = defaultConsoleLanguage;
            CurrentConsoleLanguage = currentConsoleLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultConsoleLanguage;
            CurrentResourceContext.QualifierValues["Language"] = CurrentConsoleLanguage;

            await Task.CompletedTask;
        }

        public static string GetLocalized(string resource)
        {
            try
            {
                return ResourceMap.GetValue(resource, CurrentResourceContext).ValueAsString;
            }
            catch (NullReferenceException)
            {
                try
                {
                    return ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                }
                catch (NullReferenceException)
                {
                    return resource;
                }
            }
        }
    }
}

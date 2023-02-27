using GetStoreAppHelper.Properties;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;

namespace GetStoreAppHelper.Services
{
    public static class ResourceService
    {
        private static bool IsInitialized { get; set; } = false;

        private static string DefaultAppLanguage { get; set; }

        private static string CurrentAppLanguage { get; set; }

        private static ResourceContext DefaultResourceContext { get; set; } = new ResourceContext();

        private static ResourceContext CurrentResourceContext { get; set; } = new ResourceContext();

        private static ResourceMap ResourceMap { get; } = ResourceManager.Current.MainResourceMap;

        public static async Task InitializeResourceAsync(string defaultAppLanguage, string currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage;
            CurrentAppLanguage = currentAppLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage;
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage;

            IsInitialized = true;
            await Task.CompletedTask;
        }

        public static string GetLocalized(string resource)
        {
            if (IsInitialized)
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
            else
            {
                throw new ApplicationException(Resources.ResourcesInitializeFailed);
            }
        }
    }
}

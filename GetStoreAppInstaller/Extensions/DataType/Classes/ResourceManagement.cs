using Microsoft.Windows.ApplicationModel.Resources;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    public class ResourceManagement
    {
        public ResourceMap ResourceMap { get; set; }

        public ResourceContext DefaultResourceContext { get; set; }

        public ResourceContext CurrentResourceContext { get; set; }
    }
}

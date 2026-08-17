using Microsoft.Windows.ApplicationModel.Resources;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    internal class ResourceManagement
    {
        internal ResourceMap ResourceMap { get; set; }

        internal ResourceContext DefaultResourceContext { get; set; }

        internal ResourceContext CurrentResourceContext { get; set; }
    }
}

using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class NetWorkErrorNotification : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public NetWorkErrorNotification()
        {
            this.InitializeComponent();
        }
    }
}

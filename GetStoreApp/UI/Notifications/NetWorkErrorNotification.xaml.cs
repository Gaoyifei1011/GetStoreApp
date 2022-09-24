using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class NetWorkErrorNotification : StackPanel
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public NetWorkErrorNotification()
        {
            InitializeComponent();
        }
    }
}

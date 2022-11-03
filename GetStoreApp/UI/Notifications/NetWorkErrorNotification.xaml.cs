using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class NetWorkErrorNotification : StackPanel
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public NetWorkErrorNotification()
        {
            InitializeComponent();
        }
    }
}

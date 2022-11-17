using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class NotificationControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public NotificationViewModel ViewModel { get; } = ContainerHelper.GetInstance<NotificationViewModel>();

        public NotificationControl()
        {
            InitializeComponent();
        }
    }
}

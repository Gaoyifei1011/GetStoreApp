using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class NotificationControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public NotificationViewModel ViewModel { get; } = IOCHelper.GetService<NotificationViewModel>();

        public NotificationControl()
        {
            InitializeComponent();
        }
    }
}

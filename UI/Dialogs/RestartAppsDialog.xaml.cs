using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class RestartAppsDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public RestartAppsDialog()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            InitializeComponent();
        }
    }
}

using GetStoreApp.Contracts.Services.App;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Dialogs
{
    public sealed partial class DesktopAppsDialog : ContentDialog
    {
        public IResourceService ResourceService { get; }

        public DesktopAppsDialog()
        {
            ResourceService = App.GetService<IResourceService>();
            this.InitializeComponent();
        }
    }
}

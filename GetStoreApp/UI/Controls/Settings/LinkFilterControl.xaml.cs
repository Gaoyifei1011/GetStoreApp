using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class LinkFilterControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public LinkFilterViewModel ViewModel { get; } = IOCHelper.GetService<LinkFilterViewModel>();

        public LinkFilterControl()
        {
            InitializeComponent();
        }
    }
}

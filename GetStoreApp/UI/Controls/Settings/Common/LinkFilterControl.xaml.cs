using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
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

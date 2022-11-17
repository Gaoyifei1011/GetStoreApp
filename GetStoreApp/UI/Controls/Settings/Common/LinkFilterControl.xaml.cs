using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class LinkFilterControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public LinkFilterViewModel ViewModel { get; } = ContainerHelper.GetInstance<LinkFilterViewModel>();

        public LinkFilterControl()
        {
            InitializeComponent();
        }
    }
}

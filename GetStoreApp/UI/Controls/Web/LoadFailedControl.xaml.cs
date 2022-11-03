using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Web;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Web
{
    public sealed partial class LoadFailedControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public LoadFailedViewModel ViewModel { get; } = ContainerHelper.GetInstance<LoadFailedViewModel>();

        public LoadFailedControl()
        {
            InitializeComponent();
        }
    }
}

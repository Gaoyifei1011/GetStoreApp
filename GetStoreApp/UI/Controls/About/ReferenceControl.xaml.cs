using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class ReferenceControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public ReferenceViewModel ViewModel { get; } = ContainerHelper.GetInstance<ReferenceViewModel>();

        public ReferenceControl()
        {
            InitializeComponent();
        }
    }
}

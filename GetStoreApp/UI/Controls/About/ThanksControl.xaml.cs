using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class ThanksControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public ThanksViewModel ViewModel { get; } = ContainerHelper.GetInstance<ThanksViewModel>();

        public ThanksControl()
        {
            InitializeComponent();
        }
    }
}

using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class IntroductionControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IntroductionViewModel ViewModel { get; } = ContainerHelper.GetInstance<IntroductionViewModel>();

        public IntroductionControl()
        {
            InitializeComponent();
        }
    }
}

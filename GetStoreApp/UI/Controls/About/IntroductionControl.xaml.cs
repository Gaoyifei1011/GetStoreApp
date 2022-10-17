using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.About;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.About
{
    public sealed partial class IntroductionControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public IntroductionViewModel ViewModel { get; } = IOCHelper.GetService<IntroductionViewModel>();

        public IntroductionControl()
        {
            InitializeComponent();
        }
    }
}

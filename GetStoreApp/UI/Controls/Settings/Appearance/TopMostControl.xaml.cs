using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Appearance;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Appearance
{
    public sealed partial class TopMostControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public TopMostViewModel ViewModel { get; } = ContainerHelper.GetInstance<TopMostViewModel>();

        public TopMostControl()
        {
            InitializeComponent();
        }
    }
}

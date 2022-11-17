using GetStoreApp.Contracts.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class InstallModeControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public InstallModeViewModel ViewModel { get; } = ContainerHelper.GetInstance<InstallModeViewModel>();

        public InstallModeControl()
        {
            InitializeComponent();
        }
    }
}

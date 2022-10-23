using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class InstallModeControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public InstallModeViewModel ViewModel { get; } = IOCHelper.GetService<InstallModeViewModel>();

        public InstallModeControl()
        {
            InitializeComponent();
        }
    }
}

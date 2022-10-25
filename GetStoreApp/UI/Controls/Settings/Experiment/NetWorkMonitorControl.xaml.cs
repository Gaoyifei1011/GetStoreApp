using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Experiment;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Experiment
{
    public sealed partial class NetWorkMonitorControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public NetWorkMonitorViewModel ViewModel { get; } = IOCHelper.GetService<NetWorkMonitorViewModel>();

        public NetWorkMonitorControl()
        {
            InitializeComponent();
        }
    }
}

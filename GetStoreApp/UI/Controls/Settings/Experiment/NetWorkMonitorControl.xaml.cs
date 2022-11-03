using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Experiment;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Experiment
{
    public sealed partial class NetWorkMonitorControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public NetWorkMonitorViewModel ViewModel { get; } = ContainerHelper.GetInstance<NetWorkMonitorViewModel>();

        public NetWorkMonitorControl()
        {
            InitializeComponent();
        }
    }
}

using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class TraceCleanupControl : UserControl
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public TraceCleanupViewModel ViewModel { get; } = ContainerHelper.GetInstance<TraceCleanupViewModel>();

        public TraceCleanupControl()
        {
            InitializeComponent();
        }
    }
}

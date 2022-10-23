using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings.Advanced;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    public sealed partial class TraceCleanupControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public TraceCleanupViewModel ViewModel { get; } = IOCHelper.GetService<TraceCleanupViewModel>();

        public TraceCleanupControl()
        {
            InitializeComponent();
        }
    }
}

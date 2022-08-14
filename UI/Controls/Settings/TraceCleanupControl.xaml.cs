using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class TraceCleanupControl : UserControl
    {
        public IResourceService ResourceService { get; }

        public TraceCleanupViewModel ViewModel { get; }

        public TraceCleanupControl()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<TraceCleanupViewModel>();
            InitializeComponent();
        }
    }
}

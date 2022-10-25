using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Controls.Settings.Common;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    public sealed partial class DownloadOptionsControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadOptionsViewModel ViewModel { get; } = IOCHelper.GetService<DownloadOptionsViewModel>();

        public DownloadOptionsControl()
        {
            InitializeComponent();
        }
    }
}

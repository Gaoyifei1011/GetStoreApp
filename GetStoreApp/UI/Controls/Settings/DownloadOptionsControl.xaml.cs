using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Settings;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace GetStoreApp.UI.Controls.Settings
{
    public sealed partial class DownloadOptionsControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadOptionsViewModel ViewModel { get; } = IOCHelper.GetService<DownloadOptionsViewModel>();

        public DownloadOptionsControl()
        {
            InitializeComponent();
        }

        public string DownloadFolderPath(StorageFolder folder)
        {
            return folder.Path;
        }
    }
}

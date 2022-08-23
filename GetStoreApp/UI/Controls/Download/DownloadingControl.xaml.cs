using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Download;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    public sealed partial class DownloadingControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadingViewModel ViewModel { get; } = IOCHelper.GetService<DownloadingViewModel>();

        public string OpenFolder => ResourceService.GetLocalized("/Download/OpenFolder");

        public string ContinueDownload => ResourceService.GetLocalized("/Download/Continue");

        public string PauseDownload => ResourceService.GetLocalized("/Download/Pause");

        public string DeleteTask => ResourceService.GetLocalized("/Download/DeleteTask");

        public DownloadingControl()
        {
            InitializeComponent();
        }

        public string LocalizedDownloadingCountInfo(int count)
        {
            if (count == 0)
            {
                return ResourceService.GetLocalized("/Download/DownloadingEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("/Download/DownloadingCountInfo"), count);
            }
        }
    }
}

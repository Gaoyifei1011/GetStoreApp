using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Download;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Download
{
    public sealed partial class DownloadingControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadingViewModel ViewModel { get; } = IOCHelper.GetService<DownloadingViewModel>();

        public string WaitDownload => ResourceService.GetLocalized("/Download/WaitDownload");

        public string PauseToolTip => ResourceService.GetLocalized("/Download/PauseToolTip");

        public string DeleteToolTip => ResourceService.GetLocalized("/Download/DeleteToolTip");

        public DownloadingControl()
        {
            InitializeComponent();
        }

        public string LocalizeDownloadingCountInfo(int count)
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

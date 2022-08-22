using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class DownloadPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadViewModel ViewModel { get; } = IOCHelper.GetService<DownloadViewModel>();

        public string OpenFolder => ResourceService.GetLocalized("/Download/OpenFolder");

        public string ContinueDownload => ResourceService.GetLocalized("/Download/Continue");

        public string PauseDownload => ResourceService.GetLocalized("/Download/Pause");

        public string DeleteTask => ResourceService.GetLocalized("/Download/DeleteTask");

        public DownloadPage()
        {
            InitializeComponent();
        }

        public string LocalizedDownloadCountInfo(int count)
        {
            if (count == 0)
            {
                return ResourceService.GetLocalized("/Download/DownloadEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("/Download/DownloadCountInfo"), count);
            }
        }

        public async void TestDownload()
        {
            IAria2Service Aria2Service = IOCHelper.GetService<IAria2Service>();

            await Aria2Service.AddUriAsync("http://software-download.microsoft.com/download/sg/22000.1.210604-1628.co_release_amd64fre_ADK.iso", "D:\\Downloads", "22000.1.210604-1628.co_release_amd64fre_ADK.iso");
        }
    }
}

using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace GetStoreApp.Views
{
    public sealed partial class DownloadPage : Page
    {
        public IResourceService ResourceService { get; }

        public DownloadViewModel ViewModel { get; }

        public string OpenFolder { get; }

        public string ContinueDownload { get; }

        public string PauseDownload { get; }

        public string DeleteTask { get; }

        public DownloadPage()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<DownloadViewModel>();

            OpenFolder = ResourceService.GetLocalized("/Download/OpenFolder");
            ContinueDownload = ResourceService.GetLocalized("/Download/Continue");
            PauseDownload = ResourceService.GetLocalized("/Download/Pause");
            DeleteTask = ResourceService.GetLocalized("/Download/DeleteTask");

            InitializeComponent();
        }

        public string LocalizedDownloadCountInfo(int count)
        {
            if (count == 0) return ResourceService.GetLocalized("/Download/DownloadEmpty");
            else return string.Format(ResourceService.GetLocalized("/Download/DownloadCountInfo"), count);
        }

        public async Task TestDownload()
        {
            IAria2Service Aria2Service = IOCHelper.GetService<IAria2Service>();

            await Aria2Service.AddUriAsync("http://software-download.microsoft.com/download/sg/22000.1.210604-1628.co_release_amd64fre_ADK.iso", "D:\\Downloads", "22000.1.210604-1628.co_release_amd64fre_ADK.iso");
        }
    }
}

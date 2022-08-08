using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

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
    }
}

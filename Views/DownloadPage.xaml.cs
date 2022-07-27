using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class DownloadPage : Page
    {
        public IResourceService ResourceService { get; }

        public DownloadViewModel ViewModel { get; set; }

        public string OpenFolder { get; set; }

        public string ContinueDownload { get; set; }

        public string PauseDownload { get; set; }

        public string DeleteTask { get; set; }

        public DownloadPage()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<DownloadViewModel>();

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

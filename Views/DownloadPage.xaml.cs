using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class DownloadPage : Page
    {
        public IResourceService ResourceService { get; }

        public DownloadViewModel ViewModel { get; set; }

        public DownloadPage()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<DownloadViewModel>();
            InitializeComponent();
        }

        public string LocalizedDownloadCountInfo(int count)
        {
            if (count == 0) return ResourceService.GetLocalized("/Download/HistoryEmpty");
            else return string.Format(ResourceService.GetLocalized("/Download/DownloadCountInfo"), count);
        }
    }
}

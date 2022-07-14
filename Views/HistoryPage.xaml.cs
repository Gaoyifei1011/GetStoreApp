using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class HistoryPage : Page
    {
        public IResourceService ResourceService { get; }

        public HistoryViewModel ViewModel { get; }

        public HistoryPage()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<HistoryViewModel>();
            InitializeComponent();
        }

        public string LocalizedHistoryCountInfo(int count)
        {
            if (count == 0) return ResourceService.GetLocalized("/History/HistoryEmpty");
            else return string.Format(ResourceService.GetLocalized("/History/HistoryCountInfo"), count);
        }
    }
}

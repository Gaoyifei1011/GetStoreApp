using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class HistoryPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public HistoryViewModel ViewModel { get; } = IOCHelper.GetService<HistoryViewModel>();

        public string Fillin => ResourceService.GetLocalized("/History/Fillin");

        public string FillinToolTip => ResourceService.GetLocalized("/History/FillinToolTip");

        public string Copy => ResourceService.GetLocalized("/History/Copy");

        public string CopyToolTip => ResourceService.GetLocalized("/History/CopyToolTip");

        public HistoryPage()
        {
            InitializeComponent();
        }

        public string LocalizedHistoryCountInfo(int count)
        {
            if (count == 0)
            {
                return ResourceService.GetLocalized("/History/HistoryEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("/History/HistoryCountInfo"), count);
            }
        }
    }
}

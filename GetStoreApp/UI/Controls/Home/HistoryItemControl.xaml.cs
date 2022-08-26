using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Controls.Home;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.UI.Controls.Home
{
    public sealed partial class HistoryItemControl : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public HistoryItemViewModel ViewModel { get; } = IOCHelper.GetService<HistoryItemViewModel>();

        public string Fillin => ResourceService.GetLocalized("/Home/Fillin");

        public string FillinToolTip => ResourceService.GetLocalized("/Home/FillinToolTip");

        public string Copy => ResourceService.GetLocalized("/Home/Copy");

        public string CopyToolTip => ResourceService.GetLocalized("/Home/CopyToolTip");

        public HistoryItemControl()
        {
            InitializeComponent();
        }
    }
}

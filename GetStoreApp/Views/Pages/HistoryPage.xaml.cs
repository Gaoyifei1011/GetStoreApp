using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class HistoryPage : Page
    {
        public string Fillin => ResourceService.GetLocalized("/History/Fillin");

        public string FillinToolTip => ResourceService.GetLocalized("/History/FillinToolTip");

        public string Copy => ResourceService.GetLocalized("/History/Copy");

        public string CopyToolTip => ResourceService.GetLocalized("/History/CopyToolTip");

        public HistoryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.OnNavigatedTo();
        }

        public void HistoryUnloaded(object sender, RoutedEventArgs args)
        {
            Messenger.Default.Unregister(this);
        }

        public string LocalizeHistoryCountInfo(int count)
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

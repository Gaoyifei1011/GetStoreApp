using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class HistoryPage : Page
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public HistoryViewModel ViewModel { get; } = ContainerHelper.GetInstance<HistoryViewModel>();

        public string Fillin => ResourceService.GetLocalized("/History/Fillin");

        public string FillinToolTip => ResourceService.GetLocalized("/History/FillinToolTip");

        public string Copy => ResourceService.GetLocalized("/History/Copy");

        public string CopyToolTip => ResourceService.GetLocalized("/History/CopyToolTip");

        public HistoryPage()
        {
            InitializeComponent();
        }

        public void HistoryUnloaded(object sender, RoutedEventArgs args)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
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

using ABI.System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Controls.Home;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

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

        public void HistoryUnloaded(object sender, RoutedEventArgs args)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
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

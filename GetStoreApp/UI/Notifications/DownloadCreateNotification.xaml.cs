using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class DownloadCreateNotification : StackPanel
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadCreateViewModel ViewModel { get; } = IOCHelper.GetService<DownloadCreateViewModel>();

        public DownloadCreateNotification(object[] notification)
        {
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]));
        }
    }
}

using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class FileInformationCopyNotification : StackPanel
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public FileInformationCopyViewModel ViewModel { get; } = IOCHelper.GetService<FileInformationCopyViewModel>();

        public FileInformationCopyNotification(object[] notification)
        {
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]));
        }
    }
}

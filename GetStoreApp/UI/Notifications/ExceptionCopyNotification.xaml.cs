using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class ExceptionCopyNotification : StackPanel
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ExceptionCopyViewModel ViewModel { get; } = IOCHelper.GetService<ExceptionCopyViewModel>();

        public ExceptionCopyNotification(object[] notification)
        {
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]));
        }
    }
}

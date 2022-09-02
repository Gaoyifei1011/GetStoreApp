using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class ResultIDCopyNotification : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ResultIDCopyViewModel ViewModel { get; } = IOCHelper.GetService<ResultIDCopyViewModel>();

        public ResultIDCopyNotification(object[] notification)
        {
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]));
        }
    }
}

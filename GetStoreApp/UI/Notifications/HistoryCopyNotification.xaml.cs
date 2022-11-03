using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class HistoryCopyNotification : StackPanel
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public HistoryCopyViewModel ViewModel { get; } = ContainerHelper.GetInstance<HistoryCopyViewModel>();

        public object[] Notification { get; }

        public HistoryCopyNotification(object[] notification)
        {
            Notification = notification;
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]), Convert.ToBoolean(notification[1]));
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            if (Notification.Length > 2)
            {
                CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("/Notification/HistorySelectedCopySuccessfully"), Notification[2]);
            }
        }

        public void CopySelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            if (Notification.Length > 2)
            {
                CopySelectedFailed.Text = string.Format(ResourceService.GetLocalized("/Notification/HistorySelectedCopyFailed"), Notification[2]);
            }
        }

        public bool ControlLoad(bool copyState, bool isMultiSelected, int visibilityFlag)
        {
            if (visibilityFlag == 1 && copyState && !isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag == 2 && (!copyState && !isMultiSelected))
            {
                return true;
            }
            else if (visibilityFlag == 3 && (copyState && isMultiSelected))
            {
                return true;
            }
            else if (visibilityFlag == 4 && (!copyState && isMultiSelected))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class ResultLinkCopyNotification : UserControl
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public ResultLinkCopyViewModel ViewModel { get; } = IOCHelper.GetService<ResultLinkCopyViewModel>();

        public object[] Notification { get; }

        public ResultLinkCopyNotification(object[] notification)
        {
            Notification = notification;
            InitializeComponent();
            ViewModel.Initialize(Convert.ToBoolean(notification[0]), Convert.ToBoolean(notification[1]));
        }

        public void CopySelectedSuccessLoaded(object sender, RoutedEventArgs args)
        {
            if (Notification.Length > 2)
            {
                CopySelectedSuccess.Text = string.Format(ResourceService.GetLocalized("/Notification/ResultLinkSelectedCopySuccessfully"), Notification[2]);
            }
        }

        public void CopySelectedFailedLoaded(object sender, RoutedEventArgs args)
        {
            if (Notification.Length > 2)
            {
                CopySelectedFailed.Text = string.Format(ResourceService.GetLocalized("/Notification/ResultLinkSelectedCopyFailed"), Notification[2]);
            }
        }

        public bool ControlLoad(bool copyState, bool isMultiSelected, int visibilityFlag)
        {
            if (visibilityFlag == 1 && copyState && !isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag == 2 && !copyState && !isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag == 3 && copyState && isMultiSelected)
            {
                return true;
            }
            else if (visibilityFlag == 4 && !copyState && isMultiSelected)
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

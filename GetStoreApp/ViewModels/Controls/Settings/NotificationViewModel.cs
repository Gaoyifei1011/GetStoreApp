﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class NotificationViewModel : ObservableRecipient
    {
        private INotificationService NotificationService { get; } = IOCHelper.GetService<INotificationService>();

        private bool _notification;

        public bool Notification
        {
            get { return _notification; }

            set { SetProperty(ref _notification, value); }
        }

        public IAsyncRelayCommand NotificationCommand => new AsyncRelayCommand<bool>(async (param) =>
        {
            await NotificationService.SetNotificationAsync(param);
            Notification = param;
        });

        public NotificationViewModel()
        {
            Notification = NotificationService.AppNotification;
        }
    }
}
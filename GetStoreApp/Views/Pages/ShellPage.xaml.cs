using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class ShellPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public INotificationService NotificationService { get; } = IOCHelper.GetService<INotificationService>();

        public ShellViewModel ViewModel { get; } = IOCHelper.GetService<ShellViewModel>();

        public ShellPage()
        {
            InitializeComponent();
            App.MainWindow.SetTitleBar(AppTitleBar);
            ViewModel.NavigationService.Frame = NavigationFrame;
            ViewModel.NavigationViewService.Initialize(NavigationViewControl);

            ShowInAppNotification();
        }

        private void ShowInAppNotification()
        {
            int NotificationDuration = 2500;

            WeakReferenceMessenger.Default.Register<ShellPage, InAppNotificationMessage>(this, (shellPage, inAppNotificationMessage) =>
            {
                if (NotificationService.AppNotification)
                {
                    switch (inAppNotificationMessage.Value.NotificationArgs)
                    {
                        case InAppNotificationArgs.NetWorkError: ShellNotification.Show(new NetWorkErrorNotification(), NotificationDuration); break;
                        case InAppNotificationArgs.HistoryCopy: ShellNotification.Show(new HistoryCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ResultLinkCopy: ShellNotification.Show(new ResultLinkCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ResultIDCopy: ShellNotification.Show(new ResultIDCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ResultContentCopy: ShellNotification.Show(new ResultContentCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ExceptionCopy: ShellNotification.Show(new ExceptionCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.FileInformationCopy: ShellNotification.Show(new FileInformationCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.DownloadCreate: ShellNotification.Show(new DownloadCreateNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.LanguageSettings: ShellNotification.Show(new LanguageChangeNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        default: break;
                    };
                }
            });
        }

        // 页面被卸载时，关闭消息服务
        private void ShellPageUnloaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}

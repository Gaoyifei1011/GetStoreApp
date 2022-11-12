using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enums;
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
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public INotificationService NotificationService { get; } = ContainerHelper.GetInstance<INotificationService>();

        public ShellViewModel ViewModel { get; } = ContainerHelper.GetInstance<ShellViewModel>();

        public ShellPage()
        {
            InitializeComponent();

            ViewModel.NavigationService.Frame = NavigationFrame;
            ViewModel.NavigationViewService.Initialize(NavigationViewControl);

            ShowInAppNotification();
        }

        // 显示应用内通知
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

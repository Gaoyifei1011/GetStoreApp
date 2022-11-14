using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Window;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WASDKWindow
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public INotificationService NotificationService { get; } = ContainerHelper.GetInstance<INotificationService>();

        public INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public MainWindowViewModel ViewModel { get; } = ContainerHelper.GetInstance<MainWindowViewModel>();

        public MainWindow()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = WindowFrame;

            ShowInAppNotification();
        }

        // 显示应用内通知
        private void ShowInAppNotification()
        {
            int NotificationDuration = 2500;

            WeakReferenceMessenger.Default.Register<MainWindow, InAppNotificationMessage>(this, (mainWindow, inAppNotificationMessage) =>
            {
                if (NotificationService.AppNotification)
                {
                    switch (inAppNotificationMessage.Value.NotificationArgs)
                    {
                        case InAppNotificationArgs.NetWorkError: WindowNotification.Show(new NetWorkErrorNotification(), NotificationDuration); break;
                        case InAppNotificationArgs.HistoryCopy: WindowNotification.Show(new HistoryCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ResultLinkCopy: WindowNotification.Show(new ResultLinkCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ResultIDCopy: WindowNotification.Show(new ResultIDCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ResultContentCopy: WindowNotification.Show(new ResultContentCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.ExceptionCopy: WindowNotification.Show(new ExceptionCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.FileInformationCopy: WindowNotification.Show(new FileInformationCopyNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.DownloadCreate: WindowNotification.Show(new DownloadCreateNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        case InAppNotificationArgs.LanguageSettings: WindowNotification.Show(new LanguageChangeNotification(inAppNotificationMessage.Value.NotificationValue), NotificationDuration); break;
                        default: break;
                    };
                }
            });
        }

        // 窗口被卸载时，关闭消息服务
        private void WindowUnloaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}

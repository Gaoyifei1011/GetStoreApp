using GetStoreApp.Contracts.Services.Root;
using Microsoft.Windows.AppNotifications;
using System.Collections.Specialized;
using System.Web;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public class AppNotificationService : IAppNotificationService
    {
        //private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        ~AppNotificationService()
        {
            Unregister();
        }

        public void Initialize()
        {
            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

            AppNotificationManager.Default.Register();
        }

        public void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            //TODO: Handle notification invocations when your app is already running.

            // // Navigate to a specific page based on the notification arguments.
            // if (ParseArguments(args.Argument)["action"] == "Settings")
            // {
            //    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            //    {
            //        NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
            //    });
            // }

            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                App.MainWindow.ShowMessageDialogAsync("TODO: Handle notification invocations when your app is already running.", "Notification Invoked");

                App.MainWindow.BringToFront();
            });
        }

        public bool Show(string payload)
        {
            AppNotification appNotification = new AppNotification(payload);

            AppNotificationManager.Default.Show(appNotification);

            return appNotification.Id != 0;
        }

        public NameValueCollection ParseArguments(string arguments)
        {
            return HttpUtility.ParseQueryString(arguments);
        }

        public void Unregister()
        {
            AppNotificationManager.Default.Unregister();
        }
    }
}

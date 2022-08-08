using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using Microsoft.Windows.AppNotifications;
using System.Collections.Specialized;
using System.Web;

namespace GetStoreApp.Services.App
{
    public class AppNotificationService : IAppNotificationService
    {
        private readonly INavigationService NavigationService = IOCHelper.GetService<INavigationService>();

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

            GetStoreApp.App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                GetStoreApp.App.MainWindow.ShowMessageDialogAsync("TODO: Handle notification invocations when your app is already running.", "Notification Invoked");

                GetStoreApp.App.MainWindow.BringToFront();
            });
        }

        public bool Show(string payload)
        {
            var appNotification = new AppNotification(payload);

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

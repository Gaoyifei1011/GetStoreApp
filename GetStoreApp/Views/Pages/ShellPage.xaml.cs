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
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class ShellPage : Page
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public IThemeService ThemeService { get; } = ContainerHelper.GetInstance<IThemeService>();

        public INotificationService NotificationService { get; } = ContainerHelper.GetInstance<INotificationService>();

        public ShellViewModel ViewModel { get; } = ContainerHelper.GetInstance<ShellViewModel>();

        private UISettings uiSettings { get; } = new UISettings();

        public ShellPage()
        {
            InitializeComponent();

            ViewModel.NavigationService.Frame = NavigationFrame;
            ViewModel.NavigationViewService.Initialize(NavigationViewControl);

            ShowInAppNotification();

            if (InfoHelper.GetSystemVersion()["BuildNumber"] < 22000)
            {
                FeaturesForWin10();
            }
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

        // 实验性功能
        // 这些功能仅在 Windows 10 下使用，等到WASDK 1.2正式版发布后，支持AcrylicBackdrop背景色时即可移除
        private void FeaturesForWin10()
        {
            uiSettings.ColorValuesChanged += (sender, args) =>
            {
                if (RegistryHelper.GetRegistryAppTheme() == ElementTheme.Light)
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                }
            };

            WeakReferenceMessenger.Default.Register<ShellPage, ThemeChangedMessage>(this, (shellPage, themeChangedMessage) =>
            {
                if (themeChangedMessage.Value.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (RegistryHelper.GetRegistryAppTheme() == ElementTheme.Light)
                    {
                        Background = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                    }
                    else
                    {
                        Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                    }
                }
                else if (themeChangedMessage.Value.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
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

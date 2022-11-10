using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Messages;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using Windows.UI;

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

            if (InfoHelper.GetSystemVersion()["BuildNumber"] < 22000)
            {
                Windows10Legacy();
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
        // 这些功能仅在 Windows 10 下使用，等到WASDK 1.2正式版发布后，支持AcrylicBackdrop背景色，AppWindow自定义标题栏时即可移除
        private void Windows10Legacy()
        {
            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(AppTitleBar);

            if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
            {
                UpdateTitleBar(ElementTheme.Default);
                Background = new SolidColorBrush(Colors.Transparent);
            }
            else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
            {
                UpdateTitleBar(ElementTheme.Light);
                Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                UpdateTitleBar(ElementTheme.Dark);
                Background = new SolidColorBrush(Colors.Black);
            }

            WeakReferenceMessenger.Default.Register<ShellPage, ThemeChangedMessage>(this, (shellPage, themeChangedMessage) =>
            {
                if (themeChangedMessage.Value.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    UpdateTitleBar(ElementTheme.Default);
                    Background = new SolidColorBrush(Colors.Transparent);
                }
                else if (themeChangedMessage.Value.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    UpdateTitleBar(ElementTheme.Light);
                    Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    UpdateTitleBar(ElementTheme.Dark);
                    Background = new SolidColorBrush(Colors.Black);
                }
            });
        }

        // 页面被卸载时，关闭消息服务
        private void ShellPageUnloaded(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        private void UpdateTitleBar(ElementTheme theme)
        {
            const int WAINACTIVE = 0x00;
            const int WAACTIVE = 0x01;
            const int WMACTIVATE = 0x0006;

            [DllImport("user32.dll")]
            static extern IntPtr GetActiveWindow();

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

            if (App.MainWindow.ExtendsContentIntoTitleBar)
            {
                if (theme != ElementTheme.Default)
                {
                    Application.Current.Resources["WindowCaptionForeground"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Colors.White),
                        ElementTheme.Light => new SolidColorBrush(Colors.Black),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionForegroundDisabled"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)),
                        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x66, 0x00, 0x00, 0x00)),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonBackgroundPointerOver"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF)),
                        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00)),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonBackgroundPressed"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)),
                        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x66, 0x00, 0x00, 0x00)),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonStrokePointerOver"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Colors.White),
                        ElementTheme.Light => new SolidColorBrush(Colors.Black),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };

                    Application.Current.Resources["WindowCaptionButtonStrokePressed"] = theme switch
                    {
                        ElementTheme.Dark => new SolidColorBrush(Colors.White),
                        ElementTheme.Light => new SolidColorBrush(Colors.Black),
                        _ => new SolidColorBrush(Colors.Transparent)
                    };
                }

                Application.Current.Resources["WindowCaptionBackground"] = new SolidColorBrush(Colors.Transparent);
                Application.Current.Resources["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);

                IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                if (hwnd == GetActiveWindow())
                {
                    SendMessage(hwnd, WMACTIVATE, WAINACTIVE, IntPtr.Zero);
                    SendMessage(hwnd, WMACTIVATE, WAACTIVE, IntPtr.Zero);
                }
                else
                {
                    SendMessage(hwnd, WMACTIVATE, WAACTIVE, IntPtr.Zero);
                    SendMessage(hwnd, WMACTIVATE, WAINACTIVE, IntPtr.Zero);
                }
            }
        }
    }
}

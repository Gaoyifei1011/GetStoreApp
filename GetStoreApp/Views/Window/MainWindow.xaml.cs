using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using System;
using System.IO;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WASDKWindow
    {
        public WindowsTrayIcon TrayIcon { get; set; } = new WindowsTrayIcon(Path.Combine(AppContext.BaseDirectory, "Assets/Logo/GetStoreApp.ico"), ResourceService.GetLocalized("AppDisplayName"));

        public MainWindow()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = WindowFrame;
            InitializeTrayIcon();

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    TrayIcon.Dispose();
                    Messenger.Default.Unregister(this);
                }
            });
        }

        private void InitializeTrayIcon()
        {
            TrayIcon.InitializeTrayMenu();
            TrayIcon.AddMenuItemText(1, ResourceService.GetLocalized("ShowOrHideWindow"));
            TrayIcon.AddMenuItemText(2, ResourceService.GetLocalized("Settings"));
            TrayIcon.AddMenuItemSeperator();
            TrayIcon.AddMenuItemText(3, ResourceService.GetLocalized("Exit"));

            TrayIcon.DoubleClick = () =>
            {
                DispatcherQueue.TryEnqueue(() => { ViewModel.ShowOrHideWindowCommand.Execute(null); });
            };
            TrayIcon.RightClick = () =>
            {
                DispatcherQueue.TryEnqueue(() => { TrayIcon.ShowContextMenu(); });
            };
            TrayIcon.MenuCommand = (menuid) =>
            {
                switch (menuid)
                {
                    case 1:
                        {
                            DispatcherQueue.TryEnqueue(() => { ViewModel.ShowOrHideWindowCommand.Execute(null); });
                            break;
                        }
                    case 2:
                        {
                            DispatcherQueue.TryEnqueue(() => { ViewModel.SettingsCommand.Execute(null); });
                            break;
                        }
                    case 3:
                        {
                            DispatcherQueue.TryEnqueue(() => { ViewModel.ExitCommand.Execute(null); });
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            };
        }
    }
}

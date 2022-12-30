using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WASDKWindow
    {
        public WindowsTrayIcon TrayIcon { get; set; } = new WindowsTrayIcon(
            Path.Combine(AppContext.BaseDirectory, "Assets/GetStoreApp.ico"),
            ResourceService.GetLocalized("AppDisplayName")
            );

        // 获取当前窗口的实例
        public new static MainWindow Current { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Current = this;
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

        /// <summary>
        /// 获取MainWindow的窗口句柄
        /// </summary>
        public static IntPtr GetMainWindowHandle()
        {
            if (Current is not null)
            {
                return WindowNative.GetWindowHandle(Current);
            }
            else
            {
                throw new ApplicationException(ResourceService.GetLocalized("IsMainWindowInitialized"));
            }
        }

        /// <summary>
        /// 获取MainWindow的XamlRoot
        /// </summary>
        public static XamlRoot GetMainWindowXamlRoot()
        {
            if (Current.Content.XamlRoot is not null)
            {
                return Current.Content.XamlRoot;
            }
            else
            {
                throw new ApplicationException(ResourceService.GetLocalized("IsMainWindowInitialized"));
            }
        }

        /// <summary>
        /// 初始化任务栏图标和右键菜单
        /// </summary>
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

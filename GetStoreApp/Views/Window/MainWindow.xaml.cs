using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WASDKWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = WindowFrame;

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    Program.ApplicationRoot.TrayIcon.Dispose();
                    Messenger.Default.Unregister(this);
                }
            });
        }

        /// <summary>
        /// 获取主窗口的窗口句柄
        /// </summary>
        public IntPtr GetMainWindowHandle()
        {
            IntPtr MainWindowHandle = WindowNative.GetWindowHandle(this);

            return MainWindowHandle != IntPtr.Zero
                ? MainWindowHandle
                : throw new ApplicationException(ResourceService.GetLocalized("Resources/WindowHandleInitializeFailed"));
        }

        /// <summary>
        /// 获取主窗口的XamlRoot
        /// </summary>
        public XamlRoot GetMainWindowXamlRoot()
        {
            return Content.XamlRoot is not null
                ? Content.XamlRoot
                : throw new ApplicationException(ResourceService.GetLocalized("Resources/WindowContentInitializeFailed"));
        }
    }
}

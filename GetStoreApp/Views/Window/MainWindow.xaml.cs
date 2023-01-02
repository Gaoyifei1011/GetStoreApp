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
        // 获取当前窗口的实例
        public new static MainWindow Current { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Current = this;
            NavigationService.NavigationFrame = WindowFrame;

            Messenger.Default.Register<bool>(this, MessageToken.WindowClosed, (windowClosedMessage) =>
            {
                if (windowClosedMessage)
                {
                    App.Current.TrayIcon.Dispose();
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
    }
}

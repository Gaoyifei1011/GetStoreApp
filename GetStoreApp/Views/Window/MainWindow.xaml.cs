using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Properties;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Runtime.InteropServices;
using Windows.Storage.Streams;
using Windows.System;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WinUIWindow
    {
        private WindowProc newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();
            NavigationService.NavigationFrame = WindowFrame;
        }

        /// <summary>
        /// 设置标题栏状态
        /// </summary>
        public void AppTitlebarLoaded(object sender, RoutedEventArgs args)
        {
            AppTitlebar.SetTitlebarState(WindowHelper.IsWindowMaximized);
        }

        /// <summary>
        /// 加载“主页”图标
        /// </summary>
        public async void HomeIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.Home);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            (sender as ImageIcon).Source = image;
        }

        /// <summary>
        /// 加载“历史记录”图标
        /// </summary>
        public async void HistoryIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.History);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            (sender as ImageIcon).Source = image;
        }

        /// <summary>
        /// 加载“下载管理”图标
        /// </summary>
        public async void DownloadIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.Download);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            (sender as ImageIcon).Source = image;
        }

        /// <summary>
        /// 加载“访问网页版”图标
        /// </summary>
        public async void WebIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.Web);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            (sender as ImageIcon).Source = image;
        }

        /// <summary>
        /// 加载“关于”图标
        /// </summary>
        public async void AboutIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.About);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            (sender as ImageIcon).Source = image;
        }

        /// <summary>
        /// 加载“设置”图标
        /// </summary>
        public async void SettingsIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.Settings);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            (sender as ImageIcon).Source = image;
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
                : throw new ApplicationException(ResourceService.GetLocalized("Resources/WindowXamlRootInitializeFailed"));
        }

        /// <summary>
        /// 初始化窗口过程
        /// </summary>
        public void InitializeWindowProc()
        {
            IntPtr MainWindowHandle = GetMainWindowHandle();
            newWndProc = new WindowProc(NewWindowProc);
            oldWndProc = SetWindowLongAuto(MainWindowHandle, WindowLongIndexFlags.GWL_WNDPROC, newWndProc);
        }

        /// <summary>
        /// 设置应用的背景主题色
        /// </summary>
        public void SetSystemBackdrop(string backdropName)
        {
            switch (backdropName)
            {
                case "Mica":
                    {
                        ViewModel.SystemBackdrop = new MicaSystemBackdrop() { Kind = MicaKind.Base };
                        break;
                    }
                case "MicaAlt":
                    {
                        ViewModel.SystemBackdrop = new MicaSystemBackdrop() { Kind = MicaKind.BaseAlt };
                        break;
                    }
                case "Acrylic":
                    {
                        ViewModel.SystemBackdrop = new DesktopAcrylicSystemBackdrop();
                        break;
                    }
                default:
                    {
                        ViewModel.SystemBackdrop = null;
                        break;
                    }
            }
        }

        /// <summary>
        /// 窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口显示模式发生修改时的消息
                case WindowMessage.WM_SIZE:
                    {
                        if ((SizeMode)wParam == SizeMode.SIZE_MAXIMIZED)
                        {
                            AppTitlebar.SetTitlebarState(true);
                        }
                        else if ((SizeMode)wParam == SizeMode.SIZE_RESTORED)
                        {
                            AppTitlebar.SetTitlebarState(false);
                        }
                        break;
                    }
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                        if (MinWidth >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.X = DPICalcHelper.ConvertEpxToPixel(hWnd, MinWidth);
                        }
                        if (MinHeight >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.Y = DPICalcHelper.ConvertEpxToPixel(hWnd, MinHeight);
                        }
                        if (MaxWidth > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.X = DPICalcHelper.ConvertEpxToPixel(hWnd, MaxWidth);
                        }
                        if (MaxHeight > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.Y = DPICalcHelper.ConvertEpxToPixel(hWnd, MaxHeight);
                        }
                        Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        break;
                    }
                // 窗口接收其他数据消息
                case WindowMessage.WM_COPYDATA:
                    {
                        CopyDataStruct copyDataStruct = (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));

                        // 没有任何命令参数，正常启动，应用可能被重复启动
                        if (copyDataStruct.dwData is 0)
                        {
                            WindowHelper.ShowAppWindow();

                            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                            {
                                await new AppRunningDialog().ShowAsync();
                            });
                        }
                        // 获取应用的命令参数
                        else
                        {
                            WindowHelper.ShowAppWindow();

                            string[] startupArgs = copyDataStruct.lpData.Split(' ');
                            Messenger.Default.Send(startupArgs, MessageToken.Command);
                        }
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand == SystemCommand.SC_MOUSEMENU || sysCommand == SystemCommand.SC_KEYMENU)
                        {
                            AppTitlebar.ShowTitlebarMenu();
                            return 0;
                        }
                        break;
                    }
                // 屏幕缩放比例发生变化时的消息
                case WindowMessage.WM_DPICHANGED:
                    {
                        WindowHelper.ShowAppWindow();

                        Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                        {
                            await new DPIChangedNotifyDialog().ShowAsync();
                        });
                        break;
                    }
                // 任务栏窗口右键菜单弹出时消息
                case WindowMessage.WM_SYSMENU:
                    {
                        WindowHelper.ShowAppWindow();
                        return 0;
                    }
                //两个进程通信时使用到的消息
                case WindowMessage.WM_PROCESSCOMMUNICATION:
                    {
                        CommunicationFlags flags = (CommunicationFlags)wParam;
                        if (flags == CommunicationFlags.AboutApp)
                        {
                            DispatcherQueue.TryEnqueue(() => { ViewModel.AboutAppCommand.Execute(null); });
                        }
                        else if (flags == CommunicationFlags.ShowOrHideWindow)
                        {
                            DispatcherQueue.TryEnqueue(() => { ViewModel.ShowOrHideWindowCommand.Execute(null); });
                        }
                        else if (flags == CommunicationFlags.Settings)
                        {
                            DispatcherQueue.TryEnqueue(() => { ViewModel.SettingsCommand.Execute(null); });
                        }
                        else if (flags == CommunicationFlags.Exit)
                        {
                            DispatcherQueue.TryEnqueue(() => { ViewModel.ExitCommand.Execute(null); });
                        }
                        break;
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}

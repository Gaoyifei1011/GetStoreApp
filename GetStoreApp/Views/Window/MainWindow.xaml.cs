using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Properties;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Controls.Store;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage.Streams;
using WinRT;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WinUIWindow
    {
        private WNDPROC newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            NavigationService.NavigationFrame = WindowFrame;
        }

        /// <summary>
        /// 加载“微软商店”图标
        /// </summary>
        public async void StoreIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.Store);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            sender.As<ImageIcon>().Source = image;
        }

        /// <summary>
        /// 加载“WinGet 程序包”图标
        /// </summary>
        public async void WinGetIconLoaded(object sender, RoutedEventArgs args)
        {
            InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(Resources.WinGet);
            await datawriter.StoreAsync();
            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(memoryStream);
            sender.As<ImageIcon>().Source = image;
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
            sender.As<ImageIcon>().Source = image;
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
            sender.As<ImageIcon>().Source = image;
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
            sender.As<ImageIcon>().Source = image;
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
            sender.As<ImageIcon>().Source = image;
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
            sender.As<ImageIcon>().Source = image;
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
        /// 初始化窗口
        /// </summary>
        public void InitializeWindow()
        {
            IntPtr MainWindowHandle = GetMainWindowHandle();
            newWndProc = new WNDPROC(NewWindowProc);
            oldWndProc = SetWindowLongAuto(MainWindowHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));
#if EXPERIMENTAL
            int style = GetWindowLongAuto(GetMainWindowHandle(), WindowLongIndexFlags.GWL_STYLE);
            SetWindowLongAuto(GetMainWindowHandle(), WindowLongIndexFlags.GWL_STYLE, style & ~(int)WindowStyle.WS_SYSMENU);
#endif
        }

        /// <summary>
        /// 窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口大小发生变化的消息
                case WindowMessage.WM_SIZE:
                    {
                        AppTitlebar.ViewModel.IsWindowMaximized = WindowHelper.IsWindowMaximized;
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
                        COPYDATASTRUCT copyDataStruct = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);

                        // 没有任何命令参数，正常启动，应用可能被重复启动
                        if (copyDataStruct.dwData is 0)
                        {
                            WindowHelper.ShowAppWindow();

                            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                            {
                                await new AppRunningDialog().ShowAsync();
                            });
                        }
                        // 获取应用的命令参数
                        else
                        {
                            string[] startupArgs = copyDataStruct.lpData.Split(' ');

                            if (NavigationService.GetCurrentPageType() != typeof(StorePage))
                            {
                                NavigationService.NavigateTo(typeof(StorePage));
                            }

                            StorePage storePage = NavigationService.NavigationFrame.Content.As<StorePage>();
                            if (storePage is not null)
                            {
                                RequestViewModel viewModel = storePage.Request.ViewModel;

                                viewModel.SelectedType = Convert.ToInt32(startupArgs[0]) is -1 ? viewModel.TypeList[0] : viewModel.TypeList[Convert.ToInt32(startupArgs[0])];
                                viewModel.SelectedChannel = Convert.ToInt32(startupArgs[1]) is -1 ? viewModel.ChannelList[3] : viewModel.ChannelList[Convert.ToInt32(startupArgs[1])];
                                viewModel.LinkText = startupArgs[2] is "PlaceHolderText" ? string.Empty : startupArgs[2];
                            }

                            WindowHelper.ShowAppWindow();
                        }

                        break;
                    }
                // 屏幕缩放比例发生变化时的消息
                case WindowMessage.WM_DPICHANGED:
                    {
                        WindowHelper.ShowAppWindow();

                        Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                        {
                            await new DPIChangedNotifyDialog().ShowAsync();
                        });
                        break;
                    }
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
#if EXPERIMENTAL
                        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
                        {
                            PointInt32 pt;
                            unsafe
                            {
                                User32Library.GetCursorPos(&pt);
                            }
                            if (InfoHelper.SystemVersion.Build >= 22000)
                            {
                                AppTitlebar.TitlebarMenuFlyout.ShowAt(Content,
                                    new Point(
                                    DPICalcHelper.ConvertPixelToEpx(GetMainWindowHandle(), pt.X - AppWindow.Position.X),
                                    DPICalcHelper.ConvertPixelToEpx(GetMainWindowHandle(), 32))
                                    );
                            }
                            else
                            {
                                AppTitlebar.TitlebarMenuFlyout.ShowAt(Content, new Point(pt.X - AppWindow.Position.X, 32));
                            }
                        });

                        return 0;
#endif
#if !EXPERIMENTAL
                        break;
#endif
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}

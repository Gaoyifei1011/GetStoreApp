using GetStoreApp.Helpers.Root;
using GetStoreApp.Properties;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Controls.Store;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

        public IntPtr Handle { get; }

        public OverlappedPresenter Presenter { get; }

        public MainWindow()
        {
            InitializeComponent();
            Handle = WindowNative.GetWindowHandle(this);
            NavigationService.NavigationFrame = WindowFrame;
            Presenter = AppWindow.Presenter.As<OverlappedPresenter>();
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
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
        /// 初始化窗口
        /// </summary>
        public void InitializeWindow()
        {
            newWndProc = new WNDPROC(NewWindowProc);
            oldWndProc = SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public void Show()
        {
            if (Presenter.State == OverlappedPresenterState.Maximized)
            {
                Presenter.Maximize();
            }
            else
            {
                Presenter.Restore();
            }

            AppWindow.MoveInZOrderAtTop();
            User32Library.SetForegroundWindow(Handle);
        }

        /// <summary>
        /// 最大化窗口
        /// </summary>
        public void MaximizeOrRestore()
        {
            if (Presenter.State == OverlappedPresenterState.Maximized)
            {
                Presenter.Restore();
            }
            else
            {
                Presenter.Maximize();
            }
        }

        /// <summary>
        /// 最小化窗口
        /// </summary>
        public void Minimize()
        {
            Presenter.Minimize();
        }

        /// <summary>
        /// 窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口移动的消息
                case WindowMessage.WM_MOVE:
                    {
                        if (AppTitlebar.TitlebarMenuFlyout.IsOpen)
                        {
                            AppTitlebar.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 窗口大小发生变化的消息
                case WindowMessage.WM_SIZE:
                    {
                        AppTitlebar.ViewModel.IsWindowMaximized = Presenter.State == OverlappedPresenterState.Maximized;
                        if (AppTitlebar.TitlebarMenuFlyout.IsOpen)
                        {
                            AppTitlebar.TitlebarMenuFlyout.Hide();
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
                        COPYDATASTRUCT copyDataStruct = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);

                        // 没有任何命令参数，正常启动，应用可能被重复启动
                        if (copyDataStruct.dwData is 1)
                        {
                            Show();

                            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                            {
                                await new AppRunningDialog().ShowAsync();
                            });
                        }
                        // 获取应用的命令参数
                        else if (copyDataStruct.dwData is 2)
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

                            Show();
                        }
                        // 处理通知启动的内容
                        else if (copyDataStruct.dwData is 3)
                        {
                            Task.Run(async () =>
                            {
                                await AppNotificationService.HandleAppNotificationAsync(copyDataStruct.lpData);
                            });

                            Show();
                        }

                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand == SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 45);
                            options.ShowMode = FlyoutShowMode.Standard;
                            AppTitlebar.TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        break;
                    }
                // 屏幕缩放比例发生变化时的消息
                case WindowMessage.WM_DPICHANGED:
                    {
                        if (Visible)
                        {
                            Show();
                            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                            {
                                await new DPIChangedNotifyDialog().ShowAsync();
                            });
                        }

                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        PointInt32 pt;
                        unsafe
                        {
                            User32Library.GetCursorPos(&pt);
                        }

                        FlyoutShowOptions options = new FlyoutShowOptions();
                        options.ShowMode = FlyoutShowMode.Standard;
                        options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                        new Point(DPICalcHelper.ConvertPixelToEpx(Handle, pt.X - AppWindow.Position.X - 8), DPICalcHelper.ConvertPixelToEpx(Handle, 32)) :
                        new Point(pt.X - AppWindow.Position.X - 8, 32);

                        AppTitlebar.TitlebarMenuFlyout.ShowAt(Content, options);
                        return 0;
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}

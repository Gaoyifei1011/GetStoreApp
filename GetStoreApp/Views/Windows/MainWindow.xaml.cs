using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using WinRT.Interop;

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private WNDPROC newMainWindowWndProc = null;
        private IntPtr oldMainWindowWndProc = IntPtr.Zero;

        private WNDPROC newDragAreaWindowWndProc = null;
        private IntPtr oldDragAreaWindowWndProc = IntPtr.Zero;

        private UISettings AppUISettings { get; } = new UISettings();

        public IntPtr Handle { get; }

        public OverlappedPresenter Presenter { get; }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                _windowTheme = value;
                OnPropertyChanged();
            }
        }

        private SolidColorBrush _windowBackground;

        public SolidColorBrush WindowBackground
        {
            get { return _windowBackground; }

            set
            {
                _windowBackground = value;
                OnPropertyChanged();
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                _isBackEnabled = value;
                OnPropertyChanged();
            }
        }

        private NavigationViewPaneDisplayMode _paneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;

        public NavigationViewPaneDisplayMode PaneDisplayMode
        {
            get { return _paneDisplayMode; }

            set
            {
                _paneDisplayMode = value;
                OnPropertyChanged();
            }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"Store",typeof(StorePage) },
            {"History",typeof(HistoryPage) },
            {"Download",typeof(DownloadPage) },
            {"WinGet",typeof(WinGetPage) },
            {"UWPApp",typeof(UWPAppPage) },
            {"Web",typeof(Page) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        public List<string> TagList { get; } = new List<string>()
        {
            "Store",
            "History",
            "Download",
            "WinGet",
            "UWPApp",
            "Web",
            "About",
            "Settings"
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            Handle = WindowNative.GetWindowHandle(this);
            NavigationService.NavigationFrame = WindowFrame;
            Presenter = AppWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppTitlebar.IsWindowMaximized = Presenter.State is OverlappedPresenterState.Maximized;
            PaneDisplayMode = Bounds.Width > 768 ? NavigationViewPaneDisplayMode.Left : NavigationViewPaneDisplayMode.LeftMinimal;

            SetTitleBar(AppTitlebar);
            SetTitleBarColor((Content as FrameworkElement).ActualTheme);

            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            AppUISettings.ColorValuesChanged += OnColorValuesChanged;
            (Content as FrameworkElement).ActualThemeChanged += OnActualThemeChanged;

            newMainWindowWndProc = new WNDPROC(NewWindowProc);
            oldMainWindowWndProc = SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newMainWindowWndProc));

            IntPtr childHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (childHandle != IntPtr.Zero)
            {
                int style = GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE);
                SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE, style & ~(int)WindowStyle.WS_SYSMENU);

                newDragAreaWindowWndProc = new WNDPROC(NewDragAreaWindowProc);
                oldDragAreaWindowWndProc = SetWindowLongAuto(childHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newDragAreaWindowWndProc));
            }

            if (RuntimeHelper.IsElevated)
            {
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                {
                    await ContentDialogHelper.ShowAsync(new ElevatedRunningDialog(), Content as FrameworkElement);
                });
            }
        }

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        public void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (Program.ApplicationRoot.IsAppRunning)
            {
                // 设置窗口处于非激活状态时的背景色
                if (BackdropService.AppBackdrop.SelectedValue == BackdropService.BackdropList[1].SelectedValue || BackdropService.AppBackdrop.SelectedValue == BackdropService.BackdropList[2].SelectedValue)
                {
                    MicaSystemBackdrop micaBackdrop = SystemBackdrop as MicaSystemBackdrop;

                    if (micaBackdrop is not null && micaBackdrop.BackdropConfiguration is not null)
                    {
                        if (AlwaysShowBackdropService.AlwaysShowBackdropValue)
                        {
                            micaBackdrop.BackdropConfiguration.IsInputActive = true;
                        }
                        else
                        {
                            micaBackdrop.BackdropConfiguration.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
                        }
                    }
                }
                else if (BackdropService.AppBackdrop.SelectedValue == BackdropService.BackdropList[3].SelectedValue)
                {
                    DesktopAcrylicSystemBackdrop desktopAcrylicSystemBackdrop = SystemBackdrop as DesktopAcrylicSystemBackdrop;

                    if (desktopAcrylicSystemBackdrop is not null && desktopAcrylicSystemBackdrop.BackdropConfiguration is not null)
                    {
                        if (AlwaysShowBackdropService.AlwaysShowBackdropValue)
                        {
                            desktopAcrylicSystemBackdrop.BackdropConfiguration.IsInputActive = true;
                        }
                        else
                        {
                            desktopAcrylicSystemBackdrop.BackdropConfiguration.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 窗体大小发生改变时的事件
        /// </summary>
        public void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            PaneDisplayMode = args.Size.Width > 768 ? NavigationViewPaneDisplayMode.Left : NavigationViewPaneDisplayMode.LeftCompact;
            if (AppTitlebar.TitlebarMenuFlyout.IsOpen)
            {
                AppTitlebar.TitlebarMenuFlyout.Hide();
            }
        }

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        public void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidPositionChange)
            {
                if (AppTitlebar.TitlebarMenuFlyout.IsOpen)
                {
                    AppTitlebar.TitlebarMenuFlyout.Hide();
                }
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        public async void OnAppWindowClosing(object sender, AppWindowClosingEventArgs args)
        {
            args.Cancel = true;

            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
            if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
            {
                (sender as MainWindow).Show();

                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                ContentDialogResult result = await ContentDialogHelper.ShowAsync(new ClosingWindowDialog(), Content as FrameworkElement);

                if (result is ContentDialogResult.Primary)
                {
                    Program.ApplicationRoot.Dispose();
                }
                else if (result is ContentDialogResult.Secondary)
                {
                    if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            else
            {
                Program.ApplicationRoot.Dispose();
            }
        }

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
            {
                if (ThemeService.AppTheme.SelectedValue == ThemeService.ThemeList[0].SelectedValue)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        WindowTheme = ElementTheme.Light;
                    }
                    else
                    {
                        WindowTheme = ElementTheme.Dark;
                    }
                }
            });
        }

        /// <summary>
        /// 应用主题变化时设置标题栏按钮的颜色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarColor(sender.ActualTheme);
            SetWindowBackground();
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        public void OnFrameNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        public void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName));
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        public async void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            NavigationViewItemBase navigationViewItem = args.InvokedItemContainer;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(navigationViewItem.Tag));

                if (SelectedItem != navigationItem.NavigationItem)
                {
                    if (navigationItem.NavigationItem.Tag.ToString() is "Web")
                    {
                        NavigationViewItem OriginalSelectedItem = SelectedItem;
                        await Launcher.LaunchUriAsync(new Uri("https://store.rg-adguard.net/"));
                        SelectedItem = OriginalSelectedItem;
                    }
                    else
                    {
                        NavigationService.NavigateTo(navigationItem.NavigationPage);
                    }
                }
            }
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        public void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            // 导航控件加载完成后初始化内容

            NavigationView navigationView = sender as NavigationView;
            if (navigationView is not null)
            {
                foreach (object item in navigationView.MenuItems)
                {
                    NavigationViewItem navigationViewItem = item as NavigationViewItem;
                    if (navigationViewItem is not null)
                    {
                        string Tag = Convert.ToString(navigationViewItem.Tag);

                        NavigationService.NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = Tag,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageDict[Tag],
                        });
                    }
                }

                foreach (object item in navigationView.FooterMenuItems)
                {
                    NavigationViewItem navigationViewItem = item as NavigationViewItem;
                    if (navigationViewItem is not null)
                    {
                        string Tag = Convert.ToString(navigationViewItem.Tag);

                        NavigationService.NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = Tag,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageDict[Tag],
                        });
                    }
                }

                SelectedItem = NavigationService.NavigationItemList[0].NavigationItem;
                NavigationService.NavigateTo(typeof(StorePage));
                IsBackEnabled = NavigationService.CanGoBack();
            }
        }

        /// <summary>
        /// 窗口被卸载时，注销所有事件
        /// </summary>
        public void OnNavigationViewUnLoaded(object sender, RoutedEventArgs args)
        {
            AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        public void OnBackRequestedClicked(object sender, RoutedEventArgs args)
        {
            UWPAppPage uwpAppPage = WindowFrame.Content as UWPAppPage;
            if (uwpAppPage is not null && uwpAppPage.BreadDataList.Count is 2)
            {
                uwpAppPage.BackToAppList();
            }
            else
            {
                NavigationService.NavigationFrom();
            }
        }

        /// <summary>
        /// 加载导航控件按钮的图标
        /// </summary>
        public async void OnIconLoaded(object sender, RoutedEventArgs args)
        {
            ImageIcon imageIcon = sender as ImageIcon;
            if (imageIcon is not null)
            {
                InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream();
                DataWriter datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
                datawriter.WriteBytes((byte[])imageIcon.Tag);
                await datawriter.StoreAsync();
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(memoryStream);
                (sender as ImageIcon).Source = image;
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        public void SetSystemBackdrop(string backdropName)
        {
            switch (backdropName)
            {
                case "Mica":
                    {
                        SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.Base };
                        break;
                    }
                case "MicaAlt":
                    {
                        SystemBackdrop = new MicaSystemBackdrop() { Kind = MicaKind.BaseAlt };
                        break;
                    }
                case "Acrylic":
                    {
                        SystemBackdrop = new DesktopAcrylicSystemBackdrop();
                        break;
                    }
                default:
                    {
                        SystemBackdrop = null;
                        break;
                    }
            }
            SetWindowBackground();
        }

        /// <summary>
        /// 应用背景色设置跟随系统发生变化时，修改控件的背景值
        /// </summary>
        public void SetWindowBackground()
        {
            if (BackdropService.AppBackdrop.SelectedValue == BackdropService.BackdropList[0].SelectedValue)
            {
                if ((Content as FrameworkElement).ActualTheme is ElementTheme.Light)
                {
                    WindowBackground = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else
                {
                    WindowBackground = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                }
            }
            else
            {
                WindowBackground = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// 设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarColor(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public void Show()
        {
            if (Presenter.State is OverlappedPresenterState.Maximized)
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
            if (Presenter.State is OverlappedPresenterState.Maximized)
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
        /// 更改指定窗口的属性
        /// </summary>
        private int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.GetWindowLongPtr(hWnd, nIndex);
            }
            else
            {
                return User32Library.GetWindowLong(hWnd, nIndex);
            }
        }

        /// <summary>
        /// 更改指定窗口的窗口属性
        /// </summary>
        private IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                            minMaxInfo.ptMinTrackSize.X = (int)(960 * Content.XamlRoot.RasterizationScale);
                            minMaxInfo.ptMinTrackSize.Y = (int)(600 * Content.XamlRoot.RasterizationScale);
                            Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        }

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

                            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                            {
                                await ContentDialogHelper.ShowAsync(new AppRunningDialog(), Content as FrameworkElement);
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

                            StorePage storePage = NavigationService.NavigationFrame.Content as StorePage;
                            if (storePage is not null)
                            {
                                storePage.Request.SelectedType = Convert.ToInt32(startupArgs[0]) is -1 ? storePage.Request.TypeList[0] : storePage.Request.TypeList[Convert.ToInt32(startupArgs[0])];
                                storePage.Request.SelectedChannel = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.Request.ChannelList[3] : storePage.Request.ChannelList[Convert.ToInt32(startupArgs[1])];
                                storePage.Request.LinkText = startupArgs[2] is "PlaceHolderText" ? string.Empty : startupArgs[2];
                            }

                            Show();
                        }
                        // 处理通知启动的内容
                        else if (copyDataStruct.dwData is 3)
                        {
                            Task.Run(async () =>
                            {
                                await ToastNotificationService.HandleToastNotificationAsync(copyDataStruct.lpData);
                            });

                            Show();
                        }

                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SystemCommand.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 15);
                            options.ShowMode = FlyoutShowMode.Standard;
                            AppTitlebar.TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        else if (sysCommand is SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 45);
                            options.ShowMode = FlyoutShowMode.Standard;
                            AppTitlebar.TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        break;
                    }
            }
            return User32Library.CallWindowProc(oldMainWindowWndProc, hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr NewDragAreaWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (AppTitlebar.TitlebarMenuFlyout.IsOpen)
                        {
                            AppTitlebar.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            PointInt32 pt;
                            unsafe
                            {
                                User32Library.GetCursorPos(&pt);
                            }

                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.ShowMode = FlyoutShowMode.Standard;
                            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                            new Point((pt.X - AppWindow.Position.X - 8) / Content.XamlRoot.RasterizationScale, (pt.Y - AppWindow.Position.Y) / Content.XamlRoot.RasterizationScale) :
                            new Point(pt.X - AppWindow.Position.X - 8, pt.Y - AppWindow.Position.Y);

                            AppTitlebar.TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
            }
            return User32Library.CallWindowProc(oldDragAreaWindowWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}

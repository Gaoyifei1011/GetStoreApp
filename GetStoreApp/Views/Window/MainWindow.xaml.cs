using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Window;
using GetStoreApp.Properties;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Controls.Store;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using WinRT;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : WinUIWindow, INotifyPropertyChanged
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
            }
        }

        private SolidColorBrush _windowBackground;

        public SolidColorBrush WindowBackground
        {
            get { return _windowBackground; }

            set
            {
                _windowBackground = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowBackground)));
            }
        }

        private Thickness _appTitleBarMargin;

        public Thickness AppTitleBarMargin
        {
            get { return _appTitleBarMargin; }

            set
            {
                _appTitleBarMargin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppTitleBarMargin)));
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                _isBackEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBackEnabled)));
            }
        }

        private bool _isPaneToggleButtonVisible;

        public bool IsPaneToggleButtonVisible
        {
            get { return _isPaneToggleButtonVisible; }

            set
            {
                _isPaneToggleButtonVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPaneToggleButtonVisible)));
            }
        }

        private NavigationViewPaneDisplayMode _paneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;

        public NavigationViewPaneDisplayMode PaneDisplayMode
        {
            get { return _paneDisplayMode; }

            set
            {
                _paneDisplayMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaneDisplayMode)));
            }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"Store",typeof(StorePage) },
            {"WinGet",typeof(WinGetPage) },
            {"History",typeof(HistoryPage) },
            {"Download",typeof(DownloadPage) },
            {"Web",typeof(WebPage) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        public List<string> TagList { get; } = new List<string>()
        {
            "Store",
            "WinGet",
            "History",
            "Download",
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
            Presenter = AppWindow.Presenter.As<OverlappedPresenter>();
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppTitlebar.IsWindowMaximized = Presenter.State == OverlappedPresenterState.Maximized;
            SetTitleBar(AppTitlebar);
        }

        /// <summary>
        /// 初始化窗口
        /// </summary>
        public void InitializeWindow()
        {
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
        /// 窗口激活状态发生变化的事件
        /// </summary>
        public void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            // 设置窗口处于非激活状态时的背景色
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[1].InternalName || BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[2].InternalName)
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
            else if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[3].InternalName)
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

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        public async void OnClosed(object sender, WindowEventArgs args)
        {
            args.Handled = true;

            if (AppExitService.AppExit.InternalName == AppExitService.AppExitList[0].InternalName)
            {
                sender.As<MainWindow>().AppWindow.Hide();
            }
            else
            {
                // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
                if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
                {
                    sender.As<MainWindow>().Show();

                    // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                    ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

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
        }

        /// <summary>
        /// 窗体大小发生改变时的事件
        /// </summary>
        public void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            PaneDisplayMode = args.Size.Width > 768 ? NavigationViewPaneDisplayMode.Left : NavigationViewPaneDisplayMode.LeftMinimal;
        }

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, SetWindowBackground);
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        WindowTheme = ElementTheme.Light;
                        SetTitleBarColor(ElementTheme.Light);
                    }
                    else
                    {
                        WindowTheme = ElementTheme.Dark;
                        SetTitleBarColor(ElementTheme.Dark);
                    }
                }
            });
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
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        public void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航视图控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        public void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            PropertyChanged += OnPropertyChanged;
            AppUISettings.ColorValuesChanged += OnColorValuesChanged;

            PaneDisplayMode = Bounds.Width > 768 ? NavigationViewPaneDisplayMode.Left : NavigationViewPaneDisplayMode.LeftMinimal;

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

                // 应用初次加载时设置标题栏和窗口菜单的主题色
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        SetTitleBarColor(ElementTheme.Light);
                    }
                    else
                    {
                        SetTitleBarColor(ElementTheme.Dark);
                    }
                }
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    SetTitleBarColor(ElementTheme.Light);
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    SetTitleBarColor(ElementTheme.Dark);
                }
            }
        }

        /// <summary>
        /// 窗口被卸载时，注销所有事件
        /// </summary>
        public void OnNavigationViewUnLoaded(object sender, RoutedEventArgs args)
        {
            AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
            PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        /// 可通知的属性发生更改时的事件处理
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            // 应用的主题值或窗口背景色值发生改变时设置标题栏的颜色
            if (args.PropertyName == nameof(WindowTheme) || args.PropertyName == nameof(SystemBackdrop))
            {
                Program.ApplicationRoot.TrayMenuWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, SetWindowBackground);
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        SetTitleBarColor(ElementTheme.Light);
                    }
                    else
                    {
                        SetTitleBarColor(ElementTheme.Dark);
                    }
                }
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    SetTitleBarColor(ElementTheme.Light);
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    SetTitleBarColor(ElementTheme.Dark);
                }
            }
            else if (args.PropertyName == nameof(PaneDisplayMode))
            {
                if (PaneDisplayMode == NavigationViewPaneDisplayMode.Left)
                {
                    IsPaneToggleButtonVisible = false;
                    AppTitleBarMargin = new Thickness(48, 0, 0, 0);
                }
                else if (PaneDisplayMode == NavigationViewPaneDisplayMode.LeftMinimal)
                {
                    IsPaneToggleButtonVisible = true;
                    AppTitleBarMargin = new Thickness(96, 0, 0, 0);
                }
            }
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        public void OnTapped(object sender, TappedRoutedEventArgs args)
        {
            NavigationViewItem navigationViewItem = sender as NavigationViewItem;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(navigationViewItem.Tag));
                if (SelectedItem != navigationItem.NavigationItem)
                {
                    NavigationService.NavigateTo(navigationItem.NavigationPage);
                }
            }
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
        }

        /// <summary>
        /// 应用背景色设置跟随系统发生变化时，修改控件的背景值
        /// </summary>
        public void SetWindowBackground()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (Application.Current.RequestedTheme is ApplicationTheme.Light)
                    {
                        WindowBackground = ResourceDictionaryHelper.GridResourceDict["WindowLightBrush"] as SolidColorBrush;
                    }
                    else
                    {
                        WindowBackground = ResourceDictionaryHelper.GridResourceDict["WindowDarkBrush"] as SolidColorBrush;
                    }
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    WindowBackground = ResourceDictionaryHelper.GridResourceDict["WindowLightBrush"] as SolidColorBrush;
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    WindowBackground = ResourceDictionaryHelper.GridResourceDict["WindowDarkBrush"] as SolidColorBrush;
                }
            }
            else
            {
                WindowBackground = ResourceDictionaryHelper.GridResourceDict["WindowSystemBackdropBrush"] as SolidColorBrush;
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

            if (theme == ElementTheme.Light)
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
        /// 应用主窗口消息处理
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
                        AppTitlebar.IsWindowMaximized = Presenter.State == OverlappedPresenterState.Maximized;
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

                        if (sysCommand == SystemCommand.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 15);
                            options.ShowMode = FlyoutShowMode.Standard;
                            AppTitlebar.TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        else if (sysCommand == SystemCommand.SC_KEYMENU)
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
                            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
                            {
                                await new DPIChangedNotifyDialog().ShowAsync();
                            });
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
                        new Point(DPICalcHelper.ConvertPixelToEpx(Handle, pt.X - AppWindow.Position.X - 8), DPICalcHelper.ConvertPixelToEpx(Handle, pt.Y - AppWindow.Position.Y)) :
                        new Point(pt.X - AppWindow.Position.X - 8, pt.Y - AppWindow.Position.Y);

                        AppTitlebar.TitlebarMenuFlyout.ShowAt(Content, options);
                        return 0;
                    }
            }
            return User32Library.CallWindowProc(oldDragAreaWindowWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}

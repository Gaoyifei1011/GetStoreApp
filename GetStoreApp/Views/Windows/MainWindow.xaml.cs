using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Comctl32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using GetStoreApp.WindowsAPI.PInvoke.Uxtheme;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using WinRT;
using WinRT.Interop;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    internal sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string AppManagerString = ResourceService.GetLocalized("Window/AppManager");
        private readonly string AppUpdateString = ResourceService.GetLocalized("Window/AppUpdate");
        private readonly string CheckNetWorkConnectionString = ResourceService.GetLocalized("Window/CheckNetWorkConnection");
        private readonly string DownloadString = ResourceService.GetLocalized("Window/Download");
        private readonly string HomeString = ResourceService.GetLocalized("Window/Home");
        private readonly string NetworkError1String = ResourceService.GetLocalized("Window/NetworkError1");
        private readonly string NetworkError2String = ResourceService.GetLocalized("Window/NetworkError2");
        private readonly string RunningAdministratorString = ResourceService.GetLocalized("Window/RunningAdministrator");
        private readonly string SettingsString = ResourceService.GetLocalized("Window/Settings");
        private readonly string StoreString = ResourceService.GetLocalized("Window/Store");
        private readonly string TitleString = ResourceService.GetLocalized("Window/Title");
        private readonly string WebString = ResourceService.GetLocalized("Window/Web");
        private readonly string WinGetString = ResourceService.GetLocalized("Window/WinGet");
        private ContentIsland contentIsland;
        private InputKeyboardSource inputKeyboardSource;
        private ContentCoordinateConverter contentCoordinateConverter;
        private OverlappedPresenter overlappedPresenter;
        private SUBCLASSPROC mainWindowSubClassProc;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        internal new static MainWindow Current { get; private set; }

        private string _windowTitle;

        private string WindowTitle
        {
            get { return _windowTitle; }

            set
            {
                if (!string.Equals(_windowTitle, value))
                {
                    _windowTitle = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowTitle)));
                }
            }
        }

        private SystemBackdrop _windowSystemBackdrop;

        private SystemBackdrop WindowSystemBackdrop
        {
            get { return _windowSystemBackdrop; }

            set
            {
                if (!Equals(_windowSystemBackdrop, value))
                {
                    _windowSystemBackdrop = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowSystemBackdrop)));
                }
            }
        }

        private ElementTheme _windowTheme;

        private ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowTheme)));
                }
            }
        }

        private bool _isWindowMaximized;

        private bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsWindowMaximized)));
                }
            }
        }

        private bool _isBackEnabled;

        private bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                if (!Equals(_isBackEnabled, value))
                {
                    _isBackEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsBackEnabled)));
                }
            }
        }

        private NavigationViewItemModel _selectedItem;

        private NavigationViewItemModel SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
                }
            }
        }

        internal ObservableCollection<NavigationViewItemModel> NavigationViewItemMenuItemsCollection { get; } = [];

        private ObservableCollection<NavigationViewItemModel> NavigationViewItemFooterMenuItemsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        [DynamicWindowsRuntimeCast(typeof(FrameworkElement))]
        internal MainWindow()
        {
            Current = this;
            InitializeComponent();
            InitializeWindowData(AppWindow);
            MountWindowEvent();
            MountWindowWndProc(AppWindow.Id);
            SetWindowTheme();
            SetSystemBackdrop();
            SetWindowPosition(AppWindow);
            SetClassicMenuTheme(AppWindow.TitleBar, (Content as FrameworkElement).ActualTheme);
            SetTopMost();
            CheckNetwork();
            InitializeNavigationViewItemCollection();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：命令调用处理

        /// <summary>
        /// 固定到开始屏幕
        /// </summary>
        private async void OnPinToStartScreenExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is NavigationViewItemModel navigationViewItem)
            {
                string displayName = navigationViewItem.NavigationTitle;
                string tag = navigationViewItem.NavigationTag;
                await PinToStartScreenAsync(displayName, tag);
            }
        }

        /// <summary>
        /// 固定到任务栏
        /// </summary>
        private async void OnPinToTaskbarExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is NavigationViewItemModel navigationViewItem)
            {
                string displayName = navigationViewItem.NavigationTitle;
                string tag = navigationViewItem.NavigationTag;
                await PinToTaskbarAsync(displayName, tag);
            }
        }

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                if (contentIsland is not null && !contentIsland.IsClosed && WindowSystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                {
                    materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdrop || args.WindowActivationState is not WindowActivationState.Deactivated;
                    Task.Run(NotificationService.UpdateNotificationSetting);
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }

        /// <summary>
        /// 窗口大小发生改变时的事件
        /// </summary>
        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            if (TitlebarMenuFlyout.IsOpen)
            {
                TitlebarMenuFlyout.Hide();
            }

            if (overlappedPresenter is not null)
            {
                IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            }

            if (contentIsland is not null)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1000 * contentIsland.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(600 * contentIsland.RasterizationScale);
            }
        }

        #endregion 第四部分：命令调用处理

        #region 第五部分：挂载事件处理

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            // 窗口位置发生变化
            if (args.DidPositionChange)
            {
                if (TitlebarMenuFlyout.IsOpen)
                {
                    TitlebarMenuFlyout.Hide();
                }

                if (overlappedPresenter is not null)
                {
                    IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
                }
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private async void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            args.Cancel = true;

            int count = 0;
            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();
            try
            {
                count = DownloadSchedulerService.DownloadSchedulerList.Count;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(OnAppWindowClosing), 1, e);
            }
            finally
            {
                DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
            }

            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
            if (count > 0)
            {
                Activate();

                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                ContentDialogResult contentDialogResult = await ShowDialogAsync(new ClosingWindowDialog());

                if (contentDialogResult is ContentDialogResult.Primary)
                {
                    DismountWindowEvent();
                    DismountWindowWndProc(sender.Id);
                    DownloadSchedulerService.TerminateDownload();
                    (Application.Current as MainApp).Dispose();
                }
                else if (contentDialogResult is ContentDialogResult.Secondary)
                {
                    if (GetFrameContent() is not DownloadPage)
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            else
            {
                DismountWindowEvent();
                DismountWindowWndProc(sender.Id);
                (Application.Current as MainApp).Dispose();
            }
        }

        /// <summary>
        /// 内容岛状态发生更改时触发的事件
        /// </summary>
        private void OnStateChanged(ContentIsland sender, ContentIslandStateChangedEventArgs args)
        {
            if (args.DidRasterizationScaleChange)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1000 * contentIsland.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(600 * contentIsland.RasterizationScale);
            }
        }

        /// <summary>
        /// 内容岛设置发生更改时触发的事件
        /// </summary>
        private void OnSettingChanged(ContentIslandEnvironment sender, ContentEnvironmentSettingChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                SetWindowTheme();
                StoreRegionService.UpdateDefaultRegion();
            });
        }

        /// <summary>
        /// 处理键盘系统按键事件
        /// </summary>
        private async void OnSystemKeyDown(InputKeyboardSource sender, KeyEventArgs args)
        {
            if (args.KeyStatus.IsMenuKeyDown && args.VirtualKey is global::Windows.System.VirtualKey.Space)
            {
                args.Handled = true;
                FlyoutShowOptions options = new()
                {
                    Position = new(0, 45),
                    ShowMode = FlyoutShowMode.Standard
                };
                TitlebarMenuFlyout.ShowAt(null, options);
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Restore);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(MenuFlyout)), DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                SendWindowMessage(AppWindow.Id, WindowMessageKind.Move);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(MenuFlyout)), DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                SendWindowMessage(AppWindow.Id, WindowMessageKind.Size);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Minimize);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Maximize);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Close);
        }

        /// <summary>
        /// 应用主题变化时设置标题栏按钮的颜色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(AppWindow.TitleBar, sender.ActualTheme);
            SetClassicMenuTheme(AppWindow.TitleBar, sender.ActualTheme);
        }

        /// <summary>
        /// 按下 Alt + BackSpace 键时，导航控件返回到上一页
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is global::Windows.System.VirtualKey.Back && args.KeyStatus.IsMenuKeyDown)
            {
                if (GetFrameContent() is AppManagerPage appManagerPage && appManagerPage.BreadCollection.Count is 2)
                {
                    appManagerPage.NavigateTo(appManagerPage.PageList[0], null, false);
                }
                else if (GetFrameContent() is SettingsPage settingsPage && settingsPage.BreadCollection.Count is 2)
                {
                    settingsPage.NavigateTo(settingsPage.PageList[0], null, false);
                }
                else
                {
                    NavigationFrom();
                }
            }
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackClicked(object sender, RoutedEventArgs args)
        {
            if (GetFrameContent() is AppManagerPage appManagerPage && appManagerPage.BreadCollection.Count is 2)
            {
                appManagerPage.NavigateTo(appManagerPage.PageList[0], null, false);
            }
            else if (GetFrameContent() is SettingsPage settingsPage && settingsPage.BreadCollection.Count is 2)
            {
                settingsPage.NavigateTo(settingsPage.PageList[0], null, false);
            }
            else
            {
                NavigationFrom();
            }
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(FrameworkElement)), DynamicWindowsRuntimeCast(typeof(NavigationViewItem))]
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            // 设置标题栏主题
            SetTitleBarTheme(AppWindow.TitleBar, (Content as FrameworkElement).ActualTheme);
            SelectedItem = NavigationViewItemMenuItemsCollection[0];
            NavigateTo(typeof(HomePage));

            // 初始化启动信息
            AppLaunchArguments appLaunchArguments = DesktopLaunchService.AppLaunchArguments;
            await ParseAppLaunchArgumentsAsync(appLaunchArguments, true);
        }

        /// <summary>
        /// 在当前导航控件所选项更改时发生
        /// </summary>
        private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is not null && !Equals(SelectedItem, args.SelectedItem))
            {
                SelectedItem = args.SelectedItem as NavigationViewItemModel;

                if (SelectedItem is null)
                {
                    return;
                }

                // 对应的页面为空，选中项修改为已经选择的页面
                if (SelectedItem.NavigationPage is null)
                {
                    if (Equals(SelectedItem.NavigationTag, "Web"))
                    {
                        OpenWebView();
                    }

                    Type currentPageType = GetCurrentPageType();
                    NavigationViewItemModel selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemMenuItemsCollection);
                    if (selectedNavigationViewItem is not null)
                    {
                        SelectedItem = selectedNavigationViewItem;
                    }
                    else
                    {
                        selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemFooterMenuItemsCollection);
                        SelectedItem = selectedNavigationViewItem;
                    }
                }
                // 切换到选中项对应的页面
                else
                {
                    NavigateTo(SelectedItem.NavigationPage);
                }
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            try
            {
                Type currentPageType = GetCurrentPageType();

                // 切换到选中页面对应的项
                NavigationViewItemModel selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemMenuItemsCollection);
                SelectedItem = null;
                if (selectedNavigationViewItem is not null)
                {
                    SelectedItem = selectedNavigationViewItem;
                }
                else
                {
                    selectedNavigationViewItem = GetSelectedItem(currentPageType, NavigationViewItemFooterMenuItemsCollection);
                    SelectedItem = selectedNavigationViewItem;
                }

                IsBackEnabled = CanGoBack();
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(OnNavigated), 1, e);
            }
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(OnNavigationFailed), 1, args.Exception);
            (Application.Current as MainApp).Dispose();
        }

        /// <summary>
        /// 网络状态发生变化时触发的事件
        /// </summary>
        private void OnNetworkStatusChanged(object sender)
        {
            DispatcherQueue.TryEnqueue(CheckNetwork);
        }

        /// <summary>
        /// 同步漫游应用程序数据时发生的事件
        /// </summary>
        private void OnAppLaunchActivated(object sender, AppLaunchArguments args)
        {
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
            {
                // 初始化启动信息
                await ParseAppLaunchArgumentsAsync(args, false);
            });
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (string.Equals(args.PropertyName, nameof(ThemeService.AppTheme)))
                {
                    SetWindowTheme();
                }
                if (string.Equals(args.PropertyName, nameof(BackdropService.AppBackdrop)))
                {
                    SetSystemBackdrop();
                }
                if (string.Equals(args.PropertyName, nameof(TopMostService.TopMost)))
                {
                    SetTopMost();
                }
            });
        }

        #endregion 第五部分：挂载事件处理

        #region 第六部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化窗口数据
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(OverlappedPresenter))]
        private void InitializeWindowData(AppWindow appWindow)
        {
            WindowTitle = RuntimeHelper.IsElevated ? TitleString + RunningAdministratorString : TitleString;
            overlappedPresenter = appWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(appWindow.Id);
            contentIsland = ContentIsland.FindAllForCompositor(Compositor)[0];
            inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
        }

        /// <summary>
        /// 发送窗口消息类型
        /// </summary>
        private void SendWindowMessage(Microsoft.UI.WindowId windowId, WindowMessageKind windowMessageKind)
        {
            switch (windowMessageKind)
            {
                case WindowMessageKind.Move:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MOVE, 0);
                        break;
                    }
                case WindowMessageKind.Size:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_SIZE, 0);
                        break;
                    }
                case WindowMessageKind.Minimize:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MINIMIZE, 0);
                        break;
                    }
                case WindowMessageKind.Maximize:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
                        break;
                    }
                case WindowMessageKind.Restore:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_RESTORE, 0);
                        break;
                    }
                case WindowMessageKind.Close:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_CLOSE, 0);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 挂载窗口事件
        /// </summary>
        private void MountWindowEvent()
        {
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            contentIsland.StateChanged += OnStateChanged;
            contentIsland.Environment.SettingChanged += OnSettingChanged;
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            ThemeService.PropertyChanged += OnServicePropertyChanged;
            BackdropService.PropertyChanged += OnServicePropertyChanged;
            TopMostService.PropertyChanged += OnServicePropertyChanged;
            DesktopLaunchService.AppLaunchActivated += OnAppLaunchActivated;
        }

        /// <summary>
        /// 卸载窗口事件
        /// </summary>
        private void DismountWindowEvent()
        {
            try
            {
                AppWindow.Changed -= OnAppWindowChanged;
                contentIsland.Environment.SettingChanged -= OnSettingChanged;
                inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
                ThemeService.PropertyChanged -= OnServicePropertyChanged;
                BackdropService.PropertyChanged -= OnServicePropertyChanged;
                TopMostService.PropertyChanged -= OnServicePropertyChanged;
                DesktopLaunchService.AppLaunchActivated -= OnAppLaunchActivated;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetUpgradePage), nameof(DismountWindowEvent), 1, e);
            }
        }

        /// <summary>
        /// 挂载窗口进程
        /// </summary>
        private void MountWindowWndProc(Microsoft.UI.WindowId windowId)
        {
            mainWindowSubClassProc = new(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0, nint.Zero);
        }

        /// <summary>
        /// 卸载窗口进程
        /// </summary>
        private void DismountWindowWndProc(Microsoft.UI.WindowId windowId)
        {
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        private void SetWindowTheme()
        {
            WindowTheme = string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]) ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark : Enum.TryParse(ThemeService.AppTheme, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;
        }

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        private void SetSystemBackdrop()
        {
            if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[1]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[2]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.BaseAlt);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[3]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Default);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[4]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[5]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Thin);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else
            {
                WindowSystemBackdrop = null;
                VisualStateManager.GoToState(MainPage, "BackgroundDefault", false);
            }
        }

        /// <summary>
        /// 设置窗口大小和位置
        /// </summary>
        private void SetWindowPosition(AppWindow appWindow)
        {
            // 默认直接显示到窗口中间
            if (DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Nearest) is DisplayArea displayArea && contentIsland is not null)
            {
                RectInt32 workArea = displayArea.WorkArea;
                appWindow.Move(new((workArea.Width - appWindow.Size.Width) / 2, (workArea.Height - appWindow.Size.Height) / 2));
            }
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(AppWindowTitleBar appWindowTitleBar, ElementTheme theme)
        {
            if (appWindowTitleBar is null)
            {
                return;
            }

            appWindowTitleBar.BackgroundColor = Colors.Transparent;
            appWindowTitleBar.ForegroundColor = Colors.Transparent;
            appWindowTitleBar.InactiveBackgroundColor = Colors.Transparent;
            appWindowTitleBar.InactiveForegroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                appWindowTitleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                appWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                appWindowTitleBar.ButtonHoverForegroundColor = Colors.Black;
                appWindowTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                appWindowTitleBar.ButtonPressedForegroundColor = Colors.Black;
                appWindowTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                appWindowTitleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                appWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                appWindowTitleBar.ButtonHoverForegroundColor = Colors.White;
                appWindowTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                appWindowTitleBar.ButtonPressedForegroundColor = Colors.White;
                appWindowTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private void SetClassicMenuTheme(AppWindowTitleBar appWindowTitleBar, ElementTheme theme)
        {
            if (appWindowTitleBar is null)
            {
                return;
            }

            if (theme is ElementTheme.Light)
            {
                appWindowTitleBar.PreferredTheme = TitleBarTheme.Light;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                appWindowTitleBar.PreferredTheme = TitleBarTheme.Dark;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        /// <summary>
        /// 设置窗口的置顶状态
        /// </summary>
        private void SetTopMost()
        {
            overlappedPresenter.IsAlwaysOnTop = TopMostService.TopMost;
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private nint MainWindowSubClassProc(nint hWnd, WindowMessage Msg, nuint wParam, nint lParam, uint uIdSubclass, nint dwRefData)
        {
            switch (Msg)
            {
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (TitlebarMenuFlyout.IsOpen)
                        {
                            TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam is 2 && Content is not null && Content.XamlRoot is not null)
                        {
                            PointInt32 screenPoint = new(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = Environment.OSVersion.Version.Build > 22000 ? new(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale) : new(localPoint.X, localPoint.Y)
                            };

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU)
                        {
                            return 0;
                        }
                        break;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 页面向前导航
        /// </summary>
        internal void NavigateTo(Type navigationPageType, object parameter = null)
        {
            try
            {
                // 导航到该项目对应的页面
                if (!Equals(GetCurrentPageType(), navigationPageType))
                {
                    WindowFrame.Navigate(navigationPageType, parameter);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(NavigateTo), 1, e);
            }
        }

        /// <summary>
        /// 页面向后导航
        /// </summary>
        private void NavigationFrom()
        {
            if (WindowFrame.CanGoBack)
            {
                WindowFrame.GoBack();
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        private Type GetCurrentPageType()
        {
            return WindowFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 检查当前页面是否能向后导航
        /// </summary>
        private bool CanGoBack()
        {
            return WindowFrame.CanGoBack;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        internal object GetFrameContent()
        {
            return WindowFrame.Content;
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        internal NavigationViewItemModel GetSelectedItem(Type currentPageType, ObservableCollection<NavigationViewItemModel> navigationViewItemMenuItemCollection)
        {
            foreach (NavigationViewItemModel navigationViewItem in navigationViewItemMenuItemCollection)
            {
                if (Equals(navigationViewItem.NavigationPage, currentPageType))
                {
                    return navigationViewItem;
                }
            }

            return default;
        }

        /// <summary>
        /// 显示内容对话框
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ContentDialog))]
        internal async Task<ContentDialogResult> ShowDialogAsync(ContentDialog contentDialog)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            bool isDialogOpening = false;
            if (contentDialog is not null && Content is not null)
            {
                foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
                {
                    if (popup.Child is ContentDialog)
                    {
                        isDialogOpening = true;
                        break;
                    }
                }

                if (!isDialogOpening)
                {
                    try
                    {
                        contentDialog.XamlRoot = Content.XamlRoot;
                        dialogResult = await contentDialog.ShowAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(ShowDialogAsync), 1, e);
                    }
                }
            }

            return dialogResult;
        }

        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Grid))]
        internal async Task ShowNotificationAsync(TeachingTip teachingTip, int duration = 2000)
        {
            try
            {
                if (teachingTip is not null && MainPage.Content is Grid grid)
                {
                    grid.Children.Add(teachingTip);

                    teachingTip.IsOpen = true;
                    await Task.Delay(duration);
                    teachingTip.IsOpen = false;

                    // 应用内通知关闭动画显示耗费 300 ms
                    await Task.Delay(300);
                    grid.Children.Remove(teachingTip);
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }

        /// <summary>
        /// 解析应用启动参数
        /// </summary>
        private async Task ParseAppLaunchArgumentsAsync(AppLaunchArguments appLaunchArguments, bool isFirstLaunch)
        {
            Activate();

            // 正常启动
            if (appLaunchArguments.AppLaunchKind is AppLaunchKind.Launch)
            {
                // 应用已经启动
                if (!isFirstLaunch && appLaunchArguments.IsLaunched && !(appLaunchArguments.SubParameters is not null && appLaunchArguments.SubParameters.Count > 0 && appLaunchArguments.SubParameters[0] is "Restart"))
                {
                    await ShowDialogAsync(new AppRunningDialog());
                }
            }
            // 从跳转列表处启动
            else if (appLaunchArguments.AppLaunchKind is AppLaunchKind.JumpList)
            {
                if (appLaunchArguments.SubParameters is not null && appLaunchArguments.SubParameters.Count >= 1)
                {
                    if (appLaunchArguments.SubParameters[0] is "Home" && GetFrameContent() is not HomePage)
                    {
                        NavigateTo(typeof(HomePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Store" && GetFrameContent() is not StorePage)
                    {
                        NavigateTo(typeof(StorePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppUpdate" && GetFrameContent() is not AppUpdatePage)
                    {
                        NavigateTo(typeof(AppUpdatePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "WinGet" && GetFrameContent() is not WinGetPage)
                    {
                        NavigateTo(typeof(WinGetPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppManager" && GetFrameContent() is not AppManagerPage)
                    {
                        NavigateTo(typeof(AppManagerPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Download" && GetFrameContent() is not DownloadPage)
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            // 从辅助磁贴处启动
            else if (appLaunchArguments.AppLaunchKind is AppLaunchKind.SecondaryTile)
            {
                if (appLaunchArguments.SubParameters is not null && appLaunchArguments.SubParameters.Count >= 1)
                {
                    if (appLaunchArguments.SubParameters[0] is "Home" && GetFrameContent() is not HomePage)
                    {
                        NavigateTo(typeof(HomePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Store" && GetFrameContent() is not StorePage)
                    {
                        NavigateTo(typeof(StorePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppUpdate" && GetFrameContent() is not AppUpdatePage)
                    {
                        NavigateTo(typeof(AppUpdatePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "WinGet" && GetFrameContent() is not WinGetPage)
                    {
                        NavigateTo(typeof(WinGetPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppManager" && GetFrameContent() is not AppManagerPage)
                    {
                        NavigateTo(typeof(AppManagerPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Download" && GetFrameContent() is not DownloadPage)
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            // 从共享目标启动
            else if (appLaunchArguments.AppLaunchKind is AppLaunchKind.ShareTarget)
            {
                if (appLaunchArguments.SubParameters is not null && appLaunchArguments.SubParameters.Count is 3)
                {
                    if (GetFrameContent() is StorePage storePage)
                    {
                        storePage.StoreSelector?.UpdateData(appLaunchArguments.SubParameters);
                    }
                    else
                    {
                        NavigateTo(typeof(StorePage), appLaunchArguments.SubParameters);
                    }
                }
            }
            // 从通知协议启动
            else if (appLaunchArguments.AppLaunchKind is AppLaunchKind.Protocol)
            {
                if (appLaunchArguments.SubParameters is null)
                {
                    if (!isFirstLaunch && appLaunchArguments.IsLaunched)
                    {
                        await ShowDialogAsync(new AppRunningDialog());
                    }
                }
                else
                {
                    if (appLaunchArguments.SubParameters.Count > 0)
                    {
                        if (appLaunchArguments.SubParameters[0] is "DownloadSettings")
                        {
                            if (GetFrameContent() is not SettingsPage)
                            {
                                NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.Download);
                            }
                            else if (GetFrameContent() is SettingsPage settingsPage)
                            {
                                if (!Equals(settingsPage.GetCurrentPageType(), settingsPage.PageList[0]))
                                {
                                    settingsPage.NavigateTo(settingsPage.PageList[0], AppNaviagtionArgs.Download);
                                }
                                else if (settingsPage.GetFrameContent() is SettingsItemPage settingsItemPage && !Equals(settingsItemPage.GetCurrentPageType(), settingsItemPage.PageList[3]))
                                {
                                    if (settingsItemPage.IsLoaded)
                                    {
                                        int currentIndex = settingsItemPage.PageList.FindIndex(item => Equals(item, settingsItemPage.GetCurrentPageType()));
                                        settingsItemPage.NavigateTo(settingsItemPage.PageList[3], null, 3 > currentIndex);
                                    }
                                    else
                                    {
                                        settingsItemPage.SetNavigateContent(true, settingsItemPage.PageList[3]);
                                    }
                                }
                            }
                        }
                        else if (appLaunchArguments.SubParameters[0] is "AppInstallSettings")
                        {
                            if (GetFrameContent() is not SettingsPage)
                            {
                                NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.AppInstall);
                            }
                            else if (GetFrameContent() is SettingsPage settingsPage)
                            {
                                if (!Equals(settingsPage.GetCurrentPageType(), settingsPage.PageList[0]))
                                {
                                    settingsPage.NavigateTo(settingsPage.PageList[0], AppNaviagtionArgs.AppInstall);
                                }
                                else if (settingsPage.GetFrameContent() is SettingsItemPage settingsItemPage && !Equals(settingsItemPage.GetCurrentPageType(), settingsItemPage.PageList[4]))
                                {
                                    if (settingsItemPage.IsLoaded)
                                    {
                                        int currentIndex = settingsItemPage.PageList.FindIndex(item => Equals(item, settingsItemPage.GetCurrentPageType()));
                                        settingsItemPage.NavigateTo(settingsItemPage.PageList[4], null, 4 > currentIndex);
                                    }
                                    else
                                    {
                                        settingsItemPage.SetNavigateContent(true, settingsItemPage.PageList[4]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // 从 Toast 通知启动
            else if (appLaunchArguments.AppLaunchKind is AppLaunchKind.ToastNotification)
            {
                if (appLaunchArguments.SubParameters is not null && appLaunchArguments.SubParameters.Count > 0)
                {
                    if (!isFirstLaunch && appLaunchArguments.IsLaunched && appLaunchArguments.SubParameters[0] is "OpenApp")
                    {
                        await ShowDialogAsync(new AppRunningDialog());
                    }
                    else if (appLaunchArguments.SubParameters[0] is "ViewDownloadPage")
                    {
                        if (GetFrameContent() is not DownloadPage)
                        {
                            NavigateTo(typeof(DownloadPage), AppNaviagtionArgs.Completed);
                        }
                        else if (GetFrameContent() is DownloadPage downloadPage && !Equals(downloadPage.GetCurrentPageType(), downloadPage.PageList[1]))
                        {
                            if (downloadPage.IsLoaded)
                            {
                                int currentIndex = downloadPage.PageList.FindIndex(item => Equals(item, downloadPage.GetCurrentPageType()));
                                downloadPage.NavigateTo(downloadPage.PageList[1], null, 1 > currentIndex);
                            }
                            else
                            {
                                downloadPage.SetNavigateContent(true, downloadPage.PageList[1]);
                            }
                        }
                    }
                }
            }
            // 从控制台启动
            else if (appLaunchArguments.AppLaunchKind is AppLaunchKind.Console)
            {
                if (GetFrameContent() is StorePage storePage)
                {
                    storePage.StoreSelector?.UpdateData(appLaunchArguments.SubParameters);
                }
                else
                {
                    NavigateTo(typeof(StorePage), appLaunchArguments.SubParameters);
                }
            }
            // 应用固定提示
            else if (appLaunchArguments.AppLaunchKind is AppLaunchKind.Pinner)
            {
                if (appLaunchArguments.SubParameters is not null && appLaunchArguments.SubParameters.Count is 2)
                {
                    if (string.Equals(appLaunchArguments.SubParameters[0], nameof(SecondaryTile)))
                    {
                        await ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.StartScreen, Convert.ToBoolean(appLaunchArguments.SubParameters[1])));
                    }
                    else if (string.Equals(appLaunchArguments.SubParameters[0], nameof(TaskbarManager)))
                    {
                        await ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, Convert.ToBoolean(appLaunchArguments.SubParameters[1])));
                    }
                }
            }
        }

        /// <summary>
        /// 检查网络状态
        /// </summary>
        private void CheckNetwork()
        {
            try
            {
                if (!NetWorkHelper.IsNetWorkConnected())
                {
                    Task.Run(() =>
                    {
                        // 显示网络连接异常通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(NetworkError1String);
                        appNotificationBuilder.AddText(NetworkError2String);
                        AppNotificationButton checkNetWorkConnectionButton = new(CheckNetWorkConnectionString);
                        checkNetWorkConnectionButton.Arguments.Add("action", "CheckNetWorkConnection");
                        appNotificationBuilder.AddButton(checkNetWorkConnectionButton);
                        AppNotification appNotification = appNotificationBuilder.BuildNotification();
                        ToastNotificationService.Show(appNotification);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(CheckNetwork), 1, e);
            }
        }

        /// <summary>
        /// 初始化导航信息列表
        /// </summary>
        private void InitializeNavigationViewItemCollection()
        {
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/Home.png") } },
                NavigationTitle = HomeString,
                NavigationTag = "Home",
                NavigationPage = typeof(HomePage)
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Seperator,
                NavigationIcon = null,
                NavigationTitle = null,
                NavigationTag = null,
                NavigationPage = null
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/Store.png") } },
                NavigationTitle = StoreString,
                NavigationTag = "Store",
                NavigationPage = typeof(StorePage)
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/AppUpdate.png") } },
                NavigationTitle = AppUpdateString,
                NavigationTag = "AppUpdate",
                NavigationPage = typeof(AppUpdatePage)
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Seperator,
                NavigationIcon = null,
                NavigationTitle = null,
                NavigationTag = null,
                NavigationPage = null
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/WinGet.png") } },
                NavigationTitle = WinGetString,
                NavigationTag = "WinGet",
                NavigationPage = typeof(WinGetPage)
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/AppManager.png") } },
                NavigationTitle = AppManagerString,
                NavigationTag = "AppManager",
                NavigationPage = typeof(AppManagerPage)
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Seperator,
                NavigationIcon = null,
                NavigationTitle = null,
                NavigationTag = null,
                NavigationPage = null
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/Download.png") } },
                NavigationTitle = DownloadString,
                NavigationTag = "Download",
                NavigationPage = typeof(DownloadPage)
            });
            NavigationViewItemMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/Web.png") } },
                NavigationTitle = WebString,
                NavigationTag = "Web",
                NavigationPage = null
            });
            NavigationViewItemFooterMenuItemsCollection.Add(new()
            {
                NavigationViewItemKind = NavigationViewItemKind.Item,
                NavigationIcon = new ImageIcon() { Source = new BitmapImage() { UriSource = new("ms-appx:///Assets/Icon/Control/Settings.png") } },
                NavigationTitle = SettingsString,
                NavigationTag = "Settings",
                NavigationPage = typeof(SettingsPage)
            });
        }

        /// <summary>
        /// 固定到开始屏幕
        /// </summary>
        private async Task PinToStartScreenAsync(string displayName, string tag)
        {
            if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(tag))
            {
                if (RuntimeHelper.IsElevated)
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await global::Windows.System.Launcher.LaunchUriAsync(new("getstoreapppinner:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                            {
                                {"Type", nameof(SecondaryTile) },
                                { "DisplayName", displayName },
                                { "Tag", tag },
                                { "Position", "StartScreen" }
                            });
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                    });
                }
                else
                {
                    bool isPinnedSuccessfully = false;

                    try
                    {
                        SecondaryTile secondaryTile = new(nameof(GetStoreApp) + tag)
                        {
                            DisplayName = displayName,
                            Arguments = "SecondaryTile " + tag
                        };

                        secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                        secondaryTile.VisualElements.Square150x150Logo = new(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square71x71Logo = new(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square44x44Logo = new(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                        InitializeWithWindow.Initialize(secondaryTile, Win32Interop.GetWindowFromWindowId(AppWindow.Id));
                        isPinnedSuccessfully = await secondaryTile.RequestCreateAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(OnPinToStartScreenExecuteRequested), 1, e);
                    }
                    finally
                    {
                        await ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.StartScreen, isPinnedSuccessfully));
                    }
                }
            }
        }

        /// <summary>
        /// 固定到任务栏
        /// </summary>
        private async Task PinToTaskbarAsync(string displayName, string tag)
        {
            if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(tag))
            {
                (bool needUnlock, LimitedAccessFeatureStatus limitedAccessFeatureStatus, bool isPinnedSuccessfully) pinnedResult = await Task.Run(async () =>
                {
                    LimitedAccessFeatureStatus limitedAccessFeatureStatus = LimitedAccessFeatureStatus.Unknown;
                    bool needUnlock = false;
                    bool isPinnedSuccessfully = false;

                    if (RuntimeHelper.IsElevated)
                    {
                        await global::Windows.System.Launcher.LaunchUriAsync(new("getstoreapppinner:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                            {
                                {"Type", nameof(SecondaryTile) },
                                { "DisplayName", displayName },
                                { "Tag", tag },
                                { "Position", "Taskbar" }
                            });
                    }
                    else
                    {
                        try
                        {
                            SecondaryTile secondaryTile = new(nameof(GetStoreApp) + tag)
                            {
                                DisplayName = displayName,
                                Arguments = "SecondaryTile " + tag
                            };

                            secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                            secondaryTile.VisualElements.Square150x150Logo = new(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                            secondaryTile.VisualElements.Square71x71Logo = new(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                            secondaryTile.VisualElements.Square44x44Logo = new(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                            string feature = "com.microsoft.windows.taskbar.requestPinSecondaryTile";
                            string featureId = FeatureAccessHelper.GetFeatureId(feature);
                            if (!string.IsNullOrEmpty(featureId))
                            {
                                needUnlock = true;
                                string token = FeatureAccessHelper.GenerateTokenFromFeatureId(feature, featureId);
                                string attestation = FeatureAccessHelper.GenerateAttestation(feature);
                                LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(feature, token, attestation);

                                if (accessResult.Status is LimitedAccessFeatureStatus.Available || accessResult.Status is LimitedAccessFeatureStatus.AvailableWithoutToken)
                                {
                                    isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                                }
                            }
                            else
                            {
                                isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(OnPinToTaskbarExecuteRequested), 1, e);
                        }

                        if (needUnlock && (limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unavailable || limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unknown) && !isPinnedSuccessfully)
                        {
                            await global::Windows.System.Launcher.LaunchUriAsync(new("getstoreapppinner:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                                {
                                    {"Type", nameof(SecondaryTile) },
                                    { "DisplayName", displayName },
                                    { "Tag", tag },
                                    { "Position", "Taskbar" }
                                });
                        }
                    }

                    return ValueTuple.Create(needUnlock, limitedAccessFeatureStatus, isPinnedSuccessfully);
                });

                if (!RuntimeHelper.IsElevated)
                {
                    if (pinnedResult.needUnlock)
                    {
                        if (pinnedResult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available || pinnedResult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.AvailableWithoutToken)
                        {
                            await ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, pinnedResult.isPinnedSuccessfully));
                        }
                    }
                    else
                    {
                        await ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, pinnedResult.isPinnedSuccessfully));
                    }
                }
            }
        }

        /// <summary>
        /// 打开网页浏览器
        /// </summary>
        private void OpenWebView()
        {
            Task.Run(async () =>
            {
                try
                {
                    await global::Windows.System.Launcher.LaunchUriAsync(new("getstoreappwebview:"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        #endregion 第六部分：数据操作与业务逻辑
    }
}

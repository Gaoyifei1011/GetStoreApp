using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Comctl32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using GetStoreApp.WindowsAPI.PInvoke.Uxtheme;
using Microsoft.Graphics.Display;
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
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.StartScreen;
using WinRT;
using WinRT.Interop;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ContentCoordinateConverter contentCoordinateConverter;
        private readonly ContentIsland contentIsland;
        private readonly DisplayInformation displayInformation;
        private readonly IDisplayInformation2 displayInformation2;
        private readonly InputNonClientPointerSource inputNonClientPointerSource;
        private readonly OverlappedPresenter overlappedPresenter;
        private readonly SUBCLASSPROC mainWindowSubClassProc;

        public new static MainWindow Current { get; private set; }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                }
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                if (!Equals(_isBackEnabled, value))
                {
                    _isBackEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBackEnabled)));
                }
            }
        }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            KeyValuePair.Create("Store",typeof(StorePage)),
            KeyValuePair.Create("AppUpdate", typeof(AppUpdatePage)),
            KeyValuePair.Create("WinGet", typeof(WinGetPage)),
            KeyValuePair.Create("AppManager", typeof(AppManagerPage)),
            KeyValuePair.Create("Download", typeof(DownloadPage)),
            KeyValuePair.Create<string, Type>("Web",null),
            KeyValuePair.Create("About", typeof(AboutPage)),
            KeyValuePair.Create("Settings", typeof(SettingsPage))
        ];

        public List<NavigationModel> NavigationItemList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            Current = this;
            InitializeComponent();

            // 窗口部分初始化
            overlappedPresenter = AppWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(AppWindow.Id);
            displayInformation = DisplayInformation.CreateForWindowId(AppWindow.Id);
            displayInformation2 = displayInformation.As<IDisplayInformation2>();
            contentIsland = ContentIsland.FindAllForCompositor(Compositor)[0];
            contentIsland.Environment.SettingChanged += OnSettingChanged;
            InputKeyboardSource inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;

            // 标题栏和右键菜单设置
            SetClassicMenuTheme((Content as FrameworkElement).ActualTheme);
            inputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(AppWindow.Id);

            // 挂载相应的事件
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            ApplicationData.Current.DataChanged += OnDataChanged;
            ThemeService.PropertyChanged += OnServicePropertyChanged;
            BackdropService.PropertyChanged += OnServicePropertyChanged;
            TopMostService.PropertyChanged += OnServicePropertyChanged;

            // 为应用主窗口添加窗口过程
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0, IntPtr.Zero);

            SetWindowTheme();
            SetSystemBackdrop();
            SetTopMost();
            CheckNetwork();
        }

        #region 第一部分：窗口类事件

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                if (AppWindow.IsVisible && SystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                {
                    materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue || args.WindowActivationState is not WindowActivationState.Deactivated;

                    NotificationService.UpdateNotificationSetting();
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

            if (displayInformation2 is not null && displayInformation2.GetRawPixelsPerViewPixel(out double rawPixelsPerViewPixel) is 0 && AppTitlebar.IsLoaded)
            {
                inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption,
                    [new RectInt32(
                        (int)(AppTitlebar.Margin.Left * rawPixelsPerViewPixel),
                        (int)AppTitlebar.Margin.Top,
                        (int)(AppWindow.Size.Width - AppTitlebar.Margin.Left * rawPixelsPerViewPixel),
                        (int)(AppTitlebar.ActualHeight * rawPixelsPerViewPixel))
                    ]);
            }
        }

        #endregion 第一部分：窗口类事件

        #region 第二部分：窗口辅助类挂载的事件

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
        private async void OnAppWindowClosing(object sender, AppWindowClosingEventArgs args)
        {
            args.Cancel = true;

            List<DownloadSchedulerModel> downloadSchedulerList = null;
            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();
            try
            {
                downloadSchedulerList = DownloadSchedulerService.GetDownloadSchedulerList();
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Query download scheduler list failed", e);
            }
            finally
            {
                DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
            }

            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
            if (downloadSchedulerList is not null && downloadSchedulerList.Count > 0)
            {
                Show();

                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                ContentDialogResult result = await ContentDialogHelper.ShowAsync(new ClosingWindowDialog(), Content as FrameworkElement);

                if (result is ContentDialogResult.Primary)
                {
                    AppWindow.Changed -= OnAppWindowChanged;
                    contentIsland.Environment.SettingChanged -= OnSettingChanged;
                    ApplicationData.Current.DataChanged -= OnDataChanged;
                    ThemeService.PropertyChanged -= OnServicePropertyChanged;
                    BackdropService.PropertyChanged -= OnServicePropertyChanged;
                    TopMostService.PropertyChanged -= OnServicePropertyChanged;
                    DownloadSchedulerService.TerminateDownload();
                    Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0);
                    (Application.Current as WinUIApp).Dispose();
                }
                else if (result is ContentDialogResult.Secondary)
                {
                    if (GetCurrentPageType() != typeof(DownloadPage))
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            else
            {
                AppWindow.Changed -= OnAppWindowChanged;
                contentIsland.Environment.SettingChanged -= OnSettingChanged;
                ApplicationData.Current.DataChanged -= OnDataChanged;
                ThemeService.PropertyChanged -= OnServicePropertyChanged;
                BackdropService.PropertyChanged -= OnServicePropertyChanged;
                TopMostService.PropertyChanged -= OnServicePropertyChanged;
                Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0);
                (Application.Current as WinUIApp).Dispose();
            }
        }

        /// <summary>
        /// 内容岛设置发生更改时触发的事件
        /// </summary>
        private void OnSettingChanged(ContentIslandEnvironment sender, ContentEnvironmentSettingChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (ThemeService.AppTheme.Equals(ThemeService.ThemeList[0]))
                {
                    WindowTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                }

                StoreRegionService.UpdateDefaultRegion();
            });
        }

        /// <summary>
        /// 处理键盘 Alt + Space 键弹出窗口右键菜单事件
        /// </summary>
        private void OnSystemKeyDown(InputKeyboardSource sender, KeyEventArgs args)
        {
            if (args.VirtualKey is VirtualKey.Space)
            {
                args.Handled = true;
                FlyoutShowOptions options = new()
                {
                    Position = new Point(0, 45),
                    ShowMode = FlyoutShowMode.Standard
                };
                TitlebarMenuFlyout.ShowAt(null, options);
            }
        }

        #endregion 第二部分：窗口辅助类挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_CLOSE, 0);
        }

        #endregion 第三部分：窗口右键菜单事件

        #region 第四部分：窗口内容挂载的事件

        /// <summary>
        /// 应用主题变化时设置标题栏按钮的颜色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(sender.ActualTheme);
            SetClassicMenuTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 按下 Alt + BackSpace 键时，导航控件返回到上一页
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Back && args.KeyStatus.IsMenuKeyDown)
            {
                if (WindowFrame.Content is AppManagerPage appManagerPage && appManagerPage.BreadCollection.Count is 2)
                {
                    appManagerPage.BackToAppList();
                }
                else
                {
                    NavigationFrom();
                }
            }
        }

        /// <summary>
        /// 固定到开始屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            bool isPinnedSuccessfully = false;

            try
            {
                if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
                {
                    string tag = Convert.ToString(menuFlyoutItem.Tag);
                    string displayName = string.Empty;

                    switch (tag)
                    {
                        case "Store": displayName = WindowStoreText.Text; break;
                        case "AppUpdate": displayName = WindowAppUpdateText.Text; break;
                        case "WinGet": displayName = WindowWinGetText.Text; break;
                        case "AppManager": displayName = WindowAppManagerText.Text; break;
                        case "Download": displayName = WindowDownloadText.Text; break;
                        case "Web": displayName = WindowWebText.Text; break;
                        case "About": displayName = WindowAboutText.Text; break;
                        case "Settings": displayName = WindowSettingsText.Text; break;
                    }

                    SecondaryTile secondaryTile = new("GetStoreApp" + tag)
                    {
                        DisplayName = displayName,
                        Arguments = "SecondaryTile " + tag
                    };

                    secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                    secondaryTile.VisualElements.Square150x150Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                    secondaryTile.VisualElements.Square71x71Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                    secondaryTile.VisualElements.Square44x44Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));

                    secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                    InitializeWithWindow.Initialize(secondaryTile, Win32Interop.GetWindowFromWindowId(AppWindow.Id));
                    isPinnedSuccessfully = await secondaryTile.RequestCreateAsync();
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Pin app to startscreen failed.", e);
            }
            finally
            {
                await TeachingTipHelper.ShowAsync(new QuickOperationTip(QuickOperationKind.StartScreen, isPinnedSuccessfully));
            }
        }

        #endregion 第四部分：窗口内容挂载的事件

        #region 第五部分：导航控件及其内容挂载的事件

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            // 设置标题栏主题
            SetTitleBarTheme((Content as FrameworkElement).ActualTheme);

            // 导航控件加载完成后初始化内容
            if (sender is NavigationView navigationView)
            {
                foreach (object menuItem in navigationView.MenuItems)
                {
                    if (menuItem is NavigationViewItem navigationViewItem)
                    {
                        int TagIndex = Convert.ToInt32(navigationViewItem.Tag);

                        NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[TagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[TagIndex].Value,
                        });
                    }
                }

                foreach (object footerMenuItem in navigationView.FooterMenuItems)
                {
                    if (footerMenuItem is NavigationViewItem navigationViewItem)
                    {
                        int TagIndex = Convert.ToInt32(navigationViewItem.Tag);

                        NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[TagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[TagIndex].Value,
                        });
                    }
                }

                NavigateTo(typeof(StorePage));
                SelectedItem = NavigationItemList[0].NavigationItem;
                IsBackEnabled = CanGoBack();
            }

            // 初始化启动信息
            StorageDataKind dataKind = ResultService.GetStorageDataKind();
            string dataContent = ResultService.ReadResult(dataKind);
            ResultService.SaveResult(StorageDataKind.None, string.Empty);

            if (dataKind is StorageDataKind.Launch)
            {
                string[] startupArgs = dataContent.Split(' ');

                // 正常启动重定向获得的内容
                if (startupArgs.Length >= 1)
                {
                    if (startupArgs[0] is "Launch")
                    {
                        Show();

                        // 应用已经启动
                        if (startupArgs.Length is 2 && startupArgs[1] is "IsRunning")
                        {
                            await ContentDialogHelper.ShowAsync(new AppRunningDialog(), Content as FrameworkElement);
                        }
                    }
                    else if (startupArgs[0] is "Settings")
                    {
                        if (GetCurrentPageType() != typeof(SettingsPage))
                        {
                            NavigateTo(typeof(SettingsPage));
                        }
                    }
                }
                // 带有命令参数启动重定向获得的内容
                else if (startupArgs.Length >= 2 && startupArgs[0] is "Console")
                {
                    if (GetCurrentPageType() != typeof(StorePage))
                    {
                        NavigateTo(typeof(StorePage));
                    }

                    if (startupArgs.Length is 4 && WindowFrame.Content is StorePage storePage)
                    {
                        storePage.QueryLinks.SelectedType = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.QueryLinks.TypeList[0] : storePage.QueryLinks.TypeList[Convert.ToInt32(startupArgs[1])];
                        storePage.QueryLinks.SelectedChannel = Convert.ToInt32(startupArgs[2]) is -1 ? storePage.QueryLinks.ChannelList[3] : storePage.QueryLinks.ChannelList[Convert.ToInt32(startupArgs[2])];
                        storePage.QueryLinks.LinkText = startupArgs[3] is "PlaceHolderText" ? string.Empty : startupArgs[3];
                        storePage.StoreSelectorBar.SelectedItem ??= storePage.StoreSelectorBar.Items[0];
                    }
                }
                // 跳转列表或辅助磁贴启动重定向获得的内容
                else if (startupArgs.Length >= 1 && (startupArgs[0] is "JumpList" || startupArgs[0] is "SecondaryTile"))
                {
                    if (startupArgs.Length is 2 && startupArgs[1] is "Store" && GetCurrentPageType() != typeof(StorePage))
                    {
                        NavigateTo(typeof(StorePage));
                    }
                    else if (startupArgs.Length is 2 && startupArgs[1] is "AppUpdate" && GetCurrentPageType() != typeof(AppUpdatePage))
                    {
                        NavigateTo(typeof(AppUpdatePage));
                    }
                    else if (startupArgs.Length is 2 && startupArgs[1] is "WinGet" && GetCurrentPageType() != typeof(WinGetPage))
                    {
                        NavigateTo(typeof(WinGetPage));
                    }
                    else if (startupArgs.Length is 2 && startupArgs[1] is "AppManager" && GetCurrentPageType() != typeof(AppManagerPage))
                    {
                        NavigateTo(typeof(AppManagerPage));
                    }
                    else if (startupArgs.Length is 2 && startupArgs[1] is "Download" && GetCurrentPageType() != typeof(DownloadPage))
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            // 从共享目标启动重定向获得的内容
            else if (dataKind is StorageDataKind.ShareTarget)
            {
                string[] startupArgs = dataContent.Split(' ');

                if (GetCurrentPageType() != typeof(StorePage))
                {
                    NavigateTo(typeof(StorePage));
                }

                if (startupArgs.Length is 3 && WindowFrame.Content is StorePage storePage)
                {
                    storePage.QueryLinks.SelectedType = Convert.ToInt32(startupArgs[0]) is -1 ? storePage.QueryLinks.TypeList[0] : storePage.QueryLinks.TypeList[Convert.ToInt32(startupArgs[0])];
                    storePage.QueryLinks.SelectedChannel = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.QueryLinks.ChannelList[3] : storePage.QueryLinks.ChannelList[Convert.ToInt32(startupArgs[1])];
                    storePage.QueryLinks.LinkText = startupArgs[2] is "PlaceHolderText" ? string.Empty : startupArgs[2];
                    storePage.StoreSelectorBar.SelectedItem ??= storePage.StoreSelectorBar.Items[0];
                }

                Show();
            }
            // 从 Toast 通知启动重定向获得的内容
            else if (dataKind is StorageDataKind.ToastNotification)
            {
                string[] startupArgs = dataContent.Split(' ');

                if (startupArgs.Length is 2 && startupArgs[0] is "ToastNotification" && startupArgs[1] is "OpenApp")
                {
                    if (GetCurrentPageType() != typeof(StorePage))
                    {
                        NavigateTo(typeof(StorePage));
                    }
                }
                else if (startupArgs.Length is 2 && startupArgs[0] is "ToastNotification" && startupArgs[1] is "ViewDownloadPage")
                {
                    if (GetCurrentPageType() != typeof(DownloadPage))
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }

                Show();
            }

            if (displayInformation2 is not null && displayInformation2.GetRawPixelsPerViewPixel(out double rawPixelsPerViewPixel) is 0 && AppTitlebar.IsLoaded)
            {
                inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption,
                         [new RectInt32(
                        (int)(AppTitlebar.Margin.Left * rawPixelsPerViewPixel),
                        (int)AppTitlebar.Margin.Top,
                        (int)(AppWindow.Size.Width - AppTitlebar.Margin.Left * rawPixelsPerViewPixel),
                        (int)(AppTitlebar.ActualHeight * rawPixelsPerViewPixel))
                         ]);
            }
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            if (WindowFrame.Content is AppManagerPage appManagerPage && appManagerPage.BreadCollection.Count is 2)
            {
                appManagerPage.BackToAppList();
            }
            else
            {
                NavigationFrom();
            }
        }

        /// <summary>
        /// 在当前导航控件所选项更改时发生
        /// </summary>
        private async void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer is NavigationViewItemBase navigationViewItem && navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => item.NavigationTag == PageList[Convert.ToInt32(navigationViewItem.Tag)].Key);

                if (SelectedItem != navigationItem.NavigationItem)
                {
                    if (PageList[Convert.ToInt32(navigationItem.NavigationItem.Tag)].Key is "Web")
                    {
                        await Launcher.LaunchUriAsync(new Uri("webbrowser:"));
                        sender.SelectedItem = SelectedItem;
                    }
                    else
                    {
                        NavigateTo(navigationItem.NavigationPage);
                    }
                }
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = GetCurrentPageType();
            SelectedItem = NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
            (Application.Current as WinUIApp).Dispose();
        }

        #endregion 第五部分：导航控件及其内容挂载的事件

        #region 第六部分：自定义事件

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
        private void OnDataChanged(ApplicationData sender, object args)
        {
            StorageDataKind dataKind = ResultService.GetStorageDataKind();
            string dataContent = ResultService.ReadResult(dataKind);
            ResultService.SaveResult(StorageDataKind.None, string.Empty);

            if (dataKind is StorageDataKind.Launch)
            {
                string[] startupArgs = dataContent.Split(' ');

                // 正常启动重定向获得的内容
                if (startupArgs.Length >= 1)
                {
                    if (startupArgs[0] is "Launch")
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            Show();

                            // 应用已经启动
                            if (startupArgs.Length is 2 && startupArgs[1] is "IsRunning")
                            {
                                await ContentDialogHelper.ShowAsync(new AppRunningDialog(), Content as FrameworkElement);
                            }
                        });
                    }
                    else if (startupArgs[0] is "Settings")
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            if (GetCurrentPageType() != typeof(SettingsPage))
                            {
                                NavigateTo(typeof(SettingsPage));
                            }
                        });
                    }
                }
                // 带有命令参数启动重定向获得的内容
                else if (startupArgs.Length >= 2 && startupArgs[0] is "Console")
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        if (GetCurrentPageType() != typeof(StorePage))
                        {
                            NavigateTo(typeof(StorePage));
                        }

                        if (startupArgs.Length is 4 && WindowFrame.Content is StorePage storePage)
                        {
                            storePage.QueryLinks.SelectedType = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.QueryLinks.TypeList[0] : storePage.QueryLinks.TypeList[Convert.ToInt32(startupArgs[1])];
                            storePage.QueryLinks.SelectedChannel = Convert.ToInt32(startupArgs[2]) is -1 ? storePage.QueryLinks.ChannelList[3] : storePage.QueryLinks.ChannelList[Convert.ToInt32(startupArgs[2])];
                            storePage.QueryLinks.LinkText = startupArgs[3] is "PlaceHolderText" ? string.Empty : startupArgs[3];
                            storePage.StoreSelectorBar.SelectedItem ??= storePage.StoreSelectorBar.Items[0];
                        }
                    });
                }
                // 跳转列表或辅助磁贴启动重定向获得的内容
                else if (startupArgs.Length >= 1 && (startupArgs[0] is "JumpList" || startupArgs[0] is "SecondaryTile"))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        if (startupArgs.Length is 2 && startupArgs[1] is "Store" && GetCurrentPageType() != typeof(StorePage))
                        {
                            NavigateTo(typeof(StorePage));
                        }
                        else if (startupArgs.Length is 2 && startupArgs[1] is "AppUpdate" && GetCurrentPageType() != typeof(AppUpdatePage))
                        {
                            NavigateTo(typeof(AppUpdatePage));
                        }
                        else if (startupArgs.Length is 2 && startupArgs[1] is "WinGet" && GetCurrentPageType() != typeof(WinGetPage))
                        {
                            NavigateTo(typeof(WinGetPage));
                        }
                        else if (startupArgs.Length is 2 && startupArgs[1] is "AppManager" && GetCurrentPageType() != typeof(AppManagerPage))
                        {
                            NavigateTo(typeof(AppManagerPage));
                        }
                        else if (startupArgs.Length is 2 && startupArgs[1] is "Download" && GetCurrentPageType() != typeof(DownloadPage))
                        {
                            NavigateTo(typeof(DownloadPage));
                        }
                    });
                }
            }
            // 从共享目标启动重定向获得的内容
            else if (dataKind is StorageDataKind.ShareTarget)
            {
                string[] startupArgs = dataContent.Split(' ');

                DispatcherQueue.TryEnqueue(() =>
                {
                    if (GetCurrentPageType() != typeof(StorePage))
                    {
                        NavigateTo(typeof(StorePage));
                    }

                    if (startupArgs.Length is 3 && WindowFrame.Content is StorePage storePage)
                    {
                        storePage.QueryLinks.SelectedType = Convert.ToInt32(startupArgs[0]) is -1 ? storePage.QueryLinks.TypeList[0] : storePage.QueryLinks.TypeList[Convert.ToInt32(startupArgs[0])];
                        storePage.QueryLinks.SelectedChannel = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.QueryLinks.ChannelList[3] : storePage.QueryLinks.ChannelList[Convert.ToInt32(startupArgs[1])];
                        storePage.QueryLinks.LinkText = startupArgs[2] is "PlaceHolderText" ? string.Empty : startupArgs[2];
                        storePage.StoreSelectorBar.SelectedItem ??= storePage.StoreSelectorBar.Items[0];
                    }

                    Show();
                });
            }
            // 从 Toast 通知启动重定向获得的内容
            else if (dataKind is StorageDataKind.ToastNotification)
            {
                string[] startupArgs = dataContent.Split(' ');

                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                {
                    if (startupArgs.Length is 2 && startupArgs[0] is "ToastNotification" && startupArgs[1] is "OpenApp")
                    {
                        if (GetCurrentPageType() != typeof(StorePage))
                        {
                            NavigateTo(typeof(StorePage));
                        }
                    }
                    else if (startupArgs.Length is 2 && startupArgs[0] is "ToastNotification" && startupArgs[1] is "ViewDownloadPage")
                    {
                        if (GetCurrentPageType() != typeof(DownloadPage))
                        {
                            NavigateTo(typeof(DownloadPage));
                        }
                    }

                    Show();
                });
            }
            // 任务栏应用固定结果
            else if (dataKind is StorageDataKind.TaskbarPinnedResult)
            {
                DispatcherQueue.TryEnqueue(async () =>
                {
                    await TeachingTipHelper.ShowAsync(new QuickOperationTip(QuickOperationKind.Taskbar, Convert.ToBoolean(dataContent)));
                });
            }
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (args.PropertyName.Equals(nameof(ThemeService.AppTheme)))
                {
                    SetWindowTheme();
                }
                if (args.PropertyName.Equals(nameof(BackdropService.AppBackdrop)))
                {
                    SetSystemBackdrop();
                }
                if (args.PropertyName.Equals(nameof(TopMostService.TopMostValue)))
                {
                    SetTopMost();
                }
            });
        }

        #endregion 第六部分：自定义事件

        #region 第七部分：窗口属性设置

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public void SetWindowTheme()
        {
            WindowTheme = ThemeService.AppTheme.Equals(ThemeService.ThemeList[0])
                ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark
                : Enum.TryParse(ThemeService.AppTheme.Key, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;
        }

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        private void SetSystemBackdrop()
        {
            if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[1]))
            {
                SystemBackdrop = new MaterialBackdrop(MicaKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[2]))
            {
                SystemBackdrop = new MaterialBackdrop(MicaKind.BaseAlt);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[3]))
            {
                SystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Default);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[4]))
            {
                SystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (BackdropService.AppBackdrop.Equals(BackdropService.BackdropList[5]))
            {
                SystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Thin);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else
            {
                SystemBackdrop = null;
                VisualStateManager.GoToState(MainPage, "BackgroundDefault", false);
            }

            if (SystemBackdrop is not null && AlwaysShowBackdropService.AlwaysShowBackdropValue)
            {
                (SystemBackdrop as MaterialBackdrop).BackdropConfiguration.IsInputActive = true;
            }
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(9, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(6, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(15, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(10, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private static void SetClassicMenuTheme(ElementTheme theme)
        {
            if (theme is ElementTheme.Light)
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public void Show(bool isFirstActivate = false)
        {
            if (isFirstActivate)
            {
                bool? isWindowMaximized = LocalSettingsService.ReadSetting<bool?>(ConfigKey.IsWindowMaximizedKey);
                int? windowWidth = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowWidthKey);
                int? windowHeight = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowHeightKey);
                int? windowPositionXAxis = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowPositionXAxisKey);
                int? windowPositionYAxis = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowPositionYAxisKey);

                if (isWindowMaximized.HasValue && isWindowMaximized.Value)
                {
                    overlappedPresenter.Maximize();
                }
                else
                {
                    if (windowWidth.HasValue && windowHeight.HasValue && windowPositionXAxis.HasValue && windowPositionYAxis.HasValue)
                    {
                        AppWindow.MoveAndResize(new RectInt32(windowPositionXAxis.Value, windowPositionYAxis.Value, windowWidth.Value, windowHeight.Value));
                    }
                }
            }
            else
            {
                if (overlappedPresenter.IsMinimizable)
                {
                    overlappedPresenter.Restore();
                }
            }

            Activate();
        }

        /// <summary>
        /// 设置窗口的置顶状态
        /// </summary>
        private void SetTopMost()
        {
            overlappedPresenter.IsAlwaysOnTop = TopMostService.TopMostValue;
        }

        /// <summary>
        /// 关闭窗口时保存窗口的大小和位置信息
        /// </summary>
        public void SaveWindowInformation()
        {
            LocalSettingsService.SaveSetting(ConfigKey.IsWindowMaximizedKey, IsWindowMaximized);
            LocalSettingsService.SaveSetting(ConfigKey.WindowWidthKey, AppWindow.Size.Width);
            LocalSettingsService.SaveSetting(ConfigKey.WindowHeightKey, AppWindow.Size.Height);
            LocalSettingsService.SaveSetting(ConfigKey.WindowPositionXAxisKey, AppWindow.Position.X);
            LocalSettingsService.SaveSetting(ConfigKey.WindowPositionYAxisKey, AppWindow.Position.Y);
        }

        #endregion 第七部分：窗口属性设置

        #region 第八部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        if (displayInformation2 is not null && displayInformation2.GetRawPixelsPerViewPixel(out double rawPixelsPerViewPixel) is 0)
                        {
                            MINMAXINFO minMaxInfo;

                            unsafe
                            {
                                minMaxInfo = *(MINMAXINFO*)lParam;
                            }
                            minMaxInfo.ptMinTrackSize.X = (int)(960 * rawPixelsPerViewPixel);
                            minMaxInfo.ptMinTrackSize.Y = (int)(600 * rawPixelsPerViewPixel);

                            unsafe
                            {
                                *(MINMAXINFO*)lParam = minMaxInfo;
                            }
                        }

                        break;
                    }
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
                        if (wParam is 2 && displayInformation2 is not null && displayInformation2.GetRawPixelsPerViewPixel(out double rawPixelsPerViewPixel) is 0)
                        {
                            PointInt32 screenPoint = new(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = new Point(localPoint.X / rawPixelsPerViewPixel, localPoint.Y / rawPixelsPerViewPixel)
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

        #endregion 第八部分：窗口过程

        #region 第九部分：窗口导航方法

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null)
        {
            foreach (NavigationModel navigationItem in NavigationItemList)
            {
                if (navigationItem.NavigationPage == navigationPageType)
                {
                    WindowFrame.Navigate(navigationItem.NavigationPage, parameter);
                }
            }
        }

        /// <summary>
        /// 页面向后导航
        /// </summary>
        public void NavigationFrom()
        {
            if (WindowFrame.CanGoBack)
            {
                WindowFrame.GoBack();
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return WindowFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 检查当前页面是否能向后导航
        /// </summary>
        public bool CanGoBack()
        {
            return WindowFrame.CanGoBack;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        public object GetFrameContent()
        {
            return WindowFrame.Content;
        }

        #endregion 第九部分：窗口导航方法

        /// <summary>
        /// 检查网络状态
        /// </summary>
        private void CheckNetwork()
        {
            try
            {
                ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                bool isConnected = connectionProfile is not null && connectionProfile.GetNetworkConnectivityLevel() is NetworkConnectivityLevel.InternetAccess;

                if (!isConnected)
                {
                    Task.Run(() =>
                    {
                        // 显示网络连接异常通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/NetworkError1")));
                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/NetworkError2")));
                        AppNotificationButton checkNetWorkConnectionButton = new(ResourceService.GetLocalized("Notification/CheckNetWorkConnection"));
                        checkNetWorkConnectionButton.Arguments.Add("action", "CheckNetWorkConnection");
                        appNotificationBuilder.AddButton(checkNetWorkConnectionButton);
                        AppNotification appNotification = appNotificationBuilder.BuildNotification();
                        ToastNotificationService.Show(appNotification);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Network state check failed", e);
            }
        }
    }
}

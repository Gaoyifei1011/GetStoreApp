using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.TeachingTips;
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
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.StartScreen;
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
        private readonly SUBCLASSPROC mainWindowSubClassProc;
        private readonly InputNonClientPointerSource inputNonClientPointerSource;
        private readonly ContentCoordinateConverter contentCoordinateConverter;
        private readonly OverlappedPresenter overlappedPresenter;

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
            Comctl32Library.SetWindowSubclass((IntPtr)AppWindow.Id.Value, mainWindowSubClassProc, 0, IntPtr.Zero);

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
                if (Visible && SystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                {
                    materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue || args.WindowActivationState is not WindowActivationState.Deactivated;
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

            if (Content is not null && Content.XamlRoot is not null)
            {
                inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption, [new RectInt32((int)(AppTitlebar.Margin.Left * Content.XamlRoot.RasterizationScale), (int)AppTitlebar.Margin.Top, (int)(AppWindow.Size.Width - AppTitlebar.Margin.Left * Content.XamlRoot.RasterizationScale), (int)(AppTitlebar.ActualHeight * Content.XamlRoot.RasterizationScale))]);
            }
        }

        #endregion 第一部分：窗口类事件

        #region 第二部分：窗口辅助类（AppWindow）挂载的事件

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
                    ApplicationData.Current.DataChanged -= OnDataChanged;
                    ThemeService.PropertyChanged -= OnServicePropertyChanged;
                    BackdropService.PropertyChanged -= OnServicePropertyChanged;
                    TopMostService.PropertyChanged -= OnServicePropertyChanged;
                    DownloadSchedulerService.TerminateDownload();
                    Comctl32Library.RemoveWindowSubclass((IntPtr)AppWindow.Id.Value, mainWindowSubClassProc, 0);
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
                ApplicationData.Current.DataChanged -= OnDataChanged;
                ThemeService.PropertyChanged -= OnServicePropertyChanged;
                BackdropService.PropertyChanged -= OnServicePropertyChanged;
                TopMostService.PropertyChanged -= OnServicePropertyChanged;
                Comctl32Library.RemoveWindowSubclass((IntPtr)AppWindow.Id.Value, mainWindowSubClassProc, 0);
                (Application.Current as WinUIApp).Dispose();
            }
        }

        #endregion 第二部分：窗口辅助类（AppWindow）挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Restore();
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage((IntPtr)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, 0xF010, 0);
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
                User32Library.SendMessage((IntPtr)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, 0xF000, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Minimize();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Maximize();
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            (Application.Current as WinUIApp).Dispose();
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

                    SecondaryTile secondaryTile = new("GetStoreApp" + tag)
                    {
                        DisplayName = ResourceService.GetLocalized("Resources/AppDisplayName"),
                        Arguments = "SecondaryTile " + tag
                    };

                    secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                    secondaryTile.VisualElements.Square150x150Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                    secondaryTile.VisualElements.Square71x71Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                    secondaryTile.VisualElements.Square44x44Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));

                    secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                    InitializeWithWindow.Initialize(secondaryTile, (IntPtr)AppWindow.Id.Value);
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
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
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

                SelectedItem = NavigationItemList[0].NavigationItem;
                NavigateTo(typeof(StorePage));
                if (DesktopLaunchService.InitializePage != typeof(StorePage))
                {
                    NavigateTo(DesktopLaunchService.InitializePage);
                }
                IsBackEnabled = CanGoBack();
            }

            inputNonClientPointerSource.SetRegionRects(NonClientRegionKind.Caption, [new RectInt32((int)(45 * Content.XamlRoot.RasterizationScale), 0, (int)((AppWindow.Size.Width - 45) * Content.XamlRoot.RasterizationScale), (int)(45 * Content.XamlRoot.RasterizationScale))]);
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
        private async void OnDataChanged(ApplicationData sender, object args)
        {
            StorageDataKind dataKind = ResultService.GetStorageDataKind();

            // 正常启动
            if (dataKind is StorageDataKind.DesktopLaunch)
            {
                string dataContent = ResultService.ReadResult(dataKind);
                ResultService.SaveResult(StorageDataKind.None, string.Empty);

                DispatcherQueue.TryEnqueue(() =>
                {
                    Show();

                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                    {
                        await ContentDialogHelper.ShowAsync(new AppRunningDialog(), Content as FrameworkElement);
                    });
                });
            }
            // 正常启动或从共享启动处启动，应用可能会附带启动参数
            else if (dataKind is StorageDataKind.CommandLineLaunch || dataKind is StorageDataKind.ShareTarget)
            {
                string dataContent = ResultService.ReadResult(dataKind);
                ResultService.SaveResult(StorageDataKind.None, string.Empty);

                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                {
                    string[] startupArgs = dataContent.Split(' ');

                    if (startupArgs.Length is 2 && (startupArgs[0] is "JumpList" || startupArgs[0] is "SecondaryTile"))
                    {
                        if (startupArgs[1] is "Store" && GetCurrentPageType() != typeof(StorePage))
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                NavigateTo(typeof(StorePage));
                            });
                        }
                        if (startupArgs[1] is "AppUpdate" && GetCurrentPageType() != typeof(AppUpdatePage))
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                NavigateTo(typeof(AppUpdatePage));
                            });
                        }
                        else if (startupArgs[1] is "WinGet" && GetCurrentPageType() != typeof(WinGetPage))
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                NavigateTo(typeof(WinGetPage));
                            });
                        }
                        else if (startupArgs[1] is "AppManager" && GetCurrentPageType() != typeof(AppManagerPage))
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                NavigateTo(typeof(AppManagerPage));
                            });
                        }
                        else if (startupArgs[1] is "Download" && GetCurrentPageType() != typeof(DownloadPage))
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                NavigateTo(typeof(DownloadPage));
                            });
                        }
                    }
                    else if (startupArgs.Length is 3)
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            if (GetCurrentPageType() != typeof(StorePage))
                            {
                                NavigateTo(typeof(StorePage));
                            }

                            if (WindowFrame.Content is StorePage storePage)
                            {
                                storePage.QueryLinks.SelectedType = Convert.ToInt32(startupArgs[0]) is -1 ? storePage.QueryLinks.TypeList[0] : storePage.QueryLinks.TypeList[Convert.ToInt32(startupArgs[0])];
                                storePage.QueryLinks.SelectedChannel = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.QueryLinks.ChannelList[3] : storePage.QueryLinks.ChannelList[Convert.ToInt32(startupArgs[1])];
                                storePage.QueryLinks.LinkText = startupArgs[2] is "PlaceHolderText" ? string.Empty : startupArgs[2];
                                storePage.StoreSelectorBar.SelectedItem ??= storePage.StoreSelectorBar.Items[0];
                            }
                        });
                    }

                    Show();
                });
            }
            // 正常启动或从共享启动处启动，应用可能会附带启动参数
            else if (dataKind is StorageDataKind.ToastNotification)
            {
                string dataContent = ResultService.ReadResult(dataKind);
                ResultService.SaveResult(StorageDataKind.None, string.Empty);
                await ToastNotificationService.HandleToastNotificationAsync(dataContent);

                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                {
                    Show();
                });
            }
            else if (dataKind is StorageDataKind.TaskbarPinnedResult)
            {
                string dataContent = ResultService.ReadResult(dataKind);
                ResultService.SaveResult(StorageDataKind.None, string.Empty);
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
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 255, 255, 255);
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
                bool? IsWindowMaximized = LocalSettingsService.ReadSetting<bool?>(ConfigKey.IsWindowMaximizedKey);
                int? WindowWidth = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowWidthKey);
                int? WindowHeight = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowHeightKey);
                int? WindowPositionXAxis = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowPositionXAxisKey);
                int? WindowPositionYAxis = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowPositionYAxisKey);

                if (IsWindowMaximized.HasValue && IsWindowMaximized.Value is true)
                {
                    overlappedPresenter.Maximize();
                }
                else
                {
                    if (WindowWidth.HasValue && WindowHeight.HasValue && WindowPositionXAxis.HasValue && WindowPositionYAxis.HasValue)
                    {
                        AppWindow.MoveAndResize(new RectInt32(
                            WindowPositionXAxis.Value,
                            WindowPositionYAxis.Value,
                            WindowWidth.Value,
                            WindowHeight.Value
                            ));
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
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            MINMAXINFO minMaxInfo;

                            unsafe
                            {
                                minMaxInfo = *(MINMAXINFO*)lParam;
                            }
                            minMaxInfo.ptMinTrackSize.X = (int)(960 * Content.XamlRoot.RasterizationScale);
                            minMaxInfo.ptMinTrackSize.Y = (int)(600 * Content.XamlRoot.RasterizationScale);

                            unsafe
                            {
                                *(MINMAXINFO*)lParam = minMaxInfo;
                            }
                        }

                        break;
                    }
                // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        if (ThemeService.AppTheme.Equals(ThemeService.ThemeList[0]))
                        {
                            WindowTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                        }

                        StoreRegionService.UpdateDefaultRegion();
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
                        if (wParam.ToUInt32() is 2 && Content is not null && Content.XamlRoot is not null)
                        {
                            PointInt32 screenPoint = new(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = InfoHelper.SystemVersion.Build >= 22000 ? new Point(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale) : new Point(localPoint.X, localPoint.Y)
                            };

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam.ToUInt32() & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new Point(0, 15),
                                ShowMode = FlyoutShowMode.Standard
                            };
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        else if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU)
                        {
                            if (lParam is (IntPtr)VirtualKey.Space)
                            {
                                FlyoutShowOptions options = new()
                                {
                                    Position = new Point(0, 45),
                                    ShowMode = FlyoutShowMode.Standard
                                };
                                TitlebarMenuFlyout.ShowAt(null, options);
                            }

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
                    ToastNotificationService.Show(NotificationKind.NetworkError);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Network state check failed", e);
            }
        }
    }
}

using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Backdrop;
using GetStoreApp.Helpers.Controls;
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
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.Shell;
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
        private readonly string CheckNetWorkConnectionString = ResourceService.GetLocalized("Window/CheckNetWorkConnection");
        private readonly string NetworkError1String = ResourceService.GetLocalized("Window/NetworkError1");
        private readonly string NetworkError2String = ResourceService.GetLocalized("Window/NetworkError2");
        private readonly string RunningAdministratorString = ResourceService.GetLocalized("Window/RunningAdministrator");
        private readonly string TitleString = ResourceService.GetLocalized("Window/Title");
        private readonly ContentIsland contentIsland;
        private readonly InputKeyboardSource inputKeyboardSource;
        private readonly InputPointerSource inputPointerSource;
        private readonly ContentCoordinateConverter contentCoordinateConverter;
        private readonly OverlappedPresenter overlappedPresenter;
        private readonly SUBCLASSPROC mainWindowSubClassProc;
        private ToolTip navigationViewBackButtonToolTip;

        public new static MainWindow Current { get; private set; }

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }

            set
            {
                if (!string.Equals(_windowTitle, value))
                {
                    _windowTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));
                }
            }
        }

        private SystemBackdrop _windowSystemBackdrop;

        public SystemBackdrop WindowSystemBackdrop
        {
            get { return _windowSystemBackdrop; }

            set
            {
                if (!Equals(_windowSystemBackdrop, value))
                {
                    _windowSystemBackdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowSystemBackdrop)));
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

        public List<KeyValuePair<string, Type>> PageList { get; } =
        [
            KeyValuePair.Create("Home",typeof(HomePage)),
            KeyValuePair.Create("Store",typeof(StorePage)),
            KeyValuePair.Create("AppUpdate", typeof(AppUpdatePage)),
            KeyValuePair.Create("WinGet", typeof(WinGetPage)),
            KeyValuePair.Create("AppManager", typeof(AppManagerPage)),
            KeyValuePair.Create("Download", typeof(DownloadPage)),
            KeyValuePair.Create<string, Type>("Web",null),
            KeyValuePair.Create("Settings", typeof(SettingsPage))
        ];

        public List<NavigationModel> NavigationItemList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            Current = this;
            InitializeComponent();

            // 窗口部分初始化
            WindowTitle = RuntimeHelper.IsElevated ? TitleString + RunningAdministratorString : TitleString;
            overlappedPresenter = AppWindow.Presenter.As<OverlappedPresenter>();
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(AppWindow.Id);
            contentIsland = ContentIsland.FindAllForCompositor(Compositor)[0];
            inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
            inputPointerSource = InputPointerSource.GetForIsland(contentIsland);
            ControlBackdropController.Initialize(Current);

            // 挂载相应的事件
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            contentIsland.StateChanged += OnStateChanged;
            contentIsland.Environment.SettingChanged += OnSettingChanged;
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;
            inputPointerSource.PointerReleased += OnPointerReleased;
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            ThemeService.PropertyChanged += OnServicePropertyChanged;
            BackdropService.PropertyChanged += OnServicePropertyChanged;
            TopMostService.PropertyChanged += OnServicePropertyChanged;
            DesktopLaunchService.AppLaunchActivated += OnAppLaunchActivated;

            // 标题栏和右键菜单设置
            SetClassicMenuTheme(Content.As<FrameworkElement>().ActualTheme);

            // 为应用主窗口添加窗口过程
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0, nint.Zero);

            SetWindowTheme();
            SetSystemBackdrop();
            SetTopMost();
            CheckNetwork();

            // 默认直接显示到窗口中间
            if (DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest) is DisplayArea displayArea && contentIsland is not null)
            {
                RectInt32 workArea = displayArea.WorkArea;
                AppWindow.Move(new PointInt32((workArea.Width - AppWindow.Size.Width) / 2, (workArea.Height - AppWindow.Size.Height) / 2));
            }
        }

        #region 第一部分：窗口类事件

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                if (contentIsland is not null && !contentIsland.IsClosed && WindowSystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                {
                    materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue || args.WindowActivationState is not WindowActivationState.Deactivated;
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
                    ControlBackdropController.UnInitialize();
                    AppWindow.Changed -= OnAppWindowChanged;
                    contentIsland.Environment.SettingChanged -= OnSettingChanged;
                    inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
                    inputPointerSource.PointerReleased -= OnPointerReleased;
                    ThemeService.PropertyChanged -= OnServicePropertyChanged;
                    BackdropService.PropertyChanged -= OnServicePropertyChanged;
                    TopMostService.PropertyChanged -= OnServicePropertyChanged;
                    DesktopLaunchService.AppLaunchActivated -= OnAppLaunchActivated;
                    if (navigationViewBackButtonToolTip is not null)
                    {
                        navigationViewBackButtonToolTip.Loaded -= ToolTipBackdropHelper.OnLoaded;
                    }
                    DownloadSchedulerService.TerminateDownload();
                    Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0);
                    (Application.Current as MainApp).Dispose();
                }
                else if (contentDialogResult is ContentDialogResult.Secondary)
                {
                    if (!Equals(GetCurrentPageType(), typeof(DownloadPage)))
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            else
            {
                ControlBackdropController.UnInitialize();
                AppWindow.Changed -= OnAppWindowChanged;
                contentIsland.Environment.SettingChanged -= OnSettingChanged;
                inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
                inputPointerSource.PointerReleased -= OnPointerReleased;
                ThemeService.PropertyChanged -= OnServicePropertyChanged;
                BackdropService.PropertyChanged -= OnServicePropertyChanged;
                TopMostService.PropertyChanged -= OnServicePropertyChanged;
                DesktopLaunchService.AppLaunchActivated -= OnAppLaunchActivated;
                if (navigationViewBackButtonToolTip is not null)
                {
                    navigationViewBackButtonToolTip.Loaded -= ToolTipBackdropHelper.OnLoaded;
                }
                Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0);
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
                if (string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]))
                {
                    WindowTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                }

                SetPopupControlTheme(WindowTheme);
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
                    Position = new Point(0, 45),
                    ShowMode = FlyoutShowMode.Standard
                };
                TitlebarMenuFlyout.ShowAt(null, options);
            }
            else if (args.VirtualKey is global::Windows.System.VirtualKey.F10 && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        /// <summary>
        /// 处理鼠标事件
        /// </summary>
        private async void OnPointerReleased(InputPointerSource sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.Properties.PointerUpdateKind is PointerUpdateKind.RightButtonReleased && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        #endregion 第二部分：窗口辅助类挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<MenuFlyoutItem>().Tag.As<MenuFlyout>() is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<MenuFlyoutItem>().Tag.As<MenuFlyout>() is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_CLOSE, 0);
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
            ControlBackdropController.UpdateControlTheme(sender.ActualTheme);
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
        /// 固定到开始屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<MenuFlyoutItem>().Tag.As<TextBlock>() is TextBlock textBlock)
            {
                string displayName = textBlock.Text;
                string tag = Convert.ToString(textBlock.Tag);

                if (RuntimeHelper.IsElevated)
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await global::Windows.System.Launcher.LaunchUriAsync(new Uri("getstoreapppinner:"), new global::Windows.System.LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
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
                        secondaryTile.VisualElements.Square150x150Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square71x71Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square44x44Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                        InitializeWithWindow.Initialize(secondaryTile, Win32Interop.GetWindowFromWindowId(AppWindow.Id));
                        isPinnedSuccessfully = await secondaryTile.RequestCreateAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(OnPinToStartScreenClicked), 1, e);
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
        private async void OnPinToTaskbarClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<MenuFlyoutItem>().Tag.As<TextBlock>() is TextBlock textBlock)
            {
                string displayName = textBlock.Text;
                string tag = Convert.ToString(textBlock.Tag);

                (LimitedAccessFeatureStatus limitedAccessFeatureStatus, bool isPinnedSuccessfully) pinnedRsult = await Task.Run(async () =>
                {
                    LimitedAccessFeatureStatus limitedAccessFeatureStatus = LimitedAccessFeatureStatus.Unknown;
                    bool isPinnedSuccessfully = false;

                    if (!RuntimeHelper.IsElevated)
                    {
                        try
                        {
                            SecondaryTile secondaryTile = new(nameof(GetStoreApp) + tag)
                            {
                                DisplayName = displayName,
                                Arguments = "SecondaryTile " + tag
                            };

                            secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                            secondaryTile.VisualElements.Square150x150Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                            secondaryTile.VisualElements.Square71x71Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                            secondaryTile.VisualElements.Square44x44Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                            string featureId = "com.microsoft.windows.taskbar.requestPinSecondaryTile";
                            string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                            string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                            LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);
                            limitedAccessFeatureStatus = accessResult.Status;

                            if (limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available)
                            {
                                isPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinSecondaryTileAsync(secondaryTile);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(MainWindow), nameof(OnPinToStartScreenClicked), 1, e);
                        }

                        if ((limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unavailable || limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Unknown) && !isPinnedSuccessfully)
                        {
                            await global::Windows.System.Launcher.LaunchUriAsync(new Uri("getstoreapppinner:"), new global::Windows.System.LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                            {
                                {"Type", nameof(SecondaryTile) },
                                { "DisplayName", displayName },
                                { "Tag", tag },
                                { "Position", "Taskbar" }
                            });
                        }
                    }

                    return ValueTuple.Create(limitedAccessFeatureStatus, isPinnedSuccessfully);
                });

                if (pinnedRsult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.Available || pinnedRsult.limitedAccessFeatureStatus is LimitedAccessFeatureStatus.AvailableWithoutToken)
                {
                    await ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.Taskbar, pinnedRsult.isPinnedSuccessfully));
                }
            }
        }

        #endregion 第四部分：窗口内容挂载的事件

        #region 第五部分：导航控件及其内容挂载的事件

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(NavigationViewItem))]
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            // 设置标题栏主题
            SetTitleBarTheme(Content.As<FrameworkElement>().ActualTheme);

            // 导航控件加载完成后初始化内容
            if (sender.As<NavigationView>() is NavigationView navigationView)
            {
                if (XamlTreeHelper.FindDescendant<Button>(navigationView, "NavigationViewBackButton") is Button navigationViewBackButton)
                {
                    navigationViewBackButtonToolTip = ToolTipService.GetToolTip(navigationViewBackButton).As<ToolTip>();

                    if (navigationViewBackButtonToolTip is not null)
                    {
                        navigationViewBackButtonToolTip.Background = new SolidColorBrush(Colors.Transparent);
                        navigationViewBackButtonToolTip.Loaded += ToolTipBackdropHelper.OnLoaded;
                    }
                }

                foreach (object menuItem in navigationView.MenuItems)
                {
                    try
                    {
                        if (menuItem is NavigationViewItem navigationViewItem && navigationViewItem.Tag is string tag)
                        {
                            int tagIndex = PageList.FindIndex(item => string.Equals(item.Key, tag));

                            NavigationItemList.Add(new NavigationModel()
                            {
                                NavigationTag = PageList[tagIndex].Key,
                                NavigationItem = navigationViewItem,
                                NavigationPage = PageList[tagIndex].Value,
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }

                foreach (object footerMenuItem in navigationView.FooterMenuItems)
                {
                    try
                    {
                        if (footerMenuItem is NavigationViewItem navigationViewItem && navigationViewItem.Tag is string tag)
                        {
                            int tagIndex = PageList.FindIndex(item => string.Equals(item.Key, tag));

                            NavigationItemList.Add(new NavigationModel()
                            {
                                NavigationTag = PageList[tagIndex].Key,
                                NavigationItem = navigationViewItem,
                                NavigationPage = PageList[tagIndex].Value,
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                }
            }

            NavigateTo(typeof(HomePage));
            SetPopupControlTheme(WindowTheme);

            // 初始化启动信息
            AppLaunchArguments appLaunchArguments = DesktopLaunchService.AppLaunchArguments;
            await ParseAppLaunchArgumentsAsync(appLaunchArguments, true);
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
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
        /// 在当前导航控件所选项更改时发生
        /// </summary>
        private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer is NavigationViewItemBase navigationViewItem && navigationViewItem.Tag is string tag)
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => string.Equals(item.NavigationTag, PageList[PageList.FindIndex(item => string.Equals(item.Key, tag))].Key));

                if (!Equals(SelectedItem, navigationItem.NavigationItem))
                {
                    if (PageList[PageList.FindIndex(item => string.Equals(item.Key, tag))].Key is "Web")
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await global::Windows.System.Launcher.LaunchUriAsync(new Uri("getstoreappwebview:"));
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                        });

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
            SelectedItem = NavigationItemList.Find(item => Equals(item.NavigationPage, GetCurrentPageType())).NavigationItem;
            IsBackEnabled = CanGoBack();
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
                if (string.Equals(args.PropertyName, nameof(TopMostService.TopMostValue)))
                {
                    SetTopMost();
                }
            });
        }

        #endregion 第六部分：自定义事件

        #region 第七部分：窗口及内容属性设置

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public void SetWindowTheme()
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
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private void SetClassicMenuTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            if (theme is ElementTheme.Light)
            {
                titleBar.PreferredTheme = TitleBarTheme.Light;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                titleBar.PreferredTheme = TitleBarTheme.Dark;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        /// <summary>
        /// 设置所有弹出控件主题
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(FlyoutPresenter)), DynamicWindowsRuntimeCast(typeof(Grid))]
        private void SetPopupControlTheme(ElementTheme elementTheme)
        {
            foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
            {
                popup.RequestedTheme = elementTheme;

                try
                {
                    if (popup.Child is FlyoutPresenter flyoutPresenter)
                    {
                        flyoutPresenter.RequestedTheme = elementTheme;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }

                try
                {
                    if (popup.Child is Grid grid && grid.Name is "OuterOverflowContentRootV2")
                    {
                        grid.RequestedTheme = elementTheme;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            }
        }

        /// <summary>
        /// 设置窗口的置顶状态
        /// </summary>
        private void SetTopMost()
        {
            overlappedPresenter.IsAlwaysOnTop = TopMostService.TopMostValue;
        }

        #endregion 第七部分：窗口及内容属性设置

        #region 第八部分：窗口过程

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
                                Position = Environment.OSVersion.Version.Build > 22000 ? new Point(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale) : new Point(localPoint.X, localPoint.Y)
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
                if (Equals(navigationItem.NavigationPage, navigationPageType))
                {
                    WindowFrame.Navigate(navigationItem.NavigationPage, parameter);
                    break;
                }
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
        public object GetFrameContent()
        {
            return WindowFrame.Content;
        }

        #endregion 第九部分：窗口导航方法

        #region 第十部分：显示对话框和应用通知

        /// <summary>
        /// 显示内容对话框
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ContentDialog))]
        public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog contentDialog)
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
        public async Task ShowNotificationAsync(TeachingTip teachingTip, int duration = 2000)
        {
            try
            {
                if (teachingTip is not null && MainPage.Content.As<Grid>() is Grid grid)
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

        #endregion 第十部分：显示对话框和应用通知

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
                    if (appLaunchArguments.SubParameters[0] is "Home" && !Equals(GetCurrentPageType(), typeof(HomePage)))
                    {
                        NavigateTo(typeof(HomePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Store" && !Equals(GetCurrentPageType(), typeof(StorePage)))
                    {
                        NavigateTo(typeof(StorePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppUpdate" && !Equals(GetCurrentPageType(), typeof(AppUpdatePage)))
                    {
                        NavigateTo(typeof(AppUpdatePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "WinGet" && !Equals(GetCurrentPageType(), typeof(WinGetPage)))
                    {
                        NavigateTo(typeof(WinGetPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppManager" && !Equals(GetCurrentPageType(), typeof(AppManagerPage)))
                    {
                        NavigateTo(typeof(AppManagerPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Download" && !Equals(GetCurrentPageType(), typeof(DownloadPage)))
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
                    if (appLaunchArguments.SubParameters[0] is "Home" && !Equals(GetCurrentPageType(), typeof(HomePage)))
                    {
                        NavigateTo(typeof(HomePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Store" && !Equals(GetCurrentPageType(), typeof(StorePage)))
                    {
                        NavigateTo(typeof(StorePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppUpdate" && !Equals(GetCurrentPageType(), typeof(AppUpdatePage)))
                    {
                        NavigateTo(typeof(AppUpdatePage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "WinGet" && !Equals(GetCurrentPageType(), typeof(WinGetPage)))
                    {
                        NavigateTo(typeof(WinGetPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "AppManager" && !Equals(GetCurrentPageType(), typeof(AppManagerPage)))
                    {
                        NavigateTo(typeof(AppManagerPage));
                    }
                    else if (appLaunchArguments.SubParameters[0] is "Download" && !Equals(GetCurrentPageType(), typeof(DownloadPage)))
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
                    if (Equals(GetCurrentPageType(), typeof(StorePage)))
                    {
                        (GetFrameContent() as StorePage).InitializeQueryLinksContent(appLaunchArguments.SubParameters);
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
                            if (!Equals(GetCurrentPageType(), typeof(SettingsPage)))
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
                            if (!Equals(GetCurrentPageType(), typeof(SettingsPage)))
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
                        if (!Equals(GetCurrentPageType(), typeof(DownloadPage)))
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
                if (Equals(GetCurrentPageType(), typeof(StorePage)))
                {
                    (GetFrameContent() as StorePage).InitializeQueryLinksContent(appLaunchArguments.SubParameters);
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
    }
}

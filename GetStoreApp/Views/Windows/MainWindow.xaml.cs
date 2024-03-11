using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Comctl32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.StartScreen;
using WinRT.Interop;

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SUBCLASSPROC mainWindowSubClassProc;
        private SUBCLASSPROC inputNonClientPointerSourceSubClassProc;

        private ContentCoordinateConverter contentCoordinateConverter;
        private OverlappedPresenter overlappedPresenter;

        public AppWindow CoreAppWindow { get; }

        public new static MainWindow Current { get; private set; }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                _isWindowMaximized = value;
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

        private List<KeyValuePair<string, Type>> PageList { get; } = new List<KeyValuePair<string, Type>>()
        {
            new KeyValuePair<string, Type>("Store",typeof(StorePage)),
            new KeyValuePair<string, Type>("AppUpdate", typeof(AppUpdatePage)),
            new KeyValuePair<string, Type>("WinGet", typeof(WinGetPage)),
            new KeyValuePair<string, Type>("UWPApp", typeof(UWPAppPage)),
            new KeyValuePair<string, Type>("Download", typeof(DownloadPage)),
            new KeyValuePair<string, Type>("Web",null ),
            new KeyValuePair<string, Type>("About", typeof(AboutPage)),
            new KeyValuePair<string, Type>("Settings", typeof(SettingsPage))
        };

        public List<NavigationModel> NavigationItemList { get; } = new List<NavigationModel>();

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

            // 标题栏设置
            SetTitleBar(AppTitlebar);
            SetTitleBarColor((Content as FrameworkElement).ActualTheme);

            // 挂载相应的事件
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            ApplicationData.Current.DataChanged += OnDataChanged;

            // 为应用主窗口添加窗口过程
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass((IntPtr)AppWindow.Id.Value, mainWindowSubClassProc, 0, IntPtr.Zero);

            // 为非工作区的窗口设置相应的窗口样式，添加窗口过程
            IntPtr inputNonClientPointerSourceHandle = User32Library.FindWindowEx((IntPtr)AppWindow.Id.Value, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (inputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                inputNonClientPointerSourceSubClassProc = new SUBCLASSPROC(InputNonClientPointerSourceSubClassProc);
                Comctl32Library.SetWindowSubclass((IntPtr)AppWindow.Id.Value, inputNonClientPointerSourceSubClassProc, 0, IntPtr.Zero);
            }

            // 处理提权模式下运行应用
            if (RuntimeHelper.IsElevated)
            {
                CHANGEFILTERSTRUCT changeFilterStatus = new CHANGEFILTERSTRUCT();
                changeFilterStatus.cbSize = Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT));
                User32Library.ChangeWindowMessageFilterEx((IntPtr)AppWindow.Id.Value, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_ALLOW, in changeFilterStatus);
                ToastNotificationService.Show(NotificationKind.RunAsAdministrator);
            }
        }

        #region 第一部分：窗口类事件

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            try
            {
                if (Visible && SystemBackdrop is not null)
                {
                    MaterialBackdrop materialBackdrop = SystemBackdrop as MaterialBackdrop;

                    if (materialBackdrop is not null && materialBackdrop.BackdropConfiguration is not null)
                    {
                        if (AlwaysShowBackdropService.AlwaysShowBackdropValue)
                        {
                            materialBackdrop.BackdropConfiguration.IsInputActive = true;
                        }
                        else
                        {
                            materialBackdrop.BackdropConfiguration.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 窗口大小发生改变时的事件
        /// </summary>
        private void OnSizeChanged(object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs args)
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

            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
            if (DownloadSchedulerService.DownloadingCollection.Count > 0 || DownloadSchedulerService.WaitingCollection.Count > 0)
            {
                Show();

                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                ContentDialogResult result = await ContentDialogHelper.ShowAsync(new ClosingWindowDialog(), Content as FrameworkElement);

                if (result is ContentDialogResult.Primary)
                {
                    AppWindow.Changed -= OnAppWindowChanged;
                    ApplicationData.Current.DataChanged -= OnDataChanged;
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
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage((IntPtr)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, 0xF010, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
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
            SetTitleBarColor(sender.ActualTheme);
            SetWindowBackground();
        }

        /// <summary>
        /// 按下 Alt + Space 键时，导航控件返回到上一页
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Back && args.KeyStatus.IsMenuKeyDown)
            {
                UWPAppPage uwpAppPage = WindowFrame.Content as UWPAppPage;
                if (uwpAppPage is not null && uwpAppPage.BreadCollection.Count is 2)
                {
                    uwpAppPage.BackToAppList();
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
                MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;

                if (menuFlyoutItem is not null)
                {
                    string tag = menuFlyoutItem.Tag as string;

                    SecondaryTile secondaryTile = new SecondaryTile("GetStoreApp" + tag);
                    secondaryTile.DisplayName = ResourceService.GetLocalized("Resources/AppDisplayName");
                    secondaryTile.Arguments = "SecondaryTile " + tag;

                    secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                    secondaryTile.VisualElements.Square150x150Logo = new Uri(string.Format("ms-appx:///Assets/ControlIcon/{0}.png", tag));
                    secondaryTile.VisualElements.Square71x71Logo = new Uri(string.Format("ms-appx:///Assets/ControlIcon/{0}.png", tag));
                    secondaryTile.VisualElements.Square44x44Logo = new Uri(string.Format("ms-appx:///Assets/ControlIcon/{0}.png", tag));

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
                DispatcherQueue.TryEnqueue(() =>
                {
                    TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.StartScreen, isPinnedSuccessfully));
                });
            }
        }

        #endregion 第四部分：窗口内容挂载的事件

        #region 第五部分：导航控件及其内容挂载的事件

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // 导航控件加载完成后初始化内容

            NavigationView navigationView = sender as NavigationView;
            if (navigationView is not null)
            {
                foreach (object menuItem in navigationView.MenuItems)
                {
                    NavigationViewItem navigationViewItem = menuItem as NavigationViewItem;
                    if (navigationViewItem is not null)
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
                    NavigationViewItem navigationViewItem = footerMenuItem as NavigationViewItem;
                    if (navigationViewItem is not null)
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
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            UWPAppPage uwpAppPage = WindowFrame.Content as UWPAppPage;
            if (uwpAppPage is not null && uwpAppPage.BreadCollection.Count is 2)
            {
                uwpAppPage.BackToAppList();
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
            NavigationViewItemBase navigationViewItem = args.SelectedItemContainer;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => item.NavigationTag == PageList[Convert.ToInt32(navigationViewItem.Tag)].Key);

                if (SelectedItem != navigationItem.NavigationItem)
                {
                    if (PageList[Convert.ToInt32(navigationItem.NavigationItem.Tag)].Key is "Web")
                    {
                        NavigationViewItem originalSelectedItem = SelectedItem;
                        ProcessHelper.StartProcess(Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreAppWebView.exe"), " ", out _);
                        SelectedItem = originalSelectedItem;
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
            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
            (Application.Current as WinUIApp).Dispose();
        }

        #endregion 第五部分：导航控件及其内容挂载的事件

        #region 第六部分：自定义事件

        /// <summary>
        /// 同步漫游应用程序数据时发生的事件
        /// </summary>
        private void OnDataChanged(ApplicationData sender, object args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                TeachingTipHelper.Show(new QuickOperationTip(QuickOperationKind.Taskbar, ResultService.ReadResult<bool>(ConfigKey.TaskbarPinnedResultKey)));
            });
        }

        #endregion 第六部分：自定义事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 第七部分：窗口属性设置

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        public void SetSystemBackdrop(DictionaryEntry backdropItem)
        {
            if (backdropItem.Value == BackdropService.BackdropList[1].Value)
            {
                SystemBackdrop = new MaterialBackdrop(MicaKind.Base);
            }
            else if (backdropItem.Value == BackdropService.BackdropList[2].Value)
            {
                SystemBackdrop = new MaterialBackdrop(MicaKind.BaseAlt);
            }
            else if (backdropItem.Value == BackdropService.BackdropList[3].Value)
            {
                SystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Default);
            }
            else if (backdropItem.Value == BackdropService.BackdropList[4].Value)
            {
                SystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Base);
            }
            else if (backdropItem.Value == BackdropService.BackdropList[5].Value)
            {
                SystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Thin);
            }
            else
            {
                SystemBackdrop = null;
            }

            if (SystemBackdrop is not null && AlwaysShowBackdropService.AlwaysShowBackdropValue)
            {
                (SystemBackdrop as MaterialBackdrop).BackdropConfiguration.IsInputActive = true;
            }

            SetWindowBackground();
        }

        /// <summary>
        /// 应用背景色设置跟随系统发生变化时，修改控件的背景值
        /// </summary>
        private void SetWindowBackground()
        {
            if (SystemBackdrop is null)
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

                AppWindow.SetIcon("Assets/Logo.ico");
            }

            User32Library.SetForegroundWindow((IntPtr)AppWindow.Id.Value);
            Activate();
        }

        /// <summary>
        /// 设置窗口的置顶状态
        /// </summary>
        public void SetTopMost(bool isAlwaysOnTop)
        {
            overlappedPresenter.IsAlwaysOnTop = isAlwaysOnTop;
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
        private IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                // 窗口大小发生更改后的消息
                case WindowMessage.WM_SIZE:
                    {
                        if (CoreAppWindow is not null && CoreAppWindow.Id.Value is not 0)
                        {
                            CoreAppWindow.Resize(new SizeInt32(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16));
                        }
                        break;
                    }
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
                // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        if (ThemeService.AppTheme.Value == ThemeService.ThemeList[0].Value)
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
                        break;
                    }
                // 窗口接收其他数据消息
                case WindowMessage.WM_COPYDATA:
                    {
                        COPYDATASTRUCT copyDataStruct = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);

                        // 正常启动
                        if ((ActivationKind)copyDataStruct.dwData is ActivationKind.Launch)
                        {
                            Show();

                            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
                            {
                                await ContentDialogHelper.ShowAsync(new AppRunningDialog(), Content as FrameworkElement);
                            });
                        }
                        // 正常启动或从共享启动处启动，应用可能会附带启动参数
                        else if ((ActivationKind)copyDataStruct.dwData is ActivationKind.CommandLineLaunch ||
                            (ActivationKind)copyDataStruct.dwData is ActivationKind.ShareTarget)
                        {
                            string[] startupArgs = copyDataStruct.lpData.Split(' ');

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
                                else if (startupArgs[1] is "UWPApp" && GetCurrentPageType() != typeof(UWPAppPage))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        NavigateTo(typeof(UWPAppPage));
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

                                    StorePage storePage = WindowFrame.Content as StorePage;
                                    if (storePage is not null)
                                    {
                                        storePage.QueryLinks.SelectedType = Convert.ToInt32(startupArgs[0]) is -1 ? storePage.QueryLinks.TypeList[0] : storePage.QueryLinks.TypeList[Convert.ToInt32(startupArgs[0])];
                                        storePage.QueryLinks.SelectedChannel = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.QueryLinks.ChannelList[3] : storePage.QueryLinks.ChannelList[Convert.ToInt32(startupArgs[1])];
                                        storePage.QueryLinks.LinkText = startupArgs[2] is "PlaceHolderText" ? string.Empty : startupArgs[2];
                                        storePage.StoreSelectorBar.SelectedItem ??= storePage.StoreSelectorBar.Items[0];
                                    }
                                });
                            }

                            Show();
                        }
                        // 处理应用通知启动
                        else if ((ActivationKind)copyDataStruct.dwData is ActivationKind.ToastNotification)
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
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 15);
                            options.ShowMode = FlyoutShowMode.Standard;
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        else if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 45);
                            options.ShowMode = FlyoutShowMode.Standard;
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        break;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr InputNonClientPointerSourceSubClassProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
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
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            PointInt32 screenPoint = new PointInt32(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.ShowMode = FlyoutShowMode.Standard;
                            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                            new Point(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale) :
                            new Point(localPoint.X, localPoint.Y);

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
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
    }
}

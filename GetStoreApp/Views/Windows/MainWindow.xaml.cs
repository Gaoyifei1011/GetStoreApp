using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using GetStoreApp.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
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
using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using WinRT.Interop;

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool isInvoked = false;
        private IntPtr UWPCoreHandle;

        private WNDPROC newMainWindowWndProc = null;
        private IntPtr oldMainWindowWndProc = IntPtr.Zero;

        private WNDPROC newInputNonClientPointerSourceWndProc = null;
        private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

        private OverlappedPresenter Presenter;
        private UISettings AppUISettings = new UISettings();

        public CoreWindow UWPCoreWindow { get; }

        public DisplayInformation DisplayInformation { get; }

        public IntPtr Handle { get; }

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

        private List<KeyValuePair<string, Type>> PageList = new List<KeyValuePair<string, Type>>()
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

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            // 窗口部分初始化
            Handle = WindowNative.GetWindowHandle(this);
            NavigationService.NavigationFrame = WindowFrame;
            Presenter = AppWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = Presenter.State is OverlappedPresenterState.Maximized;

            // 标题栏设置
            SetTitleBar(AppTitlebar);
            SetTitleBarColor((Content as FrameworkElement).ActualTheme);

            // 在桌面应用中创建 CoreWindow
            WindowsUILibrary.PrivateCreateCoreWindow(WINDOW_TYPE.IMMERSIVE_HOSTED, "DesktopWindowXamlSource", 0, 0, AppWindow.Size.Width, AppWindow.Size.Height, 0, Handle, typeof(ICoreWindow).GUID, out IntPtr obj);
            UWPCoreWindow = CoreWindow.FromAbi(obj);
            DisplayInformation = DisplayInformation.GetForCurrentView();

            // 挂载相应的事件
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            AppUISettings.ColorValuesChanged += OnColorValuesChanged;

            // 为应用主窗口添加窗口过程
            newMainWindowWndProc = new WNDPROC(WindowWndProc);
            oldMainWindowWndProc = SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newMainWindowWndProc));

            // 为非工作区的窗口设置相应的窗口样式，添加窗口过程
            IntPtr inputNonClientPointerSourceHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (inputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                int style = GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE);
                SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE, style & ~(int)WindowStyle.WS_SYSMENU);

                newInputNonClientPointerSourceWndProc = new WNDPROC(InputNonClientPointerSourceWndProc);
                oldInputNonClientPointerSourceWndProc = SetWindowLongAuto(inputNonClientPointerSourceHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newInputNonClientPointerSourceWndProc));
            }

            // 设置 CoreWindow 窗口的样式
            if (UWPCoreWindow is not null)
            {
                UWPCoreHandle = User32Library.FindWindowEx(IntPtr.Zero, IntPtr.Zero, typeof(CoreWindow).FullName, "DesktopWindowXamlSource");

                if (UWPCoreHandle != IntPtr.Zero)
                {
                    long style = GetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_STYLE);
                    style &= ~(long)WindowStyle.WS_POPUP;
                    SetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_STYLE, (nint)(style | (long)WindowStyle.WS_CHILDWINDOW));
                    SetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_EXSTYLE, GetWindowLongAuto(UWPCoreHandle, WindowLongIndexFlags.GWL_EXSTYLE) | (int)WindowStyleEx.WS_EX_TOOLWINDOW);
                    User32Library.SetParent(UWPCoreHandle, Handle);
                }
            }

            // 处理提权模式下运行应用
            if (RuntimeHelper.IsElevated)
            {
                CHANGEFILTERSTRUCT changeFilterStatus = new CHANGEFILTERSTRUCT();
                changeFilterStatus.cbSize = Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT));
                User32Library.ChangeWindowMessageFilterEx(Handle, WindowMessage.WM_COPYDATA, ChangeFilterAction.MSGFLT_ALLOW, in changeFilterStatus);
                ToastNotificationService.Show(NotificationKind.RunAsAdministrator);
            }
        }

        #region 第一部分：窗口类事件

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            if (Program.ApplicationRoot.IsAppRunning && SystemBackdrop is not null)
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

        /// <summary>
        /// 窗口大小发生改变时的事件
        /// </summary>
        private void OnSizeChanged(object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs args)
        {
            if (TitlebarMenuFlyout.IsOpen)
            {
                TitlebarMenuFlyout.Hide();
            }

            if (Presenter is not null)
            {
                IsWindowMaximized = Presenter.State is OverlappedPresenterState.Maximized;
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

                if (Presenter is not null)
                {
                    IsWindowMaximized = Presenter.State is OverlappedPresenterState.Maximized;
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
                    AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
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
                AppUISettings.ColorValuesChanged -= OnColorValuesChanged;
                Program.ApplicationRoot.Dispose();
            }
        }

        #endregion 第二部分：窗口辅助类（AppWindow）挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            MaximizeOrRestore();
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
                User32Library.SendMessage(Handle, WindowMessage.WM_SYSCOMMAND, 0xF010, 0);
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
                User32Library.SendMessage(Handle, WindowMessage.WM_SYSCOMMAND, 0xF000, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            Minimize();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            MaximizeOrRestore();
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.Dispose();
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
            if (args.Key == VirtualKey.Back && args.KeyStatus.IsMenuKeyDown)
            {
                UWPAppPage uwpAppPage = WindowFrame.Content as UWPAppPage;
                if (uwpAppPage is not null && uwpAppPage.BreadCollection.Count is 2)
                {
                    uwpAppPage.BackToAppList();
                }
                else
                {
                    NavigationService.NavigationFrom();
                }
            }
        }

        #endregion 第四部分：窗口内容挂载的事件

        #region 第五部分：导航控件及其内容挂载的事件

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackRequestedClicked(object sender, RoutedEventArgs args)
        {
            UWPAppPage uwpAppPage = WindowFrame.Content as UWPAppPage;
            if (uwpAppPage is not null && uwpAppPage.BreadCollection.Count is 2)
            {
                uwpAppPage.BackToAppList();
            }
            else
            {
                NavigationService.NavigationFrom();
            }
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
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
                        int TagIndex = Convert.ToInt32(navigationViewItem.Tag);

                        NavigationService.NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[TagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[TagIndex].Value,
                        });
                    }
                }

                foreach (object item in navigationView.FooterMenuItems)
                {
                    NavigationViewItem navigationViewItem = item as NavigationViewItem;
                    if (navigationViewItem is not null)
                    {
                        int TagIndex = Convert.ToInt32(navigationViewItem.Tag);

                        NavigationService.NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[TagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[TagIndex].Value,
                        });
                    }
                }

                SelectedItem = NavigationService.NavigationItemList[0].NavigationItem;
                NavigationService.NavigateTo(typeof(StorePage));
                if (DesktopLaunchService.InitializePage != typeof(StorePage))
                {
                    NavigationService.NavigateTo(DesktopLaunchService.InitializePage);
                }
                IsBackEnabled = NavigationService.CanGoBack();
            }
        }

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生
        /// </summary>
        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            NavigationViewItemBase navigationViewItem = args.InvokedItemContainer;
            if (navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == PageList[Convert.ToInt32(navigationViewItem.Tag)].Key);

                if (SelectedItem != navigationItem.NavigationItem)
                {
                    if (PageList[Convert.ToInt32(navigationItem.NavigationItem.Tag)].Key is "Web")
                    {
                        NavigationViewItem OriginalSelectedItem = SelectedItem;

                        if (!isInvoked)
                        {
                            Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
                            WinGetProcessStartupInfo.lpReserved = IntPtr.Zero;
                            WinGetProcessStartupInfo.lpDesktop = IntPtr.Zero;
                            WinGetProcessStartupInfo.lpTitle = IntPtr.Zero;
                            WinGetProcessStartupInfo.dwX = 0;
                            WinGetProcessStartupInfo.dwY = 0;
                            WinGetProcessStartupInfo.dwXSize = 0;
                            WinGetProcessStartupInfo.dwYSize = 0;
                            WinGetProcessStartupInfo.dwXCountChars = 500;
                            WinGetProcessStartupInfo.dwYCountChars = 500;
                            WinGetProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                            WinGetProcessStartupInfo.wShowWindow = WindowShowStyle.SW_SHOWNORMAL;
                            WinGetProcessStartupInfo.cbReserved2 = 0;
                            WinGetProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                            WinGetProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                            bool createResult = Kernel32Library.CreateProcess(null, Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreAppWebView.exe"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.None, IntPtr.Zero, null, ref WinGetProcessStartupInfo, out PROCESS_INFORMATION WinGetProcessInformation);

                            if (createResult)
                            {
                                isInvoked = true;
                                if (WinGetProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hProcess);
                                if (WinGetProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hThread);
                            }
                        }
                        else
                        {
                            isInvoked = false;
                        }

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
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new ApplicationException(string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName));
        }

        #endregion 第五部分：导航控件及其内容挂载的事件

        #region 第六部分：自定义事件

        /// <summary>
        /// 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
        /// </summary>
        private void OnColorValuesChanged(UISettings sender, object args)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
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
                SystemBackdrop = new MaterialBackdrop()
                {
                    IsMicaBackdrop = true,
                    MicaBackdropKind = MicaKind.Base
                };
            }
            else if (backdropItem.Value == BackdropService.BackdropList[2].Value)
            {
                SystemBackdrop = new MaterialBackdrop()
                {
                    IsMicaBackdrop = true,
                    MicaBackdropKind = MicaKind.BaseAlt
                };
            }
            else if (backdropItem.Value == BackdropService.BackdropList[3].Value)
            {
                SystemBackdrop = new MaterialBackdrop()
                {
                    IsMicaBackdrop = false,
                    DesktopAcrylicBackdropKind = DesktopAcrylicKind.Default
                };
            }
            else if (backdropItem.Value == BackdropService.BackdropList[4].Value)
            {
                SystemBackdrop = new MaterialBackdrop()
                {
                    IsMicaBackdrop = false,
                    DesktopAcrylicBackdropKind = DesktopAcrylicKind.Base
                };
            }
            else if (backdropItem.Value == BackdropService.BackdropList[5].Value)
            {
                SystemBackdrop = new MaterialBackdrop()
                {
                    IsMicaBackdrop = false,
                    DesktopAcrylicBackdropKind = DesktopAcrylicKind.Thin
                };
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
        /// 设置窗口的置顶状态
        /// </summary>
        public void SetTopMost(bool isAlwaysOnTop)
        {
            Presenter.IsAlwaysOnTop = isAlwaysOnTop;
        }

        /// <summary>
        /// 获取窗口属性
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
        /// 更改窗口属性
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

        #endregion 第七部分：窗口属性设置

        #region 第八部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr WindowWndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口大小发生更改后的消息
                case WindowMessage.WM_SIZE:
                    {
                        if (UWPCoreHandle != IntPtr.Zero)
                        {
                            User32Library.SetWindowPos(UWPCoreHandle, 0, 0, 0, lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
                        }
                        break;
                    }
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        if (DisplayInformation is not null)
                        {
                            MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                            minMaxInfo.ptMinTrackSize.X = (int)(960 * DisplayInformation.RawPixelsPerViewPixel);
                            minMaxInfo.ptMinTrackSize.Y = (int)(600 * DisplayInformation.RawPixelsPerViewPixel);
                            Marshal.StructureToPtr(minMaxInfo, lParam, true);
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

                            if (startupArgs.Length is 2)
                            {
                                if (startupArgs[1] is "Store" && NavigationService.GetCurrentPageType() != typeof(StorePage))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        NavigationService.NavigateTo(typeof(StorePage));
                                    });
                                }
                                if (startupArgs[1] is "AppUpdate" && NavigationService.GetCurrentPageType() != typeof(AppUpdatePage))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        NavigationService.NavigateTo(typeof(AppUpdatePage));
                                    });
                                }
                                else if (startupArgs[1] is "WinGet" && NavigationService.GetCurrentPageType() != typeof(WinGetPage))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        NavigationService.NavigateTo(typeof(WinGetPage));
                                    });
                                }
                                else if (startupArgs[1] is "UWPApp" && NavigationService.GetCurrentPageType() != typeof(UWPAppPage))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        NavigationService.NavigateTo(typeof(UWPAppPage));
                                    });
                                }
                                else if (startupArgs[1] is "Download" && NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                                {
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        NavigationService.NavigateTo(typeof(DownloadPage));
                                    });
                                }
                                else if (startupArgs[1] is "Web")
                                {
                                    Task.Run(() =>
                                    {
                                        Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
                                        WinGetProcessStartupInfo.lpReserved = IntPtr.Zero;
                                        WinGetProcessStartupInfo.lpDesktop = IntPtr.Zero;
                                        WinGetProcessStartupInfo.lpTitle = IntPtr.Zero;
                                        WinGetProcessStartupInfo.dwX = 0;
                                        WinGetProcessStartupInfo.dwY = 0;
                                        WinGetProcessStartupInfo.dwXSize = 0;
                                        WinGetProcessStartupInfo.dwYSize = 0;
                                        WinGetProcessStartupInfo.dwXCountChars = 500;
                                        WinGetProcessStartupInfo.dwYCountChars = 500;
                                        WinGetProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                                        WinGetProcessStartupInfo.wShowWindow = WindowShowStyle.SW_SHOWNORMAL;
                                        WinGetProcessStartupInfo.cbReserved2 = 0;
                                        WinGetProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                                        WinGetProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                                        bool createResult = Kernel32Library.CreateProcess(null, Path.Combine(InfoHelper.AppInstalledLocation, "GetStoreAppWebView.exe"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.None, IntPtr.Zero, null, ref WinGetProcessStartupInfo, out PROCESS_INFORMATION WinGetProcessInformation);

                                        if (createResult)
                                        {
                                            isInvoked = true;
                                            if (WinGetProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hProcess);
                                            if (WinGetProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hThread);
                                        }
                                    });
                                }
                            }
                            else if (startupArgs.Length is 3)
                            {
                                DispatcherQueue.TryEnqueue(() =>
                                {
                                    if (NavigationService.GetCurrentPageType() != typeof(StorePage))
                                    {
                                        NavigationService.NavigateTo(typeof(StorePage));
                                    }

                                    StorePage storePage = NavigationService.NavigationFrame.Content as StorePage;
                                    if (storePage is not null)
                                    {
                                        storePage.QueryLinks.SelectedType = Convert.ToInt32(startupArgs[0]) is -1 ? storePage.QueryLinks.TypeList[0] : storePage.QueryLinks.TypeList[Convert.ToInt32(startupArgs[0])];
                                        storePage.QueryLinks.SelectedChannel = Convert.ToInt32(startupArgs[1]) is -1 ? storePage.QueryLinks.ChannelList[3] : storePage.QueryLinks.ChannelList[Convert.ToInt32(startupArgs[1])];
                                        storePage.QueryLinks.LinkText = startupArgs[2] is "PlaceHolderText" ? string.Empty : startupArgs[2];

                                        if (storePage.SelectedIndex is not 0)
                                        {
                                            storePage.SelectedIndex = 0;
                                        }
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
                        SystemCommand sysCommand = (SystemCommand)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SystemCommand.SC_MOUSEMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 15);
                            options.ShowMode = FlyoutShowMode.Standard;
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        else if (sysCommand is SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Point(0, 45);
                            options.ShowMode = FlyoutShowMode.Standard;
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return 0;
                        }
                        break;
                    }
                // 任务栏窗口右键点击后的消息
                case WindowMessage.WM_SYSMENU:
                    {
                        if (Presenter.State is OverlappedPresenterState.Minimized)
                        {
                            Presenter.Restore();
                        }
                        break;
                    }
            }
            return User32Library.CallWindowProc(oldMainWindowWndProc, hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr InputNonClientPointerSourceWndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
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
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        if (DisplayInformation is not null)
                        {
                            PointInt32 pt = new PointInt32(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);

                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.ShowMode = FlyoutShowMode.Standard;
                            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                            new Point((pt.X - AppWindow.Position.X - 8) / DisplayInformation.RawPixelsPerViewPixel, (pt.Y - AppWindow.Position.Y) / DisplayInformation.RawPixelsPerViewPixel) :
                            new Point(pt.X - AppWindow.Position.X - 8, pt.Y - AppWindow.Position.Y);

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
            }
            return User32Library.CallWindowProc(oldInputNonClientPointerSourceWndProc, hWnd, Msg, wParam, lParam);
        }

        #endregion 第八部分：窗口过程
    }
}

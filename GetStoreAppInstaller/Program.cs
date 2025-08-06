using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.Services.Settings;
using GetStoreAppInstaller.Views.Pages;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Ole32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Uxtheme;
using GetStoreAppInstaller.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.Management.Deployment;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 获取商店应用 应用安装器
    /// </summary>
    public class Program
    {
        private static string TitleString;
        private static string RunAsAdministratorString;
        private static readonly Guid CLSID_ApplicationActivationManager = new("45BA127D-10A8-46EA-8AB7-56EA9078943C");
        private static SUBCLASSPROC mainWindowSubClassProc;
        private static InputNonClientPointerSource inputNonClientPointerSource;
        private static InputActivationListener inputActivationListener;
        private static readonly PackageManager packageManager = new();
        private static IApplicationActivationManager applicationActivationManager;

        public static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        public static AppWindow MainAppWindow { get; private set; }

        public static AppWindow CoreAppWindow { get; set; }

        public static DisplayInformation DisplayInformation { get; private set; }

        private static ContentCoordinateConverter contentCoordinateConverter;

        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            if (Ole32Library.CoCreateInstance(CLSID_ApplicationActivationManager, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IApplicationActivationManager).GUID, out nint applicationActivationManagerPtr) is 0)
            {
                applicationActivationManager = (IApplicationActivationManager)StrategyBasedComWrappers.GetOrCreateObjectForComInstance(applicationActivationManagerPtr, CreateObjectFlags.Unwrap);
            }

            if (!RuntimeHelper.IsMSIX)
            {
                PackageManager packageManager = new();
                foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
                {
                    if (package.Id.FullName.Contains("Gaoyifei1011.GetStoreApp"))
                    {
                        applicationActivationManager.ActivateApplication("Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreAppInstaller", null, ACTIVATEOPTIONS.AO_NONE, out _);
                        break;
                    }
                }
                return;
            }

            // 初始化应用启动参数
            AppActivationArguments appActivationArguments = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            if (appActivationArguments.Kind is ExtendedActivationKind.Launch)
            {
                if (RuntimeHelper.IsElevated)
                {
                    string[] argumentsArray = Environment.GetCommandLineArgs();

                    if (argumentsArray.Length > 0 && string.Equals(Path.GetExtension(argumentsArray[0]), ".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        argumentsArray[0] = argumentsArray[0].Replace(".dll", ".exe");
                    }

                    if (argumentsArray.Length is 3 && argumentsArray[2] is "--elevated")
                    {
                        foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
                        {
                            if (package.Id.FullName.Contains("Gaoyifei1011.GetStoreApp"))
                            {
                                applicationActivationManager.ActivateApplication("Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreAppInstaller", argumentsArray[1], ACTIVATEOPTIONS.AO_NONE, out _);
                                break;
                            }
                        }
                        return;
                    }
                }
            }
            else if (appActivationArguments.Kind is ExtendedActivationKind.ToastNotification)
            {
                return;
            }

            InitializeResources();
            TitleString = ResourceService.GetLocalized("Installer/Title");
            RunAsAdministratorString = ResourceService.GetLocalized("Installer/RunningAdministrator");

            // 应用主窗口
            MainAppWindow = AppWindow.Create();
            MainAppWindow.Title = RuntimeHelper.IsElevated ? TitleString + RunAsAdministratorString : TitleString;
            MainAppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            MainAppWindow.Changed += OnAppWindowChanged;
            MainAppWindow.Closing += OnAppWindowClosing;

            // 创建 CoreWindow
            WindowsUILibrary.PrivateCreateCoreWindow(WINDOW_TYPE.IMMERSIVE_HOSTED, "XamlContentCoreWindow", 0, 0, 0, 0, 0, Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), typeof(ICoreWindow).GUID, out nint coreWindowPtr);
            CoreWindow coreWindow = CoreWindow.FromAbi(coreWindowPtr);
            coreWindow.As<ICoreWindowInterop>().GetWindowHandle(out nint coreWindowHandle);
            coreWindow.PointerReleased += OnPointerReleased;
            coreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;
            DisplayInformation = DisplayInformation.GetForCurrentView();
            MainAppWindow.Resize(new SizeInt32(Convert.ToInt32(800 * DisplayInformation.RawPixelsPerViewPixel), Convert.ToInt32(560 * DisplayInformation.RawPixelsPerViewPixel)));
            (MainAppWindow.Presenter as OverlappedPresenter).PreferredMinimumWidth = Convert.ToInt32(800 * DisplayInformation.RawPixelsPerViewPixel);
            (MainAppWindow.Presenter as OverlappedPresenter).PreferredMinimumHeight = Convert.ToInt32(560 * DisplayInformation.RawPixelsPerViewPixel);

            CoreAppWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(coreWindowHandle));
            CoreAppWindow.Move(new PointInt32());
            CoreAppWindow.Resize(MainAppWindow.ClientSize);
            User32Library.SetParent(coreWindowHandle, Win32Interop.GetWindowFromWindowId(MainAppWindow.Id));
            DispatcherQueueController dispatcherQueueController = DispatcherQueueController.CreateOnCurrentThread();
            MainAppWindow.AssociateWithDispatcherQueue(dispatcherQueueController.DispatcherQueue);
            SynchronizationContext.SetSynchronizationContext(new Windows.System.DispatcherQueueSynchronizationContext(coreWindow.DispatcherQueue));
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(MainAppWindow.DispatcherQueue));
            inputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(MainAppWindow.Id);
            inputNonClientPointerSource?.SetRegionRects(NonClientRegionKind.Caption, [new RectInt32(0, 0, Convert.ToInt32(2 * DisplayInformation.RawPixelsPerViewPixel), Convert.ToInt32(45 * DisplayInformation.RawPixelsPerViewPixel)), new RectInt32(Convert.ToInt32(42 * DisplayInformation.RawPixelsPerViewPixel), 0, MainAppWindow.Size.Width - Convert.ToInt32(42 * DisplayInformation.RawPixelsPerViewPixel), Convert.ToInt32(45 * DisplayInformation.RawPixelsPerViewPixel))]);
            inputActivationListener = InputActivationListener.GetForWindowId(MainAppWindow.Id);
            inputActivationListener.InputActivationChanged += OnInputActivationChanged;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(MainAppWindow.Id);
            CoreAppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            if (LanguageService.FlowDirection is FlowDirection.RightToLeft)
            {
                SetWindowLongAuto(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), WindowLongIndexFlags.GWL_EXSTYLE, GetWindowLongAuto(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), WindowLongIndexFlags.GWL_EXSTYLE) | (int)WindowExStyle.WS_EX_LAYOUTRTL);
            }

            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(CoreAppWindow.Id);
            XamlIslandsApp app = new();

            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), mainWindowSubClassProc, 0, nint.Zero);

            int style = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE);
            style |= (int)WindowStyle.WS_CHILD;
            style &= ~unchecked((int)WindowStyle.WS_POPUP);
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE, style);

            CoreApplication.As<ICoreApplicationPrivate2>().CreateNonImmersiveView(out nint coreApplicationViewPtr);
            CoreApplicationView coreApplicationView = CoreApplicationView.FromAbi(coreApplicationViewPtr);

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
                Shell32Library.DragAcceptFiles(coreWindowHandle, true);
            }

            FrameworkView frameworkView = new();
            frameworkView.Initialize(coreApplicationView);
            frameworkView.SetWindow(coreWindow);

            SetAppIcon(MainAppWindow);
            MainAppWindow.Show();

            Application.LoadComponent(app, new Uri("ms-appx:///XamlIslandsApp.xaml"), ComponentResourceLocation.Application);
            Window.Current.Content = new InstallerPage(appActivationArguments);
            (Window.Current.Content as InstallerPage).IsWindowMaximized = (MainAppWindow.Presenter as OverlappedPresenter).State is OverlappedPresenterState.Maximized;
            Window.Current.Activate();
            frameworkView.Run();
        }

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private static void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidSizeChange)
            {
                if (DisplayInformation is not null)
                {
                    (MainAppWindow.Presenter as OverlappedPresenter).PreferredMinimumWidth = Convert.ToInt32(800 * DisplayInformation.RawPixelsPerViewPixel);
                    (MainAppWindow.Presenter as OverlappedPresenter).PreferredMinimumHeight = Convert.ToInt32(560 * DisplayInformation.RawPixelsPerViewPixel);
                }

                if (Window.Current is not null && Window.Current.Content is InstallerPage installerPage)
                {
                    CoreAppWindow.Resize(MainAppWindow.ClientSize);
                    installerPage.IsWindowMaximized = (MainAppWindow.Presenter as OverlappedPresenter).State is OverlappedPresenterState.Maximized;
                }

                if (DisplayInformation is not null)
                {
                    inputNonClientPointerSource?.SetRegionRects(NonClientRegionKind.Caption, [new RectInt32(0, 0, Convert.ToInt32(2 * DisplayInformation.RawPixelsPerViewPixel), Convert.ToInt32(45 * DisplayInformation.RawPixelsPerViewPixel)), new RectInt32(Convert.ToInt32(42 * DisplayInformation.RawPixelsPerViewPixel), 0, MainAppWindow.Size.Width - Convert.ToInt32(42 * DisplayInformation.RawPixelsPerViewPixel), Convert.ToInt32(45 * DisplayInformation.RawPixelsPerViewPixel))]);
                }
            }
            else if (args.DidPositionChange)
            {
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(CoreAppWindow.Id), WindowMessage.WM_MOVE, nuint.Zero, nint.Zero);
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private static void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            MainAppWindow.Changed -= OnAppWindowChanged;
            inputActivationListener.InputActivationChanged -= OnInputActivationChanged;
            Window.Current.CoreWindow.PointerReleased -= OnPointerReleased;
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= OnAcceleratorKeyActivated;
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), mainWindowSubClassProc, 0);

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_REMOVE);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_REMOVE);
            }

            if (Window.Current is not null && Window.Current.CoreWindow is not null)
            {
                Window.Current.CoreWindow.Close();
            }
        }

        /// <summary>
        /// 窗口焦点状态发生变化时触发的事件
        /// </summary>
        private static void OnInputActivationChanged(InputActivationListener sender, InputActivationListenerActivationChangedEventArgs args)
        {
            if (sender.State is InputActivationState.Activated)
            {
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(CoreAppWindow.Id), WindowMessage.WM_ACTIVATE, 1, 0);
            }
            else
            {
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(CoreAppWindow.Id), WindowMessage.WM_ACTIVATE, 0, 0);
            }
        }

        /// <summary>
        /// 处理鼠标事件
        /// </summary>
        private static async void OnPointerReleased(CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            if (args.CurrentPoint.Properties.PointerUpdateKind is Windows.UI.Input.PointerUpdateKind.RightButtonReleased && Window.Current.Content is InstallerPage installerPage)
            {
                await Task.Delay(50);
                SetPopupControlTheme(installerPage.ActualTheme);
            }
        }

        /// <summary>
        /// 处理键盘系统按键事件
        /// </summary>
        private static async void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (Window.Current is not null && Window.Current.Content is InstallerPage installerPage)
            {
                if (args.KeyStatus.IsMenuKeyDown && args.VirtualKey is global::Windows.System.VirtualKey.Space)
                {
                    args.Handled = true;
                    FlyoutShowOptions options = new()
                    {
                        Position = new Point(0, 45),
                        ShowMode = FlyoutShowMode.Standard
                    };
                    installerPage.TitlebarMenuFlyout.ShowAt(null, options);
                }
                else if (args.EventType is CoreAcceleratorKeyEventType.SystemKeyDown && args.VirtualKey is Windows.System.VirtualKey.F10)
                {
                    await Task.Delay(50);
                    SetPopupControlTheme(installerPage.ActualTheme);
                }
            }
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private static nint MainWindowSubClassProc(nint hWnd, WindowMessage Msg, nuint wParam, nint lParam, uint uIdSubclass, nint dwRefData)
        {
            switch (Msg)
            {
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (Window.Current is not null && Window.Current.Content is InstallerPage installerPage && installerPage.TitlebarMenuFlyout.IsOpen)
                        {
                            installerPage.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam is 2 && Window.Current is not null && Window.Current.Content is InstallerPage installerPage && DisplayInformation is not null)
                        {
                            User32Library.GetCursorPos(out PointInt32 screenPoint);
                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = new Point(localPoint.X / DisplayInformation.RawPixelsPerViewPixel, localPoint.Y / DisplayInformation.RawPixelsPerViewPixel)
                            };

                            installerPage.TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return 0;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU && lParam is (nint)Windows.System.VirtualKey.Space)
                        {
                            return 0;
                        }
                        break;
                    }
                // 提升权限时允许应用接收拖放文件消息
                case WindowMessage.WM_DROPFILES:
                    {
                        if (Window.Current is not null && Window.Current.Content is InstallerPage installerPage && Window.Current.CoreWindow is CoreWindow coreWindow)
                        {
                            Task.Run(() =>
                            {
                                List<string> filesList = [];
                                char[] dragFileCharArray = new char[260];
                                uint filesCount = Shell32Library.DragQueryFile(wParam, 0xffffffffu, null, 0);

                                for (uint index = 0; index < filesCount; index++)
                                {
                                    Array.Clear(dragFileCharArray, 0, dragFileCharArray.Length);
                                    if (Shell32Library.DragQueryFile(wParam, index, dragFileCharArray, (uint)dragFileCharArray.Length) > 0)
                                    {
                                        filesList.Add(new string(dragFileCharArray).Replace("\0", string.Empty));
                                    }
                                }

                                Shell32Library.DragQueryPoint(wParam, out PointInt32 point);
                                Shell32Library.DragFinish(wParam);

                                if (filesList.Count > 0)
                                {
                                    coreWindow.DispatcherQueue.TryEnqueue(async () =>
                                    {
                                        await installerPage.DealElevatedDragDropAsync(filesList[0]);
                                    });
                                }
                            });
                        }

                        break;
                    }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResourceService.LocalizeReosurce();

            AppInstallService.InitializeAppInstall();
            ThemeService.InitializeTheme();
        }

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private static void SetAppIcon(AppWindow appWindow)
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            nint[] hIcons = new nint[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标
            if (successCount >= 1 && hIcons[0] != nint.Zero)
            {
                appWindow.SetIcon(Win32Interop.GetIconIdFromIcon(hIcons[0]));
            }
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        public static void SetTitleBarTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = MainAppWindow.TitleBar;

            titleBar.BackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.ForegroundColor = Windows.UI.Colors.Transparent;
            titleBar.InactiveBackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.InactiveForegroundColor = Windows.UI.Colors.Transparent;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        public static void SetClassicMenuTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = MainAppWindow.TitleBar;

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
        public static void SetPopupControlTheme(ElementTheme elementTheme)
        {
            foreach (Popup popup in VisualTreeHelper.GetOpenPopups(Window.Current))
            {
                ElementTheme actualTheme = elementTheme;
                popup.RequestedTheme = actualTheme;

                if (popup.Child is FlyoutPresenter flyoutPresenter)
                {
                    flyoutPresenter.RequestedTheme = actualTheme;
                }

                if (popup.Child is Grid grid && grid.Name is "OuterOverflowContentRootV2")
                {
                    grid.RequestedTheme = actualTheme;
                }
            }
        }

        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public static async Task ShowNotificationAsync(TeachingTip teachingTip, int duration = 2000)
        {
            if (teachingTip is not null && Window.Current.Content is Page page && page.Content is Grid grid)
            {
                try
                {
                    grid.Children.Add(teachingTip);

                    teachingTip.IsOpen = true;

                    await Task.Delay(duration);
                    teachingTip.IsOpen = false;

                    // 应用内通知关闭动画显示耗费 300 ms
                    await Task.Delay(300);
                    grid.Children.Remove(teachingTip);
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            }
        }

        /// <summary>
        /// 获取窗口属性
        /// </summary>
        private static int GetWindowLongAuto(nint hWnd, WindowLongIndexFlags nIndex)
        {
            return nint.Size is 8 ? User32Library.GetWindowLongPtr(hWnd, nIndex) : User32Library.GetWindowLong(hWnd, nIndex);
        }

        /// <summary>
        /// 更改窗口属性
        /// </summary>
        private static nint SetWindowLongAuto(nint hWnd, WindowLongIndexFlags nIndex, nint dwNewLong)
        {
            return nint.Size is 8 ? User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong) : User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
        }
    }
}

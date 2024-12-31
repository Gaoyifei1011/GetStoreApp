using GetStoreAppInstaller.Pages;
using GetStoreAppInstaller.Services.Controls.Settings;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Uxtheme;
using GetStoreAppInstaller.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
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
        private static SUBCLASSPROC mainWindowSubClassProc;
        private static InputNonClientPointerSource inputNonClientPointerSource;
        private static InputActivationListener inputActivationListener;

        public static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        public static AppWindow MainAppWindow { get; private set; }

        private static AppWindow CoreAppWindow { get; set; }

        public static DisplayInformation DisplayInformation { get; private set; }

        private static ContentCoordinateConverter contentCoordinateConverter;

        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            InitializeResources();

            // 应用主窗口
            MainAppWindow = AppWindow.Create();
            MainAppWindow.Title = "获取商店应用 应用安装器";
            MainAppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            MainAppWindow.Changed += OnAppWindowChanged;
            MainAppWindow.Closing += OnAppWindowClosing;

            // 创建 CoreWindow
            WindowsUILibrary.PrivateCreateCoreWindow(WINDOW_TYPE.IMMERSIVE_HOSTED, "XamlContentCoreWindow", 0, 0, 0, 0, 0, Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), typeof(ICoreWindow).GUID, out IntPtr coreWindowPtr);
            CoreWindow coreWindow = CoreWindow.FromAbi(coreWindowPtr);
            coreWindow.As<ICoreWindowInterop>().GetWindowHandle(out IntPtr coreWindowHandle);
            CoreAppWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(coreWindowHandle));
            CoreAppWindow.Move(new PointInt32());
            CoreAppWindow.Resize(MainAppWindow.ClientSize);
            User32Library.SetParent(coreWindowHandle, Win32Interop.GetWindowFromWindowId(MainAppWindow.Id));
            DisplayInformation = DisplayInformation.GetForCurrentView();
            MainAppWindow.Resize(new SizeInt32((int)(800 * DisplayInformation.RawPixelsPerViewPixel), (int)(560 * DisplayInformation.RawPixelsPerViewPixel)));
            DispatcherQueueController dispatcherQueueController = DispatcherQueueController.CreateOnCurrentThread();
            MainAppWindow.AssociateWithDispatcherQueue(dispatcherQueueController.DispatcherQueue);
            SynchronizationContext.SetSynchronizationContext(new Windows.System.DispatcherQueueSynchronizationContext(coreWindow.DispatcherQueue));
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(MainAppWindow.DispatcherQueue));
            inputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(MainAppWindow.Id);
            inputNonClientPointerSource?.SetRegionRects(NonClientRegionKind.Caption, [new RectInt32(0, 0, MainAppWindow.Size.Width, (int)(45 * DisplayInformation.RawPixelsPerViewPixel))]);
            inputActivationListener = InputActivationListener.GetForWindowId(MainAppWindow.Id);
            inputActivationListener.InputActivationChanged += OnInputActivationChanged;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(MainAppWindow.Id);
            new XamlIslandsApp();

            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), mainWindowSubClassProc, 0, IntPtr.Zero);

            CoreApplication.As<ICoreApplicationPrivate2>().CreateNonImmersiveView(out IntPtr coreApplicationViewPtr);
            CoreApplicationView coreApplicationView = CoreApplicationView.FromAbi(coreApplicationViewPtr);

            int style = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE);
            style |= (int)WindowStyle.WS_CHILD;
            style &= ~unchecked((int)WindowStyle.WS_POPUP);
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE, style);

            FrameworkView frameworkView = new();
            frameworkView.Initialize(coreApplicationView);
            frameworkView.SetWindow(coreWindow);

            SetAppIcon(MainAppWindow);
            MainAppWindow.Show();
            XamlControlsResources xamlControlsResources = [];
            xamlControlsResources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/XamlIslands/MenuFlyout.xaml") });
            Application.Current.Resources = xamlControlsResources;
            Window.Current.Content = new MainPage();
            (Window.Current.Content as MainPage).IsWindowMaximized = (MainAppWindow.Presenter as OverlappedPresenter).State is OverlappedPresenterState.Maximized;
            frameworkView.Run();
        }

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private static void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidSizeChange)
            {
                CoreAppWindow.Resize(MainAppWindow.ClientSize);

                if (Window.Current is not null && Window.Current.Content is not null)
                {
                    (Window.Current.Content as MainPage).IsWindowMaximized = (MainAppWindow.Presenter as OverlappedPresenter).State is OverlappedPresenterState.Maximized;
                }

                if (DisplayInformation is not null)
                {
                    inputNonClientPointerSource?.SetRegionRects(NonClientRegionKind.Caption, [new RectInt32(0, 0, MainAppWindow.Size.Width, (int)(45 * DisplayInformation.RawPixelsPerViewPixel))]);
                }
            }
            else if (args.DidPositionChange)
            {
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(CoreAppWindow.Id), WindowMessage.WM_MOVE, UIntPtr.Zero, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private static void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            MainAppWindow.Changed -= OnAppWindowChanged;
            inputActivationListener.InputActivationChanged -= OnInputActivationChanged;
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), mainWindowSubClassProc, 0);
            Window.Current.CoreWindow.Close();
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
        /// 应用主窗口消息处理
        /// </summary>
        private static IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        if (DisplayInformation is not null)
                        {
                            MINMAXINFO minMaxInfo;

                            unsafe
                            {
                                minMaxInfo = *(MINMAXINFO*)lParam;
                            }
                            minMaxInfo.ptMinTrackSize.X = (int)(800 * DisplayInformation.RawPixelsPerViewPixel);
                            minMaxInfo.ptMinTrackSize.Y = (int)(560 * DisplayInformation.RawPixelsPerViewPixel);

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
                        if ((Window.Current.Content as MainPage).TitlebarMenuFlyout.IsOpen)
                        {
                            (Window.Current.Content as MainPage).TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam is 2 && Window.Current.Content is not null && DisplayInformation is not null)
                        {
                            Point currentPoint = Window.Current.CoreWindow.PointerPosition;
                            PointInt32 screenPoint = new()
                            {
                                X = Convert.ToInt32(currentPoint.X),
                                Y = Convert.ToInt32(currentPoint.Y),
                            };

                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = new Point(localPoint.X / DisplayInformation.RawPixelsPerViewPixel, localPoint.Y / DisplayInformation.RawPixelsPerViewPixel)
                            };

                            (Window.Current.Content as MainPage).TitlebarMenuFlyout.ShowAt(Window.Current.Content, options);
                        }
                        return 0;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU && lParam is (IntPtr)Windows.System.VirtualKey.Space)
                        {
                            return 0;
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
        }

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private static void SetAppIcon(AppWindow appWindow)
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            IntPtr[] hIcons = new IntPtr[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标
            if (successCount >= 1 && hIcons[0] != IntPtr.Zero)
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
                titleBar.ButtonForegroundColor = Windows.UI.Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(9, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(6, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.Black;
            }
            else
            {
                titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(15, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(10, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
                titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.White;
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        public static void SetClassicMenuTheme(ElementTheme theme)
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
        /// 获取窗口属性
        /// </summary>
        private static int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
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
        private static IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
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
    }
}

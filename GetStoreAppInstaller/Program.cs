using GetStoreAppInstaller.Helpers.Root;
using GetStoreAppInstaller.Pages;
using GetStoreAppInstaller.Services.Controls.Settings;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Shell32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Uxtheme;
using GetStoreAppInstaller.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.Management.Deployment;
using Windows.System;
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

        public static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        public static AppWindow CoreAppWindow { get; set; }

        public static DisplayInformation DisplayInformation { get; private set; }

        private static ContentCoordinateConverter contentCoordinateConverter;

        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            if (!RuntimeHelper.IsMSIX)
            {
                PackageManager packageManager = new();
                foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
                {
                    if (package.Id.FullName.Contains("Gaoyifei1011.GetStoreApp"))
                    {
                        IReadOnlyList<AppListEntry> appListEntryList = package.GetAppListEntries();
                        foreach (AppListEntry appListEntry in appListEntryList)
                        {
                            if (appListEntry.AppUserModelId.Equals("Gaoyifei1011.GetStoreApp_pystbwmrmew8c!GetStoreAppInstaller"))
                            {
                                appListEntry.LaunchAsync().GetResults();
                                break;
                            }
                        }
                    }
                }
                return;
            }

            // 初始化应用启动参数
            AppActivationArguments appActivationArguments = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            if (appActivationArguments.Kind is ExtendedActivationKind.ToastNotification)
            {
                return;
            }

            InitializeResources();

            // 创建 CoreWindow
            WindowsUILibrary.PrivateCreateCoreWindow(WINDOW_TYPE.NOT_IMMERSIVE, ResourceService.GetLocalized("Installer/AppTitle"), 200, 200, 800, 560, 0, IntPtr.Zero, typeof(ICoreWindow).GUID, out IntPtr coreWindowPtr);
            CoreWindow coreWindow = CoreWindow.FromAbi(coreWindowPtr);
            coreWindow.As<ICoreWindowInterop>().GetWindowHandle(out IntPtr coreWindowHandle);
            CoreAppWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(coreWindowHandle));
            DisplayInformation = DisplayInformation.GetForCurrentView();
            CoreAppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            CoreAppWindow.Changed += OnAppWindowChanged;
            CoreAppWindow.Closing += OnAppWindowClosing;
            SetAppIcon(CoreAppWindow);

            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(coreWindow.DispatcherQueue));
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(CoreAppWindow.Id);
            new XamlIslandsApp();

            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(coreWindowHandle, mainWindowSubClassProc, 0, IntPtr.Zero);

            int style = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE);
            style &= ~unchecked((int)WindowStyle.WS_POPUP);
            style |= (int)WindowStyle.WS_OVERLAPPEDWINDOW;
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE, style);

            CoreApplication.As<ICoreApplicationPrivate2>().CreateNonImmersiveView(out IntPtr coreApplicationViewPtr);
            CoreApplicationView coreApplicationView = CoreApplicationView.FromAbi(coreApplicationViewPtr);

            IntPtr inputNonClientPointerSourceHandle = User32Library.FindWindowEx(coreWindowHandle, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (inputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                SetWindowLongAuto(inputNonClientPointerSourceHandle, WindowLongIndexFlags.GWL_EXSTYLE, GetWindowLongAuto(inputNonClientPointerSourceHandle, WindowLongIndexFlags.GWL_EXSTYLE) | (int)WindowExStyle.WS_EX_TRANSPARENT);
            }

            IntPtr reunionWindowingCaptionControlsHandle = User32Library.FindWindowEx(coreWindowHandle, IntPtr.Zero, "ReunionWindowingCaptionControls", null);

            if (reunionWindowingCaptionControlsHandle != IntPtr.Zero)
            {
                SetWindowLongAuto(reunionWindowingCaptionControlsHandle, WindowLongIndexFlags.GWL_EXSTYLE, GetWindowLongAuto(reunionWindowingCaptionControlsHandle, WindowLongIndexFlags.GWL_EXSTYLE) | (int)WindowExStyle.WS_EX_TRANSPARENT);
            }

            if (RuntimeHelper.IsElevated)
            {
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_DROPFILES, ChangeFilterFlags.MSGFLT_ADD);
                User32Library.ChangeWindowMessageFilter(WindowMessage.WM_COPYGLOBALDATA, ChangeFilterFlags.MSGFLT_ADD);
                Shell32Library.DragAcceptFiles(coreWindowHandle, true);
            }

            FrameworkView frameworkView = new();
            frameworkView.Initialize(coreApplicationView);
            frameworkView.SetWindow(coreWindow);

            XamlControlsResources xamlControlsResources = [];
            xamlControlsResources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/XamlIslands/MenuFlyout.xaml") });
            xamlControlsResources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/XamlIslands/TeachingTip.xaml") });
            Application.Current.Resources = xamlControlsResources;
            Window.Current.Content = new MainPage(appActivationArguments);
            (Window.Current.Content as MainPage).IsWindowMaximized = (CoreAppWindow.Presenter as OverlappedPresenter).State is OverlappedPresenterState.Maximized;
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
                if (Window.Current is not null && Window.Current.Content is MainPage mainPage)
                {
                    mainPage.IsWindowMaximized = (CoreAppWindow.Presenter as OverlappedPresenter).State is OverlappedPresenterState.Maximized;
                }
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private static void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            CoreAppWindow.Changed -= OnAppWindowChanged;
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(CoreAppWindow.Id), mainWindowSubClassProc, 0);

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
                // 处理窗口非客户区域消息
                case WindowMessage.WM_NCHITTEST:
                    {
                        if (Window.Current is not null && Window.Current.CoreWindow is not null)
                        {
                            Point currentPoint = Window.Current.CoreWindow.PointerPosition;
                            PointInt32 screenPoint = new()
                            {
                                X = Convert.ToInt32(currentPoint.X),
                                Y = Convert.ToInt32(currentPoint.Y),
                            };

                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            if (CoreAppWindow.Presenter is OverlappedPresenter overlappedPresenter)
                            {
                                if (overlappedPresenter.State is OverlappedPresenterState.Maximized)
                                {
                                    if (localPoint.Y < 30 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        if (localPoint.X < 0)
                                        {
                                            return (IntPtr)HITTEST.HTLEFT;
                                        }
                                        else if (localPoint.X < 2 * DisplayInformation.RawPixelsPerViewPixel)
                                        {
                                            return (IntPtr)HITTEST.HTCAPTION;
                                        }
                                        else if (localPoint.X < 42 * DisplayInformation.RawPixelsPerViewPixel)
                                        {
                                            return (IntPtr)HITTEST.HTCLIENT;
                                        }
                                        else
                                        {
                                            if (CoreAppWindow.Size.Width - localPoint.X <= 16)
                                            {
                                                return (IntPtr)HITTEST.HTRIGHT;
                                            }
                                            else if (CoreAppWindow.Size.Width - localPoint.X <= 16 + 46)
                                            {
                                                return (IntPtr)HITTEST.HTCLOSE;
                                            }
                                            else if (CoreAppWindow.Size.Width - localPoint.X <= 16 + 46 * 2)
                                            {
                                                return (IntPtr)HITTEST.HTMAXBUTTON;
                                            }
                                            else if (CoreAppWindow.Size.Width - localPoint.X <= 16 + 46 * 3)
                                            {
                                                return (IntPtr)HITTEST.HTMINBUTTON;
                                            }
                                            else
                                            {
                                                return (IntPtr)HITTEST.HTCAPTION;
                                            }
                                        }
                                    }
                                    else if (localPoint.Y < 45 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        return localPoint.X < 0 ? (IntPtr)HITTEST.HTLEFT : CoreAppWindow.Size.Width - localPoint.X <= 16 ? (IntPtr)HITTEST.HTRIGHT : (IntPtr)HITTEST.HTCAPTION;
                                    }
                                }
                                else
                                {
                                    if (localPoint.Y < 4)
                                    {
                                        return localPoint.X < 0 ? (IntPtr)HITTEST.HTTOPRIGHT : CoreAppWindow.Size.Width - localPoint.X <= 20 ? (IntPtr)HITTEST.HTTOPRIGHT : (IntPtr)HITTEST.HTTOP;
                                    }
                                    else if (localPoint.Y < 30 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        if (localPoint.X < 0)
                                        {
                                            return (IntPtr)HITTEST.HTLEFT;
                                        }
                                        else if (localPoint.X < 2 * DisplayInformation.RawPixelsPerViewPixel)
                                        {
                                            return (IntPtr)HITTEST.HTCAPTION;
                                        }
                                        else if (localPoint.X < 42 * DisplayInformation.RawPixelsPerViewPixel)
                                        {
                                            return (IntPtr)HITTEST.HTCLIENT;
                                        }
                                        else
                                        {
                                            if (CoreAppWindow.Size.Width - localPoint.X <= 20)
                                            {
                                                return (IntPtr)HITTEST.HTRIGHT;
                                            }
                                            else if (CoreAppWindow.Size.Width - localPoint.X <= 16 + 46)
                                            {
                                                return (IntPtr)HITTEST.HTCLOSE;
                                            }
                                            else if (CoreAppWindow.Size.Width - localPoint.X <= 16 + 46 * 2)
                                            {
                                                return (IntPtr)HITTEST.HTMAXBUTTON;
                                            }
                                            else if (CoreAppWindow.Size.Width - localPoint.X <= 16 + 46 * 3)
                                            {
                                                return (IntPtr)HITTEST.HTMINBUTTON;
                                            }
                                            else
                                            {
                                                return (IntPtr)HITTEST.HTCAPTION;
                                            }
                                        }
                                    }
                                    else if (localPoint.Y < 45 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        return localPoint.X < 0 ? (IntPtr)HITTEST.HTLEFT : CoreAppWindow.Size.Width - localPoint.X <= 16 ? (IntPtr)HITTEST.HTRIGHT : (IntPtr)HITTEST.HTCAPTION;
                                    }
                                }
                            }
                        }

                        break;
                    }
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (Window.Current is not null && Window.Current.Content is MainPage mainPage && mainPage.TitlebarMenuFlyout.IsOpen)
                        {
                            mainPage.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam is 2 && Window.Current is not null && Window.Current.CoreWindow is not null && Window.Current.Content is MainPage mainPage && DisplayInformation is not null)
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

                            mainPage.TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return 0;
                    }
                // 处理 Alt + space 按键弹出窗口右键菜单的消息
                case WindowMessage.WM_SYSKEYDOWN:
                    {
                        if ((wParam == (UIntPtr)VirtualKey.Space) && ((lParam & 0x20000000) is not 0) && Window.Current is not null && Window.Current.Content is MainPage mainPage)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new Point(0, 45),
                                ShowMode = FlyoutShowMode.Standard
                            };
                            mainPage.TitlebarMenuFlyout.ShowAt(null, options);
                        }

                        break;
                    }
                // 提升权限时允许应用接收拖放文件消息
                case WindowMessage.WM_DROPFILES:
                    {
                        if (Window.Current is not null && Window.Current.Content is MainPage mainPage && Window.Current.CoreWindow is CoreWindow coreWindow)
                        {
                            Task.Run(() =>
                            {
                                List<string> filesList = [];
                                char[] dragFileCharArray = new char[260];
                                uint filesCount = Shell32Library.DragQueryFile(wParam, 0xffffffffu, null, 0);

                                for (uint index = 0; index < filesCount; index++)
                                {
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
                                        await mainPage.DealElevatedDragDropAsync(filesList[0]);
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

            AlwaysShowBackdropService.InitializeAlwaysShowBackdrop();
            BackdropService.InitializeBackdrop();
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
            AppWindowTitleBar titleBar = CoreAppWindow.TitleBar;

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

using GetStoreAppInstaller.Services.Controls.Settings;
using GetStoreAppInstaller.Services.Root;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Dwmapi;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using WinRT;

// 抑制 CA1806，CA1421 警告
#pragma warning disable CA1806,CA1421

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 获取商店应用 应用安装器
    /// </summary>
    public partial class Program
    {
        public static DisplayInformation DisplayInformation { get; private set; }

        private static SUBCLASSPROC mainWindowSubClassProc;

        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            InitializeResources();

            // 创建 CoreWindow
            WindowsUILibrary.PrivateCreateCoreWindow(WINDOW_TYPE.NOT_IMMERSIVE, "获取商店应用 应用安装器", 400, 400, 960, 600, 0, IntPtr.Zero, typeof(ICoreWindow).GUID, out IntPtr coreWindowPtr);
            CoreWindow coreWindow = CoreWindow.FromAbi(coreWindowPtr);
            coreWindow.As<ICoreWindowInterop>().GetWindowHandle(out IntPtr coreWindowHandle);
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(coreWindow.DispatcherQueue));
            DisplayInformation = DisplayInformation.GetForCurrentView();
            coreWindow.FlowDirection = LanguageService.FlowDirection is FlowDirection.LeftToRight ? CoreWindowFlowDirection.LeftToRight : CoreWindowFlowDirection.RightToLeft;
            new XamlIslandsApp();

            // 设置 CoreWindow 窗口样式
            WindowStyle windowStyle = (WindowStyle)GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE);
            windowStyle |= WindowStyle.WS_OVERLAPPEDWINDOW;
            windowStyle &= ~WindowStyle.WS_POPUP;
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE, (IntPtr)windowStyle);

            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(coreWindowHandle, mainWindowSubClassProc, 0, IntPtr.Zero);
            User32Library.SetWindowPos(coreWindowHandle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_FRAMECHANGED);

            CoreApplication.As<ICoreApplicationPrivate2>().CreateNonImmersiveView(out IntPtr coreApplicationViewPtr);
            CoreApplicationView coreApplicationView = CoreApplicationView.FromAbi(coreApplicationViewPtr);

            // 创建 UI 窗口
            FrameworkView frameworkView = new();
            frameworkView.Initialize(coreApplicationView);
            frameworkView.SetWindow(coreWindow);
            SetAppIcon(coreWindowHandle);
            coreWindow.Activate();
            XamlControlsResources xamlControlsResources = [];
            xamlControlsResources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("ms-appx:///Styles/XamlIslands/MenuFlyout.xaml") });
            Application.Current.Resources = xamlControlsResources;
            Window.Current.Content = new MainPage();
            Window.Current.SizeChanged += OnSizeChanged;
            frameworkView.Run();
        }

        #region 第一部分：窗口所挂载的事件

        /// <summary>
        /// 窗口大小发生改变时触发的事件
        /// </summary>
        private static void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            if (Window.Current.Content is not null)
            {
                Window.Current.CoreWindow.As<ICoreWindowInterop>().GetWindowHandle(out IntPtr coreWindowhandle);
                (Window.Current.Content as MainPage).IsWindowMaximized = User32Library.IsZoomed(coreWindowhandle);
            }
        }

        #endregion 第一部分：窗口所挂载的事件

        #region 第二部分：窗口初始化

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
        private static void SetAppIcon(IntPtr hwnd)
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
                User32Library.SendMessage(hwnd, WindowMessage.WM_SETICON, 1, hIcons[0]);
                User32Library.SendMessage(hwnd, WindowMessage.WM_SETICON, 0, hIcons[0]);
            }
        }

        #endregion 第二部分：窗口初始化

        #region 第三部分：窗口过程处理

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private static IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            if (DwmapiLibrary.DwmDefWindowProc(hWnd, Msg, wParam, lParam, out IntPtr result))
            {
                return result;
            }

            switch (Msg)
            {
                // 窗口移动时的消息
                case WindowMessage.WM_MOVE:
                    {
                        if (Window.Current is not null && Window.Current.Content is not null)
                        {
                            (Window.Current.Content as MainPage).IsWindowMaximized = User32Library.IsZoomed(hWnd);

                            if ((Window.Current.Content as MainPage).TitlebarMenuFlyout.IsOpen)
                            {
                                (Window.Current.Content as MainPage).TitlebarMenuFlyout.Hide();
                            }
                        }

                        break;
                    }
                // 窗口关闭时的消息
                case WindowMessage.WM_CLOSE:
                    {
                        Window.Current.SizeChanged -= OnSizeChanged;
                        Comctl32Library.RemoveWindowSubclass(hWnd, mainWindowSubClassProc, 0);
                        (Application.Current as XamlIslandsApp).Dispose();
                        break;
                    }

                // 重新计算窗口工作区大小和位置时的消息
                case WindowMessage.WM_NCCALCSIZE:
                    {
                        if (wParam != UIntPtr.Zero)
                        {
                            NCCALCSIZE_PARAMS nccalcsize_params = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(lParam);

                            PointInt32 border = new()
                            {
                                X = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER),
                                Y = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CYFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER)
                            };

                            if (User32Library.IsZoomed(hWnd))
                            {
                                nccalcsize_params.rgrc[0].left += border.X;
                                nccalcsize_params.rgrc[0].top += border.Y;
                                nccalcsize_params.rgrc[0].right -= border.X;
                                nccalcsize_params.rgrc[0].bottom -= border.Y;
                            }
                            else
                            {
                                nccalcsize_params.rgrc[0].left += 1;
                                nccalcsize_params.rgrc[0].top += 1;
                                nccalcsize_params.rgrc[0].right -= 1;
                                nccalcsize_params.rgrc[0].bottom -= 1;
                            }

                            Marshal.StructureToPtr(nccalcsize_params, lParam, false);
                            return 768;
                        }

                        break;
                    }
                // 发送到窗口以确定窗口的哪个部分对应于特定的屏幕坐标的消息
                case WindowMessage.WM_NCHITTEST:
                    {
                        PointInt32 currentPoint = new(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
                        User32Library.MapWindowPoints(IntPtr.Zero, hWnd, ref currentPoint, 2);

                        PointInt32 border = new()
                        {
                            X = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER),
                            Y = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CYFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER)
                        };

                        User32Library.GetWindowRect(hWnd, out RECT windowRect);

                        // 窗口左边界
                        if (currentPoint.X < border.X)
                        {
                            // 需要考虑镜像反转（文字从右到左）
                            if (Window.Current.CoreWindow is not null && Window.Current.CoreWindow.FlowDirection is CoreWindowFlowDirection.LeftToRight)
                            {
                                // 窗口左上角
                                if (currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTTOPLEFT;
                                }
                                // 窗口左下角
                                else if (windowRect.bottom - windowRect.top - currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTBOTTOMLEFT;
                                }
                                // 窗口左边界
                                else
                                {
                                    return (IntPtr)HITTEST.HTLEFT;
                                }
                            }
                            else
                            {
                                // 窗口右上角
                                if (currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTTOPRIGHT;
                                }
                                // 窗口右下角
                                else if (windowRect.bottom - windowRect.top - currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTBOTTOMRIGHT;
                                }
                                // 窗口右边界
                                else
                                {
                                    return (IntPtr)HITTEST.HTRIGHT;
                                }
                            }
                        }
                        // 窗口右边界
                        else if (windowRect.right - windowRect.left - currentPoint.X < border.X)
                        {
                            // 需要考虑镜像反转（文字从右到左）
                            if (Window.Current.CoreWindow is not null && Window.Current.CoreWindow.FlowDirection is CoreWindowFlowDirection.LeftToRight)
                            {
                                // 窗口右上角
                                if (currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTTOPRIGHT;
                                }
                                // 窗口右下角
                                else if (windowRect.bottom - windowRect.top - currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTBOTTOMRIGHT;
                                }
                                // 窗口右边界
                                else
                                {
                                    return (IntPtr)HITTEST.HTRIGHT;
                                }
                            }
                            else
                            {
                                // 窗口左上角
                                if (currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTTOPLEFT;
                                }
                                // 窗口左下角
                                else if (windowRect.bottom - windowRect.top - currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTBOTTOMLEFT;
                                }
                                // 窗口左边界
                                else
                                {
                                    return (IntPtr)HITTEST.HTLEFT;
                                }
                            }
                        }
                        // 窗口上边界，下边界，标题栏和窗口内容
                        else
                        {
                            if (User32Library.IsZoomed(hWnd))
                            {
                                // 窗口上边界
                                if (currentPoint.Y < 0)
                                {
                                    return (IntPtr)HITTEST.HTTOP;
                                }
                                // 窗口下边界
                                else if (windowRect.bottom - windowRect.top - currentPoint.Y < 0)
                                {
                                    return (IntPtr)HITTEST.HTBOTTOM;
                                }
                                else
                                {
                                    // 窗口标题栏和窗口三大按钮
                                    if (currentPoint.Y < 30 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        // 关闭按钮
                                        if (windowRect.right - windowRect.left - currentPoint.X < 46 * DisplayInformation.RawPixelsPerViewPixel + 2 * border.Y)
                                        {
                                            return (IntPtr)HITTEST.HTCLOSE;
                                        }
                                        // 最大化按钮
                                        else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 2 * DisplayInformation.RawPixelsPerViewPixel + 2 * border.Y)
                                        {
                                            return (IntPtr)HITTEST.HTMAXBUTTON;
                                        }
                                        // 最小化按钮
                                        else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 3 * DisplayInformation.RawPixelsPerViewPixel + 2 * border.Y)
                                        {
                                            return (IntPtr)HITTEST.HTMINBUTTON;
                                        }
                                        else
                                        {
                                            return (IntPtr)HITTEST.HTCAPTION;
                                        }
                                    }
                                    // 窗口内容区
                                    else
                                    {
                                        return (IntPtr)HITTEST.HTCLIENT;
                                    }
                                }
                            }
                            else
                            {
                                // 窗口上边界
                                if (currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTTOP;
                                }
                                // 窗口下边界
                                else if (windowRect.bottom - windowRect.top - currentPoint.Y < border.Y)
                                {
                                    return (IntPtr)HITTEST.HTBOTTOM;
                                }
                                else
                                {
                                    // 窗口标题栏和窗口三大按钮
                                    if (currentPoint.Y < (30 + border.Y) * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        // 关闭按钮
                                        if (windowRect.right - windowRect.left - currentPoint.X < 46 * DisplayInformation.RawPixelsPerViewPixel)
                                        {
                                            return (IntPtr)HITTEST.HTCLOSE;
                                        }
                                        // 最大化按钮
                                        else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 2 * DisplayInformation.RawPixelsPerViewPixel)
                                        {
                                            return (IntPtr)HITTEST.HTMAXBUTTON;
                                        }
                                        // 最小化按钮
                                        else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 3 * DisplayInformation.RawPixelsPerViewPixel)
                                        {
                                            return (IntPtr)HITTEST.HTMINBUTTON;
                                        }
                                        else
                                        {
                                            return (IntPtr)HITTEST.HTCAPTION;
                                        }
                                    }
                                    // 窗口内容区
                                    else
                                    {
                                        return (IntPtr)HITTEST.HTCLIENT;
                                    }
                                }
                            }
                        }
                    }
                // 窗口框架重绘时的消息
                case WindowMessage.WM_NCPAINT:
                    {
                        MARGINS margins = new()
                        {
                            cxLeftWidth = -1,
                            cxRightWidth = -1,
                            cyTopHeight = -1,
                            cyBottomHeight = -1,
                        };

                        DwmapiLibrary.DwmExtendFrameIntoClientArea(hWnd, ref margins);
                        break;
                    }
                // 处理非客户区区域以指示活动或非活动状态时的消息
                case WindowMessage.WM_NCACTIVATE:
                    {
                        if (DwmapiLibrary.DwmIsCompositionEnabled(out bool enabled) is not 0 && !enabled)
                        {
                            return 1;
                        }
                        break;
                    }
                // 处理窗口非工作区左键释放的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (Window.Current is not null && Window.Current.Content is not null)
                        {
                            (Window.Current.Content as MainPage).IsWindowMaximized = User32Library.IsZoomed(hWnd);

                            if ((Window.Current.Content as MainPage).TitlebarMenuFlyout.IsOpen)
                            {
                                (Window.Current.Content as MainPage).TitlebarMenuFlyout.Hide();
                            }
                        }

                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam.ToUInt32() is 2 && DisplayInformation is not null)
                        {
                            PointInt32 screenPoint = new(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
                            User32Library.MapWindowPoints(IntPtr.Zero, hWnd, ref screenPoint, 2);

                            PointInt32 border = new()
                            {
                                X = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER),
                                Y = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CYFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER)
                            };

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = new Point(screenPoint.X / DisplayInformation.RawPixelsPerViewPixel, screenPoint.Y / DisplayInformation.RawPixelsPerViewPixel)
                            };

                            (Window.Current.Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return 0;
                    }
                // 处理按下 Alt + Space 时的键盘消息，弹出窗口菜单
                case WindowMessage.WM_SYSKEYDOWN:
                    {
                        VirtualKey virtualKey = (VirtualKey)wParam;
                        CoreVirtualKeyStates coreVirtualKeyStates = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Menu);
                        if (virtualKey is VirtualKey.Space && coreVirtualKeyStates is CoreVirtualKeyStates.Down)
                        {
                            User32Library.SendMessage(hWnd, WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_KEYMENU, (IntPtr)VirtualKey.Space);
                        }
                        break;
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
                            (Window.Current.Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
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
                                (Window.Current.Content as MainPage).TitlebarMenuFlyout.ShowAt(null, options);
                            }

                            return 0;
                        }
                        break;
                    }
                // 处理鼠标按键释放的消息
                case WindowMessage.WM_CAPTURECHANGED:
                    {
                        User32Library.GetCursorPos(out PointInt32 currentPoint);
                        User32Library.MapWindowPoints(IntPtr.Zero, hWnd, ref currentPoint, 2);

                        PointInt32 border = new()
                        {
                            X = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER),
                            Y = User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CYFRAME) + User32Library.GetSystemMetrics(SYSTEMMETRICSINDEX.SM_CXPADDEDBORDER)
                        };

                        User32Library.GetWindowRect(hWnd, out RECT windowRect);

                        if (currentPoint.X >= border.X && windowRect.right - windowRect.left - currentPoint.X >= border.X)
                        {
                            // 窗口处于最大化状态
                            if (User32Library.IsZoomed(hWnd))
                            {
                                // 窗口处于右上角三个按键范围内
                                if (currentPoint.Y >= 0 && windowRect.bottom - windowRect.top - currentPoint.Y >= 0 && currentPoint.Y < 30 * DisplayInformation.RawPixelsPerViewPixel)
                                {
                                    // 关闭按钮
                                    if (windowRect.right - windowRect.left - currentPoint.X < 46 * DisplayInformation.RawPixelsPerViewPixel + 2 * border.Y)
                                    {
                                        User32Library.SendMessage(hWnd, WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_CLOSE, 0);
                                    }
                                    // 最大化按钮
                                    else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 2 * DisplayInformation.RawPixelsPerViewPixel + 2 * border.Y)
                                    {
                                        User32Library.SendMessage(hWnd, WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_RESTORE, 0);
                                    }
                                    // 最小化按钮
                                    else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 3 * DisplayInformation.RawPixelsPerViewPixel + 2 * border.Y)
                                    {
                                        User32Library.SendMessage(hWnd, WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MINIMIZE, 0);
                                    }
                                }
                            }
                            else
                            {
                                // 窗口处于右上角三个按键范围内
                                if (currentPoint.Y >= 0 && windowRect.bottom - windowRect.top - currentPoint.Y >= 0 && currentPoint.Y < 30 * DisplayInformation.RawPixelsPerViewPixel)
                                {
                                    // 关闭按钮
                                    if (windowRect.right - windowRect.left - currentPoint.X < 46 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        User32Library.SendMessage(hWnd, WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_CLOSE, 0);
                                    }
                                    // 最大化按钮
                                    else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 2 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        User32Library.SendMessage(hWnd, WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
                                    }
                                    // 最小化按钮
                                    else if (windowRect.right - windowRect.left - currentPoint.X < 46 * 3 * DisplayInformation.RawPixelsPerViewPixel)
                                    {
                                        User32Library.SendMessage(hWnd, WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MINIMIZE, 0);
                                    }
                                }
                            }
                        }

                        break;
                    }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第三部分：窗口过程处理

        #region 第四部分：窗口辅助工具

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

        private static int GET_X_LPARAM(IntPtr lParam)
        {
            return unchecked((short)(long)lParam);
        }

        private static int GET_Y_LPARAM(IntPtr lParam)
        {
            return unchecked((short)((long)lParam >> 16));
        }

        #endregion 第四部分：窗口辅助工具
    }
}

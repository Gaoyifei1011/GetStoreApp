using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 获取商店应用 应用安装器
    /// </summary>
    public partial class Program
    {
        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            WindowsUILibrary.PrivateCreateCoreWindow(WINDOW_TYPE.NOT_IMMERSIVE, "GetStoreAppInstaller", 100, 100, 500, 500, 0, IntPtr.Zero, typeof(ICoreWindow).GUID, out IntPtr coreWindowPtr);
            CoreWindow coreWindow = CoreWindow.FromAbi(coreWindowPtr);
            coreWindow.As<ICoreWindowInterop>().GetWindowHandle(out IntPtr coreWindowHandle);
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(coreWindow.DispatcherQueue));
            new XamlIslandsApp();

            CoreApplication.As<ICoreApplicationPrivate2>().CreateNonImmersiveView(out IntPtr coreApplicationViewPtr);
            CoreApplicationView coreApplicationView = CoreApplicationView.FromAbi(coreApplicationViewPtr);

            int style = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE);
            style &= ~(int)WindowStyle.WS_CHILD;
            style &= ~(int)WindowStyle.WS_CLIPCHILDREN;
            style &= ~(int)WindowStyle.WS_CLIPSIBLINGS;
            style &= ~unchecked((int)WindowStyle.WS_POPUP);
            style |= (int)WindowStyle.WS_OVERLAPPEDWINDOW;
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE, style);

            int exStyle = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_EXSTYLE);
            exStyle &= ~(int)WindowExStyle.WS_EX_NOREDIRECTIONBITMAP;
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_EXSTYLE, exStyle);

            User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_HIDE);
            User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_MINIMIZE);
            User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_HIDE);
            User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_SHOWNORMAL);

            coreWindow.Activate();

            FrameworkView frameworkView = new();
            frameworkView.Initialize(coreApplicationView);
            frameworkView.SetWindow(coreWindow);

            Application.Current.Resources = new XamlControlsResources();
            Window.Current.Content = new MainPage();
            frameworkView.Run();
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

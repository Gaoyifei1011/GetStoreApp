using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
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
    [GeneratedComInterface, Guid("CD292360-2763-4085-8A9F-74B224A29175")]
    public partial interface ICoreWindowFactory
    {
        /// <summary>
        /// 获取由当前Windows 运行时类实现的接口。
        /// </summary>
        /// <param name="iidCount">当前 Windows 运行时 对象实现的接口数，不包括 IUnknown 和 IInspectable 实现。</param>
        /// <param name="iids">指向数组的指针，该数组包含当前 Windows 运行时 对象实现的每个接口的 IID。 排除 IUnknown 和 IInspectable 接口。</param>
        /// <returns>此函数可以返回以下值。S_OK 和 E_OUTOFMEMORY</returns>
        [PreserveSig]
        int GetIids(out ulong iidCount, out IntPtr iids);

        /// <summary>
        /// 获取当前Windows 运行时 对象的完全限定名称。
        /// </summary>
        /// <param name="className">当前Windows 运行时对象的完全限定名称。</param>
        /// <returns>此函数可以返回以下值。S_OK、E_OUTOFMEMORY 和 E_ILLEGAL_METHOD_CALL</returns>
        [PreserveSig]
        int GetRuntimeClassName(out IntPtr className);

        /// <summary>
        /// 获取当前Windows 运行时对象的信任级别。
        /// </summary>
        /// <param name="trustLevel">当前Windows 运行时对象的信任级别。 默认值为 BaseLevel。</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetTrustLevel(out TrustLevel trustLevel);

        [PreserveSig]
        int CreateCoreWindow(IntPtr windowTitle, out IntPtr coreWindow);

        [PreserveSig]
        int WindowReuseAllowed([MarshalAs(UnmanagedType.Bool)] out bool value);
    }

    /// <summary>
    /// 获取商店应用 应用安装器
    /// </summary>
    public class Program
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
            coreWindow.Activate();
            int style = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE);
            //style &= ~(int)WindowStyle.WS_CHILD;
            style |= (int)WindowStyle.WS_CLIPCHILDREN;
            //style &= ~unchecked((int)WindowStyle.WS_POPUP);
            style |= (int)WindowStyle.WS_OVERLAPPEDWINDOW;
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE, style);

            int exStyle = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_EXSTYLE);
            exStyle &= ~(int)WindowExStyle.WS_EX_NOREDIRECTIONBITMAP;
            exStyle |= (int)WindowExStyle.WS_EX_OVERLAPPEDWINDOW;
            exStyle |= (int)WindowExStyle.WS_EX_LAYERED;
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_EXSTYLE, exStyle);

            //User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_HIDE);
            //User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_MINIMIZE);
            //User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_HIDE);
            //User32Library.ShowWindow(coreWindowHandle, WindowShowStyle.SW_SHOWNORMAL);

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

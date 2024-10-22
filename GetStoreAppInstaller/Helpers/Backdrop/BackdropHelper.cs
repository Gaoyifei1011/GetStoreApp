using System;
using Windows.System;
using Windows.UI.Composition.Desktop;
using WindowsTools.WindowsAPI.ComTypes;

namespace GetStoreAppInstaller.Helpers.Backdrop
{
    /// <summary>
    /// 背景色辅助类
    /// </summary>
    public static class BackdropHelper
    {
        /// <summary>
        /// 初始化 DesktopWindowTarget（表示作为合成目标的窗口）
        /// </summary>
        public static DesktopWindowTarget InitializeDesktopWindowTarget(IntPtr handle, bool isTopMost)
        {
            if (handle == IntPtr.Zero)
            {
                throw new NullReferenceException("窗口句柄无效");
            }

            IntPtr desktopWindowTargetPtr = IntPtr.Zero;
            if (DispatcherQueue.GetForCurrentThread() is not null)
            {
                ICompositorDesktopInterop interop = Window.Current.Compositor as object as ICompositorDesktopInterop;
                interop.CreateDesktopWindowTarget(handle, isTopMost, out desktopWindowTargetPtr);
            }

            return desktopWindowTargetPtr != IntPtr.Zero ? DesktopWindowTarget.FromAbi(desktopWindowTargetPtr) : null;
        }
    }
}

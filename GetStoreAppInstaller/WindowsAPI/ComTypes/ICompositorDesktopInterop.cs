using System;
using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.ComTypes
{
    [ComImport, Guid("29E691FA-4567-4DCA-B319-D0F207EB6807"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICompositorDesktopInterop
    {
        [PreserveSig]
        int CreateDesktopWindowTarget(IntPtr hwndTarget, [MarshalAs(UnmanagedType.Bool)] bool isTopmost, out IntPtr result);

        [PreserveSig]
        int EnsureOnThread(int threadId);
    }
}

using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FILETIME
    {
        private uint dwLowDateTime;
        private uint dwHighDateTime;
    }
}

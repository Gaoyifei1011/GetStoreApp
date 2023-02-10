using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WIN32_FIND_DATA
    {
        public const int MAX_PATH = 260;

        private uint dwFileAttributes;
        private FILETIME ftCreationTime;
        private FILETIME ftLastAccessTime;
        private FILETIME ftLastWriteTime;
        private uint nFileSizeHight;
        private uint nFileSizeLow;
        private uint dwOID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        private string cFileName;
    }
}

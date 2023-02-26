using System;
using System.Runtime.InteropServices;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.Kernel32
{
    public class Kernel32Library
    {
        private const string Kernel32 = "Kernel32.dll";

        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetModuleHandle([In][Optional] string lpModuleName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern void GetStartupInfoW([Out] out STARTUPINFO lpStartupInfo);
    }
}

using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.UI.Controls.Custom
{
    public class WinUIMessageBox
    {
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr HWND, string lpText, string lpCaption, uint Type);

        public const int MB_OK = 0x00000000;
        public const int MB_OKCANCEL = 0x00000001;
        public const int MB_ABORTRETRYIGNORE = 0x00000002;
        public const int MB_YESNOCANCEL = 0x00000003;
        public const int MB_YESNO = 0x00000004;
        public const int MB_RETRYCANCEL = 0x00000005;
        public const int MB_CANCELTRYCONTINUE = 0x00000006;
        public const int MB_ICONHAND = 0x00000010;
        public const int MB_ICONQUESTION = 0x00000020;
        public const int MB_ICONEXCLAMATION = 0x00000030;
        public const int MB_ICONASTERISK = 0x00000040;
        public const int MB_USERICON = 0x00000080;
        public const int MB_ICONWARNING = MB_ICONEXCLAMATION;
        public const int MB_ICONERROR = MB_ICONHAND;
        public const int MB_ICONINFORMATION = MB_ICONASTERISK;
        public const int MB_ICONSTOP = MB_ICONHAND;

        public const int MB_DEFBUTTON1 = 0x00000000;
        public const int MB_DEFBUTTON2 = 0x00000100;
        public const int MB_DEFBUTTON3 = 0x00000200;
        public const int MB_DEFBUTTON4 = 0x00000300;
        public const int MB_APPLMODAL = 0x00000000;
        public const int MB_SYSTEMMODAL = 0x00001000;
        public const int MB_TASKMODAL = 0x00002000;
        public const int MB_HELP = 0x00004000; // Help Button
        public const int MB_NOFOCUS = 0x00008000;
        public const int MB_SETFOREGROUND = 0x00010000;
        public const int MB_DEFAULT_DESKTOP_ONLY = 0x00020000;
        public const int MB_TOPMOST = 0x00040000;
        public const int MB_RIGHT = 0x00080000;
        public const int MB_RTLREADING = 0x00100000;
        public const int MB_SERVICE_NOTIFICATION = 0x00200000;
        public const int MB_SERVICE_NOTIFICATION_NT3X = 0x00040000;
        public const int MB_TYPEMASK = 0x0000000F;
        public const int MB_ICONMASK = 0x000000F0;
        public const int MB_DEFMASK = 0x00000F00;
        public const int MB_MODEMASK = 0x00003000;
        public const int MB_MISCMASK = 0x0000C000;

        public static int ShowMessageBox(IntPtr HWND, string lpText, string lpCaption, uint Type)
        {
            return MessageBox(HWND, lpText, lpCaption, Type);
        }
    }
}

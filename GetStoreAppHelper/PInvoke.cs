using System;
using System.Runtime.InteropServices;

namespace Native.Win32.System.Console
{
    public static class Text
    {
        [Flags]
        public enum CharacterAttributes : ushort
        { }
    }
}

namespace Native.Win32.System.LibraryLoader
{
    public static class Module
    {
        [DllImport(
            "Kernel32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
        public static extern IntPtr GetModuleHandleW(
            [In][Optional] string lpModuleName);
    }
}

namespace Native.Win32.System.Threading
{
    public static class Startup
    {
        [Flags]
        public enum Flag : uint
        { }

        [StructLayout(
            LayoutKind.Sequential,
            CharSet = CharSet.Unicode)]
        public struct InfoW
        {
            public uint cb;
            public IntPtr lpReserved;
            public IntPtr lpDesktop;
            public IntPtr lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public Console.Text.CharacterAttributes dwFillAttribute;
            public Flag dwFlags;
            public UI.WindowsAndMessaging.Window.ShowCmd wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [DllImport(
            "Kernel32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true)]
        public static extern void GetStartupInfoW(
            [Out] out InfoW lpStartupInfo);
    }
}

namespace Native.Win32.UI.Input.KeyboardAndMouse
{
    public static class Keyboard
    {
        [Flags]
        public enum Notifications : uint
        {
            SysKeyDown = 0x0104
        }

        [Flags]
        public enum VirtualKey : ushort
        {
            F4 = 0x73
        }
    }
}

namespace Native.Win32.UI.WindowsAndMessaging
{
    public static class Message
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Msg
        {
            public IntPtr hwnd;
            public uint message;
            public UIntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
        }

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true)]
        public static extern IntPtr DispatchMessageW([In] in Msg lpMsg);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMessageW(
            [Out] out Msg lpMsg,
            [In][Optional] IntPtr hWnd,
            [In] uint wMsgFilterMin,
            [In] uint wMsgFilterMax);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true)]
        public static extern IntPtr SendMessageW(
            [In] IntPtr hWnd,
            [In] uint Msg,
            [In][Optional] UIntPtr wParam,
            [In][Optional] IntPtr lParam);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage([In] in Msg lpMsg);
    }

    public static class Window
    {
        [Flags]
        public enum ExStyle : uint
        {
            Left = 0x00000000
        }

        [Flags]
        public enum ShowCmd : uint
        { }

        [Flags]
        public enum Style : uint
        {
            Caption = 0x00C00000,
            MinimizeBox = 0x00020000,
            MaximizeBox = 0x00010000,
            OverLapped = 0x00000000,

            OverLappedWindow =
                OverLapped | Caption | SysMenu
                | ThickFrame | MinimizeBox | MaximizeBox,

            SysMenu = 0x00080000,
            ThickFrame = 0x00040000
        }

        public const int CreateUseDefault = unchecked((int)0x80000000);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
        public static extern IntPtr CreateWindowExW(
            [In] ExStyle dwExStyle,
            [In][Optional] string lpClassName,
            [In][Optional] string lpWindowName,
            [In] Style dwStyle,
            [In] int X,
            [In] int Y,
            [In] int nWidth,
            [In] int nHeight,
            [In][Optional] IntPtr hWndParent,
            [In][Optional] IntPtr hMenu,
            [In][Optional] IntPtr hInstance,
            [In][Optional] IntPtr lpParam);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(
            [In] IntPtr hWnd,
            [In] ShowCmd nCmdShow);

        [DllImport(
            "User32.dll",
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow([In] IntPtr hWnd);

        public static class Ancestor
        {
            [Flags]
            public enum Flag : uint
            {
                Root = 2
            }

            [DllImport(
                "User32.dll",
                CharSet = CharSet.Unicode,
                ExactSpelling = true)]
            public static extern IntPtr GetAncestor(
                [In] IntPtr hwnd,
                [In] Flag gaFlags);
        }
    }
}

using Native.Win32.System.LibraryLoader;
using Native.Win32.System.Threading;
using Native.Win32.UI.Input.KeyboardAndMouse;
using Native.Win32.UI.WindowsAndMessaging;
using System;
using System.Runtime.InteropServices;

// 获取商店应用辅助程序，创建任务栏菜单图标，使用Mile Xaml(Xaml Islands)实现任务栏右键菜单外观现代化，为获取商店应用程序提供模态对话框支持，该部分未来可能会采用C++开发。（尚未进行）
namespace GetStoreAppHelper
{
    public static class Program
    {
        [STAThread]
        public static int Main()
        {
            App app = new();

            var hWnd = Window.CreateWindowExW(
                dwExStyle: Window.ExStyle.Left,
                lpClassName: "Mile.Xaml.ContentWindow",
                lpWindowName: "获取商店应用 辅助程序",
                dwStyle: Window.Style.OverLappedWindow,
                X: Window.CreateUseDefault,
                Y: 0,
                nWidth: Window.CreateUseDefault,
                nHeight: 0,
                hInstance: Module.GetModuleHandleW(null),
                lpParam: Marshal.GetIUnknownForObject(new MainPage()));
            if (hWnd == IntPtr.Zero)
            {
                return -1;
            }

            Startup.InfoW info = new();
            info.cb = (uint)Marshal.SizeOf(info);
            Startup.GetStartupInfoW(out info);

            Window.ShowWindow(hWnd, info.wShowWindow);
            Window.UpdateWindow(hWnd);

            Message.Msg msg;
            while (Message.GetMessageW(
                lpMsg: out msg,
                wMsgFilterMin: 0,
                wMsgFilterMax: 0))
            {
                if (msg.message == (uint)Keyboard.Notifications.SysKeyDown
                    && msg.wParam == (UIntPtr)Keyboard.VirtualKey.F4)
                {
                    Message.SendMessageW(
                        Window.Ancestor.GetAncestor(
                            hWnd,
                            Window.Ancestor.Flag.Root),
                        msg.message,
                        msg.wParam,
                        msg.lParam);
                    continue;
                }

                Message.TranslateMessage(in msg);
                Message.DispatchMessageW(in msg);
            }

            app.Dispose();
            // Due to the design of .Net Native, the program won't exit when returning a number.
            return unchecked((int)msg.wParam.ToUInt32());
        }
    }
}

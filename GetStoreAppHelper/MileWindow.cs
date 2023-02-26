using GetStoreAppHelper.WindowsAPI.PInvoke.Kernel32;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace GetStoreAppHelper
{
    public class MileWindow
    {
        private STARTUPINFO StartupInfo = new STARTUPINFO();

        public PointInt32 Position = new PointInt32();

        public PointInt32 Size = new PointInt32();

        private bool isWindowCreated = false;

        public string Title { get; set; } = string.Empty;

        public IntPtr Handle { get; private set; } = IntPtr.Zero;

        public object Content { get; set; } = null;

        public MileWindow([Optional] string title, [Optional] object content, [Optional] PointInt32 position, [Optional] PointInt32 size)
        {
            if (title is not null) Title = title;
            if (content is not null) Content = content;
            if (!position.Equals(Position))
            {
                Position.X = position.X;
                Position.Y = position.Y;
            }
            if (!size.Equals(Size))
            {
                Size.X = size.X;
                Size.Y = size.Y;
            }
        }

        public void InitializeWindow()
        {
            Handle = User32Library.CreateWindowEx(
                WindowStylesEx.WS_EX_LEFT,
                "Mile.Xaml.ContentWindow",
                Title,
                WindowStyles.WS_OVERLAPPEDWINDOW,
                Position.X,
                Position.Y,
                Size.X,
                Size.Y,
                IntPtr.Zero,
                IntPtr.Zero,
                Kernel32Library.GetModuleHandle(null),
                Marshal.GetIUnknownForObject(Content)
                );

            if (Handle == IntPtr.Zero)
            {
                throw new ApplicationException("初始化窗口失败");
            }
            else
            {
                isWindowCreated = true;
            }
        }

        public void Activate()
        {
            if (isWindowCreated)
            {
                StartupInfo.cb = Marshal.SizeOf(StartupInfo);
                Kernel32Library.GetStartupInfoW(out StartupInfo);

                User32Library.ShowWindow(Handle, StartupInfo.wShowWindow);
                User32Library.UpdateWindow(Handle);

                while (User32Library.GetMessage(out MSG msg, IntPtr.Zero, WindowMessage.WM_NULL, WindowMessage.WM_NULL))
                {
                    if (msg.message == WindowMessage.WM_SYSKEYDOWN && msg.wParam == (IntPtr)VirtualKey.VK_F4)
                    {
                        User32Library.SendMessage(User32Library.GetAncestor(Handle, GetAncestorFlags.GA_ROOT), msg.message, msg.wParam, msg.lParam);
                        continue;
                    }

                    User32Library.TranslateMessage(ref msg);
                    User32Library.DispatchMessage(ref msg);
                }
            }
        }
    }
}

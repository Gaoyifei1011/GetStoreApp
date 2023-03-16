using GetStoreAppHelper.Services;
using GetStoreAppHelper.WindowsAPI.PInvoke.Kernel32;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppHelper
{
    /// <summary>
    /// 应用窗口类
    /// </summary>
    public class MileWindow
    {
        public PointInt32 Position = new PointInt32();

        public PointInt32 Size = new PointInt32();

        public bool IsWindowCreated { get; private set; } = false;

        public string Title { get; set; } = string.Empty;

        public IntPtr Handle { get; private set; } = IntPtr.Zero;

        public UIElement Content { get; set; } = null;

        public MileWindow([Optional] string title, [Optional] UIElement content, [Optional] PointInt32 position, [Optional] PointInt32 size)
        {
            Title = title is not null ? title : string.Empty;
            Content = content is not null ? content : new ContentControl();
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

        /// <summary>
        /// 初始化应用窗口
        /// </summary>
        public void InitializeWindow()
        {
            Handle = User32Library.CreateWindowEx(
                WindowStylesEx.WS_EX_TRANSPARENT | WindowStylesEx.WS_EX_LAYERED | WindowStylesEx.WS_EX_TOOLWINDOW,
                "Mile.Xaml.ContentWindow",
                Title,
                WindowStyles.WS_POPUP,
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
                throw new ApplicationException(ResourceService.GetLocalized("HelperResources/WindowHandleInitializeFailed"));
            }
            else
            {
                IsWindowCreated = true;
            }
        }

        /// <summary>
        /// 激活窗口
        /// </summary>
        public void Activate()
        {
            if (IsWindowCreated)
            {
                User32Library.ShowWindow(Handle, WindowShowStyle.SW_SHOWDEFAULT);
                User32Library.UpdateWindow(Handle);

                while (User32Library.GetMessage(out MSG msg, IntPtr.Zero, WindowMessage.WM_NULL, WindowMessage.WM_NULL))
                {
                    if (msg.message == WindowMessage.WM_SYSKEYDOWN && msg.wParam == (IntPtr)VirtualKey.VK_F4)
                    {
                        User32Library.PostMessage(User32Library.GetAncestor(Handle, GetAncestorFlags.GA_ROOT), msg.message, msg.wParam, msg.lParam);
                        continue;
                    }

                    User32Library.TranslateMessage(ref msg);
                    User32Library.DispatchMessage(ref msg);
                }
            }
        }
    }
}

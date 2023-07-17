using GetStoreApp.Properties;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 将任务栏通知区域图标的消息转发到该消息窗口中
    /// </summary>
    public class TrayMessageWindow : IDisposable
    {
        // 从任务栏图标接收的消息的 ID。
        public const int CallbackMessageId = 0x400;

        // 如果任务栏已（重新）启动，则收到的消息的 ID。
        private readonly uint taskbarRestartMessageId;

        // 用于跟踪鼠标向上事件是否只是双击的后果，因此需要禁止显示。
        private bool isDoubleClick;

        // 在调用释放后立即设置为 true。
        private bool IsDisposed;

        // 消息窗口的句柄
        public IntPtr TrayMessagehWnd { get; private set; }

        // 如果用户在任务栏图标区域内单击或移动，则触发。
        public event Action<WindowMessage> MouseEventReceived;

        // 如果任务栏已创建或重新启动，则触发。需要重置任务栏图标。
        public event Action TaskbarCreated;

        private WNDCLASS wc;

        public TrayMessageWindow()
        {
            // 创建窗口类，用于将任务栏图标的消息转发到该消息窗口中
            wc.style = 0;
            wc.lpfnWndProc = TrayMessageWndProc;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = string.Empty;
            wc.lpszClassName = Resources.WinUIWindowClassName;

            IntPtr ptrWndClass = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WNDCLASS)));
            Marshal.StructureToPtr(wc, ptrWndClass, false);
            // 创建窗口类后需要注册窗口类
            User32Library.RegisterClass(ptrWndClass);
            Marshal.FreeHGlobal(ptrWndClass);

            taskbarRestartMessageId = User32Library.RegisterWindowMessage("TaskbarCreated");

            // 创建窗口
            TrayMessagehWnd = User32Library.CreateWindowEx(
                WindowStyleEx.WS_EX_LEFT,
                Resources.WinUIWindowClassName,
                ResourceService.GetLocalized("Window/TrayMessageWindowName"),
                WindowStyle.WS_TILED,
                0,
                0,
                1,
                1,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );

            if (TrayMessagehWnd == IntPtr.Zero)
            {
                throw new ApplicationException(ResourceService.GetLocalized("Resources/WindowHandleInitializeFailed"));
            }
        }

        /// <summary>
        /// 从任务栏窗口的回调过程
        /// </summary>
        private IntPtr TrayMessageWndProc(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam)
        {
            // 如果任务栏重新启动（例如，由于资源管理器关闭），请重新创建图标
            if (Convert.ToUInt32(msg) == taskbarRestartMessageId)
            {
                TaskbarCreated.Invoke();
            }
            else
            {
                // 检查是否为回调消息
                if (Convert.ToUInt32(msg) != CallbackMessageId)
                {
                    // 这不是回调消息，但请确保它不是我们需要处理的其他内容
                    switch (msg)
                    {
                        case WindowMessage.WM_DPICHANGED:
                            break;
                    }
                }
                // 处理任务栏图标转发的消息
                else
                {
                    switch ((WindowMessage)lParam.ToInt32())
                    {
                        // 处理单击左键消息
                        case WindowMessage.WM_LBUTTONUP:
                            if (!isDoubleClick)
                            {
                                MouseEventReceived?.Invoke(WindowMessage.WM_LBUTTONUP);
                            }
                            isDoubleClick = false;
                            break;
                        // 处理单击右键消息
                        case WindowMessage.WM_RBUTTONUP:
                            MouseEventReceived?.Invoke(WindowMessage.WM_RBUTTONUP);
                            break;
                        // 处理双击左键消息
                        case WindowMessage.WM_LBUTTONDBLCLK:
                            isDoubleClick = true;
                            MouseEventReceived?.Invoke(WindowMessage.WM_LBUTTONDBLCLK);
                            break;

                        default:
                            break;
                    }
                }
            }

            // 将消息传递到默认窗口过程
            return User32Library.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TrayMessageWindow()
        {
            Dispose(false);
        }

        /// <summary>
        /// 删除接收窗口消息的窗口挂钩并关闭基础帮助程序窗口。
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                User32Library.DestroyWindow(TrayMessagehWnd);
                wc.lpfnWndProc = null;
            }
            IsDisposed = true;
        }
    }
}

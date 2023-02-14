using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.ComponentModel;
using Windows.Foundation;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 通过基础帮助程序窗口的窗口消息从任务栏图标接收消息。
    /// </summary>
    public class WindowMessageSink : IDisposable
    {
        /// <summary>
        /// 从任务栏图标接收的消息的 ID。
        /// </summary>
        public const int CallbackMessageId = 0x400;

        /// <summary>
        /// 如果任务栏已（重新）启动，则收到的消息的 ID。
        /// </summary>
        private uint taskbarRestartMessageId;

        /// <summary>
        /// 用于跟踪鼠标向上事件是否只是双击的后果，因此需要禁止显示。
        /// </summary>
        private bool isDoubleClick;

        /// <summary>
        /// 在调用释放后立即设置为 true。
        /// </summary>
        private bool IsDisposed;

        /// <summary>
        /// 一个委托，用于处理接收窗口消息的隐藏本机窗口的消息。存储此引用可确保我们不会丢失对消息窗口的引用。
        /// </summary>
        private WindowProcedureHandler messageHandler;

        /// <summary>
        /// 窗口类 ID。
        /// </summary>
        internal string WindowId { get; private set; }

        /// <summary>
        /// 消息窗口的句柄。
        /// </summary>
        internal IntPtr MessageWindowHandle { get; private set; }

        /// <summary>
        ///基础图标的版本。定义如何解释传入消息。
        /// </summary>
        public byte Version { get; } = 0x4;

        /// <summary>
        /// 如果用户在任务栏图标区域内单击或移动，则触发。
        /// </summary>
        public event Action<MouseEvent> MouseEventReceived;

        /// <summary>
        /// 如果任务栏已创建或重新启动，则触发。需要重置任务栏图标。
        /// </summary>
        public event Action TaskbarCreated;

        /// <summary>
        /// 创建一个新的消息接收器，用于接收来自给定任务栏图标的消息。
        /// </summary>
        public WindowMessageSink()
        {
            CreateMessageWindow();
        }

        /// <summary>
        /// 创建用于从任务栏图标接收消息的帮助程序消息窗口。
        /// </summary>
        private void CreateMessageWindow()
        {
            // 为窗口生成唯一 ID
            WindowId = "GetStoreApp" + GuidHelper.CreateNewGuid();

            // 注册窗口消息处理程序
            messageHandler = OnWindowMessageReceived;

            // 创建一个简单的窗口类，该类通过 messageHandler 委托引用
            WindowClass wc;

            wc.style = 0;
            wc.lpfnWndProc = messageHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = string.Empty;
            wc.lpszClassName = WindowId;

            // 注册窗口类
            User32Library.RegisterClass(ref wc);

            // 获取用于指示任务栏已重新启动的消息。这用于在任务栏重新启动时重新添加图标
            taskbarRestartMessageId = User32Library.RegisterWindowMessage("TaskbarCreated");

            // 创建消息窗口
            MessageWindowHandle = User32Library.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero);

            if (MessageWindowHandle == IntPtr.Zero)
            {
                throw new Win32Exception(ResourceService.GetLocalized("Resources/WindowHandleInitializeFailed"));
            }
        }

        /// <summary>
        /// 从任务栏区域接收消息的回调方法。
        /// </summary>
        private IntPtr OnWindowMessageReceived(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId == taskbarRestartMessageId)
            {
                // 如果任务栏重新启动（例如，由于 Win 资源管理器关闭），请重新创建图标
                var listener = TaskbarCreated;
                listener?.Invoke();
            }

            // 转发消息
            ProcessWindowMessage(messageId, wParam, lParam);

            // 将消息传递到默认窗口过程
            return User32Library.DefWindowProc(hWnd, messageId, wParam, lParam);
        }

        /// <summary>
        /// 处理传入的系统消息。
        /// </summary>
        /// <param name="msg">回调 ID。</param>
        /// <param name="wParam">
        /// 如果版本为 <see cref="NotifyIconVersion.Vista"/> 或更高版本，则此参数可用于解析鼠标坐标。目前未使用。
        /// </param>
        /// <param name="lParam">提供有关事件的信息。</param>
        private void ProcessWindowMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            // 检查是否为回调消息
            if (msg != CallbackMessageId)
            {
                // 这不是回调消息，但请确保它不是我们需要处理的其他内容
                switch ((WindowMessage)msg)
                {
                    case WindowMessage.WM_DPICHANGED:
                        break;
                }
                return;
            }

            WindowMessage message = (WindowMessage)lParam.ToInt32();
            switch (message)
            {
                case WindowMessage.WM_LBUTTONUP:
                    if (!isDoubleClick)
                    {
                        MouseEventReceived?.Invoke(MouseEvent.IconLeftMouseUp);
                    }
                    isDoubleClick = false;
                    break;

                case WindowMessage.WM_RBUTTONUP:
                    MouseEventReceived?.Invoke(MouseEvent.IconRightMouseUp);
                    break;

                case WindowMessage.WM_LBUTTONDBLCLK:
                    isDoubleClick = true;
                    MouseEventReceived?.Invoke(MouseEvent.IconDoubleClick);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 释放对象。
        /// </summary>
        /// <remarks>此方法在设计上不是虚拟的。派生类应重写 <see cref="Dispose(bool)"/>.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);

            // 此对象将由 Dispose 方法清理。因此，您应该调用 GC.SuppressFinalize() 将此对象从终结队列中删除，并防止此对象的终结代码再次执行。
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 仅当 <see cref="Dispose()"/> 方法未被调用时，此析构函数才会运行。这使此基类有机会完成。
        /// <para>
        /// 注意： 不要在从此类派生的类型中提供析构函数。
        /// </para>
        /// </summary>
        ~WindowMessageSink()
        {
            Dispose(false);
        }

        /// <summary>
        /// 删除接收窗口消息的窗口挂钩并关闭基础帮助程序窗口。
        /// </summary>
        private void Dispose(bool disposing)
        {
            // 如果组件已释放，则不执行任何操作
            if (IsDisposed)
            {
                return;
            };
            IsDisposed = disposing;

            // 始终销毁非托管句柄（即使从 GC 调用）
            User32Library.DestroyWindow(MessageWindowHandle);
            messageHandler = null;
        }
    }
}

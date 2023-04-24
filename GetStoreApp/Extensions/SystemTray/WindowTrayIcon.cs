using GetStoreApp.Contracts.Command;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;

namespace GetStoreApp.Extensions.SystemTray
{
    /// <summary>
    /// 任务栏托盘图标
    /// </summary>
    public class WindowTrayIcon : IDisposable
    {
        private readonly object lockObject = new object();
        public static readonly object SyncRoot = new object();

        private bool isDisposed;
        private NOTIFYICONDATA data;
        private readonly TrayMessageWindow trayMessageWindow;

        public IntPtr Handle { get; private set; } = IntPtr.Zero;

        public Action Click { get; set; }

        public Action RightClick { get; set; }

        public Action DoubleClick { get; set; }

        public IRelayCommand ClickCommand { get; set; }

        public IRelayCommand RightClickCommand { get; set; }

        public IRelayCommand DoubleClickCommand { get; set; }

        public bool IsTaskbarIconCreated { get; private set; }

        public WindowTrayIcon(string title)
        {
            trayMessageWindow = new TrayMessageWindow();
            data = NOTIFYICONDATA.Initialize(trayMessageWindow.TrayMessagehWnd, title);
            Handle = trayMessageWindow.TrayMessagehWnd;

            // 创建任务栏图标
            CreateTaskbarIcon();

            // 注册事件侦听器
            trayMessageWindow.MouseEventReceived += OnMouseEventReceived;
            trayMessageWindow.TaskbarCreated += OnTaskbarCreated;
        }

        /// <summary>
        /// 任务栏创建时，移除旧的任务栏图标并创建新的任务栏图标
        /// </summary>
        private void OnTaskbarCreated()
        {
            RemoveTaskbarIcon();
            CreateTaskbarIcon();
        }

        /// <summary>
        /// 鼠标事件接收后的操作
        /// </summary>
        private void OnMouseEventReceived(MouseEvent mouseEvent)
        {
            if (mouseEvent is MouseEvent.IconLeftMouseUp)
            {
                Click?.Invoke();
                ClickCommand?.Execute(null);
            }
            else if (mouseEvent is MouseEvent.IconRightMouseUp)
            {
                RightClick?.Invoke();
                RightClickCommand.Execute(null);
            }
            else if (mouseEvent is MouseEvent.IconDoubleClick)
            {
                DoubleClick?.Invoke();
                DoubleClickCommand.Execute(null);
            }
        }

        /// <summary>
        /// 创建任务栏图标
        /// </summary>
        private void CreateTaskbarIcon()
        {
            lock (lockObject)
            {
                if (IsTaskbarIconCreated)
                {
                    return;
                }

                const NotifyIconFlags flags = NotifyIconFlags.NIF_MESSAGE | NotifyIconFlags.NIF_ICON | NotifyIconFlags.NIF_TIP;

                bool status = WriteIconData(ref data, NotifyIconMessage.NIM_ADD, flags);
                if (!status)
                {
                    // 无法创建图标 - 我们可以假设这是因为资源管理器尚未运行。
                    // 稍后再试一次，而不是引发异常。通常，如果稍后加载 windows shell，则会从 OnTaskbarCreated 重新调用此方法
                    return;
                }

                IsTaskbarIconCreated = true;
            }
        }

        /// <summary>
        /// 移除任务栏图标
        /// </summary>
        private void RemoveTaskbarIcon()
        {
            lock (lockObject)
            {
                if (!IsTaskbarIconCreated)
                {
                    return;
                }

                WriteIconData(ref data, NotifyIconMessage.NIM_DELETE, NotifyIconFlags.NIF_MESSAGE);
                IsTaskbarIconCreated = false;
            }
        }

        /// <summary>
        /// 使用给定 <see cref="NotifyIconData"/> 实例提供的数据更新任务栏图标。
        /// </summary>
        /// <param name="data">通知图标的配置设置。</param>
        /// <param name="command">对图标进行操作（例如删除图标）。</param>
        /// <param name="flags">定义设置 <paramref name="data"/> 结构的哪些成员。</param>
        /// <returns>如果数据已成功写入，则为 True。</returns>
        /// <remarks>有关详细信息，请参阅 MSDN 上的Shell_NotifyIcon文档。</remarks>
        public static unsafe bool WriteIconData(ref NOTIFYICONDATA data, NotifyIconMessage message, NotifyIconFlags flags)
        {
            data.uFlags = flags;
            lock (SyncRoot)
            {
                fixed (NOTIFYICONDATA* pdata = &data)
                {
                    return Shell32Library.Shell_NotifyIcon(message, pdata);
                }
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

        ~WindowTrayIcon()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            // 如果组件已释放，则不执行任何操作
            if (isDisposed) return;

            if (disposing)
            {
                // 始终销毁非托管句柄（即使从 GC 调用）
                trayMessageWindow.Dispose();

                // 移除任务栏图标
                RemoveTaskbarIcon();

                // 注销事件监听器
                trayMessageWindow.MouseEventReceived -= OnMouseEventReceived;
                trayMessageWindow.TaskbarCreated -= OnTaskbarCreated;
            }
            isDisposed = true;
        }
    }
}

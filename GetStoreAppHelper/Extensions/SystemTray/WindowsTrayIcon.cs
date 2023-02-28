using GetStoreAppHelper.WindowsAPI.PInvoke.Shell32;
using System;

namespace GetStoreAppHelper.Extensions.SystemTray
{
    /// <summary>
    /// 任务栏托盘图标
    /// </summary>
    public class WindowsTrayIcon : IDisposable
    {
        private readonly object lockObject = new object();
        public static readonly object SyncRoot = new object();

        private NOTIFYICONDATA iconData;

        private bool IsDisposed;

        /// <summary>
        /// 从任务栏图标接收消息
        /// </summary>
        private readonly WindowMessageSink messageSink;

        public Action LeftClick { get; set; }

        public Action RightClick { get; set; }

        public Action DoubleClick { get; set; }

        public bool IsTaskbarIconCreated { get; private set; }

        public WindowsTrayIcon(string iconFile, string title)
        {
            messageSink = new WindowMessageSink();

            // 初始化图标数据结构
            iconData = NOTIFYICONDATA.CreateDefault(messageSink.MessageWindowHandle, iconFile, title);

            // 创建任务栏图标
            CreateTaskbarIcon();

            // 注册事件侦听器
            messageSink.MouseEventReceived += OnMouseEventReceived;
            messageSink.TaskbarCreated += OnTaskbarCreated;
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
                LeftClick?.Invoke();
            }
            else if (mouseEvent is MouseEvent.IconRightMouseUp)
            {
                RightClick?.Invoke();
            }
            else if (mouseEvent is MouseEvent.IconDoubleClick)
            {
                DoubleClick?.Invoke();
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

                const IconDataMembers members = IconDataMembers.Message | IconDataMembers.Icon | IconDataMembers.Tip;

                var status = WriteIconData(ref iconData, NotifyCommand.NIM_ADD, members);
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

                WriteIconData(ref iconData, NotifyCommand.NIM_DELETE, IconDataMembers.Message);
                IsTaskbarIconCreated = false;
            }
        }

        /// <summary>
        /// 使用给定 <see cref="NOTIFYICONDATA"/> 实例提供的数据更新任务栏图标。
        /// </summary>
        /// <param name="data">通知图标的配置设置。</param>
        /// <param name="command">对图标进行操作（例如删除图标）。</param>
        /// <param name="flags">定义设置 <paramref name="data"/> 结构的哪些成员。</param>
        /// <returns>如果数据已成功写入，则为 True。</returns>
        /// <remarks>有关详细信息，请参阅 MSDN 上的Shell_NotifyIcon文档。</remarks>
        public static bool WriteIconData(ref NOTIFYICONDATA data, NotifyCommand command, IconDataMembers flags)
        {
            data.ValidMembers = flags;
            lock (SyncRoot)
            {
                return Shell32Library.Shell_NotifyIcon(command, ref data);
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
            GC.SuppressFinalize(this);
        }

        ~WindowsTrayIcon()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            // 如果组件已释放，则不执行任何操作
            if (IsDisposed)
            {
                return;
            };

            if (disposing)
            {
                // 始终销毁非托管句柄（即使从 GC 调用）
                messageSink.Dispose();

                // 移除任务栏图标
                RemoveTaskbarIcon();

                // 注销事件监听器
                messageSink.MouseEventReceived -= OnMouseEventReceived;
                messageSink.TaskbarCreated -= OnTaskbarCreated;
            }

            IsDisposed = true;
        }
    }
}

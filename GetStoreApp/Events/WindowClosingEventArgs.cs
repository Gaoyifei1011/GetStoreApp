using GetStoreApp.Extensions.WindowExtension;
using System;

namespace GetStoreApp.Events
{
    /// <summary>
    /// 自定义Window 应用 Sdk（Winui3）窗口关闭事件
    /// </summary>
    public class WindowClosingEventArgs : EventArgs
    {
        public DesktopWindow Window { get; private set; }

        public WindowClosingEventArgs(DesktopWindow window)
        {
            Window = window;
        }

        public void TryCloseWindow()
        {
            Window.IsClosing = true;
            Window.Close();
        }
    }
}

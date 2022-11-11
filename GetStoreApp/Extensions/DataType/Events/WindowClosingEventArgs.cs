using GetStoreApp.Views.Window;
using System;

namespace GetStoreApp.Extensions.DataType.Events
{
    public class WindowClosingEventArgs : EventArgs
    {
        public WASDKWindow Window { get; private set; }
        public WindowClosingEventArgs(WASDKWindow window)
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

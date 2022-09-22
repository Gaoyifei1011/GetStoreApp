using System.Runtime.InteropServices;

namespace GetStoreAppConsole
{
    /// <summary>
    /// 控制台窗口设置
    /// </summary>
    public class ConsoleWindow
    {
        [DllImport("User32.dll")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("User32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        public const int SW_HIDE = 0;
        public const int SW_NORMAL = 1;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;

        /// <summary>
        /// 隐藏控制台程序窗口
        /// </summary>
        public static void HideConsoleWindow(string consoleTitle)
        {
            ShowWindow(FindWindow(null, consoleTitle), SW_HIDE);
        }
    }
}

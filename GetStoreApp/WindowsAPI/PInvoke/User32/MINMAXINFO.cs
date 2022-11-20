using GetStoreApp.WindowsAPI.PInvoke.WindowsCore;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含有关窗口的最大大小和位置及其最小和最大跟踪大小的信息。
    /// </summary>
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }
}

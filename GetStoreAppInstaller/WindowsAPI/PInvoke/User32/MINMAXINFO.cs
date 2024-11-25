using Windows.Graphics;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含有关窗口的最大大小和位置及其最小和最大跟踪大小的信息。
    /// </summary>
    public struct MINMAXINFO
    {
        /// <summary>
        /// 保留;请勿使用。
        /// </summary>
        public PointInt32 ptReserved;

        /// <summary>
        /// 最大宽度 (x 成员) ，最大高度 (y 成员) 窗口。 对于顶级窗口，此值基于主监视器的宽度。
        /// </summary>
        public PointInt32 ptMaxSize;

        /// <summary>
        /// 最大化窗口左侧 (x 成员) 的位置，最大化窗口顶部的位置 (y 成员) 。 对于顶级窗口，此值基于主监视器的位置。
        /// </summary>
        public PointInt32 ptMaxPosition;

        /// <summary>
        /// 窗口的最小跟踪宽度 (x 成员) 和最小跟踪高度 (y 成员) 。 可以从系统指标 SM_CXMINTRACK 以编程方式获取此值，
        /// </summary>
        public PointInt32 ptMinTrackSize;

        /// <summary>
        /// 最大跟踪宽度 (x 成员) 和最大跟踪高度 (y 成员) 窗口。 此值基于虚拟屏幕的大小，可以通过编程方式从系统指标 SM_CXMAXTRACK 获取，
        /// </summary>
        public PointInt32 ptMaxTrackSize;
    }
}

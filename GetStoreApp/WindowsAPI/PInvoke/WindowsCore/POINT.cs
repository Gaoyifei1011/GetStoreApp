using System.Drawing;

namespace GetStoreApp.WindowsAPI.PInvoke.WindowsCore
{
    /// <summary>
    /// POINT 结构定义点的 x 坐标和 y 坐标。
    /// </summary>
    public struct POINT
    {
        /// <summary>
        ///  指定点的 x 坐标。
        /// </summary>
        public int x;

        /// <summary>
        /// 指定点的 y 坐标。
        /// </summary>
        public int y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.x, point.y);
        }

        public static implicit operator POINT(Point point)
        {
            return new POINT { x = point.X, y = point.Y };
        }
    }
}

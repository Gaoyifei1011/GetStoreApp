using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// <see cref="MONITORINFO"> 结构包含有关显示监视器的信息。
    /// <see cref="User32Library.GetMonitorInfo"> 函数将信息存储在 <see cref="MONITORINFO"> 结构或 MONITORINFOEX 结构中。
    /// MONITORINFO 结构是 MONITORINFOEX 结构的子集。 MONITORINFOEX 结构添加字符串成员以包含显示监视器的名称。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        /// <summary>
        /// 在调用 <see cref="User32Library.GetMonitorInfo"> 函数之前将此成员设置为sizeof ( MONITORINFO )该成员。 这样，函数就可以确定要传递给它的结构类型。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 一个 <see cref="RECT"> 结构，指定以虚拟屏幕坐标表示的显示监视器矩形。 请注意，如果监视器不是主显示监视器，则一些矩形的坐标可能是负值。
        /// </summary>
        public RECT rcMonitor;

        /// <summary>
        /// 一个 <see cref="RECT"> 结构，指定显示监视器的工作区矩形，以虚拟屏幕坐标表示。 请注意，如果监视器不是主显示监视器，则一些矩形的坐标可能是负值。
        /// </summary>
        public RECT rcWork;

        /// <summary>
        /// 一组表示显示监视器属性的标志。MONITORINFOF_PRIMARY 表示这是主显示器。
        /// </summary>
        public uint dwFlags;
    }
}

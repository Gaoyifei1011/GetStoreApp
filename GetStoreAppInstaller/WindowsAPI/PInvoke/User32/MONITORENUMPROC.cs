using System.Runtime.InteropServices;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 与 EnumDisplayMonitors 函数一起使用的应用程序定义的回调函数。 MONITORENUMPROC 类型定义指向此回调函数的指针。 MonitorEnumProc 是应用程序定义的函数名称的占位符。
    /// </summary>
    /// <param name="hMonitor">显示监视器的句柄。 此值始终为非 NULL。 此参数通常命名为 hMonitor。</param>
    /// <param name="hdcMonitor">
    /// 设备上下文的句柄。 此参数通常命名为 hdcMonitor。
    /// 设备上下文具有适合 hMonitor 标识的显示监视器的颜色属性。 设备上下文的剪辑区域设置为由 EnumDisplayMonitors 的 hdc 参数标识的设备上下文可见区域的交集、EnumDisplayMonitors 的 lprcClip 参数指向的矩形以及显示监视器矩形。
    /// 如果 EnumDisplayMonitors 的 hdc 参数为 NULL，则此值为 NULL。
    /// </param>
    /// <param name="lprcMonitor">
    /// 指向 RECT 结构的指针。 此参数通常命名为 lprcMonitor。
    /// 如果 hdcMonitor 为非 NULL，则此矩形是 hdcMonitor 和显示监视器矩形标识的设备上下文剪辑区域的交集。 矩形坐标是设备上下文坐标。
    /// 如果 hdcMonitor 为 NULL，则此矩形是显示监视器矩形。 矩形坐标是虚拟屏幕坐标。
    /// </param>
    /// <param name="dwData">EnumDisplayMonitors 直接传递给枚举函数的应用程序定义数据。 此参数通常命名为 dwData。</param>
    /// <returns>若要继续枚举，请返回 TRUE。若要停止枚举，请返回 FALSE。</returns>
	[UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate bool MONITORENUMPROC(nint hmonitor, nint hdc, nint rect, nint data);
}

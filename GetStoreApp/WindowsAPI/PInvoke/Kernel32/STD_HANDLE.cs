using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// GetStdHandle 和 SetStdHandle 方法的标准句柄。
    /// </summary>
    [Flags]
    public enum STD_HANDLE
    {
        /// <summary>
        /// 标准输入设备。 最初，这是输入缓冲区 CONIN$ 的控制台。
        /// </summary>
        STD_INPUT_HANDLE = -10,

        /// <summary>
        /// 标准输出设备。 最初，这是活动控制台屏幕缓冲区 CONOUT$。
        /// </summary>
        STD_OUTPUT_HANDLE = -11,

        /// <summary>
        /// 标准错误设备。 最初，这是活动控制台屏幕缓冲区 CONOUT$。
        /// </summary>
        STD_ERROR_HANDLE = -12
    }
}

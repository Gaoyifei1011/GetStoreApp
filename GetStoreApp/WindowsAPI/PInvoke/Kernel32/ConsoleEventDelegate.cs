using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    // 定义 ConsoleEventDelegate 委托类型
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate bool ConsoleEventDelegate(int dwCtrlType);
}

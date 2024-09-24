using System.Runtime.InteropServices;

namespace GetStoreAppShellExtension.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static partial class Kernel32Library
    {
        public const string Kernel32 = "kernel32.dll";

        /// <summary>
        /// 获取调用进程的包路径。
        /// </summary>
        /// <param name="pathLength">输入时， 路径缓冲区的大小（以字符为单位）。 输出时，返回的包路径的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="path">包路径。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [LibraryImport(Kernel32, EntryPoint = "GetCurrentPackagePath", SetLastError = false), PreserveSig]
        public static unsafe partial int GetCurrentPackagePath(ref uint pathLength, char* path);
    }
}

using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static partial class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        /// <summary>
        /// 检索指定模块的模块句柄。 模块必须已由调用进程加载。
        /// </summary>
        /// <param name="modName">
        /// 加载的模块的名称 (.dll 或 .exe 文件) 。 如果省略文件扩展名，则会追加默认库扩展名 .dll。 文件名字符串可以包含尾随点字符 (.) ，以指示模块名称没有扩展名。 字符串不必指定路径。 指定路径时，请务必使用反斜杠 (\) ，而不是使用 /) (正斜杠。 名称 (大小写独立比较，) 当前映射到调用进程的地址空间的模块的名称。
        /// 如果此参数为 NULL， 则 GetModuleHandle 返回用于创建调用进程(.exe 文件) 的文件的句柄。
        /// GetModuleHandle 函数不会检索使用 LOAD_LIBRARY_AS_DATAFILE 标志加载的模块的句柄。 有关详细信息，请参阅 LoadLibraryEx。
        /// </param>
        /// <returns>如果函数成功，则返回值是指定模块的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", SetLastError = false), PreserveSig]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string modName);

        /// <summary>
        /// 检索性能计数器的频率。 性能计数器的频率在系统启动时固定，并且在所有处理器中保持一致。 因此，只需在应用程序初始化时查询频率，并且可以缓存结果。
        /// </summary>
        /// <param name="lpFrequency">
        /// 指向接收当前性能计数器频率（以每秒计数为单位）的变量的指针。 如果安装的硬件不支持高分辨率性能计数器，则此参数可以为零， (在运行 Windows XP 或更高版本) 的系统上不会发生这种情况。</param>
        /// <returns>如果安装的硬件支持高分辨率性能计数器，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode, EntryPoint = "QueryPerformanceFrequency", SetLastError = false), PreserveSig]
        public static extern bool QueryPerformanceFrequency(ref long lpFrequency);
    }
}

using System;
using System.Runtime.InteropServices;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static class Kernel32Library
    {
        private const string Kernel32 = "Kernel32.dll";

        /// <summary>
        /// 检索指定模块的模块句柄。 该模块必须由调用进程加载。
        /// </summary>
        /// <param name="lpModuleName">加载的模块的名称 (.dll或.exe文件) 。 如果省略文件扩展名，则会追加默认库扩展名.dll。 文件名字符串可以包含尾随点字符 (.) ，以指示模块名称没有扩展名。 字符串不必指定路径。 指定路径时，请确保使用反斜杠 (\) ，而不是 (/) 正斜杠。 将名称单独 (大小写) 与当前映射到调用进程的地址空间的模块的名称进行比较。
        /// 如果此参数为 NULL， <see cref="GetModuleHandle"> 将返回用于创建调用进程(.exe 文件) 的文件的句柄。<see cref="GetModuleHandle"> 函数不会检索使用 LOAD_LIBRARY_AS_DATAFILE 标志加载的模块的句柄。</param>
        /// <returns>如果函数成功，则返回值是指定模块的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetModuleHandle([In][Optional] string lpModuleName);

        /// <summary>
        /// 检索创建调用进程时指定的 <see cref="STARTUPINFO"> 结构的内容。
        /// </summary>
        /// <param name="lpStartupInfo">指向接收启动信息的 <see cref="STARTUPINFO"> 结构的指针。</param>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetStartupInfoW", ExactSpelling = true, SetLastError = false)]
        public static extern void GetStartupInfo([Out] out STARTUPINFO lpStartupInfo);
    }
}

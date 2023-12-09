using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TaskbarPinner.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// kernel32.dll 函数库
    /// </summary>
    public static class Kernel32Library
    {
        private const string Kernel32 = "kernel32.dll";

        public const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

        /// <summary>
        /// 获取调用进程的包全名。
        /// </summary>
        /// <param name="packageFullNameLength">输入时， packageFullName 缓冲区的大小（以字符为单位）。 输出时，返回包全名的大小（以字符为单位），包括 null 终止符。</param>
        /// <param name="packageFullName">包全名。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetCurrentPackageFullName", SetLastError = true)]
        public static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        /// <summary>
        /// 将指定的打包模块及其依赖项加载到调用进程的地址空间中
        /// </summary>
        /// <param name="libraryName">
        /// 要加载的打包模块的文件名。 该模块可以是 (.dll文件) 的库模块，也可以是) (.exe文件的可执行模块。
        /// 如果此参数指定模块名称而不指定路径，并且省略文件扩展名，则函数会将默认库扩展.dll追加到模块名称。 若要防止函数将.dll追加到模块名称，请在模块名称字符串中包含尾随点字符 (.) 。
        /// 如果此参数指定路径，则函数将搜索该路径来查找模块。 路径不能是绝对路径，也不能是路径中包含“..”的相对路径。 指定路径时，请务必使用反斜杠 (\) ，而不是使用 /) (正斜杠。 有关路径的详细信息，请参阅 命名文件、路径和命名空间。
        /// 如果指定的模块已在进程中加载，则函数将返回已加载模块的句柄。 模块必须最初是从进程的包依赖项关系图加载的。
        /// 如果加载指定的模块导致系统加载其他关联的模块，则该函数首先搜索已加载的模块，然后搜索进程的包依赖项关系图。
        /// </param>
        /// <param name="reserved">此参数为保留参数。 它必须为 0。</param>
        /// <returns>如果函数成功，则返回值是已加载模块的句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "LoadPackagedLibrary", SetLastError = true)]
        public static extern IntPtr LoadPackagedLibrary([MarshalAs(UnmanagedType.LPWStr)] string libraryName, int reserved = 0);
    }
}

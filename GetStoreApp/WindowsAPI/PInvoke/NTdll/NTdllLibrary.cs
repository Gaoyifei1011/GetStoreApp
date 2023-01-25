using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.NTdll
{
    public static class NTdllLibrary
    {
        private const string NtDll = "NTdll.dll";

        /// <summary>
        /// 检索有关指定进程的信息。
        /// </summary>
        /// <param name="processHandle">要检索其信息的进程的句柄。</param>
        /// <param name="processInformationClass">要检索的进程信息的类型。</param>
        /// <param name="processInformation">
        /// 当 ProcessInformationClass 参数为 ProcessBasicInformation 时，ProcessInformation 参数指向的缓冲区应足够大
        /// </param>
        /// <param name="processInformationLength">ProcessInformation 参数指向的缓冲区的大小（以字节为单位）。</param>
        /// <param name="returnLength">
        /// 指向函数返回所请求信息大小的变量的指针。 如果函数成功，则这是 由 ProcessInformation 参数指向的缓冲区中写入的信息的大小，
        /// ( 如果缓冲区太小，则这是成功接收信息所需的最小缓冲区大小) 。</param>
        /// <returns>该函数返回 NTSTATUS 成功或错误代码。</returns>
        [DllImport(NtDll, CharSet = CharSet.Ansi, EntryPoint = "NtQueryInformationProcess", SetLastError = false)]
        public static extern int NtQueryInformationProcess(int processHandle, PROCESSINFOCLASS processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, ref int returnLength);
    }
}

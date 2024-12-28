using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Rstrtmgr
{
    /// <summary>
    /// 按进程的 PID 和进程开始的时间唯一标识进程。 可以将RM_UNIQUE_PROCESS结构的数组传递给 RmRegisterResources 函数。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct RM_UNIQUE_PROCESS
    {
        /// <summary>
        /// PID(产品标识符) 。
        /// </summary>
        public int dwProcessId;

        /// <summary>
        /// 进程的创建时间。 时间作为 FILETIME 结构提供，由 GetProcessTimes 函数的 lpCreationTime 参数返回。
        /// </summary>
        public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }
}

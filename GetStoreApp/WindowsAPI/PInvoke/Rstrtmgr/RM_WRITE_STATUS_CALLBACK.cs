using System.Runtime.InteropServices;

namespace WindowsTools.WindowsAPI.PInvoke.Rstrtmgr
{
    /// <summary>
    /// RM_WRITE_STATUS_CALLBACK函数可由控制重启管理器的用户界面实现。 启动重启管理器会话的安装程序可以将指向此函数的指针传递给重启管理器函数，以接收一定百分比的完整性。 完整性百分比严格增加，并描述正在执行的当前操作和受影响的应用程序的名称。
    /// </summary>
    /// <param name="nPercentComplete">一个介于 0 和 100 之间的整数值，指示已关闭或重启的应用程序总数的百分比。</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void RM_WRITE_STATUS_CALLBACK(uint nPercentComplete);
}

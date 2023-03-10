using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using System;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 一个位域，用于确定进程创建窗口时是否使用某些 <see cref="STARTUPINFO"/> 成员。
    /// </summary>
    [Flags]
    public enum STARTF
    {
        /// <summary>
        /// 无标志
        /// </summary>
        None = 0x0,

        /// <summary>
        /// 指示在调用 <see cref="CreateProcessAsUser(IntPtr, string, string, SECURITY_ATTRIBUTES*, SECURITY_ATTRIBUTES*, bool, CreateProcessFlags, void*, string, ref STARTUPINFO, out PROCESS_INFORMATION)"/> 后，游标处于反馈模式两秒。 (“鼠标”控制面板实用工具中的“指针”选项卡) 显示“在后台工作”光标。
        /// 如果在这两秒内，进程进行第一次 GUI 调用，则系统会为进程再提供 5 秒。 如果在这五秒内进程显示窗口，则系统会为进程再提供 5 秒的时间以完成该窗口的绘制。
        /// 无论进程是否正在绘制，系统在首次调用 <see cref="User32Library.GetMessage"> 后关闭反馈游标。
        /// </summary>
        STARTF_FORCEONFEEDBACK = 0x00000040,

        /// <summary>
        /// 指示在进程启动时，反馈游标被强制关闭。 将显示“普通选择”游标。
        /// </summary>
        STARTF_FORCEOFFFEEDBACK = 0x00000080,

        /// <summary>
        /// 指示进程创建的任何窗口都不能固定在任务栏上。
        /// 此标志必须与 <see cref="STARTF_TITLEISAPPID"/> 结合使用。
        /// </summary>
        STARTF_PREVENTPINNING = 0x00002000,

        /// <summary>
        /// 指示进程应在全屏模式下运行，而不是在窗口模式下运行。
        /// 此标志仅适用于在 x86 计算机上运行的控制台应用程序。
        /// </summary>
        STARTF_RUNFULLSCREEN = 0x00000020,

        /// <summary>
        /// lpTitle 成员包含 AppUserModelID。 此标识符控制任务栏和 “开始” 菜单显示应用程序的方式，并使它能够与正确的快捷方式和跳转列表相关联。 通常，应用程序将使用 SetCurrentProcessExplicitAppUserModelID 和 GetCurrentProcessExplicitAppUserModelID 函数，而不是设置此标志。 有关详细信息，请参阅 应用程序用户模型 ID。
        /// 如果使用 <see cref="STARTF_PREVENTPINNING"/>，则无法将应用程序窗口固定到任务栏上。 应用程序使用任何 AppUserModelID 相关的窗口属性仅覆盖该窗口的此设置。
        /// 此标志不能与 <see cref="STARTF_TITLEISLINKNAME"/> 一起使用。
        /// </summary>
        STARTF_TITLEISAPPID = 0x00001000,

        /// <summary>
        /// lpTitle 成员包含用户调用启动此过程的快捷文件 (.lnk) 的路径。 调用指向已启动的应用程序的 .lnk 文件时，通常由 shell 设置此值。 大多数应用程序不需要设置此值。
        /// 此标志不能与 <see cref="STARTF_TITLEISAPPID"/> 一起使用。
        /// </summary>
        STARTF_TITLEISLINKNAME = 0x00000800,

        /// <summary>
        /// 命令行来自不受信任的源。 有关详细信息，请参阅“备注”。
        /// </summary>
        STARTF_UNTRUSTEDSOURCE = 0x00008000,

        /// <summary>
        /// <see cref="STARTUPINFO.dwXCountChars"/> 和 <see cref="STARTUPINFO.dwYCountChars"/> 员包含其他信息。
        /// </summary>
        STARTF_USECOUNTCHARS = 0x00000008,

        /// <summary>
        /// <see cref="STARTUPINFO.dwFillAttribute"/> 成员包含其他信息。
        /// </summary>
        STARTF_USEFILLATTRIBUTE = 0x00000010,

        /// <summary>
        /// <see cref="STARTUPINFO.hStdInput"/> 成员包含其他信息。
        /// 此标志不能与 <see cref="STARTF_USESTDHANDLES"/> 一起使用。
        /// </summary>
        STARTF_USEHOTKEY = 0x00000200,

        /// <summary>
        /// <see cref="STARTUPINFO.dwX"/> 和 <see cref="STARTUPINFO.dwY"/> 成员包含其他信息。
        /// </summary>
        STARTF_USEPOSITION = 0x00000004,

        /// <summary>
        /// <see cref="STARTUPINFO.wShowWindow"/> 成员包含其他信息。
        /// </summary>
        STARTF_USESHOWWINDOW = 0x00000001,

        /// <summary>
        /// <see cref="STARTUPINFO.dwXSize"/> 和 <see cref="STARTUPINFO.dwYSize"/> 成员包含其他信息。
        /// </summary>
        STARTF_USESIZE = 0x00000002,

        /// <summary>
        /// <see cref="STARTUPINFO.hStdInput"/>, <see cref="STARTUPINFO.hStdOutput"/> 和 <see cref="STARTUPINFO.hStdError"/> 成员包含其他信息。
        /// 如果在调用某个进程创建函数时指定此标志，则句柄必须可继承，并且该函数的 bInheritHandles 参数必须设置为 TRUE。 有关详细信息，请参阅 句柄继承。
        /// 如果在调用 <see cref="GetStartupInfo(STARTUPINFO*)"/> 函数时指定了此标志，则这些成员是进程创建期间指定的句柄值或INVALID_HANDLE_VALUE。
        /// 不再需要句柄时，必须使用 CloseHandle 关闭句柄。
        /// 此标志不能与 <see cref="STARTF_USEHOTKEY"/> 一起使用。
        /// </summary>
        STARTF_USESTDHANDLES = 0x00000100,
    }
}

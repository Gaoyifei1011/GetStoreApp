using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 包含名字对象绑定操作期间使用的参数。可以使用 BIND_OPTS2 或 BIND_OPTS3 结构代替 BIND_OPTS 结构。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BIND_OPTS
    {
        /// <summary>
        /// 此结构的大小（以字节为单位）。
        /// </summary>
        public int cbStruct;

        /// <summary>
        /// 控制名字对象绑定操作的各个方面的标志。 此值是 BIND_FLAGS 枚举中位标志的任意组合。 CreateBindCtx 函数将此成员初始化为零。
        /// </summary>
        public int grfFlags;

        /// <summary>
        /// 当打开包含由该名字对象标识的对象的文件时应使用的标志。 可能的值为 STGM 常量。 绑定操作在加载文件时，在调用 <see cref="IPersistFile.Load"> 时使用这些标志。 如果对象已运行，绑定操作会忽略这些标志。 CreateBindCtx 函数将此字段初始化为 STGM_READWRITE。
        /// </summary>
        public int grfMode;

        /// <summary>
        /// 调用方希望绑定操作完成的时钟时间（以毫秒为单位）。 此成员允许调用方在速度至关重要时限制操作的执行时间。 值为零表示没有截止时间。
        /// 调用 <see cref="IMoniker.GetTimeOfLastChange"> 方法时，调用方最常使用此功能，但也可以将其应用于其他操作。CreateBindCtx 函数将此字段初始化为零。
        /// </summary>
        public int dwTickCountDeadline;
    }
}

using System;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 控制文件操作的标志。
    /// </summary>
    [Flags]
    public enum FileOperationFlags : uint
    {
        /// <summary>
        /// pTo 成员指定多个目标文件（pFrom中的每个源文件一个），而不是要存储所有源文件的一个目录。
        /// </summary>
        FOF_MULTIDESTFILES = 0x0001,

        /// <summary>
        /// 未使用。
        /// </summary>
        FOF_CONFIRMMOUSE = 0x0002,

        /// <summary>
        /// 不显示进度对话框。
        /// </summary>
        FOF_SILENT = 0x0004,

        /// <summary>
        /// 如果已存在具有目标名称的项，请在移动、复制或重命名操作中为正在操作的项提供新名称。
        /// </summary>
        FOF_RENAMEONCOLLISION = 0x0008,

        /// <summary>
        /// 对于显示的任何对话框，以 “是”对“全部 ”做出响应。
        /// </summary>
        FOF_NOCONFIRMATION = 0x0010,

        /// <summary>
        /// 如果指定了 FOF_RENAMEONCOLLISION 并重命名了任何文件，请将包含其旧名称和新名称的名称映射对象分配给 hNameMappings 成员。 不再需要时，必须使用 SHFreeNameMappings 释放此对象。
        /// </summary>
        FOF_WANTMAPPINGHANDLE = 0x0020,

        /// <summary>
        /// 如果可能，请保留撤消信息。
        /// 在 Windows Vista 之前，只能从执行原始操作的同一进程中撤消操作。
        /// 在 Windows Vista 及更高版本中，撤消的范围是用户会话。 在用户会话中运行的任何进程都可以撤消另一个操作。 撤消状态保留在 Explorer.exe 进程中，只要该进程正在运行，它就可以协调撤消函数。
        /// 如果源文件参数不包含完全限定的路径和文件名，则忽略此标志。
        /// </summary>
        FOF_ALLOWUNDO = 0x0040,

        /// <summary>
        /// 如果指定了通配符文件名 (，则仅对文件 (不在文件夹) ) 上执行操作。
        /// </summary>
        FOF_FILESONLY = 0x0080,

        /// <summary>
        /// 显示进度对话框，但不会在操作时显示单个文件名。
        /// </summary>
        FOF_SIMPLEPROGRESS = 0x0100,

        /// <summary>
        /// 如果操作要求创建新文件夹，请不要确认创建新文件夹。
        /// </summary>
        FOF_NOCONFIRMMKDIR = 0x0200,

        /// <summary>
        /// 如果发生错误，请不要向用户显示消息。 如果在未FOFX_EARLYFAILURE的情况下设置此标志，则会将任何错误视为用户在对话框中选择了 “忽略 ”或“ 继续 ”。 它会停止当前操作，设置一个标志以指示某个操作已中止，并继续执行操作的其余部分。
        /// </summary>
        FOF_NOERRORUI = 0x0400,

        /// <summary>
        /// 请勿复制项的安全属性。
        /// </summary>
        FOF_NOCOPYSECURITYATTRIBS = 0x0800,

        /// <summary>
        /// 仅在本地文件夹中操作。 不要以递归方式操作到子目录中。
        /// </summary>
        FOF_NORECURSION = 0x1000,

        /// <summary>
        /// 不要将连接的项作为组移动。 仅移动指定的文件。
        /// </summary>
        FOF_NO_CONNECTED_ELEMENTS = 0x2000,

        /// <summary>
        /// 如果在删除操作期间销毁而不是回收文件或文件夹，则发送警告。 此标志部分替代 FOF_NOCONFIRMATION。
        /// </summary>
        FOF_WANTNUKEWARNING = 0x4000,

        /// <summary>
        /// 未使用。
        /// </summary>
        FOF_NORECURSEREPARSE = 0x8000,

        /// <summary>
        /// 了解 Shell 命名空间交接点。 默认情况下，不输入交接点。 有关交接点的详细信息，请参阅 指定命名空间扩展的位置。
        /// </summary>
        FOFX_NOSKIPJUNCTIONS = 0x00010000,

        /// <summary>
        /// 如果可能，请在目标中创建硬链接，而不是文件的新实例。
        /// </summary>
        FOFX_PREFERHARDLINK = 0x00020000,

        /// <summary>
        /// 如果操作需要提升的权限，并且FOF_NOERRORUI标志设置为禁用错误 UI，则仍会显示 UAC UI 提示。
        /// </summary>
        FOFX_SHOWELEVATIONPROMPT = 0x00040000,

        /// <summary>
        /// 在 Windows 8 中引入。 删除文件时，将其发送到回收站，而不是永久删除它。
        /// </summary>
        FOFX_RECYCLEONDELETE = 0x00080000,

        /// <summary>
        /// 如果FOFX_EARLYFAILURE与 FOF_NOERRORUI 一起设置，则在任何操作中遇到任何错误时，将停止整个操作集。 仅当设置了FOF_NOERRORUI时，此标志才有效。
        /// </summary>
        FOFX_EARLYFAILURE = 0x00100000,

        /// <summary>
        /// 重命名冲突的方式是保留文件扩展名。 仅当同时设置了FOF_RENAMEONCOLLISION时，此标志才有效。
        /// </summary>
        FOFX_PRESERVEFILEEXTENSIONS = 0x00200000,

        /// <summary>
        /// 如果发生冲突，请根据 Date Modified 属性保留较新的文件或文件夹。 此操作会自动完成，不会向用户显示提示 UI。
        /// </summary>
        FOFX_KEEPNEWERFILE = 0x00400000,

        /// <summary>
        /// 请勿使用复制挂钩。
        /// </summary>
        FOFX_NOCOPYHOOKS = 0x00800000,

        /// <summary>
        /// 不允许最小化进度对话框。
        /// </summary>
        FOFX_NOMINIMIZEBOX = 0x01000000,

        /// <summary>
        /// 执行跨卷移动操作时，将源项的安全属性复制到目标项。 如果没有此标志，目标项将接收其新文件夹的安全属性。
        /// </summary>
        FOFX_MOVEACLSACROSSVOLUMES = 0x02000000,

        /// <summary>
        /// 不要在进度对话框中显示源项的路径。
        /// </summary>
        FOFX_DONTDISPLAYSOURCEPATH = 0x04000000,

        /// <summary>
        /// 不要在进度对话框中显示目标项的路径。
        /// </summary>
        FOFX_DONTDISPLAYDESTPATH = 0x08000000,

        /// <summary>
        /// 在 Windows Vista SP1 中引入。 用户需要权限提升要求，因此不要显示要求确认提升权限的对话框。
        /// </summary>
        FOFX_REQUIREELEVATION = 0x10000000,

        /// <summary>
        /// 在 Windows 8 中引入。 文件操作由用户调用，应放置在撤消堆栈上。 此标志优先于FOF_ALLOWUNDO。
        /// </summary>
        FOFX_ADDUNDORECORD = 0x20000000,

        /// <summary>
        /// 在 Windows 7 中引入。 在进度对话框中显示 “正在下载 ”而不是 “复制 ”消息。
        /// </summary>
        FOFX_COPYASDOWNLOAD = 0x40000000,

        /// <summary>
        /// 在 Windows 7 中引入。 不要在进度对话框中显示位置行。
        /// </summary>
        FOFX_DONTDISPLAYLOCATIONS = 0x80000000
    }
}

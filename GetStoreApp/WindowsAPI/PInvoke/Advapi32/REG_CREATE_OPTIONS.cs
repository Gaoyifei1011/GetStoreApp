using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 注册表创建项的配置选项
    /// </summary>
    [Flags]
    public enum REG_CREATE_OPTIONS
    {
        /// <summary>
        /// 此键不可变;这是默认值。
        /// 信息存储在文件中，并在系统重启时保留。 RegSaveKey 函数保存不可变的键。
        /// </summary>
        REG_OPTION_NON_VOLATILE = 0x00000000,  // 0x0000

        /// <summary>
        /// 函数创建的所有键都是可变的。 此信息存储在内存中，并且在卸载相应的注册表配置单元时不保留此信息。 对于 HKEY_LOCAL_MACHINE，仅当系统启动完全关闭时，才会发生这种情况。 对于 RegLoadKey 函数加载的注册表项，在执行相应的 RegUnLoadKey 时会发生这种情况。 RegSaveKey 函数不保存易失性密钥。 对于已存在的键，将忽略此标志。
        /// </summary>
        REG_OPTION_VOLATILE = 0x00000001,      // 0x0001

        /// <summary>
        /// 此键是符号链接。 目标路径分配给键的 L“SymbolicLinkValue”值。 目标路径必须是绝对注册表路径。
        /// </summary>
        REG_OPTION_CREATE_LINK = 0x00000002,

        /// <summary>
        /// 如果设置了此标志，函数将忽略 samDesired 参数，并尝试使用备份或还原密钥所需的访问权限打开密钥。 如果调用线程启用了SE_BACKUP_NAME特权，则使用ACCESS_SYSTEM_SECURITY打开密钥，KEY_READ访问权限。 如果调用线程启用了SE_RESTORE_NAME特权（从 Windows Vista 开始），则会使用ACCESS_SYSTEM_SECURITY、DELETE 和KEY_WRITE访问权限打开密钥。 如果两个特权都已启用，则密钥具有这两种权限的组合访问权限。 有关详细信息，请参阅使用特殊特权运行。
        /// </summary>
        REG_OPTION_BACKUP_RESTORE = 0x00000004
    };
}

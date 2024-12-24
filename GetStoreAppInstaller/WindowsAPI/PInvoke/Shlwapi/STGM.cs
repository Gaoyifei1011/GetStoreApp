using System;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.Shlwapi
{
    [Flags]
    public enum STGM
    {
        /// <summary>
        /// 指示在直接模式下，每次对存储或流元素的更改都会在发生时写入。 如果既未指定 STGM_DIRECT ，也未指定 STGM_TRANSACTED ，则这是默认值。
        /// </summary>
        STGM_DIRECT = 0x00000000,

        /// <summary>
        /// 指示在事务处理模式下，仅当调用显式提交操作时，才会缓冲和写入更改。
        /// </summary>
        STGM_TRANSACTED = 0x00010000,

        /// <summary>
        /// 在有限但经常使用的情况下，提供复合文件的更快实现。
        /// </summary>
        STGM_SIMPLE = 0x08000000,

        /// <summary>
        /// 指示对象为只读，这意味着无法进行修改。
        /// </summary>
        STGM_READ = STGM_DIRECT,

        /// <summary>
        /// 允许您保存对对象的更改，但不允许访问其数据。
        /// </summary>
        STGM_WRITE = 0x00000001,

        /// <summary>
        /// 启用对对象数据的访问和修改。
        /// </summary>
        STGM_READWRITE = 0x00000002,

        /// <summary>
        /// 指定不拒绝对象的后续打开读取或写入访问。
        /// </summary>
        STGM_SHARE_DENY_NONE = 0x00000040,

        /// <summary>
        /// 阻止其他人随后在 STGM_READ 模式下打开对象。 它通常用于根存储对象。
        /// </summary>
        STGM_SHARE_DENY_READ = 0x00000030,

        /// <summary>
        /// 防止其他人随后打开对象进行 STGM_WRITE 或 STGM_READWRITE 访问。 在事务处理模式下，共享 STGM_SHARE_DENY_WRITE 或 STGM_SHARE_EXCLUSIVE 可以显著提高性能，因为它们不需要快照。
        /// </summary>
        STGM_SHARE_DENY_WRITE = 0x00000020,

        /// <summary>
        /// 防止其他人随后以任何模式打开对象。
        /// </summary>
        STGM_SHARE_EXCLUSIVE = 0x00000010,

        /// <summary>
        /// 打开具有对最近提交的版本的独占访问权限的存储对象。
        /// </summary>
        STGM_PRIORITY = 0x00040000,

        /// <summary>
        /// 指示释放根存储对象时，基础文件将被自动销毁。
        /// </summary>
        STGM_DELETEONRELEASE = 0x04000000,

        /// <summary>
        /// 指示在事务处理模式下，通常使用临时暂存文件来保存修改，直到调用 Commit 方法。
        /// </summary>
        STGM_NOSCRATCH = 0x00100000,

        /// <summary>
        /// 指示应在新对象替换现有存储对象或流之前删除它。仅当成功删除现有对象时才指定此标志时，才会创建新对象。
        /// </summary>
        STGM_CREATE = 0x00001000,

        /// <summary>
        /// 创建新对象，同时保留名为“Contents”的流中的现有数据。 对于存储对象或字节数组，无论现有文件或字节数组当前是否包含分层存储对象，旧数据都会格式化为流。 此标志只能在创建根存储对象时使用。
        /// </summary>
        STGM_CONVERT = 0x00020000,

        /// <summary>
        /// 如果存在具有指定名称的现有对象，则会导致创建操作失败。 在这种情况下，返回 STG_E_FILEALREADYEXISTS 。 这是默认创建模式;也就是说，如果未指定其他创建标志，则隐式 STGM_FAILIFTHERE 。
        /// </summary>
        STGM_FAILIFTHERE = STGM_DIRECT,

        /// <summary>
        /// 当使用 STGM_TRANSACTED 且不使用 STGM_SHARE_EXCLUSIVE 或 STGM_SHARE_DENY_WRITE 打开存储对象时 ，使用此标志。
        /// </summary>
        STGM_NOSNAPSHOT = 0x00200000,

        /// <summary>
        /// 支持单编写器、多读取程序文件操作的直接模式。
        /// </summary>
        STGM_DIRECT_SWMR = 0x00400000,
    }
}

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// IStream 接口允许读取和写入数据流对象。 Stream 对象包含结构化存储对象中的数据，其中存储提供 结构。 简单数据可以直接写入流，但最常见的是，流是嵌套在存储对象中的元素。 它们类似于标准文件。
    /// IStream 接口定义的方法类似于 MS-DOS FAT 文件函数。 例如，每个流对象都有自己的访问权限和查找指针。 DOS 文件和流对象之间的main区别在于，在后一种情况下，使用 IStream 接口指针而不是文件句柄打开流。
    /// 此接口中的方法将对象的数据显示为可以读取或写入的连续字节序列。 还有一些方法可用于提交和还原在事务处理模式下打开的流上的更改，以及限制对流中某个字节范围的访问的方法。流可以在不消耗文件系统资源的情况下长时间保持打开状态。 IUnknown：：Release 方法类似于文件上的 close 函数。 释放后，流对象不再有效，并且无法使用。
    /// </summary>
    [GeneratedComInterface, Guid("0000000C-0000-0000-C000-000000000046")]
    public partial interface IStream : ISequentialStream
    {
        /// <summary>
        /// Seek 方法将查找指针更改为新位置。 新位置相对于流的开头、流的末尾或当前查找指针。
        /// </summary>
        /// <param name="dlibMove">要添加到 dwOrigin 参数指示的位置的位移。 如果 dwOrigin是STREAM_SEEK_SET，则会将其解释为无符号值，而不是有符号值。</param>
        /// <param name="dwOrigin">dlibMove 中指定的位移的原点。 原点可以是文件 (STREAM_SEEK_SET ) 的开头、当前查找指针 (STREAM_SEEK_CUR) ，也可以是文件 (STREAM_SEEK_END) 的末尾。 有关值的详细信息，请参阅 STREAM_SEEK 枚举。</param>
        /// <param name="plibNewPosition">指向此方法从流开头写入新搜寻指针值的位置的指针的指针。可以将此指针设置为 NULL。 在这种情况下，此方法不提供新的搜寻指针。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int Seek(long dlibMove, int dwOrigin, nint /* optional ulong* */ plibNewPosition);

        /// <summary>
        /// SetSize 方法更改流对象的大小。
        /// </summary>
        /// <param name="libNewSize">指定流的新大小（以字节为单位）。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int SetSize(ulong libNewSize);

        /// <summary>
        /// CopyTo 方法将指定数量的字节从流中的当前搜寻指针复制到另一个流中的当前查找指针。
        /// </summary>
        /// <param name="pstm">指向目标流的指针。 pstm 指向的流可以是新流或源流的克隆。</param>
        /// <param name="cb">要从源流复制的字节数。</param>
        /// <param name="pcbRead">指向此方法写入从源读取的实际字节数的位置的指针。 可以将此指针设置为 NULL。 在这种情况下，此方法不提供实际读取的字节数。</param>
        /// <param name="pcbWritten">指向此方法写入要写入到目标的实际字节数的位置的指针。 可以将此指针设置为 NULL。 在这种情况下，此方法不提供实际写入的字节数。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int CopyTo([MarshalAs(UnmanagedType.Interface)] IStream pstm, ulong cb, nint pcbRead, nint pcbWritten);

        /// <summary>
        /// Commit 方法可确保对在事务处理模式下打开的流对象所做的任何更改都反映在父存储中。 如果流对象在直接模式下打开， 则 IStream：：Commit 除了将所有内存缓冲区刷新到下一级存储对象之外，没有其他效果。 流的 COM 复合文件实现不支持在事务处理模式下打开流。
        /// </summary>
        /// <param name="grfCommitFlags">控制提交对流对象的更改的方式。 有关这些值的定义，请参阅 STGC 枚举。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int Commit(uint grfCommitFlags);

        /// <summary>
        /// Revert 方法放弃自上次 IStream：：Commit 调用以来对事务处理流所做的所有更改。 在直接模式下打开的流和使用 IStream：：Revert 的 COM 复合文件实现的流上，此方法不起作用。
        /// </summary>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int Revert();

        /// <summary>
        /// LockRegion 方法限制对流中指定字节范围的访问。 支持此功能是可选的，因为某些文件系统不提供此功能。
        /// </summary>
        /// <param name="libOffset">指定范围开头的字节偏移量的整数。</param>
        /// <param name="cb">指定要限制的范围长度的整数（以字节为单位）。</param>
        /// <param name="dwLockType">指定请求访问范围的限制。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int LockRegion(ulong libOffset, ulong cb, uint dwLockType);

        /// <summary>
        /// UnlockRegion 方法删除对以前使用 IStream：：LockRegion 限制的字节范围的访问限制。
        /// </summary>
        /// <param name="libOffset">指定范围开头的字节偏移量的整数。</param>
        /// <param name="cb">指定要限制的范围长度的整数（以字节为单位）。</param>
        /// <param name="dwLockType">指定请求访问范围的限制。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int UnlockRegion(ulong libOffset, ulong cb, uint dwLockType);

        /// <summary>
        /// Stat 方法检索此流的 STATSTG 结构。
        /// </summary>
        /// <param name="pstatstg">指向 STATSTG 结构的指针，此方法在其中放置有关此流对象的信息。</param>
        /// <param name="grfStatFlag">指定此方法不返回 STATSTG 结构中的某些成员，从而保存内存分配操作。 值取自 STATFLAG 枚举。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int Stat(out nint pstatstg, uint grfStatFlag);

        /// <summary>
        /// Clone 方法使用自己的 seek 指针创建新的流对象，该对象引用与原始流相同的字节。
        /// </summary>
        /// <param name="ppstm">成功后，指针指向指向新流对象的 IStream 指针的位置。 如果发生错误，此参数为 NULL。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
    }
}

using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// IStream 接口允许读取和写入数据流对象。 流对象包含结构化存储对象中的数据，其中存储提供 结构。 简单数据可以直接写入流，但最常见的是，流是嵌套在存储对象中的元素。 它们类似于标准文件。
    /// IStream 接口定义的方法类似于 MS-DOS FAT 文件函数。 例如，每个流对象都有自己的访问权限和查找指针。 DOS 文件和流对象之间的主要区别在于，在后一种情况下，流是使用 IStream 接口指针而不是文件句柄打开的。
    /// 此接口中的方法将对象的数据显示为可以读取或写入的连续字节序列。 还有一些方法可用于提交和还原在事务处理模式下打开的流上的更改，以及限制对流中某个字节范围的访问的方法。
    /// 流可以在不消耗文件系统资源的情况下长时间保持打开状态。 IUnknown::Release 方法类似于文件上的 close 函数。 释放后，流对象不再有效，并且无法使用。
    /// 异步名字对象的客户端可以在数据拉取或数据推送模型之间进行选择，以驱动异步 <see cref="IMoniker.BindToStorage"> 操作和接收异步通知。 有关详细信息 ，请参阅 URL 名字对象 。 下表比较了这两个下载模型中 IBindStatusCallback::OnDataAvailable 中返回的异步 ISequentialStream::Read 和 <see cref="Seek"> 调用的行为：
    /// </summary>
    [Guid("0000000c-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IStream
    {
        /// <summary>
        /// <see cref="Read"> 方法从流对象读取指定数量的字节到内存中，从当前搜寻指针开始。
        /// </summary>
        /// <param name="pv">指向流数据读取到的缓冲区的指针。</param>
        /// <param name="cb">要从流对象读取的数据字节数。</param>
        /// <param name="pcbRead">指向 ULONG 变量的指针，该变量接收从流对象读取的实际字节数。</param>
        void Read([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out] byte[] pv, int cb, IntPtr pcbRead);

        /// <summary>
        /// <see cref="Write"> 方法将指定的字节数写入流对象，从当前查找指针开始。
        /// </summary>
        /// <param name="pv">指向包含要写入流的数据的缓冲区的指针。 即使 <param name="cb"> 为零，也必须为此参数提供有效的指针。</param>
        /// <param name="cb">要尝试写入流的数据字节数。 此值可以为零。</param>
        /// <param name="pcbWritten">指向 ULONG 变量的指针，此方法将写入流对象的实际字节数。 调用方可以将此指针设置为 NULL，在这种情况下，此方法不提供写入的实际字节数。</param>
        void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbWritten);

        /// <summary>
        /// <see cref="Seek"> 方法将 查找 指针更改为新位置。 新位置相对于流的开头、流的末尾或当前查找指针。
        /// </summary>
        /// <param name="dlibMove">要添加到 <param name="dwOrigin"> 参数指示的位置的偏移量。 如果 dwOriginSTREAM_SEEK_SET，则将其解释为无符号值，而不是带符号值。</param>
        /// <param name="dwOrigin"><param name="dlibMove"> 中指定的位移的原点。 源可以是文件开头 (STREAM_SEEK_SET) 、当前查找指针 (STREAM_SEEK_CUR) 或 文件末尾 (STREAM_SEEK_END) 。 有关值的详细信息，请参阅 STREAM_SEEK 枚举。</param>
        /// <param name="plibNewPosition">指向此方法从流开头写入新查找指针的值的位置的指针的指针。</param>
        void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);

        /// <summary>
        /// <see cref="SetSize"> 方法更改流对象的大小。
        /// </summary>
        /// <param name="libNewSize">指定流的新大小（以字节为单位）。</param>
        void SetSize(long libNewSize);

        /// <summary>
        /// <see cref="CopyTo"> 方法将流中的当前查找指针中的指定字节数复制到另一个流中的当前查找指针。
        /// </summary>
        /// <param name="pstm">指向目标流的指针。 <param name="pstm"> 指向的流可以是新流或源流的克隆。</param>
        /// <param name="cb">要从源流复制的字节数。</param>
        /// <param name="pcbRead">指向此方法写入从源读取的实际字节数的位置的指针。 可以将此指针设置为 NULL。 在这种情况下，此方法不提供读取的实际字节数。</param>
        /// <param name="pcbWritten">指向此方法写入要写入到目标的实际字节数的位置的指针。 可以将此指针设置为 NULL。 在这种情况下，此方法不提供写入的实际字节数。</param>
        void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);

        /// <summary>
        /// <see cref="Commit"> 方法可确保在事务处理模式下打开的流对象所做的任何更改都反映在父存储中。 如果流对象在直接模式下打开， 则 <see cref="Commit"> 除了将所有内存缓冲区刷新到下一级存储对象之外，也不起作用。 流的 COM 复合文件实现不支持在事务处理模式下打开流。
        /// </summary>
        /// <param name="grfCommitFlags">控制提交对流对象的更改的方式。 有关这些值的定义，请参阅 STGC 枚举。</param>
        void Commit(int grfCommitFlags);

        /// <summary>
        /// <see cref="Revert"> 方法放弃自上次 <see cref="Commit"> 调用以来对事务处理流所做的所有更改。 在直接模式下打开的流，并使用 <see cref="Revert"> 的 COM 复合文件实现流，此方法不起作用。
        /// </summary>
        void Revert();

        /// <summary>
        /// <see cref="LockRegion"> 方法限制对流中指定字节范围的访问。 支持此功能是可选的，因为某些文件系统不提供此功能。
        /// </summary>
        /// <param name="libOffset">指定范围开头的字节偏移量的整数。</param>
        /// <param name="cb">指定要限制的范围长度（以字节为单位）的整数。</param>
        /// <param name="dwLockType">指定在访问范围时请求的限制。</param>
        void LockRegion(long libOffset, long cb, int dwLockType);

        /// <summary>
        /// <see cref="UnlockRegion"> 方法删除以前使用 <see cref="LockRegion"> 限制的一系列字节的访问限制。
        /// </summary>
        /// <param name="libOffset">指定范围的开头的字节偏移量。</param>
        /// <param name="cb">指定要限制的范围长度（以字节为单位）。</param>
        /// <param name="dwLockType">指定以前放置在该区域上的访问限制。</param>
        void UnlockRegion(long libOffset, long cb, int dwLockType);

        /// <summary>
        /// <see cref="Stat"> 方法检索此流的 <see cref="STATSTG"> 结构。
        /// </summary>
        /// <param name="pstatstg">指向 <see cref="STATSTG"> 结构的指针，此方法在其中放置有关此流对象的信息。</param>
        /// <param name="grfStatFlag">指定此方法不返回 <see cref="STATSTG"> 结构中的某些成员，从而节省内存分配操作。 值取自 <see cref="STATSTG"> 枚举。</param>
        void Stat(out STATSTG pstatstg, int grfStatFlag);

        /// <summary>
        /// Clone 方法创建一个新的流对象，其自己的查找指针引用与原始流相同的字节。
        /// </summary>
        /// <param name="ppstm">成功后，指向指向新流对象的 <see cref="IStream"> 指针的位置的指针。 如果发生错误，此参数为 NULL。</param>
        void Clone(out IStream ppstm);
    }
}

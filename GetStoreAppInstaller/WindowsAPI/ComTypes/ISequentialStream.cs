using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// ISequentialStream 接口支持对流对象进行简化的顺序访问。 IStream 接口从 ISequentialStream 继承其 Read 和 Write 方法。
    /// </summary>
    [GeneratedComInterface, Guid("0C733A30-2A1C-11CE-ADE5-00AA0044773D")]
    public partial interface ISequentialStream
    {
        /// <summary>
        /// Read 方法从当前搜寻指针开始，将指定数量的字节从流对象读取到内存中。
        /// </summary>
        /// <param name="pv">指向要读取流数据的缓冲区的指针。</param>
        /// <param name="cb">要从流对象读取的数据的字节数。</param>
        /// <param name="pcbRead">指向 ULONG 变量的指针，该变量接收从流对象读取的实际字节数。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int Read(nint pv, uint cb, out uint pcbRead);

        /// <summary>
        /// Write 方法从当前搜寻指针处开始，将指定数量的字节写入流对象。
        /// </summary>
        /// <param name="pv">指向缓冲区的指针，该缓冲区包含要写入流的数据。 即使 cb 为零，也必须为此参数提供有效的指针。</param>
        /// <param name="cb">要尝试写入流的数据字节数。 此值可以为零。</param>
        /// <param name="pcbWritten">指向 ULONG 变量的指针，此方法在其中写入流对象的实际字节数。 调用方可以将此指针设置为 NULL，在这种情况下，此方法不提供实际写入的字节数。</param>
        /// <returns>此方法可以返回其中一个值。</returns>
        [PreserveSig]
        int Write(nint pv, uint cb, out uint pcbWritten);
    }
}

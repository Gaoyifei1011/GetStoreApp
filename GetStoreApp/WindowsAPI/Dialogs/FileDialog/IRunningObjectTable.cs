using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 管理对正在运行的对象表 （ROT） 的访问，ROT（每个工作站上全局可访问的查找表）。工作站的 ROT 跟踪那些可以通过名字对象标识且当前在工作站上运行的对象。当客户端尝试将名字对象绑定到对象时，名字对象会检查 ROT 以查看对象是否已运行;这允许名字对象绑定到当前实例，而不是加载新实例。
    /// </summary>
    [Guid("00000010-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IRunningObjectTable
    {
        /// <summary>
        /// 在正在运行的对象表中注册对象及其标识名字对象， (ROT) 。
        /// </summary>
        /// <param name="grfFlags">指定 ROT 对 <param name="punkObject"> 的引用是否弱或强，并通过在 ROT 中的条目控制对对象的访问。</param>
        /// <param name="punkObject">指向正在注册为正在运行的对象的指针。</param>
        /// <param name="pmkObjectName">指向标识 <param name="punkObject"> 的名字对象的指针。</param>
        /// <returns></returns>
        int Register(int grfFlags, [MarshalAs(UnmanagedType.Interface)] object punkObject, IMoniker pmkObjectName);

        /// <summary>
        /// 从正在运行的对象表中删除一个条目， (ROT) ，该条目以前是通过调用 <see cref="Register"> 注册的。
        /// </summary>
        /// <param name="dwRegister">要吊销的 ROT 条目的标识符。</param>
        void Revoke(int dwRegister);

        /// <summary>
        /// 确定指定的名字对象是否当前正在运行。
        /// </summary>
        /// <param name="pmkObjectName">指向名字对象上的 <see cref="IMoniker"> 接口的指针。</param>
        /// <returns>如果对象处于运行状态，则返回值为 TRUE。 否则为 FALSE。</returns>
        [PreserveSig]
        int IsRunning(IMoniker pmkObjectName);

        /// <summary>
        /// 确定指定名字对象标识的对象是否正在运行，如果是，则检索指向该对象的指针。
        /// </summary>
        /// <param name="pmkObjectName">指向名字对象上的 <see cref="IMoniker"> 接口的指针。</param>
        /// <param name="ppunkObject">指向 IUnknown 指针变量的指针，该变量接收指向正在运行对象的接口指针。 成功后，实现会在对象上调用 AddRef ;调用方负责调用 发布。 如果对象未运行或发生错误，则实现会将 <param name="ppunkObject"> 设置为 NULL。</param>
        /// <returns></returns>
        [PreserveSig]
        int GetObject(IMoniker pmkObjectName, [MarshalAs(UnmanagedType.Interface)] out object ppunkObject);

        /// <summary>
        /// 记录上次修改正在运行的对象的时间。 必须先将对象注册到正在运行的对象表， (ROT) 。 此方法将上次更改的时间存储在 ROT 中。
        /// </summary>
        /// <param name="dwRegister">已更改对象的 ROT 条目的标识符。 此值以前由 <see cref="Register"> 返回。</param>
        /// <param name="pfiletime">指向包含对象上次更改时间的 <see cref="FILETIME"> 结构的指针。</param>
        void NoteChangeTime(int dwRegister, ref FILETIME pfiletime);

        /// <summary>
        /// 检索上次修改对象的时间。
        /// </summary>
        /// <param name="pmkObjectName">指向名字对象上的 <see cref="IMoniker"> 接口的指针。</param>
        /// <param name="pfiletime">指向接收对象上次更改时间的 <see cref="FILETIME"> 结构的指针。</param>
        /// <returns></returns>
        [PreserveSig]
        int GetTimeOfLastChange(IMoniker pmkObjectName, out FILETIME pfiletime);

        /// <summary>
        /// 创建并返回指向枚举器的指针，该枚举器可以列出当前在正在运行的对象表中注册的所有对象的名字对象 (ROT) 。
        /// </summary>
        /// <param name="ppenumMoniker">指向 <see cref="IEnumMoniker"> 指针变量的指针，该变量接收指向 ROT 的新枚举器的接口指针。 成功后，实现会在枚举器上调用 AddRef ;调用方负责调用 发布。 如果发生错误，则为实现将 <param name="ppenumMoniker"> 设置为 NULL。</param>
        void EnumRunning(out IEnumMoniker ppenumMoniker);
    }
}

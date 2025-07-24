using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 允许创建对象的类。
    /// </summary>
    [GeneratedComInterface, Guid("00000001-0000-0000-C000-000000000046")]
    public partial interface IClassFactory
    {
        /// <summary>
        /// 创建未初始化的对象。
        /// </summary>
        /// <param name="pUnkOuter">如果对象是作为聚合的一部分创建的，请指定指向聚合的控制 IUnknown 接口的指针。 否则，此参数必须为 NULL。</param>
        /// <param name="riid">对接口标识符的引用，用于与新创建的对象通信。 如果 pUnkOuter 为 NULL，则此参数通常是初始化接口的 IID;如果 pUnkOuter 为非 NULL，则必须IID_IUnknown riid 。</param>
        /// <param name="ppvObject">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，*ppvObject 包含请求的接口指针。 如果 对象不支持 riid 中指定的接口，则实现必须将 *ppvObject 设置为 NULL。</param>
        /// <returns>此方法可以返回标准返回值E_INVALIDARG、E_OUTOFMEMORY和E_UNEXPECTED 和 S_OK、CLASS_E_NOAGGREGATION、E_NOINTERFACE。</returns>
        [PreserveSig]
        int CreateInstance(nint pUnkOuter, in Guid riid, out nint ppvObject);

        /// <summary>
        /// 锁定在内存中打开的对象应用程序。 这样可以更快地创建实例。
        /// </summary>
        /// <param name="fLock">如果 为 TRUE，则递增锁计数;如果 为 FALSE，则递减锁计数。</param>
        /// <returns>此方法可返回标准返回值E_OUTOFMEMORY、E_UNEXPECTED、E_FAIL和S_OK。</returns>
        [PreserveSig]
        int LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
    }
}

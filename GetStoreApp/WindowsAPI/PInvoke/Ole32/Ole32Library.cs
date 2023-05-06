using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Ole32
{
    /// <summary>
    /// Ole32.dll 函数库
    /// </summary>
    public static class Ole32Library
    {
        private const string Ole32 = "Ole32.dll";

        /// <summary>
        /// 创建和默认初始化与指定 CLSID 关联的类的单个对象。
        /// 如果只想在本地系统上创建一个对象，请调用 CoCreateInstance 。 若要在远程系统上创建单个对象，请调用 CoCreateInstanceEx 函数。 若要基于单个 CLSID 创建多个对象，请调用 CoGetClassObject 函数。
        /// </summary>
        /// <param name="rclsid">与要用于创建对象的数据和代码关联的 CLSID。</param>
        /// <param name="pUnkOuter">
        /// 如果 为 NULL，则指示对象未作为聚合的一部分创建。 如果为非 NULL，则指向聚合对象的 IUnknown 接口的指针 (控制 IUnknown) 。
        /// </param>
        /// <param name="dwClsContext">管理新创建对象的代码将在其中运行的上下文。 这些值取自枚举 CLSCTX。</param>
        /// <param name="riid">对要用于与对象通信的接口标识符的引用。</param>
        /// <param name="ppv">接收 riid 中请求的接口指针的指针变量的地址。 成功返回后，*ppv 包含请求的接口指针。 失败后，*ppv 包含 NULL。</param>
        [DllImport(Ole32, EntryPoint = "CoCreateInstance", PreserveSig = false, SetLastError = true)]
        public static extern void CoCreateInstance([In] ref Guid rclsid, IntPtr pUnkOuter, CLSCTX dwClsContext, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
    }
}

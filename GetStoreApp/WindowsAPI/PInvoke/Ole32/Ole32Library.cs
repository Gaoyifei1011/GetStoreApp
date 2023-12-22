using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Ole32
{
    /// <summary>
    /// Ole32.dll 函数库
    /// </summary>
    public static partial class Ole32Library
    {
        private const string Ole32 = "ole32.dll";

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
        [LibraryImport(Ole32, EntryPoint = "CoCreateInstance", SetLastError = true)]
        public static partial int CoCreateInstance(ref Guid rclsid, IntPtr pUnkOuter, CLSCTX dwClsContext, ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// 初始化 COM 库以供调用线程使用，设置线程的并发模型，并根据需要为线程创建一个新单元。
        /// </summary>
        /// <param name="pvReserved">此参数是保留的，必须为 NULL。</param>
        /// <param name="dwCoInit">线程的并发模型和初始化选项。 此参数的值取自 COINIT 枚举。 可以使用 COINIT 中值的任意组合，但不能同时设置COINIT_APARTMENTTHREADED和COINIT_MULTITHREADED标志。 默认值为 COINIT_MULTITHREADED。</param>
        /// <returns></returns>
        [LibraryImport(Ole32, EntryPoint = "CoInitializeEx", SetLastError = false)]
        public static partial int CoInitializeEx(IntPtr pvReserved, COINIT dwCoInit);

        /// <summary>
        /// 关闭当前线程上的 COM 库，卸载线程加载的所有 DLL，释放线程维护的任何其他资源，并强制关闭线程上的所有 RPC 连接。
        /// </summary>
        [LibraryImport(Ole32, EntryPoint = "CoUninitialize", SetLastError = false)]
        public static partial void CoUninitialize();
    }
}

using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Combase
{
    public static partial class CombaseLibrary
    {
        private const string Combase = "combase.dll";

        /// <summary>
        /// 使用指定的并发模型初始化当前线程上的 Windows 运行时。
        /// </summary>
        /// <param name="initType">线程的并发模型。 默认值为 RO_INIT_MULTITHREADED。</param>
        /// <returns>
        /// 此函数可以返回标准返回值 E_INVALIDARG、 E_OUTOFMEMORY和 E_UNEXPECTED，以及 S_OK、S_FALSE、RPC_E_CHANGED_MODE
        /// </returns>
        [LibraryImport(Combase, EntryPoint = "RoInitialize", SetLastError = false)]
        public static partial int RoInitialize(RO_INIT_TYPE initType);

        /// <summary>
        /// 关闭当前线程上的 Windows 运行时。
        /// </summary>
        [LibraryImport(Combase, EntryPoint = "RoUninitialize", SetLastError = false)]
        public static partial void RoUninitialize();
    }
}

using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreAppWidget.WindowsAPI.PInvoke.Ole32
{
    /// <summary>
    /// Ole32.dll 函数库
    /// </summary>
    public static partial class Ole32Library
    {
        private const string Ole32 = "ole32.dll";

        /// <summary>
        /// 将 EXE 类对象注册到 OLE，以便其他应用程序可以连接到该对象。
        /// </summary>
        /// <param name="rclsid">要注册的 CLSID。</param>
        /// <param name="pUnk">指向正在发布其可用性的类对象上的 IUnknown 接口的指针。</param>
        /// <param name="dwClsContext">要在其中运行可执行代码的上下文。 有关这些上下文值的信息，请参阅 CLSCTX 枚举。</param>
        /// <param name="flags">指示如何与类对象建立连接。 有关这些标志的信息，请参阅 REGCLS 枚举。</param>
        /// <param name="lpdwRegister">指向标识已注册的类对象的值的指针;稍后由 CoRevokeClassObject 函数用来撤销注册。</param>
        /// <returns>此函数可以返回标准返回值E_INVALIDARG、E_OUTOFMEMORY和E_UNEXPECTED，以及S_OK。</returns>
        [LibraryImport(Ole32, EntryPoint = "CoRegisterClassObject", SetLastError = false), PreserveSig]
        public static partial int CoRegisterClassObject(in Guid rclsid, IntPtr pUnk, CLSCTX dwClsContext, REGCLS flags, out uint lpdwRegister);

        /// <summary>
        /// 通知 OLE 以前使用 CoRegisterClassObject 函数注册的类对象不再可供使用。
        /// </summary>
        /// <param name="dwRegister">以前从 CoRegisterClassObject 函数返回的令牌。</param>
        /// <returns>此函数可以返回标准返回值E_INVALIDARG、E_OUTOFMEMORY和E_UNEXPECTED，以及S_OK。</returns>
        [LibraryImport(Ole32, EntryPoint = "CoRevokeClassObject", SetLastError = false), PreserveSig]
        public static partial int CoRevokeClassObject(uint dwRegister);
    }
}

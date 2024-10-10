using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// 应用程序定义的回调函数，用于处理 EnumSystemGeoID 函数提供的枚举地理位置信息。GEO_ENUMPROC 类型定义指向此回调函数的指针。EnumGeoInfoProc 是应用程序定义的函数名称的占位符。
    /// </summary>
    /// <param name="GeoId">要检查的地理位置的标识符。</param>
    /// <returns>返回 TRUE 以继续枚举，否则返回 FALSE。</returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public unsafe delegate bool GEO_ENUMNAMEPROC(IntPtr unmamedParam1, IntPtr unmamedParam2);
}

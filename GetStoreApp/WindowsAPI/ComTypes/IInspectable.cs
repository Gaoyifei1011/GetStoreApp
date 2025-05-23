﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    [GeneratedComInterface, Guid("AF86E2E0-B12D-4C6A-9C5A-D7AA65101E90")]
    public partial interface IInspectable
    {
        /// <summary>
        /// 获取由当前Windows 运行时类实现的接口。
        /// </summary>
        /// <param name="iidCount">当前 Windows 运行时 对象实现的接口数，不包括 IUnknown 和 IInspectable 实现。</param>
        /// <param name="iids">指向数组的指针，该数组包含当前 Windows 运行时 对象实现的每个接口的 IID。 排除 IUnknown 和 IInspectable 接口。</param>
        /// <returns>此函数可以返回以下值。S_OK 和 E_OUTOFMEMORY</returns>
        [PreserveSig]
        int GetIids(out ulong iidCount, out nint iids);

        /// <summary>
        /// 获取当前Windows 运行时 对象的完全限定名称。
        /// </summary>
        /// <param name="className">当前Windows 运行时对象的完全限定名称。</param>
        /// <returns>此函数可以返回以下值。S_OK、E_OUTOFMEMORY 和 E_ILLEGAL_METHOD_CALL</returns>
        [PreserveSig]
        int GetRuntimeClassName(out nint className);

        /// <summary>
        /// 获取当前Windows 运行时对象的信任级别。
        /// </summary>
        /// <param name="trustLevel">当前Windows 运行时对象的信任级别。 默认值为 BaseLevel。</param>
        /// <returns>此方法始终返回 S_OK。</returns>
        [PreserveSig]
        int GetTrustLevel(out TrustLevel trustLevel);
    }
}

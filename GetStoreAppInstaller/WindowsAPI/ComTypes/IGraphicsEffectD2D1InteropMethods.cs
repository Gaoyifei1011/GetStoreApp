using GetStoreAppInstaller.WindowsAPI.ComTypes;
using System;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.GetStoreAppInstaller.WindowsAPI.ComTypes
{
    public static unsafe class IGraphicsEffectD2D1InteropMethods
    {
        public static Guid IID { get; } = typeof(IGraphicsEffectD2D1Interop).GUID;

        public static IntPtr AbiToProjectionVftablePtr { get; } = (IntPtr)((IIUnknownDerivedDetails)typeof(IGraphicsEffectD2D1Interop).GetCustomAttribute(typeof(IUnknownDerivedAttribute<,>))).ManagedVirtualMethodTable;
    }
}

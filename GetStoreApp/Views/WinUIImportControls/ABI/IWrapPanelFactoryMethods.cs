using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    internal static class IWrapPanelFactoryMethods
    {
        public static ref readonly Guid IID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)
                [
                    212, 87, 188, 69, 159, 81, 170, 83, 185, 240,
                    1, 74, 171, 152, 247, 78
                ]));
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.Controls.WrapPanel CreateInstance(IObjectReference _obj, object baseInterface, out object innerInterface)
        {
            nint thisPtr = _obj.ThisPtr;
            ObjectReferenceValue value = default;
            nint ptr = default;
            nint intPtr = default;
            try
            {
                value = MarshalInspectable<object>.CreateMarshaler2(baseInterface);
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint, nint*, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 6 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint, nint*, nint*, int>))))(thisPtr, MarshalInspectable<object>.GetAbi(value), &ptr, &intPtr));
                innerInterface = MarshalInspectable<object>.FromAbi(ptr);
                return WrapPanel.FromAbi(intPtr);
            }
            finally
            {
                MarshalInspectable<object>.DisposeMarshaler(value);
                MarshalInspectable<object>.DisposeAbi(ptr);
                WrapPanel.DisposeAbi(intPtr);
            }
        }
    }
}
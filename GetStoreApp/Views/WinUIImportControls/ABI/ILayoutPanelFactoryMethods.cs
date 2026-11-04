using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    internal static class ILayoutPanelFactoryMethods
    {
        public static ref readonly Guid IID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)
                [
                    203, 196, 55, 255, 65, 245, 53, 90, 171, 31,
                    144, 146, 0, 146, 199, 28
                ]));
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.Controls.LayoutPanel CreateInstance(IObjectReference _obj, object baseInterface, out object innerInterface)
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
                return LayoutPanel.FromAbi(intPtr);
            }
            finally
            {
                MarshalInspectable<object>.DisposeMarshaler(value);
                MarshalInspectable<object>.DisposeAbi(ptr);
                LayoutPanel.DisposeAbi(intPtr);
            }
        }
    }
}

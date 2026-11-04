using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// 抑制 IDE1006,IDE0130 警告
#pragma warning disable IDE1006,IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    internal static class IWrapPanelMethods
    {
        public static ref readonly Guid IID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)
                [
                    31, 168, 18, 34, 197, 218, 92, 86, 187, 234,
                    90, 95, 180, 248, 47, 104
                ]));
            }
        }

        internal static unsafe double get_ItemSpacing(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            double result = 0.0;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, double*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 8 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, double*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_ItemSpacing(IObjectReference _obj, double value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, double, int>)(*(nint*)(*(nint*)(void*)thisPtr + 9 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, double, int>))))(thisPtr, value));
        }

        internal static unsafe WrapPanelItemsStretch get_ItemsStretch(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            WrapPanelItemsStretch result = WrapPanelItemsStretch.None;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, WrapPanelItemsStretch*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 14 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, WrapPanelItemsStretch*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_ItemsStretch(IObjectReference _obj, WrapPanelItemsStretch value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, WrapPanelItemsStretch, int>)(*(nint*)(*(nint*)(void*)thisPtr + 15 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, WrapPanelItemsStretch, int>))))(thisPtr, value));
        }

        internal static unsafe double get_LineSpacing(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            double result = 0.0;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, double*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 10 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, double*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_LineSpacing(IObjectReference _obj, double value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, double, int>)(*(nint*)(*(nint*)(void*)thisPtr + 11 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, double, int>))))(thisPtr, value));
        }

        internal static unsafe Orientation get_Orientation(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            Orientation result = Orientation.Vertical;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, Orientation*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 12 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, Orientation*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_Orientation(IObjectReference _obj, Orientation value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, Orientation, int>)(*(nint*)(*(nint*)(void*)thisPtr + 13 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, Orientation, int>))))(thisPtr, value));
        }

        internal static unsafe global::Microsoft.UI.Xaml.Thickness get_Padding(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            global::Microsoft.UI.Xaml.Thickness result = default;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 6 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_Padding(IObjectReference _obj, global::Microsoft.UI.Xaml.Thickness value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness, int>)(*(nint*)(*(nint*)(void*)thisPtr + 7 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness, int>))))(thisPtr, value));
        }
    }
}
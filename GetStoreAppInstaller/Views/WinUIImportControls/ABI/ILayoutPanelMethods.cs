using ABI.Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// 抑制 IDE1006，IDE0130 警告
#pragma warning disable IDE1006,IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    internal static class ILayoutPanelMethods
    {
        public static ref readonly Guid IID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)
                [
                    83, 137, 207, 70, 235, 249, 110, 92, 169, 118,
                    27, 142, 67, 176, 96, 158
                ]));
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.Media.Brush get_BorderBrush(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            nint intPtr = default;
            try
            {
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 8 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint*, int>))))(thisPtr, &intPtr));
                return Brush.FromAbi(intPtr);
            }
            finally
            {
                Brush.DisposeAbi(intPtr);
            }
        }

        internal static unsafe void set_BorderBrush(IObjectReference _obj, global::Microsoft.UI.Xaml.Media.Brush value)
        {
            nint thisPtr = _obj.ThisPtr;
            ObjectReferenceValue value2 = default;
            try
            {
                value2 = Brush.CreateMarshaler2(value);
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint, int>)(*(nint*)(*(nint*)(void*)thisPtr + 9 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint, int>))))(thisPtr, MarshalInspectable<object>.GetAbi(value2)));
            }
            finally
            {
                MarshalInspectable<object>.DisposeMarshaler(value2);
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.Thickness get_BorderThickness(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            global::Microsoft.UI.Xaml.Thickness result = default;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 10 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_BorderThickness(IObjectReference _obj, global::Microsoft.UI.Xaml.Thickness value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness, int>)(*(nint*)(*(nint*)(void*)thisPtr + 11 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness, int>))))(thisPtr, value));
        }

        internal static unsafe global::Microsoft.UI.Xaml.CornerRadius get_CornerRadius(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            global::Microsoft.UI.Xaml.CornerRadius result = default;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.CornerRadius*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 14 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.CornerRadius*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_CornerRadius(IObjectReference _obj, global::Microsoft.UI.Xaml.CornerRadius value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.CornerRadius, int>)(*(nint*)(*(nint*)(void*)thisPtr + 15 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.CornerRadius, int>))))(thisPtr, value));
        }

        internal static unsafe global::Microsoft.UI.Xaml.Controls.Layout get_Layout(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            nint intPtr = default;
            try
            {
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 6 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint*, int>))))(thisPtr, &intPtr));
                return Layout.FromAbi(intPtr);
            }
            finally
            {
                Layout.DisposeAbi(intPtr);
            }
        }

        internal static unsafe void set_Layout(IObjectReference _obj, global::Microsoft.UI.Xaml.Controls.Layout value)
        {
            nint thisPtr = _obj.ThisPtr;
            ObjectReferenceValue value2 = default;
            try
            {
                value2 = Layout.CreateMarshaler2(value);
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint, int>)(*(nint*)(*(nint*)(void*)thisPtr + 7 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint, int>))))(thisPtr, MarshalInspectable<object>.GetAbi(value2)));
            }
            finally
            {
                MarshalInspectable<object>.DisposeMarshaler(value2);
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.Thickness get_Padding(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            global::Microsoft.UI.Xaml.Thickness result = default;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 12 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness*, int>))))(thisPtr, &result));
            return result;
        }

        internal static unsafe void set_Padding(IObjectReference _obj, global::Microsoft.UI.Xaml.Thickness value)
        {
            nint thisPtr = _obj.ThisPtr;
            ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness, int>)(*(nint*)(*(nint*)(void*)thisPtr + 13 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, global::Microsoft.UI.Xaml.Thickness, int>))))(thisPtr, value));
        }
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    internal static class ILayoutPanelStaticsMethods
    {
        public static ref readonly Guid IID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)
                [
                    233, 218, 61, 43, 57, 30, 215, 91, 146, 64,
                    22, 249, 32, 74, 9, 42
                ]));
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_BorderBrushProperty(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            nint intPtr = default;
            try
            {
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 7 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint*, int>))))(thisPtr, &intPtr));
                return DependencyProperty.FromAbi(intPtr);
            }
            finally
            {
                DependencyProperty.DisposeAbi(intPtr);
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_BorderThicknessProperty(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            nint intPtr = default;
            try
            {
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 8 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint*, int>))))(thisPtr, &intPtr));
                return DependencyProperty.FromAbi(intPtr);
            }
            finally
            {
                DependencyProperty.DisposeAbi(intPtr);
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_CornerRadiusProperty(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            nint intPtr = default;
            try
            {
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 10 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint*, int>))))(thisPtr, &intPtr));
                return DependencyProperty.FromAbi(intPtr);
            }
            finally
            {
                DependencyProperty.DisposeAbi(intPtr);
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_LayoutProperty(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            nint intPtr = default;
            try
            {
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 6 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint*, int>))))(thisPtr, &intPtr));
                return DependencyProperty.FromAbi(intPtr);
            }
            finally
            {
                DependencyProperty.DisposeAbi(intPtr);
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_PaddingProperty(IObjectReference _obj)
        {
            nint thisPtr = _obj.ThisPtr;
            nint intPtr = default;
            try
            {
                ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 9 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint*, int>))))(thisPtr, &intPtr));
                return DependencyProperty.FromAbi(intPtr);
            }
            finally
            {
                DependencyProperty.DisposeAbi(intPtr);
            }
        }
    }
}

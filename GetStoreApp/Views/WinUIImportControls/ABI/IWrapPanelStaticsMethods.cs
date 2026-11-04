using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

// 抑制 IDE1006,IDE0130 警告
#pragma warning disable IDE1006,IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    internal static class IWrapPanelStaticsMethods
    {
        public static ref readonly Guid IID
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)
                [
                    93, 208, 228, 224, 81, 106, 134, 85, 160, 212,
                    87, 5, 224, 163, 162, 202
                ]));
            }
        }

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_ItemSpacingProperty(IObjectReference _obj)
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

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_ItemsStretchProperty(IObjectReference _obj)
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

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_LineSpacingProperty(IObjectReference _obj)
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

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_OrientationProperty(IObjectReference _obj)
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

        internal static unsafe global::Microsoft.UI.Xaml.DependencyProperty get_PaddingProperty(IObjectReference _obj)
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
    }
}
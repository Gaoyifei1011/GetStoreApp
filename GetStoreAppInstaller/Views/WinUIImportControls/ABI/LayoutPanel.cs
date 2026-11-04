using System.ComponentModel;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.Microsoft.UI.Xaml.Controls
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct LayoutPanel
    {
        public static IObjectReference CreateMarshaler(global::Microsoft.UI.Xaml.Controls.LayoutPanel obj)
        {
            if (obj is not null)
            {
                return MarshalInspectable<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.CreateMarshaler<IUnknownVftbl>(obj, ILayoutPanelMethods.IID);
            }

            return null;
        }

        public static ObjectReferenceValue CreateMarshaler2(global::Microsoft.UI.Xaml.Controls.LayoutPanel obj)
        {
            return MarshalInspectable<object>.CreateMarshaler2(obj, ILayoutPanelMethods.IID);
        }

        public static nint GetAbi(IObjectReference value)
        {
            if (value is not null)
            {
                return MarshalInterfaceHelper<object>.GetAbi(value);
            }

            return nint.Zero;
        }

        public static global::Microsoft.UI.Xaml.Controls.LayoutPanel FromAbi(nint thisPtr)
        {
            return global::Microsoft.UI.Xaml.Controls.LayoutPanel.FromAbi(thisPtr);
        }

        public static nint FromManaged(global::Microsoft.UI.Xaml.Controls.LayoutPanel obj)
        {
            if (obj is not null)
            {
                return CreateMarshaler2(obj).Detach();
            }

            return nint.Zero;
        }

        public static MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.MarshalerArray CreateMarshalerArray(global::Microsoft.UI.Xaml.Controls.LayoutPanel[] array)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.CreateMarshalerArray2(array, o => CreateMarshaler2(o));
        }

        public static (int length, nint data) GetAbiArray(object box)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.GetAbiArray(box);
        }

        public static global::Microsoft.UI.Xaml.Controls.LayoutPanel[] FromAbiArray(object box)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.FromAbiArray(box, FromAbi);
        }

        public static void CopyAbiArray(global::Microsoft.UI.Xaml.Controls.LayoutPanel[] array, object box)
        {
            MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.CopyAbiArray(array, box, FromAbi);
        }

        public static (int length, nint data) FromManagedArray(global::Microsoft.UI.Xaml.Controls.LayoutPanel[] array)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.FromManagedArray(array, o => FromManaged(o));
        }

        public static void DisposeMarshaler(IObjectReference value)
        {
            MarshalInspectable<object>.DisposeMarshaler(value);
        }

        public static void DisposeMarshalerArray(MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.MarshalerArray array)
        {
            MarshalInterfaceHelper<global::Microsoft.UI.Xaml.Controls.LayoutPanel>.DisposeMarshalerArray(array);
        }

        public static void DisposeAbi(nint abi)
        {
            MarshalInspectable<object>.DisposeAbi(abi);
        }

        public static void DisposeAbiArray(object box)
        {
            MarshalInspectable<object>.DisposeAbiArray(box);
        }
    }
}

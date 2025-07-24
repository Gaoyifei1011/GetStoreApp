using System;
using WinRT;
using WinRT.Interop;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace ABI.Microsoft.UI.Content
{
    public struct ContentExternalBackdropLink
    {
        public static IObjectReference CreateMarshaler(global::Microsoft.UI.Content.ContentExternalBackdropLink obj)
        {
            return obj is null ? null : MarshalInspectable<global::Microsoft.UI.Content.ContentExternalBackdropLink>.CreateMarshaler<IUnknownVftbl>(obj, IContentExternalBackdropLinkMethods.IID);
        }

        public static ObjectReferenceValue CreateMarshaler2(global::Microsoft.UI.Content.ContentExternalBackdropLink obj)
        {
            return MarshalInspectable<object>.CreateMarshaler2(obj, IContentExternalBackdropLinkMethods.IID);
        }

        public static nint GetAbi(IObjectReference value)
        {
            return value is null ? 0 : MarshalInterfaceHelper<object>.GetAbi(value);
        }

        public static global::Microsoft.UI.Content.ContentExternalBackdropLink FromAbi(nint thisPtr)
        {
            return global::Microsoft.UI.Content.ContentExternalBackdropLink.FromAbi(thisPtr);
        }

        public static nint FromManaged(global::Microsoft.UI.Content.ContentExternalBackdropLink obj)
        {
            return obj is null ? 0 : CreateMarshaler2(obj).Detach();
        }

        public static MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.MarshalerArray CreateMarshalerArray(global::Microsoft.UI.Content.ContentExternalBackdropLink[] array)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.CreateMarshalerArray2(array, CreateMarshaler2);
        }

        public static (int Length, nint Data) GetAbiArray(object box)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.GetAbiArray(box);
        }

        public static global::Microsoft.UI.Content.ContentExternalBackdropLink[] FromAbiArray(object box)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.FromAbiArray(box, FromAbi);
        }

        public static void CopyAbiArray(global::Microsoft.UI.Content.ContentExternalBackdropLink[] array, object box)
        {
            MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.CopyAbiArray(array, box, FromAbi);
        }

        public static (int Length, nint Data) FromManagedArray(global::Microsoft.UI.Content.ContentExternalBackdropLink[] array)
        {
            return MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.FromManagedArray(array, FromManaged);
        }

        public static void DisposeMarshaler(IObjectReference value)
        {
            MarshalInspectable<object>.DisposeMarshaler(value);
        }

        public static void DisposeMarshalerArray(MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.MarshalerArray array)
        {
            MarshalInterfaceHelper<global::Microsoft.UI.Content.ContentExternalBackdropLink>.DisposeMarshalerArray(array);
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

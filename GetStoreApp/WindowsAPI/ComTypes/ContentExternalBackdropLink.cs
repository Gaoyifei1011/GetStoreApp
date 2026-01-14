using ABI.System;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using System;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    public partial class ContentExternalBackdropLink : Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop, IDisposable
    {
        private IContentExternalBackdropLink nativeObject;

        public static ContentExternalBackdropLink Create(Compositor compositor)
        {
            using IObjectReference objectReference = ActivationFactory.Get("Microsoft.UI.Content.ContentExternalBackdropLink", typeof(IContentExternalBackdropLinkStatics).GUID);
            IContentExternalBackdropLinkStatics contentExternalBackdropLinkStatics = (IContentExternalBackdropLinkStatics)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(objectReference.ThisPtr, CreateObjectFlags.None);
            contentExternalBackdropLinkStatics.Create((compositor as IWinRTObject).NativeObject.ThisPtr, out IContentExternalBackdropLink contentExternalBackdropLink);
            return new ContentExternalBackdropLink()
            {
                nativeObject = contentExternalBackdropLink
            };
        }

        public DispatcherQueue DispatcherQueue
        {
            get
            {
                nativeObject.get_DispatcherQueue(out nint dispatcherqueue);
                return DispatcherQueue.FromAbi(dispatcherqueue);
            }
        }

        public CompositionBorderMode ExternalBackdropBorderMode
        {
            get
            {
                nativeObject.get_ExternalBackdropBorderMode(out CompositionBorderMode compositionBorderMode);
                return compositionBorderMode;
            }

            set
            {
                nativeObject.set_ExternalBackdropBorderMode(value);
            }
        }

        public Visual PlacementVisual
        {
            get
            {
                nativeObject.get_PlacementVisual(out nint placementVisual);
                return Visual.FromAbi(placementVisual);
            }
        }

        public Windows.UI.Composition.CompositionBrush SystemBackdrop
        {
            get
            {
                return ABI.Microsoft.UI.Composition.ICompositionSupportsSystemBackdropMethods.get_SystemBackdrop(nativeObject.As<IWinRTObject>().NativeObject);
            }

            set
            {
                ABI.Microsoft.UI.Composition.ICompositionSupportsSystemBackdropMethods.set_SystemBackdrop(nativeObject.As<IWinRTObject>().NativeObject, value);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ContentExternalBackdropLink()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && nativeObject is not null)
            {
                IContentExternalBackdropLink obj = nativeObject;
                nativeObject = null;

                ComWrappers.TryGetComInstance(obj, out nint punk);
                if (Marshal.QueryInterface(punk, IDisposableMethods.IID, out nint ptr) == 0)
                {
                    using ObjectReference<IUnknownVftbl> objRef = ObjectReference<IUnknownVftbl>.Attach(ref ptr, IDisposableMethods.IID);
                    IDisposableMethods.Dispose(objRef);
                }
            }
        }
    }
}

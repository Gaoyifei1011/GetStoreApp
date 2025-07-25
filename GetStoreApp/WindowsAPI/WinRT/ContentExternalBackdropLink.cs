﻿using ABI.Microsoft.UI.Content;
using Microsoft.Foundation;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Foundation.Metadata;
using WinRT;
using WinRT.Interop;

// 抑制 CA1822，IDE0060，IDE0130 警告
#pragma warning disable CA1822,IDE0060,IDE0130

namespace Microsoft.UI.Content
{
    [WindowsRuntimeType("Microsoft.UI")]
    [WindowsRuntimeHelperType(typeof(ABI.Microsoft.UI.Content.ContentExternalBackdropLink))]
    [ContentExternalBackdropLinkRcwFactory]
    [ProjectedRuntimeClass(typeof(IContentExternalBackdropLink))]
    [ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
    public sealed class ContentExternalBackdropLink(IObjectReference objRef) : IDisposable, ICompositionSupportsSystemBackdrop, ICustomQueryInterface, IWinRTObject, IEquatable<ContentExternalBackdropLink>
    {
        private static volatile IObjectReference objRefMicrosoftUIContentIContentExternalBackdropLinkStatics;
        private readonly IObjectReference inner = objRef.As(IContentExternalBackdropLinkMethods.IID);
        private volatile IObjectReference objRefSystemIDisposable;
        private volatile IObjectReference objRefMicrosoftUICompositionICompositionSupportsSystemBackdrop;
        private volatile ConcurrentDictionary<RuntimeTypeHandle, IObjectReference> queryInterfaceCache;
        private volatile ConcurrentDictionary<RuntimeTypeHandle, object> additionalTypeData;

        public DispatcherQueue DispatcherQueue
        {
            get => IContentExternalBackdropLinkMethods.get_DispatcherQueue(ObjRefMicrosoftUIContentIContentExternalBackdropLink);
        }

        public CompositionBorderMode ExternalBackdropBorderMode
        {
            get => IContentExternalBackdropLinkMethods.get_ExternalBackdropBorderMode(ObjRefMicrosoftUIContentIContentExternalBackdropLink);
            set => IContentExternalBackdropLinkMethods.set_ExternalBackdropBorderMode(ObjRefMicrosoftUIContentIContentExternalBackdropLink, value);
        }

        public Visual PlacementVisual
        {
            get => IContentExternalBackdropLinkMethods.get_PlacementVisual(ObjRefMicrosoftUIContentIContentExternalBackdropLink);
        }

        public global::Windows.UI.Composition.CompositionBrush SystemBackdrop
        {
            get => ABI.Microsoft.UI.Composition.ICompositionSupportsSystemBackdropMethods.get_SystemBackdrop(ObjRefMicrosoftUICompositionICompositionSupportsSystemBackdrop);
            set => ABI.Microsoft.UI.Composition.ICompositionSupportsSystemBackdropMethods.set_SystemBackdrop(ObjRefMicrosoftUICompositionICompositionSupportsSystemBackdrop, value);
        }

        bool IWinRTObject.HasUnwrappableNativeObject { get => true; }

        IObjectReference IWinRTObject.NativeObject { get => inner; }

        ConcurrentDictionary<RuntimeTypeHandle, IObjectReference> IWinRTObject.QueryInterfaceCache { get => queryInterfaceCache ?? MakeQueryInterfaceCache(); }

        ConcurrentDictionary<RuntimeTypeHandle, object> IWinRTObject.AdditionalTypeData { get => additionalTypeData ?? MakeAdditionalTypeData(); }

        private static IObjectReference ObjRefMicrosoftUIContentIContentExternalBackdropLinkStatics
        {
            get
            {
                IObjectReference factory = objRefMicrosoftUIContentIContentExternalBackdropLinkStatics;
                return factory is not null && factory.IsInCurrentContext ? factory : objRefMicrosoftUIContentIContentExternalBackdropLinkStatics = ActivationFactory.Get("Microsoft.UI.Content.ContentExternalBackdropLink", IContentExternalBackdropLinkStaticsMethods.IID);
            }
        }

        private nint ThisPtr { get => inner?.ThisPtr ?? ((IWinRTObject)this).NativeObject.ThisPtr; }

        private IObjectReference ObjRefMicrosoftUIContentIContentExternalBackdropLink { get => inner; }

        private IObjectReference ObjRefSystemIDisposable => objRefSystemIDisposable ?? MakeObjRefSystemIDisposable();

        private IObjectReference ObjRefMicrosoftUICompositionICompositionSupportsSystemBackdrop => objRefMicrosoftUICompositionICompositionSupportsSystemBackdrop ?? MakeObjRefMicrosoftUICompositionICompositionSupportsSystemBackdrop();

        public static bool operator ==(ContentExternalBackdropLink x, ContentExternalBackdropLink y)
        {
            return (x?.ThisPtr ?? 0) == (y?.ThisPtr ?? 0);
        }

        public static bool operator !=(ContentExternalBackdropLink x, ContentExternalBackdropLink y)
        {
            return !(x == y);
        }

        public static TInterface As<TInterface>()
        {
            return ActivationFactory.Get("Microsoft.UI.Content.ContentExternalBackdropLink").AsInterface<TInterface>();
        }

        public static ContentExternalBackdropLink Create(Compositor compositor)
        {
            return IContentExternalBackdropLinkStaticsMethods.Create(ObjRefMicrosoftUIContentIContentExternalBackdropLinkStatics, compositor);
        }

        public static ContentExternalBackdropLink FromAbi(nint thisPtr)
        {
            return thisPtr is 0 ? default! : MarshalInspectable<ContentExternalBackdropLink>.FromAbi(thisPtr);
        }

        public bool Equals(ContentExternalBackdropLink other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is ContentExternalBackdropLink that && this == that;
        }

        public override int GetHashCode()
        {
            return ThisPtr.GetHashCode();
        }

        public void Dispose() => ABI.System.IDisposableMethods.Dispose(ObjRefSystemIDisposable);

        CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out nint ppv)
        {
            ppv = 0;
            if (IsOverridableInterface(iid) || IID.IID_IInspectable == iid)
            {
                return CustomQueryInterfaceResult.NotHandled;
            }

            if (((IWinRTObject)this).NativeObject.TryAs(iid, out ppv) >= 0)
            {
                return CustomQueryInterfaceResult.Handled;
            }

            return CustomQueryInterfaceResult.NotHandled;
        }

        private IObjectReference MakeObjRefSystemIDisposable()
        {
            Interlocked.CompareExchange(ref objRefSystemIDisposable, ((IWinRTObject)this).NativeObject.As<IUnknownVftbl>(ABI.System.IDisposableMethods.IID), null);
            return objRefSystemIDisposable;
        }

        private IObjectReference MakeObjRefMicrosoftUICompositionICompositionSupportsSystemBackdrop()
        {
            Interlocked.CompareExchange(ref objRefMicrosoftUICompositionICompositionSupportsSystemBackdrop, ((IWinRTObject)this).NativeObject.As<IUnknownVftbl>(ABI.Microsoft.UI.Composition.ICompositionSupportsSystemBackdropMethods.IID), null);
            return objRefMicrosoftUICompositionICompositionSupportsSystemBackdrop;
        }

        private ConcurrentDictionary<RuntimeTypeHandle, IObjectReference> MakeQueryInterfaceCache()
        {
            Interlocked.CompareExchange(ref queryInterfaceCache, new(), null);
            return queryInterfaceCache;
        }

        private ConcurrentDictionary<RuntimeTypeHandle, object> MakeAdditionalTypeData()
        {
            Interlocked.CompareExchange(ref additionalTypeData, new(), null);
            return additionalTypeData;
        }

        private bool IsOverridableInterface(Guid iid)
        {
            return false;
        }

        private struct InterfaceTag<TInterface>;
    }
}

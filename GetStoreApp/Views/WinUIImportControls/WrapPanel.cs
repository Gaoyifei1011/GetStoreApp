using System;
using System.Runtime.InteropServices;
using System.Threading;
using ABI.Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.CustomAttributes;
using Windows.Foundation.Metadata;
using WinRT;
using WinRT.Interop;

// 抑制 IDE0130 警告
#pragma warning disable IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    [WindowsRuntimeType("Microsoft.UI.Xaml")]
    [WindowsRuntimeHelperType(typeof(ABI.Microsoft.UI.Xaml.Controls.WrapPanel))]
    [WrapPanelRcwFactory]
    [ProjectedRuntimeClass(typeof(IWrapPanel))]
    [MUXPropertyChangedCallback(enable = true)]
    [MUXPropertyChangedCallbackMethodName(value = "OnPropertyChanged")]
    [ContractVersion(typeof(XamlContract), 720896u)]
    public class WrapPanel : Panel, ICustomQueryInterface, IWinRTObject, IDynamicInterfaceCastable, IEquatable<WrapPanel>
    {
        private static class _IWrapPanelFactoryMethods
        {
            public static unsafe nint CreateInstance(IObjectReference _obj, object baseInterface, out nint innerInterface)
            {
                nint thisPtr = _obj.ThisPtr;
                ObjectReferenceValue value = default;
                nint intPtr = default;
                nint result = default;
                try
                {
                    value = MarshalInspectable<object>.CreateMarshaler2(baseInterface);
                    ExceptionHelpers.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint, nint*, nint*, int>)(*(nint*)(*(nint*)(void*)thisPtr + 6 * (nint)sizeof(delegate* unmanaged[Stdcall]<nint, nint, nint*, nint*, int>))))(thisPtr, MarshalInspectable<object>.GetAbi(value), &intPtr, &result));
                    innerInterface = intPtr;
                    return result;
                }
                finally
                {
                    MarshalInspectable<object>.DisposeMarshaler(value);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        private struct InterfaceTag<I>
        {
        }

        private readonly IObjectReference _inner;

        private volatile IObjectReference ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel;

        private static volatile IObjectReference ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory;

        private static volatile IObjectReference ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics;

        private nint ThisPtr
        {
            get
            {
                if (_inner != null)
                {
                    return _inner.ThisPtr;
                }

                return ((IWinRTObject)this).NativeObject.ThisPtr;
            }
        }

        private IObjectReference _objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel => ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel ?? Make___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel();

        private static IObjectReference _objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory
        {
            get
            {
                IObjectReference __objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory = ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory;
                if (__objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory != null && __objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory.IsInCurrentContext)
                {
                    return __objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory;
                }

                return ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory = ActivationFactory.Get("Microsoft.UI.Xaml.Controls.WrapPanel", IWrapPanelFactoryMethods.IID);
            }
        }

        private static IObjectReference _objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics
        {
            get
            {
                IObjectReference __objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics = ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics;
                if (__objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics != null && __objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics.IsInCurrentContext)
                {
                    return __objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics;
                }

                return ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics = ActivationFactory.Get("Microsoft.UI.Xaml.Controls.WrapPanel", IWrapPanelStaticsMethods.IID);
            }
        }

        public static DependencyProperty ItemSpacingProperty => IWrapPanelStaticsMethods.get_ItemSpacingProperty(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics);

        public static DependencyProperty ItemsStretchProperty => IWrapPanelStaticsMethods.get_ItemsStretchProperty(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics);

        public static DependencyProperty LineSpacingProperty => IWrapPanelStaticsMethods.get_LineSpacingProperty(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics);

        public static DependencyProperty OrientationProperty => IWrapPanelStaticsMethods.get_OrientationProperty(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics);

        public static DependencyProperty PaddingProperty => IWrapPanelStaticsMethods.get_PaddingProperty(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelStatics);

        bool IWinRTObject.HasUnwrappableNativeObject => GetType() == typeof(WrapPanel);

        IObjectReference IWinRTObject.NativeObject => _inner;

        public double ItemSpacing
        {
            get
            {
                return IWrapPanelMethods.get_ItemSpacing(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel);
            }
            set
            {
                IWrapPanelMethods.set_ItemSpacing(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel, value);
            }
        }

        public WrapPanelItemsStretch ItemsStretch
        {
            get
            {
                return IWrapPanelMethods.get_ItemsStretch(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel);
            }
            set
            {
                IWrapPanelMethods.set_ItemsStretch(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel, value);
            }
        }

        public double LineSpacing
        {
            get
            {
                return IWrapPanelMethods.get_LineSpacing(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel);
            }
            set
            {
                IWrapPanelMethods.set_LineSpacing(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel, value);
            }
        }

        public Orientation Orientation
        {
            get
            {
                return IWrapPanelMethods.get_Orientation(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel);
            }
            set
            {
                IWrapPanelMethods.set_Orientation(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel, value);
            }
        }

        public Thickness Padding
        {
            get
            {
                return IWrapPanelMethods.get_Padding(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel);
            }
            set
            {
                IWrapPanelMethods.set_Padding(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel, value);
            }
        }

        private IObjectReference Make___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel()
        {
            Interlocked.CompareExchange(ref ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel, ((IWinRTObject)this).NativeObject.As<IUnknownVftbl>(IWrapPanelMethods.IID), null);
            return ___objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanel;
        }

        public WrapPanel()
            : base(DerivedComposed.Instance)
        {
            bool flag = GetType() != typeof(WrapPanel);
            nint newInstance = _IWrapPanelFactoryMethods.CreateInstance(_objRef_global__Microsoft_UI_Xaml_Controls_IWrapPanelFactory, flag ? this : null, out nint innerInterface);
            try
            {
                ComWrappersHelper.Init(flag, this, newInstance, innerInterface, IWrapPanelMethods.IID, out _inner);
            }
            finally
            {
                Marshal.Release(innerInterface);
            }
        }

        public new static I As<I>()
        {
            return ActivationFactory.Get("Microsoft.UI.Xaml.Controls.WrapPanel").AsInterface<I>();
        }

        public new static WrapPanel FromAbi(nint thisPtr)
        {
            if (thisPtr == nint.Zero)
            {
                return null;
            }

            return MarshalInspectable<WrapPanel>.FromAbi(thisPtr);
        }

        protected internal WrapPanel(IObjectReference objRef) : base(DerivedComposed.Instance)
        {
            _inner = objRef.As(IWrapPanelMethods.IID);
        }

        public static bool operator ==(WrapPanel x, WrapPanel y)
        {
            return (x?.ThisPtr ?? nint.Zero) == (y?.ThisPtr ?? nint.Zero);
        }

        public static bool operator !=(WrapPanel x, WrapPanel y)
        {
            return !(x == y);
        }

        public bool Equals(WrapPanel other)
        {
            return this == other;
        }

        [DynamicWindowsRuntimeCast(typeof(WrapPanel))]
        public override bool Equals(object obj)
        {
            if (obj is WrapPanel wrapPanel)
            {
                return this == wrapPanel;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ThisPtr.GetHashCode();
        }

        protected WrapPanel(DerivedComposed _) : base(_)
        {
        }

        protected override bool IsOverridableInterface(Guid iid)
        {
            return base.IsOverridableInterface(iid);
        }

        CustomQueryInterfaceResult ICustomQueryInterface.GetInterface(ref Guid iid, out nint ppv)
        {
            ppv = nint.Zero;
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
    }
}

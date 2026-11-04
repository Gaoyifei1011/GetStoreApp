using ABI.Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.CustomAttributes;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Foundation.Metadata;
using WinRT;
using WinRT.Interop;

// 抑制 IDE1006，IDE0130 警告
#pragma warning disable IDE1006,IDE0130

namespace Microsoft.UI.Xaml.Controls
{
    [WindowsRuntimeType("Microsoft.UI.Xaml")]
    [WindowsRuntimeHelperType(typeof(ABI.Microsoft.UI.Xaml.Controls.LayoutPanel))]
    [LayoutPanelRcwFactory]
    [ProjectedRuntimeClass(typeof(ILayoutPanel))]
    [MUXPropertyChangedCallback(enable = true)]
    [MUXPropertyChangedCallbackMethodName(value = "OnPropertyChanged")]
    [ContractVersion(typeof(XamlContract), 720896u)]
    public class LayoutPanel : Panel, ICustomQueryInterface, IWinRTObject, IDynamicInterfaceCastable, IEquatable<LayoutPanel>
    {
        private static class _ILayoutPanelFactoryMethods
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

        private volatile IObjectReference ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel;

        private static volatile IObjectReference ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory;

        private static volatile IObjectReference ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics;

        private nint ThisPtr
        {
            get
            {
                if (_inner is not null)
                {
                    return _inner.ThisPtr;
                }

                return ((IWinRTObject)this).NativeObject.ThisPtr;
            }
        }

        private IObjectReference _objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel => ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel ?? Make___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel();

        private static IObjectReference _objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory
        {
            get
            {
                IObjectReference __objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory = ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory;
                if (__objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory is not null && __objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory.IsInCurrentContext)
                {
                    return __objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory;
                }

                return ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory = ActivationFactory.Get("Microsoft.UI.Xaml.Controls.LayoutPanel", ILayoutPanelFactoryMethods.IID);
            }
        }

        private static IObjectReference _objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics
        {
            get
            {
                IObjectReference __objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics = ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics;
                if (__objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics is not null && __objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics.IsInCurrentContext)
                {
                    return __objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics;
                }

                return ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics = ActivationFactory.Get("Microsoft.UI.Xaml.Controls.LayoutPanel", ILayoutPanelStaticsMethods.IID);
            }
        }

        public static DependencyProperty BorderBrushProperty => ILayoutPanelStaticsMethods.get_BorderBrushProperty(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics);

        public static DependencyProperty BorderThicknessProperty => ILayoutPanelStaticsMethods.get_BorderThicknessProperty(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics);

        public static DependencyProperty CornerRadiusProperty => ILayoutPanelStaticsMethods.get_CornerRadiusProperty(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics);

        public static DependencyProperty LayoutProperty => ILayoutPanelStaticsMethods.get_LayoutProperty(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics);

        public static DependencyProperty PaddingProperty => ILayoutPanelStaticsMethods.get_PaddingProperty(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelStatics);

        bool IWinRTObject.HasUnwrappableNativeObject => GetType() == typeof(LayoutPanel);

        IObjectReference IWinRTObject.NativeObject => _inner;

        public Brush BorderBrush
        {
            get
            {
                return ILayoutPanelMethods.get_BorderBrush(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel);
            }
            set
            {
                ILayoutPanelMethods.set_BorderBrush(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel, value);
            }
        }

        public Thickness BorderThickness
        {
            get
            {
                return ILayoutPanelMethods.get_BorderThickness(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel);
            }
            set
            {
                ILayoutPanelMethods.set_BorderThickness(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel, value);
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return ILayoutPanelMethods.get_CornerRadius(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel);
            }
            set
            {
                ILayoutPanelMethods.set_CornerRadius(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel, value);
            }
        }

        public Layout Layout
        {
            get
            {
                return ILayoutPanelMethods.get_Layout(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel);
            }
            set
            {
                ILayoutPanelMethods.set_Layout(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel, value);
            }
        }

        public Thickness Padding
        {
            get
            {
                return ILayoutPanelMethods.get_Padding(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel);
            }
            set
            {
                ILayoutPanelMethods.set_Padding(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel, value);
            }
        }

        private IObjectReference Make___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel()
        {
            Interlocked.CompareExchange(ref ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel, ((IWinRTObject)this).NativeObject.As<IUnknownVftbl>(ILayoutPanelMethods.IID), null);
            return ___objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanel;
        }

        public LayoutPanel() : base(DerivedComposed.Instance)
        {
            bool flag = GetType() != typeof(LayoutPanel);
            nint newInstance = _ILayoutPanelFactoryMethods.CreateInstance(_objRef_global__Microsoft_UI_Xaml_Controls_ILayoutPanelFactory, flag ? this : null, out nint innerInterface);
            try
            {
                ComWrappersHelper.Init(flag, this, newInstance, innerInterface, ILayoutPanelMethods.IID, out _inner);
            }
            finally
            {
                Marshal.Release(innerInterface);
            }
        }

        public new static I As<I>()
        {
            return ActivationFactory.Get("Microsoft.UI.Xaml.Controls.LayoutPanel").AsInterface<I>();
        }

        public new static LayoutPanel FromAbi(nint thisPtr)
        {
            if (thisPtr == nint.Zero)
            {
                return null;
            }

            return MarshalInspectable<LayoutPanel>.FromAbi(thisPtr);
        }

        protected internal LayoutPanel(IObjectReference objRef) : base(DerivedComposed.Instance)
        {
            _inner = objRef.As(ILayoutPanelMethods.IID);
        }

        public static bool operator ==(LayoutPanel x, LayoutPanel y)
        {
            return (x?.ThisPtr ?? nint.Zero) == (y?.ThisPtr ?? nint.Zero);
        }

        public static bool operator !=(LayoutPanel x, LayoutPanel y)
        {
            return !(x == y);
        }

        public bool Equals(LayoutPanel other)
        {
            return this == other;
        }

        [DynamicWindowsRuntimeCast(typeof(LayoutPanel))]
        public override bool Equals(object obj)
        {
            if (obj is LayoutPanel layoutPanel)
            {
                return this == layoutPanel;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ThisPtr.GetHashCode();
        }

        protected LayoutPanel(DerivedComposed _) : base(_)
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

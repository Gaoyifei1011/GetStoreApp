using GetStoreAppWebView.Helpers.Backdrop;
using GetStoreAppWebView.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics.Effects;

namespace GetStoreAppWebView.UI.Backdrop
{
    [Guid("811D79A4-DE28-4454-8094-C64685F8BD4C")]
    public class OpacityEffect : IGraphicsEffect, IGraphicsEffectSource, IGraphicsEffectD2D1Interop
    {
        private Guid clsid = new("811D79A4-DE28-4454-8094-C64685F8BD4C");

        public D2D1_BUFFER_PRECISION BufferPrecision { get; set; }

        public bool CacheOutput { get; set; }

        public float Opacity { get; set; } = 1.0f;

        public IGraphicsEffectSource Source { get; set; }

        private string _name = string.Empty;

        public string Name
        {
            get { return _name; }

            set { _name = value; }
        }

        public int GetEffectId(out Guid id)
        {
            id = clsid;
            return 0;
        }

        public static bool IsSupported
        {
            get { return true; }
        }

        public int GetNamedPropertyMapping(string name, out uint index, out GRAPHICS_EFFECT_PROPERTY_MAPPING mapping)
        {
            switch (name)
            {
                case nameof(Opacity):
                    {
                        index = 0;
                        mapping = GRAPHICS_EFFECT_PROPERTY_MAPPING.GRAPHICS_EFFECT_PROPERTY_MAPPING_DIRECT;
                        break;
                    }
                default:
                    {
                        index = 0xFF;
                        mapping = (GRAPHICS_EFFECT_PROPERTY_MAPPING)0xFF;
                        break;
                    }
            }

            return 0;
        }

        public int GetProperty(uint index, out IntPtr source)
        {
            if (index is 0)
            {
                BackdropHelper.PropertyValueStatics.Value.CreateSingle(Opacity, out IntPtr ptr);
                if (ptr != IntPtr.Zero)
                {
                    source = ptr;
                    return 0;
                }
            }

            source = IntPtr.Zero;
            return -2147483637;
        }

        public int GetPropertyCount(out uint count)
        {
            count = 1;
            return 0;
        }

        public int GetSource(uint index, out IntPtr source)
        {
            source = Marshal.GetComInterfaceForObject(Source, typeof(IGraphicsEffectSource));
            return 0;
        }

        public int GetSourceCount(out uint count)
        {
            count = 1;
            return 0;
        }
    }
}

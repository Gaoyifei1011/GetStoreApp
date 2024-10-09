using GetStoreAppWebView.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Graphics.Effects;

namespace GetStoreAppWebView.UI.Backdrop
{
    [Guid("811D79A4-DE28-4454-8094-C64685F8BD4C")]
    public sealed partial class OpacityEffect : IGraphicsEffect, IGraphicsEffectSource, IGraphicsEffectD2D1Interop
    {
        private readonly IPropertyValueStatics propertyValue = PropertyValue.As<IPropertyValueStatics>();

        public string Name { get; set; } = string.Empty;

        public float Opacity { get; set; } = 1.0f;

        public IGraphicsEffectSource Source { get; set; }

        public int GetEffectId(out Guid id)
        {
            id = typeof(OpacityEffect).GUID;
            return 0;
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
                propertyValue.CreateSingle(Opacity, out IntPtr ptr);
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

        public int GetSource(uint index, out IGraphicsEffectSource source)
        {
            source = Source;
            return 0;
        }

        public int GetSourceCount(out uint count)
        {
            count = 1;
            return 0;
        }
    }
}

using GetStoreAppInstaller.WindowsAPI.ComTypes;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Graphics.Effects;

namespace GetStoreAppInstaller.UI.Backdrop
{
    [Guid("1FEB6D69-2FE6-4AC9-8C58-1D7F93E7A6A5")]
    public sealed partial class GaussianBlurEffect : IGraphicsEffect, IGraphicsEffectSource, IGraphicsEffectD2D1Interop
    {
        private readonly IPropertyValueStatics propertyValue = PropertyValue.As<IPropertyValueStatics>();

        public string Name { get; set; } = string.Empty;

        public float BlurAmount { get; set; } = 3.0f;

        public EffectOptimization Optimization { get; set; } = EffectOptimization.Balanced;

        public EffectBorderMode BorderMode { get; set; } = EffectBorderMode.Soft;

        public IGraphicsEffectSource Source { get; set; }

        public int GetEffectId(out Guid id)
        {
            id = typeof(GaussianBlurEffect).GUID;
            return 0;
        }

        public int GetNamedPropertyMapping(string name, out uint index, out GRAPHICS_EFFECT_PROPERTY_MAPPING mapping)
        {
            switch (name)
            {
                case nameof(BlurAmount):
                    {
                        index = 0;
                        mapping = GRAPHICS_EFFECT_PROPERTY_MAPPING.GRAPHICS_EFFECT_PROPERTY_MAPPING_DIRECT;
                        break;
                    }
                case nameof(Optimization):
                    {
                        index = 1;
                        mapping = GRAPHICS_EFFECT_PROPERTY_MAPPING.GRAPHICS_EFFECT_PROPERTY_MAPPING_DIRECT;
                        break;
                    }
                case nameof(BorderMode):
                    {
                        index = 2;
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
                propertyValue.CreateSingle(BlurAmount, out IntPtr ptr);

                if (ptr != IntPtr.Zero)
                {
                    source = ptr;
                    return 0;
                }
            }
            else if (index is 1)
            {
                propertyValue.CreateUInt32((uint)Optimization, out IntPtr ptr);

                if (ptr != IntPtr.Zero)
                {
                    source = ptr;
                    return 0;
                }
            }
            else if (index is 2)
            {
                propertyValue.CreateUInt32((uint)BorderMode, out IntPtr ptr);

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
            count = 3;
            return 0;
        }

        public int GetSource(uint index, out IGraphicsEffectSource source)
        {
            if (index is 0)
            {
                source = Source;
                return 0;
            }
            else
            {
                source = null;
                return 2147483637;
            }
        }

        public int GetSourceCount(out uint count)
        {
            count = 1;
            return 0;
        }
    }
}

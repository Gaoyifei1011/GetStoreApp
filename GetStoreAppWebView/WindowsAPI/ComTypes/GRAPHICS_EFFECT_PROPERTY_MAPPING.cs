namespace GetStoreAppWebView.WindowsAPI.ComTypes
{
    /// <summary>
    /// 指示强类型效果属性如何映射到基础 Direct2D 效果属性。 此枚举支持 Windows.UI.Composition API，不应直接在代码中使用。
    /// </summary>
    public enum GRAPHICS_EFFECT_PROPERTY_MAPPING
    {
        /// <summary>
        /// 指定值不能映射到 Direct2D 效果属性。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_UNKNOWN = 0,

        /// <summary>
        /// 指定可以在 Direct2D 效果属性上按原样设置值。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_DIRECT = 1,

        /// <summary>
        /// 指定该值映射到矢量类型 Direct2D 效果属性的 X 分量。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORX = 2,

        /// <summary>
        /// 指定该值映射到矢量类型 Direct2D 效果属性的 Y 分量。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORY = 3,

        /// <summary>
        /// 指定该值映射到矢量类型 Direct2D 效果属性的 Z 分量。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORZ = 4,

        /// <summary>
        /// 指定值映射到矢量类型 Direct2D 效果属性的 W 分量。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORW = 5,

        /// <summary>
        /// 指定 rect 值映射到 Vector4 Direct2D 效果属性。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_RECT_TO_VECTOR4 = 6,

        /// <summary>
        /// 指定在对 Direct2D 效果属性进行设置之前，需要将值从弧度转换为度。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_RADIANS_TO_DEGREES = 7,

        /// <summary>
        /// 指定在对效果属性进行设置之前，需要将颜色矩阵 alpha 模式枚举值转换为等效的 Direct2D 枚举值。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_COLORMATRIX_ALPHA_MODE = 8,

        /// <summary>
        /// 指定在 Direct2D 效果属性上设置之前，需要将 Windows.UI.Color 值转换为 RGB Vector3。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_COLOR_TO_VECTOR3 = 9,

        /// <summary>
        /// 指定在 Direct2D 效果属性上设置之前，需要将 Windows.UI.Color 值转换为 RGBA Vector4。
        /// </summary>
        GRAPHICS_EFFECT_PROPERTY_MAPPING_COLOR_TO_VECTOR4 = 10
    }
}

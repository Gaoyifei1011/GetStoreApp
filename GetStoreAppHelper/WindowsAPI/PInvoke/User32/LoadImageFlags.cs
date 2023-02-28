using System;

namespace GetStoreAppHelper.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 表示各种图像类型。
    /// </summary>
    [Flags]
    public enum LoadImageFlags : uint
    {
        /// <summary>
        /// 默认标志;它不执行任何作用。
        /// </summary>
        LR_DEFAULTCOLOR = 0x0,

        /// <summary>
        /// 加载黑白图像。
        /// </summary>
        LR_MONOCHROME = 0x00000001,

        /// <summary>
        /// 从 lpszName 指定的文件中加载独立图像， (图标、游标或位图文件) 。
        /// </summary>
        LR_LOADFROMFILE = 0x00000010,

        /// <summary>
        /// 检索图像中第一个像素的颜色值，并将颜色表中的相应条目替换为默认窗口颜色 (COLOR_WINDOW) 。 使用该条目的图像中的所有像素都将成为默认窗口颜色。
        /// 此值仅适用于具有相应颜色表的图像。
        /// 如果要加载颜色深度大于 8bpp 的位图，请不要使用此选项。
        /// 如果 fuLoad 同时包括 <see cref="LR_LOADTRANSPARENT"> 和 <see cref="LR_LOADMAP3DCOLORS"> 值， <see cref="LR_LOADTRANSPARENT"> 优先。 但是，颜色表项替换为 COLOR_3DFACE 而不是 COLOR_WINDOW。
        /// </summary>
        LR_LOADTRANSPARENT = 0x00000020,

        /// <summary>
        /// 如果 cxDesired 或 cyDesired 值设置为零，则使用由光标或图标的系统指标值指定的宽度或高度。
        /// 如果未指定此标志， 并且 cxDesired 和 cyDesired 设置为零，则函数将使用实际资源大小。 如果资源包含多个图像，则函数使用第一个图像的大小。
        /// </summary>
        LR_DEFAULTSIZE = 0x00000040,

        /// <summary>
        /// 使用真正的 VGA 颜色。
        /// </summary>
        LR_VGACOLOR = 0x00000080,

        /// <summary>
        /// 在颜色表中搜索图像，并将以下灰色阴影替换为相应的三维颜色。
        /// <list type="bullet">
        /// <item>Dk Gray，RGB (128，128，128) 与 COLOR_3DSHADOW</item>
        /// <item>Gray, RGB(192，192，192)  与 COLOR_3DFACE</item>
        /// <item>Lt Gray, RGB (223，223，223 ) 与 COLOR_3DLIGHT</item>
        /// </list>
        /// 如果要加载颜色深度大于 8bpp 的位图，请不要使用此选项。
        /// </summary>
        LR_LOADMAP3DCOLORS = 0x00001000,

        /// <summary>
        /// 当 uType 参数指定 <see cref="ImageType.IMAGE_BITMAP"> 时，会导致函数返回 DIB 节位图而不是兼容的位图。 此标志可用于加载位图而不将其映射到显示设备的颜色。
        /// </summary>
        LR_CREATEDIBSECTION = 0x00002000,

        /// <summary>
        /// 如果多次加载映像，则共享映像句柄。 如果未设置 <see cref="LR_SHARED"> ，则对同一资源的 <see cref="User32Library.LoadImage"> 的第二次调用将再次加载映像并返回不同的句柄。
        /// 使用此标志时，系统会在不再需要资源时销毁资源。
        /// 不要对具有非标准大小的图像使用 <see cref="LR_SHARED"> ，这些图像在加载后可能会更改，或者从文件加载。
        /// 加载系统图标或游标时，必须使用 <see cref="LR_SHARED"> ，否则函数将无法加载资源。
        /// 无论请求的大小如何，此函数都会找到缓存中请求的资源名称的第一个映像。
        /// </summary>
        LR_SHARED = 0x00008000,
    }
}

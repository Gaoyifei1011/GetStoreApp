namespace GetStoreAppWebView.WindowsAPI.ComTypes
{
    /// <summary>
    /// 表示 Direct2D 中图像处理管道的位深度。
    /// </summary>
    public enum D2D1_BUFFER_PRECISION : uint
    {
        /// <summary>
        /// 未指定缓冲区精度。
        /// </summary>
        D2D1_BUFFER_PRECISION_UNKNOWN = 0,

        /// <summary>
        /// 每个通道使用 8 位规范化整数。
        /// </summary>
        D2D1_BUFFER_PRECISION_8BPC_UNORM = 1,

        /// <summary>
        /// 每个通道使用 8 位规范化整数标准 RGB 数据。
        /// </summary>
        D2D1_BUFFER_PRECISION_8BPC_UNORM_SRGB = 2,

        /// <summary>
        /// 每个通道使用 16 位规范化整数。
        /// </summary>
        D2D1_BUFFER_PRECISION_16BPC_UNORM = 3,

        /// <summary>
        /// 每个通道使用 16 位浮点数。
        /// </summary>
        D2D1_BUFFER_PRECISION_16BPC_FLOAT = 4,

        /// <summary>
        /// 每个通道使用 32 位浮点数。
        /// </summary>
        D2D1_BUFFER_PRECISION_32BPC_FLOAT = 5,

        /// <summary>
        /// 强制此枚举编译为大小为 32 位。 如果没有此值，某些编译器将允许此枚举编译为 32 位以外的大小。
        /// 请勿使用此值。
        /// </summary>
        D2D1_BUFFER_PRECISION_FORCE_DWORD = 0xffffffff
    }
}

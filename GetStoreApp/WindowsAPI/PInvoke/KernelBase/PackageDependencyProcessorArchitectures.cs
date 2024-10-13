using System;

namespace GetStoreApp.WindowsAPI.PInvoke.KernelBase
{
    /// <summary>
    /// 为使用 TryCreatePackageDependency 函数创建的框架包依赖项定义处理器体系结构。
    /// </summary>
    [Flags]
    public enum PackageDependencyProcessorArchitectures
    {
        /// <summary>
        /// 未指定处理器体系结构。
        /// </summary>
        PackageDependencyProcessorArchitectures_None = 0x0000,

        /// <summary>
        /// 指定非特定体系结构。
        /// </summary>
        PackageDependencyProcessorArchitectures_Neutral = 0x0001,

        /// <summary>
        /// 指定 x86 体系结构。
        /// </summary>
        PackageDependencyProcessorArchitectures_X86 = 0x0002,

        /// <summary>
        /// 指定 x64 体系结构。
        /// </summary>
        PackageDependencyProcessorArchitectures_X64 = 0x0004,

        /// <summary>
        /// 指定 ARM 体系结构。
        /// </summary>
        PackageDependencyProcessorArchitectures_Arm = 0x0008,

        /// <summary>
        /// 指定 ARM64 体系结构。
        /// </summary>
        PackageDependencyProcessorArchitectures_Arm64 = 0x0010,

        /// <summary>
        /// 指定 x86/A64 体系结构。
        /// </summary>
        PackageDependencyProcessorArchitectures_X86A64 = 0x0020,
    }
}

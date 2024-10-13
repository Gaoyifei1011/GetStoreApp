using System;

namespace GetStoreApp.WindowsAPI.PInvoke.KernelBase
{
    /// <summary>
    /// 定义在使用 AddPackageDependency 函数添加对框架包的运行时引用时可以应用的选项。
    /// </summary>
    [Flags]
    public enum AddPackageDependencyOptions
    {
        /// <summary>
        /// 未应用任何选项。
        /// </summary>
        AddPackageDependencyOptions_None = 0x00000000,

        /// <summary>
        /// 如果包图中存在多个包，其排名与调用 AddPackageDependency 相同，则解析的包将先于排名相同的其他包。
        /// </summary>
        AddPackageDependencyOptions_PrependIfRankCollision = 0x00000001,
    }
}

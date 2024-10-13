using System;

namespace GetStoreApp.WindowsAPI.PInvoke.KernelBase
{
    /// <summary>
    /// 定义在使用 TryCreatePackageDependency 函数创建包依赖项时可以应用的选项。
    /// </summary>
    [Flags]
    public enum CreatePackageDependencyOptions
    {
        /// <summary>
        /// 未应用任何选项。
        /// </summary>
        CreatePackageDependencyOptions_None = 0x00000000,

        /// <summary>
        /// 在固定包依赖项时禁用依赖项解析。 这对于作为目标用户以外的用户上下文运行的安装程序非常有用， (例如，作为 LocalSystem) 运行的安装程序。
        /// </summary>
        CreatePackageDependencyOptions_DoNotVerifyDependencyResolution = 0x00000001,

        /// <summary>
        /// 定义系统的包依赖项，默认情况下 (所有用户均可访问，包依赖项是为特定用户) 定义的。 此选项要求调用方具有管理权限。
        /// </summary>
        CreatePackageDependencyOptions_ScopeIsSystem = 0x00000002,
    }
}

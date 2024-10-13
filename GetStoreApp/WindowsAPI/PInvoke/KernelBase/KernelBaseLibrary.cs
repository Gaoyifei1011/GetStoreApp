using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;

namespace GetStoreApp.WindowsAPI.PInvoke.KernelBase
{
    /// <summary>
    /// KernelBase.dll 函数库
    /// </summary>
    public static partial class KernelBaseLibrary
    {
        private const string KernelBase = "kernelBase.dll";

        /// <summary>
        /// 使用 TryCreatePackageDependency 方法添加前面创建的框架包依赖项的运行时引用和指定选项。 此方法成功返回后，应用可以激活类型并使用框架包中的内容。
        /// </summary>
        /// <param name="packageDependencyId">要解析并添加到调用进程的包图的包依赖项的 ID。 此参数必须与通过使用 TryCreatePackageDependency 函数通过) CreatePackageDependencyOptions_ScopeIsSystem 选项对调用用户或系统 (定义的 包 依赖项匹配，否则返回错误。</param>
        /// <param name="rank">用于将解析的包添加到调用方包图的排名。</param>
        /// <param name="options">添加包依赖项时要应用的选项。</param>
        /// <param name="packageDependencyContext">添加的包依赖项的句柄。 此句柄在传递到 RemovePackageDependency 之前有效。</param>
        /// <param name="packageFullName">此方法返回时，包含指向以 null 结尾的 Unicode 字符串的指针的地址，该字符串指定已解析依赖项的包的全名。 调用 HeapFree 不再需要此资源后，调用方负责释放此资源。</param>
        /// <returns>如果函数成功，则返回 ERROR_SUCCESS。 否则，函数将返回错误代码。</returns>
        [LibraryImport(KernelBase, EntryPoint = "AddPackageDependency", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial int AddPackageDependency([MarshalAs(UnmanagedType.LPWStr)] string packageDependencyId, int rank, AddPackageDependencyOptions options, out IntPtr packageDependencyContext, [MarshalAs(UnmanagedType.LPWStr)] out string packageFullName);

        /// <summary>
        /// 使用指定的包系列名称、最低版本和其他条件，为当前应用的框架包依赖项创建安装时引用。
        /// </summary>
        /// <param name="user">包依赖项的用户范围。 如果为 NULL，则使用调用方的用户上下文。 如果指定 了CreatePackageDependencyOptions_ScopeIsSystem ，则必须为 NULL。</param>
        /// <param name="packageFamilyName">要依赖的框架包的包系列名称。</param>
        /// <param name="minVersion">要对其具有依赖项的框架包的最低版本。</param>
        /// <param name="packageDependencyProcessorArchitectures">包依赖项的处理器体系结构。</param>
        /// <param name="lifetimeKind">用于定义包依赖项生存期的项目类型。</param>
        /// <param name="lifetimeArtifact">用于定义包依赖项生存期的项目的名称。 如果PackageDependencyLifetimeKind_Process lifetimeKind 参数，则必须为 NULL。 有关详细信息，请参阅备注。</param>
        /// <param name="options">创建包依赖项时要应用的选项。</param>
        /// <param name="packageDependencyId">此方法返回时，包含指向以 null 结尾的 Unicode 字符串的指针的地址，该字符串指定新包依赖项的 ID。 调用 HeapFree 不再需要此资源后，调用方负责释放此资源。</param>
        /// <returns>如果该函数成功，则返回 ERROR_SUCCESS。 否则，该函数将返回错误代码。</returns>
        [LibraryImport(KernelBase, EntryPoint = "TryCreatePackageDependency", SetLastError = true, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial int TryCreatePackageDependency(IntPtr user, [MarshalAs(UnmanagedType.LPWStr)] string packageFamilyName, PackageVersion minVersion, PackageDependencyProcessorArchitectures packageDependencyProcessorArchitectures, PackageDependencyLifetimeKind lifetimeKind, [MarshalAs(UnmanagedType.LPWStr)] string lifetimeArtifact, CreatePackageDependencyOptions options, [MarshalAs(UnmanagedType.LPWStr)] out string packageDependencyId);
    }
}

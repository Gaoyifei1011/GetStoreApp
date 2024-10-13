namespace GetStoreApp.WindowsAPI.PInvoke.KernelBase
{
    /// <summary>
    /// 指定指示用于定义包依赖项生存期的项目类型的值。
    /// </summary>
    public enum PackageDependencyLifetimeKind
    {
        /// <summary>
        /// 当前进程是生存期项目。 当进程终止时，包依赖项将隐式删除。
        /// </summary>
        PackageDependencyLifetimeKind_Process = 0,

        /// <summary>
        /// 生存期项目是绝对文件名或路径。 包依赖项在删除时隐式删除。
        /// </summary>
        PackageDependencyLifetimeKind_FilePath = 1,

        /// <summary>
        /// 生存期项目是 根\子项格式的注册表项，其中 root 为下列项之一：HKEY_LOCAL_MACHINE、HKEY_CURRENT_USER、HKEY_CLASSES_ROOT或HKEY_USERS。 包依赖项在删除时隐式删除。
        /// </summary>
        PackageDependencyLifetimeKind_RegistryKey = 2,
    }
}

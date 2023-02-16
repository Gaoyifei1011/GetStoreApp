namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    /// <summary>
    /// 用于确定如何比较两个 Shell 项。 <see cref="IShellItem.Compare"> 使用此枚举类型。
    /// </summary>
    public enum SICHINTF : uint
    {
        /// <summary>
        /// 这与 <see cref="IShellItem.Compare"> 接口的 iOrder 参数相关，并指示比较基于文件夹视图中的显示。
        /// </summary>
        SICHINT_DISPLAY = 0x00000000,

        /// <summary>
        /// Shell 项的两个实例的确切比较。
        /// </summary>
        SICHINT_ALLFIELDS = 0x80000000,

        /// <summary>
        /// 这与 <see cref="IShellItem.Compare"> 接口的 iOrder 参数相关，并指示比较基于规范名称。
        /// </summary>
        SICHINT_CANONICAL = 0x10000000,

        /// <summary>
        /// Windows 7 及更高版本。 如果 Shell 项不相同，请测试文件系统路径。
        /// </summary>
        SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
    }
}

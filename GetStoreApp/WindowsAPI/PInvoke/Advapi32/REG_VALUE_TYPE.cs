// 抑制 CA1069 警告
#pragma warning disable CA1069

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// 注册表值类型
    /// </summary>
    public enum REG_VALUE_TYPE : uint
    {
        /// <summary>
        /// 没有定义的值类型。
        /// </summary>
        REG_NONE = 0,

        /// <summary>
        /// 以 null 结尾的字符串。 它是 Unicode 或 ANSI 字符串，具体取决于是使用 Unicode 还是 ANSI 函数。
        /// </summary>
        REG_SZ = 1,

        /// <summary>
        /// 一个以 null 结尾的字符串，其中包含对环境变量的未扩展引用，例如，%PATH%。 它是 Unicode 或 ANSI 字符串，具体取决于是使用 Unicode 还是 ANSI 函数。 若要展开环境变量引用，请使用 ExpandEnvironmentStrings 函数。
        /// </summary>
        REG_EXPAND_SZ = 2,

        /// <summary>
        /// 任何形式的二进制数据。
        /// </summary>
        REG_BINARY = 3,

        /// <summary>
        /// 32 位数字。
        /// </summary>
        REG_DWORD = 4,

        /// <summary>
        /// 小端格式的 32 位数字。 Windows 设计为在小端计算机体系结构上运行。 因此，此值在 Windows 头文件中定义为 REG_DWORD。
        /// </summary>
        REG_DWORD_LITTLE_ENDIAN = 4,

        /// <summary>
        /// big-endian 格式的 32 位数字。 某些 UNIX 系统支持大端体系结构。
        /// </summary>
        REG_DWORD_BIG_ENDIAN = 5,

        /// <summary>
        /// 一个以 null 结尾的 Unicode 字符串，其中包含通过使用 REG_OPTION_CREATE_LINK调用 RegCreateKeyEx 函数创建的符号链接的目标路径。
        /// </summary>
        REG_LINK = 6,

        /// <summary>
        /// 以空字符串结尾的字符串序列（\0）。
        /// </summary>
        REG_MULTI_SZ = 7,

        /// <summary>
        /// 资源地图中的资源列表。
        /// </summary>
        REG_RESOURCE_LIST = 8,

        /// <summary>
        /// 硬件描述中的资源列表。
        /// </summary>
        REG_FULL_RESOURCE_DESCRIPTOR = 9,

        /// <summary>
        /// 资源需求清单。
        /// </summary>
        REG_RESOURCE_REQUIREMENTS_LIST = 10,

        /// <summary>
        /// 64 位数字。
        /// </summary>
        REG_QWORD = 11,

        /// <summary>
        /// 采用小端格式的 64 位数字。 Windows 设计为在小端计算机体系结构上运行。 因此，此值在 Windows 头文件中定义为 REG_QWORD。
        /// </summary>
        REG_QWORD_LITTLE_ENDIAN = 11,
    }
}

using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// TOKEN_PRIVILEGES 结构包含有关访问令牌的一组特权的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct TOKEN_PRIVILEGES
    {
        /// <summary>
        /// 必须将其设置为 Privileges 数组中的条目数。
        /// </summary>
        internal int PrivilegeCount;

        /// <summary>
        /// 指定 LUID 值。
        /// </summary>
        internal LUID Luid;

        /// <summary>
        /// 指定 LUID 的属性。 此值最多包含 32 个一位标志。 其含义取决于 LUID 的定义和使用。
        /// </summary>
        internal int Attributes;
    }
}

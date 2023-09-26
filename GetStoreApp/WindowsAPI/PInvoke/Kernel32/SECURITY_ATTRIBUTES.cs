using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// SECURITY_ATTRIBUTES 结构包含对象的安全描述符，并指定通过指定此结构检索的句柄是否可继承。此结构为由各种函数（如 CreateFile、CreatePipe、CreateProcess、RegCreateKeyEx 或 RegSaveKeyEx）创建的对象提供安全设置。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SECURITY_ATTRIBUTES
    {
        /// <summary>
        /// 此结构的大小（以字节为单位）。
        /// 将此值设置为 SECURITY_ATTRIBUTES 结构的大小。
        /// </summary>
        public int nLength;

        /// <summary>
        /// 指向控制对对象的访问的SECURITY_DESCRIPTOR结构的指针。如果此成员的值为 NULL，则会为对象分配与调用进程的访问令牌关联的默认安全描述符。这与通过分配 NULL 自由访问控制列表 （DACL） 向每个人授予访问权限不同。默认情况下，进程的访问令牌中的默认 DACL 仅允许访问令牌表示的用户。
        /// </summary>
        public IntPtr lpSecurityDescriptor;

        /// <summary>
        /// 一个布尔值，它指定在创建新进程时是否继承返回的句柄。如果此成员为 TRUE，则新进程将继承句柄。
        /// </summary>
        public bool bInheritHandle;
    }
}

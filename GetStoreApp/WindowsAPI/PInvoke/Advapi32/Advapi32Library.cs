using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// advapi32.dll 函数库
    /// </summary>
    public static partial class Advapi32Library
    {
        private const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// GetTokenInformation 函数检索有关访问令牌的指定类型信息。 调用过程必须具有适当的访问权限才能获取信息。 若要确定用户是否是特定组的成员，请使用 CheckTokenMembership 函数。 若要确定应用容器令牌的组成员身份，请使用 CheckTokenMembershipEx 函数。
        /// </summary>
        /// <param name="TokenHandle">从中检索信息的访问令牌的句柄。 如果 TokenInformationClass 指定 TokenSource ，则句柄必须具有TOKEN_QUERY_SOURCE访问权限。 对于所有其他 TokenInformationClass 值，句柄必须具有TOKEN_QUERY访问权限。</param>
        /// <param name="TokenInformationClass">指定 TOKEN_INFORMATION_CLASS 枚举类型的值，用于标识函数检索的信息类型。 检查 TokenIsAppContainer 并使其返回 0 的任何调用方还应验证调用方令牌是否不是标识级别模拟令牌。 如果当前令牌不是应用容器，而是标识级令牌，则应返回 AccessDenied。</param>
        /// <param name="TokenInformation">指向函数填充的缓冲区的指针，其中包含请求的信息。 放入此缓冲区的结构取决于 TokenInformationClass 参数指定的信息类型。</param>
        /// <param name="TokenInformationLength">指定 TokenInformation 参数指向的缓冲区的大小（以字节为单位）。 如果 TokenInformation 为 NULL，则此参数必须为零。</param>
        /// <param name="ReturnLength">指向一个变量的指针，该变量接收 TokenInformation 参数指向的缓冲区所需的字节数。 如果此值大于 TokenInformationLength 参数中指定的值，函数将失败，并且不会在缓冲区中存储任何数据。如果 TokenInformationClass 参数的值为 TokenDefaultDacl，并且令牌没有默认的 DACL，则该函数会将 ReturnLength 指向的变量设置为 sizeof(TOKEN_DEFAULT_DACL)，并将 TOKEN_DEFAULT_DACL 结构的 DefaultDacl 成员设置为 NULL。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Advapi32, EntryPoint = "GetTokenInformation", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetTokenInformation(IntPtr tokenHandle, TOKEN_INFORMATION_CLASS tokenInformationClass, IntPtr tokenInformation, uint tokenInformationLength, out uint returnLength);

        /// <summary>
        /// OpenProcessToken 函数打开与进程关联的访问令牌。
        /// </summary>
        /// <param name="processHandle">打开其访问令牌的进程句柄。 此过程必须具有PROCESS_QUERY_LIMITED_INFORMATION访问权限。 有关详细信息，请参阅 进程安全和访问权限 。</param>
        /// <param name="desiredAccess">指定一个访问掩码，该掩码 指定对访问令牌的请求访问类型。 这些请求的访问类型与令牌 (DACL) 的自由访问控制列表 进行比较，以确定授予或拒绝哪些访问权限。</param>
        /// <param name="tokenHandle">指向在函数返回时标识新打开的访问令牌的句柄的指针。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Advapi32, EntryPoint = "OpenProcessToken", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);
    }
}

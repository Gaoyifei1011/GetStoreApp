using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
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
        [LibraryImport(Advapi32, EntryPoint = "GetTokenInformation", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            uint TokenInformationLength,
            out uint ReturnLength);

        /// <summary>
        /// OpenProcessToken 函数打开与进程关联的访问令牌。
        /// </summary>
        /// <param name="processHandle">打开其访问令牌的进程句柄。 此过程必须具有PROCESS_QUERY_LIMITED_INFORMATION访问权限。 有关详细信息，请参阅 进程安全和访问权限 。</param>
        /// <param name="desiredAccess">指定一个访问掩码，该掩码 指定对访问令牌的请求访问类型。 这些请求的访问类型与令牌 (DACL) 的自由访问控制列表 进行比较，以确定授予或拒绝哪些访问权限。</param>
        /// <param name="tokenHandle">指向在函数返回时标识新打开的访问令牌的句柄的指针。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Advapi32, EntryPoint = "OpenProcessToken", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        /// <summary>
        /// 关闭指定注册表项的句柄。
        /// </summary>
        /// <param name="hKey">要关闭的打开键的句柄。 该句柄必须由 RegOpenKeyEx 函数打开。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegCloseKey", SetLastError = true)]
        public static partial int RegCloseKey(UIntPtr hKey);

        /// <summary>
        /// 创建指定的注册表项。 如果该项已存在，函数将打开它。 请注意，键名称不区分大小写。
        /// 若要对项执行事务处理注册表操作，请调用 RegCreateKeyTransacted 函数。
        /// 备份或还原系统状态（包括系统文件和注册表配置单元）的应用程序应使用 卷影复制服务 ，而不是注册表功能。
        /// </summary>
        /// <param name="hKey">
        /// 打开的注册表项的句柄。 调用进程必须具有对密钥KEY_CREATE_SUB_KEY访问权限。 有关详细信息，请参阅 注册表项安全和访问权限。
        /// 针对注册表项的安全描述符（而不是获取句柄时指定的访问掩码）检查密钥创建的访问权限。 因此，即使 hKey 是使用 samDesired KEY_READ 打开的，它也可用于修改注册表的操作（如果其安全描述符允许）。
        /// 此句柄由 RegCreateKeyEx 或 RegOpenKeyEx 函数返回。
        /// </param>
        /// <param name="lpSubKey">
        /// 此函数打开或创建的子项的名称。 指定的子项必须是 由 hKey 参数标识的键的子项;它在注册表树中最多可以有 32 个级别。 有关键名称的详细信息，请参阅 注册表的结构。
        /// 如果 lpSubKey 是指向空字符串的指针， 则 phkResult 将接收 由 hKey 指定的键的新句柄。
        /// 此参数不能为 NULL。
        /// </param>
        /// <param name="Reserved">此参数是保留的，必须为零。</param>
        /// <param name="lpClass">此键的用户定义类类型。 此参数可以忽略。 此参数可以为 NULL。</param>
        /// <param name="dwOptions"></param>
        /// <param name="samDesired">一个掩码，指定要创建的密钥的访问权限。 有关详细信息，请参阅 注册表项安全和访问权限。</param>
        /// <param name="secAttrs">
        /// 指向 SECURITY_ATTRIBUTES 结构的指针，该结构确定是否可由子进程继承返回的句柄。 如果 lpSecurityAttributes 为 NULL，则无法继承句柄。
        /// 结构的 lpSecurityDescriptor 成员为新密钥指定安全描述符。 如果 lpSecurityAttributes 为 NULL，则密钥将获取默认的安全描述符。 密钥的默认安全描述符中的 ACL 继承自其直接父密钥。
        /// </param>
        /// <param name="hkResult">
        /// 一个变量的指针，此变量指向已打开或已创建的项的句柄。 如果该键不是预定义的注册表项之一，请在使用完句柄后调用 RegCloseKey 函数。
        /// </param>
        /// <param name="lpdwDisposition">
        /// 指向接收以下处置值之一的变量的指针。
        /// </param>
        /// <returns></returns>
        [LibraryImport(Advapi32, EntryPoint = "RegCreateKeyExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int RegCreateKeyEx(UIntPtr hKey, string lpSubKey, int Reserved, string lpClass, REG_CREATE_OPTIONS dwOptions, int samDesired, ref SECURITY_ATTRIBUTES secAttrs, out UIntPtr hkResult, out int lpdwDisposition);

        /// <summary>
        /// 从注册表的指定平台特定视图中删除子项及其值。 请注意，键名称不区分大小写。
        /// </summary>
        /// <param name="hKey">
        /// 打开的注册表项的句柄。 此密钥的访问权限不会影响删除操作。 有关访问权限的详细信息，请参阅 注册表项安全和访问权限。
        /// 此句柄由 RegCreateKeyEx 或 RegOpenKeyEx 函数返回</param>
        /// <param name="lpSubKey">
        /// 要删除的文件的名称。 此键必须是 由 hKey 参数的值指定的键的子项。函数使用 DELETE 访问权限打开子项。键名称不区分大小写。此参数的值不能为 NULL。
        /// </param>
        /// <param name="samDesired">访问掩码 指定注册表的平台特定视图。</param>
        /// <param name="Reserved">此参数是保留的，必须为零。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegDeleteKeyExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int RegDeleteKeyEx(UIntPtr hKey, string lpSubKey, int samDesired, int Reserved);

        /// <summary>
        /// 从指定的注册表项中删除命名值。 请注意，值名称不区分大小写。
        /// </summary>
        /// <param name="hKey">
        /// 打开的注册表项的句柄。 密钥必须已使用KEY_SET_VALUE访问权限打开。 有关详细信息，请参阅 注册表项安全和访问权限。
        /// 此句柄由 RegCreateKeyEx、 RegCreateKeyTransacted、 RegOpenKeyEx 或 RegOpenKeyTransacted 函数返回。
        /// </param>
        /// <param name="lpValueName">要删除的注册表值。 如果此参数为 NULL 或空字符串，则删除由 RegSetValue 函数设置的值。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>

        [LibraryImport(Advapi32, EntryPoint = "RegDeleteValueW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int RegDeleteValue(UIntPtr hKey, string lpValueName);

        /// <summary>
        /// 打开指定的注册表项。 请注意，键名称不区分大小写。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。</param>
        /// <param name="lpSubKey">要打开的注册表子项的名称。键名称不区分大小写。</param>
        /// <param name="ulOptions">指定打开键时要应用的选项。</param>
        /// <param name="samDesired">一个掩码，指定要打开的密钥的所需访问权限。 如果密钥的安全描述符不允许调用进程的请求访问，函数将失败。</param>
        /// <param name="phkResult">一个变量的指针，此变量指向已打开键的句柄。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegOpenKeyExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int RegOpenKeyEx(UIntPtr hKey, string lpSubKey, int ulOptions, REG_ACCESS_MASK samDesired, ref UIntPtr phkResult);

        /// <summary>
        /// 检索与打开的注册表项关联的指定值名称的类型和数据。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。 必须使用KEY_QUERY_VALUE访问权限打开密钥。</param>
        /// <param name="lpValueName">注册表值的名称。</param>
        /// <param name="lpReserved">此参数是保留的，必须为 NULL。</param>
        /// <param name="lpType">指向一个变量的指针，该变量接收一个代码，指示存储在指定值中的数据的类型。</param>
        /// <param name="lpData">指向接收值数据的缓冲区的指针。</param>
        /// <param name="lpcbData">指向一个变量的指针，该变量指定 lpData 参数指向的缓冲区的大小（以字节为单位）。</param>
        /// <returns>
        /// 如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 系统错误代码。
        /// 如果 lpData 缓冲区太小，无法接收数据，函数将返回 ERROR_MORE_DATA。如果 lpValueName 注册表值不存在，该函数将返回ERROR_FILE_NOT_FOUND。
        /// </returns>
        [LibraryImport(Advapi32, EntryPoint = "RegQueryValueExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int RegQueryValueEx(UIntPtr hKey, string lpValueName, int lpReserved, out uint lpType, [In, Out] byte[] lpData, ref uint lpcbData);

        /// <summary>
        /// 设置注册表项下指定值的数据和类型。
        /// </summary>
        /// <param name="hKey">
        /// 打开的注册表项的句柄。 密钥必须已使用KEY_SET_VALUE访问权限打开。 有关详细信息，请参阅 注册表项安全和访问权限。
        /// 此句柄由 RegCreateKeyEx、 RegCreateKeyTransacted、 RegOpenKeyEx 或 RegOpenKeyTransacted 函数返回。
        /// </param>
        /// <param name="lpValueName">
        /// 要设置的值的名称。 如果键中尚不存在具有此名称的值，则函数会将其添加到键中。
        /// 如果 lpValueName 为 NULL 或空字符串“”，则该函数将设置键的未命名值或默认值的类型和数据。
        /// 有关详细信息，请参阅 注册表元素大小限制。
        /// 注册表项没有默认值，但它们可以有一个未命名的值，该值可以是任何类型的。
        /// </param>
        /// <param name="Reserved">此参数是保留的，必须为零。</param>
        /// <param name="dwType">lpData 参数指向的数据类型。 有关可能类型的列表，请参阅 注册表值类型。</param>
        /// <param name="lpData">要存储的数据。
        /// 对于基于字符串的类型（如 REG_SZ），字符串必须以 null 结尾。 对于 REG_MULTI_SZ 数据类型，字符串必须以两个 null 字符结尾。</param>
        /// <param name="cbData">
        /// lpData 参数指向的信息的大小（以字节为单位）。 如果数据的类型为 REG_SZ、REG_EXPAND_SZ 或 REG_MULTI_SZ， 则 cbData 必须包含终止 null 字符的大小。
        /// </param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegSetValueExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int RegSetValueEx(UIntPtr hKey, string lpValueName, int Reserved, int dwType, [In, Out] byte[] lpData, int cbData);
    }
}

using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// advapi32.dll 函数库
    /// </summary>
    public static partial class Advapi32Library
    {
        private const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// AdjustTokenPrivileges 函数启用或禁用指定访问令牌中的特权。 启用或禁用访问令牌中的特权需要TOKEN_ADJUST_PRIVILEGES访问权限。
        /// </summary>
        /// <param name="TokenHandle">包含要修改的权限的访问令牌的句柄。 句柄必须具有TOKEN_ADJUST_PRIVILEGES令牌的访问权限。 如果 PreviousState 参数不为 NULL，则句柄还必须具有TOKEN_QUERY访问权限。</param>
        /// <param name="DisableAllPrivileges">指定函数是否禁用令牌的所有特权。 如果此值为 TRUE，则函数将禁用所有特权并忽略 NewState 参数。 如果为 FALSE，则函数根据 NewState 参数指向的信息修改权限。</param>
        /// <param name="NewState">指向 TOKEN_PRIVILEGES 结构的指针，该结构指定特权及其属性的数组。 如果 DisableAllPrivileges 参数为 FALSE， 则 AdjustTokenPrivileges 函数将启用、禁用或删除令牌的这些特权。 下表描述了 AdjustTokenPrivileges 函数基于特权属性执行的操作。</param>
        /// <param name="BufferLength">指定 PreviousState 参数指向的缓冲区的大小（以字节为单位）。 如果 PreviousState 参数为 NULL，则此参数可以为 零。</param>
        /// <param name="PreviousState">
        /// 指向缓冲区的指针，该缓冲区由 函数填充TOKEN_PRIVILEGES 结构，该结构包含函数修改的任何特权的先前状态。 也就是说，如果此函数修改了权限，则权限及其以前的状态包含在 PreviousState 引用的TOKEN_PRIVILEGES结构中。 如果 TOKEN_PRIVILEGES 的 PrivilegeCount 成员为零，则此函数未更改任何特权。 此参数可以为 NULL。
        /// 如果指定的缓冲区太小，无法接收修改的权限的完整列表，则函数将失败，并且不会调整任何特权。 在这种情况下， 函数将 ReturnLength 参数指向的变量设置为保存修改的权限的完整列表所需的字节数。
        /// </param>
        /// <param name="ReturnLength">指向变量的指针，该变量接收 PreviousState 参数指向的缓冲区的所需大小（以字节为单位）。 如果 PreviousState 为 NULL，此参数可以为 NULL。</param>
        /// <returns>如果该函数成功，则返回值为非零值。</returns>
        [LibraryImport(Advapi32, EntryPoint = "AdjustTokenPrivileges", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool AdjustTokenPrivileges(nint TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint BufferLength, nint PreviousState, nint ReturnLength);

        /// <summary>
        /// 启动指定计算机的关闭和可选重启，并选择性地记录关闭的原因。
        /// </summary>
        /// <param name="lpMachineName">要关闭的计算机的网络名称。 如果 lpMachineNameNULL 或空字符串，则该函数将关闭本地计算机。</param>
        /// <param name="lpMessage">要显示在关闭对话框中的消息。 如果没有消息，则可以 NULL 此参数。</param>
        /// <param name="dwTimeout">
        /// 应显示关闭对话框的时间长度（以秒为单位）。 显示此对话框时，AbortSystemShutdown 函数可以停止关闭。
        /// 如果 dwTimeout 不为零，InitiateSystemShutdownEx 在指定计算机上显示对话框。 该对话框显示调用函数的用户的名称，显示由 lpMessage 参数指定的消息，并提示用户注销。 创建对话框时会发出蜂鸣声，并保留在系统中的其他窗口之上。 对话框可以移动，但不能关闭。 计时器在关闭前将倒计时剩余时间。
        /// 如果 dwTimeout 为零，则计算机关闭而不显示对话框，AbortSystemShutdown无法停止关闭。
        /// </param>
        /// <param name="bForceAppsClosed">如果此参数 TRUE，则具有未保存更改的应用程序将被强行关闭。 如果此参数 FALSE，系统将显示一个对话框，指示用户关闭应用程序。</param>
        /// <param name="bRebootAfterShutdown">如果此参数 TRUE，则计算机在关闭后立即重启。 如果此参数 FALSE，系统会将所有缓存刷新到磁盘并安全地关闭系统。</param>
        /// <param name="dwReason">
        /// 启动关闭的原因。 此参数必须是系统关闭原因代码之一。
        /// 如果此参数为零，则默认值为未定义的关闭，该关闭记录为“找不到此原因的标题”。 默认情况下，它也是计划外关闭。 根据系统的配置方式，计划外关闭会触发创建包含系统状态信息的文件，该文件可能会延迟关闭。 因此，不要对此参数使用零。
        /// </param>
        /// <returns>如果函数成功，则返回值为非零。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Advapi32, EntryPoint = "InitiateSystemShutdownExW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool InitiateSystemShutdownEx([MarshalAs(UnmanagedType.LPWStr)] string lpMachineName, [MarshalAs(UnmanagedType.LPWStr)] string lpMessage, uint dwTimeout, [MarshalAs(UnmanagedType.Bool)] bool bForceAppsClosed, [MarshalAs(UnmanagedType.Bool)] bool bRebootAfterShutdown, SHTDN_REASON dwReason);

        /// <summary>
        /// LookupPrivilegeValue 函数检索指定系统上用于本地表示指定特权名称的 本地唯一标识符（LUID）。
        /// </summary>
        /// <param name="lpSystemName">指向以 null 结尾的字符串的指针，该字符串指定检索特权名称的系统的名称。 如果指定了 null 字符串，则该函数将尝试在本地系统上查找特权名称。</param>
        /// <param name="lpName">指向以 null 结尾的字符串的指针，该字符串指定 Winnt.h 头文件中定义的特权名称。 例如，此参数可以指定常量、SE_SECURITY_NAME或其相应的字符串“SeSecurityPrivilege”。</param>
        /// <param name="lpLuid">指向接收 LUID 的变量的指针，该变量在 lpSystemName 参数指定的系统上已知特权。</param>
        /// <returns>如果函数成功，该函数将返回非零。如果函数失败，则返回零。 </returns>
        [LibraryImport(Advapi32, EntryPoint = "LookupPrivilegeValueW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool LookupPrivilegeValue([MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPWStr)] string lpName, out LUID lpLuid);

        /// <summary>
        /// OpenProcessToken 函数打开与进程关联的访问令牌。
        /// </summary>
        /// <param name="processHandle">打开其访问令牌的进程句柄。 此过程必须具有PROCESS_QUERY_LIMITED_INFORMATION访问权限。 有关详细信息，请参阅 进程安全和访问权限 。</param>
        /// <param name="desiredAccess">指定一个访问掩码，该掩码 指定对访问令牌的请求访问类型。 这些请求的访问类型与令牌 (DACL) 的自由访问控制列表 进行比较，以确定授予或拒绝哪些访问权限。</param>
        /// <param name="tokenHandle">指向在函数返回时标识新打开的访问令牌的句柄的指针。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(Advapi32, EntryPoint = "OpenProcessToken", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool OpenProcessToken(nint processHandle, uint desiredAccess, out nint tokenHandle);

        /// <summary>
        /// 关闭指定注册表项的句柄。
        /// </summary>
        /// <param name="hKey">要关闭的打开键的句柄。 该句柄必须由 <see cref="RegOpenKeyEx"> 函数打开。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegCloseKey", SetLastError = false), PreserveSig]
        public static partial int RegCloseKey(nint hKey);

        /// <summary>
        /// 打开指定的注册表项。 请注意，键名称不区分大小写。
        /// </summary>
        /// <param name="hKey">打开的注册表项的句柄。</param>
        /// <param name="lpSubKey">要打开的注册表子项的名称。键名称不区分大小写。</param>
        /// <param name="ulOptions">指定打开键时要应用的选项。</param>
        /// <param name="samDesired">一个掩码，指定要打开的密钥的所需访问权限。 如果密钥的安全描述符不允许调用进程的请求访问，函数将失败。</param>
        /// <param name="phkResult">一个变量的指针，此变量指向已打开键的句柄。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegOpenKeyExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial int RegOpenKeyEx(nuint hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey, int ulOptions, RegistryAccessRights samDesired, out nint phkResult);

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
        /// 如果 lpData 缓冲区太小，无法接收数据，函数将返回ERROR_MORE_DATA。如果 lpValueName 注册表值不存在，该函数将返回ERROR_FILE_NOT_FOUND。
        /// </returns>
        [LibraryImport(Advapi32, EntryPoint = "RegQueryValueExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        public static partial int RegQueryValueEx(nint hKey, [MarshalAs(UnmanagedType.LPWStr)] string lpValueName, nint lpReserved, out REG_VALUE_TYPE lpType, [Out] byte[] lpData, ref int lpcbData);
    }
}

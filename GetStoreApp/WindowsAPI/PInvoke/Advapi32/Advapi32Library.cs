using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Advapi32
{
    /// <summary>
    /// Advapi32.dll 函数库
    /// </summary>
    public static partial class Advapi32Library
    {
        private const string Advapi32 = "advapi32.dll";

        /// <summary>
        /// 关闭指定注册表项的句柄。
        /// </summary>
        /// <param name="hKey">要关闭的打开键的句柄。 该句柄必须由 <see cref="RegOpenKeyEx"> 函数打开。</param>
        /// <returns>如果函数成功，则返回值为 ERROR_SUCCESS。如果函数失败，则返回值为 Winerror.h 中定义的非零错误代码。</returns>
        [LibraryImport(Advapi32, EntryPoint = "RegCloseKey", SetLastError = true)]
        public static partial int RegCloseKey(UIntPtr hKey);

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
        public static partial int RegOpenKeyEx(UIntPtr hKey, string lpSubKey, int ulOptions, RegistryAccessRights samDesired, ref UIntPtr phkResult);

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
        [LibraryImport(Advapi32, EntryPoint = "RegQueryValueExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int RegQueryValueEx(UIntPtr hKey, string lpValueName, int lpReserved, out uint lpType, byte[] lpData, ref uint lpcbData);
    }
}

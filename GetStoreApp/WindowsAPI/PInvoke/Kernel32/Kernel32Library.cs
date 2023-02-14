using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// Kernel32.dll 函数库
    /// </summary>
    public static class Kernel32Library
    {
        private const string Kernel32 = "Kernel32.dll";

        /// <summary>
        /// 为调用进程分配一个新的控制台。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Ansi, EntryPoint = "AllocConsole", SetLastError = false)]
        public static extern bool AllocConsole();

        /// <summary>
        /// 将调用进程附加到指定进程的控制台作为客户端应用程序
        /// </summary>
        /// <param name="dwProcessId">
        /// 将调用进程附加到指定进程的控制台作为客户端应用程序，默认值为ATTACH_PARENT_PROCESS = -1
        /// </param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "AttachConsole", SetLastError = false)]
        public static extern bool AttachConsole(int dwProcessId = -1);

        /// <summary>
        /// 关闭打开的对象句柄。
        /// </summary>
        /// <param name="hObject">打开对象的有效句柄。</param>
        /// <returns>
        /// 如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。
        /// 如果应用程序在调试器下运行，则如果函数收到无效的句柄值或伪句柄值，该函数将引发异常。
        /// 如果两次关闭句柄，或者对 FindFirstFile 函数返回的句柄调用 CloseHandle，而不是调用 FindClose 函数，则可能会出现这种情况。
        /// </returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "CloseHandle", SetLastError = false)]
        public static extern int CloseHandle(int hObject);

        /// <summary>枚举操作系统上可用的地理位置标识符 (类型 GEOID) 。</summary>
        /// <param name="geoClass">
        /// 要枚举标识符的地理位置类。 目前仅支持GEOCLASS_NATION。 此类型会导致函数枚举操作系统上国家/地区的所有地理标识符。
        /// </param>
        /// <param name="parentGeoId">保留。 此参数必须为 0。</param>
        /// <param name="lpGeoEnumProc">指向应用程序定义的回调函数 EnumGeoInfoProc 的指针。 EnumSystemGeoID 函数对此回调函数进行重复调用，直到返回 FALSE。</param>
        /// <returns>如果成功，则返回非零值，否则返回 0。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "EnumSystemGeoID", SetLastError = true)]
        public static extern int EnumSystemGeoID(int geoClass, int parentGeoId, EnumGeoInfoProc lpGeoEnumProc);

        /// <summary>
        /// 从其控制台中分离调用进程。
        /// </summary>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Ansi, EntryPoint = "FreeConsole", SetLastError = false)]
        public static extern bool FreeConsole();

        /// <summary>
        /// 检索与调用进程关联的控制台所使用的窗口句柄。
        /// </summary>
        /// <returns>返回值是与调用进程关联的控制台所使用的窗口的句柄; 如果没有此类关联的控制台，则为 NULL 。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Ansi, EntryPoint = "GetConsoleWindow", SetLastError = false)]
        public static extern IntPtr GetConsoleWindow();

        /// <summary>检索有关指定地理位置的信息。</summary>
        /// <param name="location">要获取信息的地理位置的标识符。可以通过调用 EnumSystemGeoID 来获取可用值。</param>
        /// <param name="geoType">
        /// 要检索的信息类型。 可能的值由 SYSGEOTYPE 枚举定义。 如果 GeoType 的值GEO_LCID，该函数将检索区域设置标识符。
        /// 如果 GeoType 的值GEO_RFC1766，该函数将检索符合 RFC 4646 (Windows Vista) 的字符串名称。
        /// </param>
        /// <param name="lpGeoData">指向此函数检索信息的缓冲区的指针。</param>
        /// <param name="cchData">
        /// 由 lpGeoData 指示的缓冲区的大小。 大小是函数 ANSI 版本的字节数，或 Unicode 版本的单词数。
        /// 如果函数返回所需的缓冲区大小，应用程序可以将此参数设置为 0。
        /// </param>
        /// <param name="langId">
        /// 语言的标识符，与 Location 值一起使用。 应用程序可以将此参数设置为 0，并为 GeoType 指定了GEO_RFC1766或GEO_LCID。
        /// 此设置会导致函数通过调用 GetUserDefaultLangID 检索语言标识符。
        /// </param>
        /// <returns>
        /// 返回 (ANSI) 或单词 (Unicode) 在输出缓冲区中检索的地理位置信息的字节数。
        /// 如果 cchData 设置为 0，则函数返回缓冲区所需的大小。如果函数不成功，则返回 0。
        /// </returns>
        [DllImport(Kernel32, CharSet = CharSet.Unicode, EntryPoint = "GetGeoInfo", SetLastError = true)]
        public static extern int GetGeoInfo(int location, SYSGEOTYPE geoType, StringBuilder lpGeoData, int cchData, int langId);

        /// <summary>
        /// 打开现有的本地进程对象。
        /// </summary>
        /// <param name="dwDesiredAccess">
        /// 对进程对象的访问。根据进程的安全描述符检查此访问权限。此参数可以是一个或多个进程访问权限。
        /// 如果调用方已启用SeDebugPrivilege 特权，则无论安全描述符的内容如何，都会授予请求的访问权限。</param>
        /// <param name="bInheritHandle">如果此值为 TRUE，则此进程创建的进程将继承句柄。否则，进程不会继承此句柄。</param>
        /// <param name="dwProcessId">
        /// 要打开的本地进程的标识符。
        /// 如果指定的进程是系统空闲进程 （0x00000000），则该函数将失败，最后一个错误代码为。
        /// 如果指定的进程是系统进程或客户端服务器运行时子系统 （CSRSS） 进程之一，则此函数将失败，
        /// 最后一个错误代码是因为它们的访问限制阻止用户级代码打开它们。ERROR_INVALID_PARAMETERERROR_ACCESS_DENIED
        /// </param>
        /// <returns>如果函数成功，则返回值是指定进程的打开句柄。如果函数失败，则返回值为 NULL。</returns>
        [DllImport(Kernel32, CharSet = CharSet.Ansi, EntryPoint = "OpenProcess", SetLastError = false)]
        public static extern int OpenProcess(EDesiredAccess dwDesiredAccess, bool bInheritHandle, int dwProcessId);
    }
}

using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Version
{
    /// <summary>
    /// Version.dll 函数库。
    /// </summary>
    public partial class VersionLibrary
    {
        private const string Version = "Version.dll";

        /// <summary>
        /// 检索指定文件的版本信息。
        /// </summary>
        /// <param name="lptstrFilename">文件的名称。 如果未指定完整路径，该函数将使用 LoadLibrary 函数指定的搜索序列。</param>
        /// <param name="dwHandle">忽略此参数。</param>
        /// <param name="dwLen">
        /// lpData 参数指向的缓冲区的大小（以字节为单位）。
        /// 首先调用 <see cref="GetFileVersionInfoSize"> 函数，以确定文件版本信息的大小（以字节为单位）。 <param name="dwLen"> 成员应等于或大于该值。
        /// 如果 <param name="lpData"> 指向的缓冲区不够大，函数会将文件的版本信息截断为缓冲区的大小。
        /// </param>
        /// <param name="lpData">
        /// 指向接收文件版本信息的缓冲区的指针。可以在对 <see cref="VerQueryValue"> 函数的后续调用中使用此值从缓冲区中检索数据。
        /// </param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(Version, EntryPoint = "GetFileVersionInfoW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetFileVersionInfo(string lptstrFilename, int dwHandle, int dwLen, byte[] lpData);

        /// <summary>
        /// 确定操作系统是否可以检索指定文件的版本信息。 如果版本信息可用， <see cref="GetFileVersionInfoSize"> 将返回该信息的大小（以字节为单位）。
        /// </summary>
        /// <param name="lptstrFilename">感兴趣的文件的名称。 该函数使用 LoadLibrary 函数指定的搜索序列。</param>
        /// <param name="lpdwHandle">指向函数设置为零的变量的指针。</param>
        /// <returns>如果函数成功，则返回值是文件版本信息的大小（以字节为单位）。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(Version, EntryPoint = "GetFileVersionInfoSizeW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int GetFileVersionInfoSize(string lptstrFilename, out IntPtr lpdwHandle);

        /// <summary>
        /// 从指定的版本信息资源检索指定的版本信息。 若要检索适当的资源，在调用 <see cref="VerQueryValue"> 之前，必须先调用 <see cref="GetFileVersionInfoSize"> 函数，然后调用 <see cref="GetFileVersionInfo"> 函数。
        /// </summary>
        /// <param name="pBlock"><see cref="GetFileVersionInfo"> 函数返回的版本信息资源。</param>
        /// <param name="lpSubBlock">
        /// 要检索的版本信息值。 字符串必须由反斜杠 (\) 分隔的名称组成，并且必须具有以下形式之一。
        /// \
        /// 根块。 该函数检索指向版本信息资源的 VS_FIXEDFILEINFO 结构的指针。
        /// \VarFileInfo\Translation
        /// Var 变量信息结构中的转换数组 - 此结构的 Value 成员。 该函数检索指向此语言和代码页标识符数组的指针。 应用程序可以使用这些标识符访问特定于语言的 StringTable 结构， (使用版本信息资源中的 szKey 成员) 。
        /// \StringFileInfo\lang-codepage\string-name
        /// 特定于语言的 StringTable 结构中的值。 lang-codepage 名称是作为资源翻译数组中的 DWORD 找到的语言和代码页标识符对的串联。 此处的 lang-codepage 名称必须指定为十六进制字符串。 字符串名称必须是以下“备注”部分所述的预定义字符串之一。 该函数检索特定于所指示语言和代码页的字符串值。
        /// </param>
        /// <param name="lplpBuffer">
        /// 此方法返回时，包含指向 <param name="pBlock"> 指向的缓冲区中请求的版本信息的指针的地址。 释放关联的 <param name="pBlock"> 内存时，会释放 <param name="lplpBuffer"> 指向的内存。
        /// </param>
        /// <param name="puLen">
        /// 此方法返回时，包含指向 <param name="lplpBuffer"> 指向的请求数据大小的指针：对于版本信息值，存储在 <param name="lplpBuffer"> 处的字符串的长度（以字符为单位）：对于转换数组值，存储在 lplpBuffer 处的数组的大小（以字节为单位）;对于根块，结构的大小（以字节为单位）。
        /// </param>
        /// <returns>
        /// 如果指定的版本信息结构存在，并且版本信息可用，则返回值为非零。 如果长度缓冲区的地址为零，则没有值可用于指定的版本信息名称。如果指定的名称不存在或指定的资源无效，则返回值为零。
        /// </returns>
        [LibraryImport(Version, EntryPoint = "VerQueryValueW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool VerQueryValue(byte[] pBlock, string lpSubBlock, out IntPtr lplpBuffer, out IntPtr puLen);
    }
}

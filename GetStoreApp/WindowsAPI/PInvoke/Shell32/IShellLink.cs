using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 公开创建、修改和解析 Shell 链接的方法。
    /// </summary>
    [ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellLink
    {
        /// <summary>
        /// 获取 Shell 链接对象的目标的路径和文件名。
        /// </summary>
        /// <param name="pszFile">接收 Shell 链接对象目标的路径和文件名的缓冲区的地址。</param>
        /// <param name="cch"><param name="pszFile"> 参数指向的缓冲区的大小（以字符为单位）包括终止 null 字符。 可返回的最大路径大小为MAX_PATH。 此参数通常通过调用 ARRAYSIZE (<param name="pszFile">) 来设置。 ARRAYSIZE 宏在 Winnt.h 中定义。</param>
        /// <param name="pfd">指向 <see cref="WIN32_FIND_DATA"> 结构的指针，该结构接收有关 Shell 链接对象的目标的信息。 如果此参数为 NULL，则不会返回其他信息。</param>
        /// <param name="fFlags">指定要检索的路径信息的类型的标志。 </param>
        [PreserveSig]
        void GetPath([MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszFile, int cch, ref IntPtr pfd, uint fFlags);

        /// <summary>
        /// 获取 Shell 链接对象目标的项目标识符列表。
        /// </summary>
        /// <param name="ppidl">此方法返回时，包含 PIDL 的地址。</param>
        [PreserveSig]
        void GetIDList(out IntPtr ppidl);

        /// <summary>
        /// 设置指向命令行管理程序链接对象的项标识符列表 （PIDL） 的指针。
        /// </summary>
        /// <param name="ppidl">对象的完全限定的 PIDL。</param>
        [PreserveSig]
        void SetIDList(IntPtr ppidl);

        /// <summary>
        /// 获取 Shell 链接对象的说明字符串。
        /// </summary>
        /// <param name="pszName"></param>
        /// <param name="cch">要复制到 <param name="pszName"> 参数指向的缓冲区的最大字符数。</param>
        [PreserveSig]
        void GetDescription([MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszName, int cch);

        /// <summary>
        /// 设置 Shell 链接对象的说明。 说明可以是任何应用程序定义的字符串。
        /// </summary>
        /// <param name="pszName">指向包含新说明字符串的缓冲区的指针。</param>
        [PreserveSig]
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// 获取 Shell 链接对象的工作目录的名称。
        /// </summary>
        /// <param name="pszDir">接收工作目录名称的缓冲区的地址。</param>
        /// <param name="cch">要复制到 <param name="pszDir"> 参数指向的缓冲区的最大字符数。 如果工作目录的名称超过此参数指定的最大值，则将其截断。</param>
        [PreserveSig]
        void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszDir, int cch);

        /// <summary>
        /// 设置 Shell 链接对象的工作目录的名称。
        /// </summary>
        /// <param name="pszDir">包含新工作目录名称的缓冲区的地址。</param>
        [PreserveSig]
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        /// <summary>
        /// 获取与 Shell 链接对象关联的命令行参数。
        /// </summary>
        /// <param name="pszArgs">指向缓冲区的指针，当此方法成功返回时，接收命令行参数。</param>
        /// <param name="cch">可以复制到 <param name="pszArgs"> 参数提供的缓冲区的最大字符数。 对于 Unicode 字符串，最大字符串长度没有限制。 对于 ANSI 字符串，返回的字符串的最大长度因 Windows 版本而异，具体取决于 Windows 2000 之前的 MAX_PATH，在 Windows 2000 及更高版本中的 Commctrl.h) 中定义的 INFOTIPSIZE (。</param>
        [PreserveSig]
        void GetArguments([MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszArgs, int cch);

        /// <summary>
        /// 设置 Shell 链接对象的命令行参数。
        /// </summary>
        /// <param name="pszArgs">指向包含新命令行参数的缓冲区的指针。 对于 Unicode 字符串，最大字符串长度没有限制。 对于 ANSI 字符串，返回的字符串的最大长度因 Windows 版本而异，MAX_PATH Windows 2000 之前的 Windows 2000 和 INFOTIPSIZE (在 Windows 2000 及更高版本中定义的 Commctrl.h) 。</param>
        [PreserveSig]
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        /// <summary>
        /// 获取 Shell 链接对象的键盘快捷方式 (热键) 。
        /// </summary>
        /// <param name="pwHotkey">键盘快捷方式的地址。 虚拟密钥代码采用低顺序字节，修饰符标志采用高阶字节。 修饰符标志可以是以下值的组合。</param>
        [PreserveSig]
        void GetHotkey(out ushort pwHotkey);

        /// <summary>
        /// 设置 Shell 链接对象的键盘快捷方式 (热键) 。
        /// </summary>
        /// <param name="wHotkey">新的键盘快捷方式。 虚拟密钥代码采用低顺序字节，修饰符标志采用高阶字节。 修饰符标志可以是 <see cref="GetHotkey"> 方法说明中指定的值的组合。</param>
        [PreserveSig]
        void SetHotkey(ushort wHotkey);

        /// <summary>
        /// 获取 Shell 链接对象的 show 命令。
        /// </summary>
        /// <param name="piShowCmd">指向命令的指针。</param>
        [PreserveSig]
        void GetShowCmd(out int piShowCmd);

        /// <summary>
        /// 设置命令行管理程序链接对象的 show 命令。show 命令设置窗口的初始显示状态。
        /// </summary>
        /// <param name="iShowCmd">命令。</param>
        [PreserveSig]
        void SetShowCmd(int iShowCmd);

        /// <summary>
        /// 获取 Shell 链接对象的图标 (路径和索引) 的位置。
        /// </summary>
        /// <param name="pszIconPath">接收包含图标的文件路径的缓冲区的地址。</param>
        /// <param name="cch">要复制到 <param name="pszIconPath"> 参数指向的缓冲区的最大字符数。</param>
        /// <param name="piIcon">接收图标索引的值的地址。</param>
        [PreserveSig]
        void GetIconLocation([MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszIconPath, int cch, out int piIcon);

        /// <summary>
        /// 设置 Shell 链接对象的图标 (路径和索引) 的位置。
        /// </summary>
        /// <param name="pszIconPath">要包含包含图标的文件路径的缓冲区的地址。</param>
        /// <param name="iIcon">图标的索引。</param>
        [PreserveSig]
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

        /// <summary>
        /// 设置 Shell 链接对象的相对路径。
        /// </summary>
        /// <param name="pszPathRel">包含快捷方式文件的完全限定路径的缓冲区的地址，相对于应执行快捷方式解析的缓冲区。 它应该是文件名，而不是文件夹名称。</param>
        /// <param name="dwReserved">保留。 将此参数设置为零。</param>
        [PreserveSig]
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

        /// <summary>
        /// 尝试查找 Shell 链接的目标，即使它已被移动或重命名也是如此。
        /// </summary>
        /// <param name="hwnd">Shell 将用作对话框的父窗口的句柄。 如果 Shell 在解析 Shell 链接时需要提示用户获取详细信息，则 Shell 会显示对话框。</param>
        /// <param name="fFlags">操作标志。 </param>
        [PreserveSig]
        void Resolve(IntPtr hwnd, uint fFlags);

        /// <summary>
        /// 设置 Shell 链接对象目标的路径和文件名。
        /// </summary>
        /// <param name="pszFile">包含新路径的缓冲区的地址。</param>
        [PreserveSig]
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 提供对应用安装程序文件的 XML DOM 的访问权限。
    /// </summary>
    [GeneratedComInterface, Guid("7325B83D-0185-42C4-82AC-BE34AB1A2A8A")]
    public partial interface IAppxAppInstallerReader
    {
        /// <summary>
        /// 获取应用安装程序文件的 XML DOM。
        /// </summary>
        /// <param name="dom">接收指向 IXMLDOMDocument 的指针，该指针表示应用安装程序文件的 XML DOM。</param>
        /// <returns>如果成功，则返回 S_OK。</returns>
        [PreserveSig]
        int GetXmlDom(out IntPtr dom);
    }
}

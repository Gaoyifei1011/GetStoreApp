using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 使对象能够从磁盘文件加载或保存到磁盘文件，而不是存储对象或流。 由于打开文件所需的信息因应用程序而异，因此对象上的 <see cref="Load"> 的实现还必须打开其磁盘文件。
    /// </summary>
    [Guid("0000010b-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IPersistFile
    {
        /// <summary>
        /// 检索对象的 CLSID (类标识符) 。
        /// </summary>
        /// <param name="pClassID">指向返回时接收 CLSID 的位置的指针。 CLSID 是一个全局唯一标识符 (GUID) ，它唯一表示一个对象类，该类定义可以操作对象的数据的代码。</param>
        void GetClassID(out Guid pClassID);

        /// <summary>
        /// 检索与对象关联的文件的当前名称。 如果没有当前工作文件，此方法将检索对象的默认保存提示。
        /// </summary>
        /// <param name="ppszFileName"></param>
        void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);

        /// <summary>
        /// 确定对象自上次保存到其当前文件以来是否已更改。
        /// </summary>
        /// <returns>此方法返回 S_OK 以指示对象已更改。否则，它将返回S_FALSE。</returns>
        [PreserveSig]
        int IsDirty();

        /// <summary>
        /// 打开指定文件并从文件内容初始化对象。
        /// </summary>
        /// <param name="pszFileName">要打开的文件的绝对路径。</param>
        /// <param name="dwMode">打开文件时要使用的访问模式。 可能的值取自 STGM 枚举。 如果有必要，该方法可以将此值视为建议，并添加更严格的权限。 如果 <param name="dwMode"> 为 0，则实现应使用用户打开文件时使用任何默认权限打开文件。</param>
        void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, int dwMode);

        /// <summary>
        /// 将对象的副本保存到指定文件。
        /// </summary>
        /// <param name="pszFileName">应将对象保存到的文件的绝对路径。 如果 <param name="pszFileName"> 为 NULL，则对象应将其数据保存到当前文件（如果有）。</param>
        /// <param name="fRemember">指示 <param name="pszFileName"> 参数是否用作当前工作文件。 如果 为 TRUE， <param name="pszFileName"> 将成为当前文件，并且该对象应在保存后清除其脏标志。 如果 为 FALSE，则此保存操作是“ 另存为复制...” 操作。 在这种情况下，当前文件不变，对象不应清除其脏标志。 如果 <param name="pszFileName"> 为 NULL，则实现应忽略 <param name="fRemember"> 标志。</param>
        void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);

        /// <summary>
        /// 通知该对象它可以写入它的文件。 它通过通知对象，它可以从 NoScribble 模式还原 (，在该模式中，它不能写入其文件) ，并将其 (，) 。 组件在收到 <see cref="Save"> 调用时进入 NoScribble 模式。
        /// </summary>
        /// <param name="pszFileName">以前保存对象的文件的绝对路径。</param>
        void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);


    }
}

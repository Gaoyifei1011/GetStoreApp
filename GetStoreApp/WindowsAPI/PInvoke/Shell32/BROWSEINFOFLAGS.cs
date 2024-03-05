using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    [Flags]
    public enum BROWSEINFOFLAGS
    {
        /// <summary>
        ///  仅返回文件系统目录。 如果用户选择不属于文件系统的文件夹，“ 确定” 按钮将灰显。
        /// </summary>
        BIF_RETURNONLYFSDIRS = 0x0001,

        /// <summary>
        /// 不要在对话框的树视图控件中包含域级别以下的网络文件夹。
        /// </summary>
        BIF_DONTGOBELOWDOMAIN = 0x0002,

        /// <summary>
        /// 在对话框中包括状态区域。 回调函数可以通过向对话框发送消息来设置状态文本。 指定BIF_NEWDIALOGSTYLE时，不支持此标志。
        /// </summary>
        BIF_STATUSTEXT = 0x0004,

        /// <summary>
        /// 仅返回文件系统上级。 上级是命名空间层次结构中根文件夹下的子文件夹。 如果用户选择不属于文件系统的根文件夹的上级，则 “确定” 按钮将灰显。
        /// </summary>
        BIF_RETURNFSANCESTORS = 0x0008,

        /// <summary>
        /// 在浏览对话框中包括允许用户键入项名称的编辑控件。
        /// </summary>
        BIF_EDITBOX = 0x0010,

        /// <summary>
        ///  如果用户在编辑框中键入无效的名称，浏览对话框将调用应用程序的 BrowseCallbackProc ，并 显示BFFM_VALIDATEFAILED 消息。 如果未指定BIF_EDITBOX，则忽略此标志。
        /// </summary>
        BIF_VALIDATE = 0x0020,

        /// <summary>
        /// 使用新的用户界面。 设置此标志为用户提供了可以调整大小的较大对话框。 对话框具有多个新功能，包括：对话框中的拖放功能、重新排序、快捷菜单、新文件夹、删除和其他快捷菜单命令。
        /// </summary>
        BIF_NEWDIALOGSTYLE = 0x0040,

        /// <summary>
        /// 浏览对话框可以显示 URL。 还必须设置BIF_USENEWUI和BIF_BROWSEINCLUDEFILES标志。 如果未设置这三个标志中的任何一个，浏览器对话框将拒绝 URL。 即使设置了这些标志，仅当包含所选项目的文件夹支持 URL 时，浏览对话框才会显示 URL。 调用文件夹的 IShellFolder：：GetAttributesOf 方法以请求选定项的属性时，文件夹必须设置 SFGAO_FOLDER 属性标志。 否则，浏览对话框将不会显示 URL。
        /// </summary>
        BIF_BROWSEINCLUDEURLS = 0x0080,

        /// <summary>
        /// 使用新的用户界面，包括编辑框。 此标志等效于 BIF_EDITBOX |BIF_NEWDIALOGSTYLE。
        /// </summary>
        BIF_USENEWUI = BIF_NEWDIALOGSTYLE | BIF_EDITBOX,

        /// <summary>
        /// 与 BIF_NEWDIALOGSTYLE 结合使用时，将用法提示添加到对话框中，以代替编辑框。 BIF_EDITBOX重写此标志。
        /// </summary>
        BIF_UAHINT = 0x00000100,

        /// <summary>
        /// 不要在浏览对话框中包括 “新建文件夹” 按钮。
        /// </summary>
        BIF_NONEWFOLDERBUTTON = 0x00000200,

        /// <summary>
        /// 当所选项是快捷方式时，返回快捷方式本身的 PIDL，而不是其目标。
        /// </summary>
        BIF_NOTRANSLATETARGETS = 0x00000400,

        /// <summary>
        /// 仅返回计算机。 如果用户选择计算机之外的任何内容，“确定”按钮就会变灰。
        /// </summary>
        BIF_BROWSEFORCOMPUTER = 0x00001000,

        /// <summary>
        /// 仅允许选择打印机。 如果用户选择打印机之外的任何内容，“确定”按钮就会变灰。
        /// 在 Windows XP 及更高版本中，最佳做法是使用 Windows XP 样式的对话框，将对话框的根设置为“ 打印机和传真 ”文件夹 (CSIDL_PRINTERS) 。
        /// </summary>
        BIF_BROWSEFORPRINTER = 0x00002000,

        /// <summary>
        /// 浏览对话框显示文件以及文件夹。
        /// </summary>
        BIF_BROWSEINCLUDEFILES = 0x00004000,

        /// <summary>
        /// 浏览对话框可以显示远程系统上的可共享资源。 这适用于想要在本地系统上公开远程共享的应用程序。 还必须设置BIF_NEWDIALOGSTYLE标志。
        /// </summary>
        BIF_SHAREABLE = 0x00008000,

        /// <summary>
        /// Windows 7 及更高版本。 允许浏览具有 .zip 文件扩展名的文件夹接合点，例如库或压缩文件。
        /// </summary>
        BIF_BROWSEFILEJUNCTIONS = 0x00010000
    }
}

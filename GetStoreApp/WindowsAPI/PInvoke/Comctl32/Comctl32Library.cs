using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Comctl32
{
    /// <summary>
    /// Comctl32.dll 函数库
    /// </summary>
    public static partial class Comctl32Library
    {
        public const string Comctl32 = "comctl32.dll";

        /// <summary>
        /// TaskDialog 函数创建、显示和操作任务对话框。 任务对话框包含应用程序定义的消息文本和标题、图标以及预定义的按钮的任意组合。 此函数不支持注册回调函数以接收通知。
        /// </summary>
        /// <param name="hWndOwner">要创建的任务对话框的所有者窗口的句柄。 如果此参数为 NULL，则任务对话框没有所有者窗口。</param>
        /// <param name="hInstance">句柄，该模块包含 由 pszIcon 成员标识的图标资源，以及 pszWindowTitle 和 pszMainInstruction 成员标识的字符串资源。 如果此参数为 NULL， pszIcon 必须为 NULL 或指向以 null 结尾的 Unicode 字符串的指针，其中包含系统资源标识符，例如，TD_ERROR_ICON。</param>
        /// <param name="pszWindowTitle">指向要用于任务对话框标题的字符串的指针。 此参数是一个以 null 结尾的 Unicode 字符串，其中包含文本或通过 MAKEINTRESOURCE 宏传递的整数资源标识符。 如果此参数为 NULL，则使用可执行文件的文件名。</param>
        /// <param name="pszMainInstruction">指向要用于主指令的字符串的指针。 此参数是一个以 null 结尾的 Unicode 字符串，其中包含文本或通过 MAKEINTRESOURCE 宏传递的整数资源标识符。 如果不需要主指令，此参数可以为 NULL 。</param>
        /// <param name="pszContent">指向用于以较小字体显示在主指令下方的其他文本的字符串的指针。 此参数是一个以 null 结尾的 Unicode 字符串，其中包含文本或通过 MAKEINTRESOURCE 宏传递的整数资源标识符。 如果不需要其他文本，可以为 NULL 。</param>
        /// <param name="dwCommonButtons">
        /// 类型： TASKDIALOG_COMMON_BUTTON_FLAGS
        /// 指定对话框中显示的推送按钮。 此参数可以是以下组中的标志的组合。
        /// 注意 如果未指定任何按钮，则默认情况下，对话框将包含 “确定 ”按钮。
        /// TDCBF_OK_BUTTON：任务对话框包含按下按钮： 确定。
        /// TDCBF_YES_BUTTON：任务对话框包含按钮： 是。
        /// TDCBF_NO_BUTTON：任务对话框包含按钮： 否。
        /// TDCBF_CANCEL_BUTTON：任务对话框包含按钮： 取消。 必须为此对话框指定此按钮，才能响应 alt-F4 和转义) 的典型取消(操作。
        /// TDCBF_RETRY_BUTTON：任务对话框包含推送按钮： 重试。
        /// TDCBF_CLOSE_BUTTON：任务对话框包含按钮： 关闭。
        /// </param>
        /// <param name="pszIcon">
        /// 指向标识在任务对话框中显示的图标的字符串的指针。 此参数必须是传递给 MAKEINTRESOURCE 宏或以下预定义值的整数资源标识符。 如果此参数为 NULL，则不会显示任何图标。 如果 hInstance 参数为 NULL 且未使用其中一个预定义值， 则 TaskDialog 函数将失败。
        /// TD_ERROR_ICON：任务对话框中会显示一个非索引符号图标。
        /// TD_INFORMATION_ICON：由圆圈中的小写字母 i 组成的图标显示在任务对话框中。
        /// TD_SHIELD_ICON：任务对话框中会显示一个安全防护图标。
        /// TD_WARNING_ICON：任务对话框中会显示感叹号图标。
        /// </param>
        /// <param name="pnButton">
        /// 当此函数返回时，包含指向接收以下值之一的整数位置的指针：
        /// 0：函数调用失败。 有关详细信息，请参阅返回值。
        /// IDCANCEL：已选择“取消”按钮，按 Alt-F4，已按下转义，或者用户单击关闭窗口按钮。
        /// IDNO：未选择任何按钮。
        /// IDOK：已选择“确定”按钮。
        /// IDRETRY：已选择“重试”按钮。
        /// IDYES：已选择“是”按钮。
        /// </param>
        /// <returns>
        /// 此函数可以返回其中一个值。
        /// S_OK：操作已成功完成。
        /// E_OUTOFMEMORY：内存不足，无法完成操作。
        /// E_INVALIDARG：一个或多个参数无效。
        /// E_FAIL：此操作失败。
        /// </returns>
        [LibraryImport(Comctl32, EntryPoint = "TaskDialog", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int TaskDialog(IntPtr hWndOwner, IntPtr hInstance, string pszWindowTitle, string pszMainInstruction, string pszContent, TASKDIALOG_COMMON_BUTTON_FLAGS dwCommonButtons, TASKDIALOGICON pszIcon, out TaskDialogResult pnButton);
    }
}

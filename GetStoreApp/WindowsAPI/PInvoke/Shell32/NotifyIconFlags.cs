using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 指示设置了<see cref="NotifyIconData"/> 结构的哪些成员，从而包含有效数据或向工具提示提供有关其应如何显示的其他信息。
    /// </summary>
    [Flags]
    public enum NotifyIconFlags
    {
        /// <summary>
        /// 消息 ID 已设置。指定该标志时 uCallbackMessage 成员有效。
        /// </summary>
        NIF_MESSAGE = 0x01,

        /// <summary>
        /// 通知图标已设置。指定该标志时 hIcon 成员有效。
        /// </summary>
        NIF_ICON = 0x02,

        /// <summary>
        /// 工具提示已设置。指定该标志时 szTip 成员有效。
        /// </summary>
        NIF_TIP = 0x04,

        /// <summary>
        /// 状态信息已设置。指定该标志时 dwState 和 dwStateMask 成员有效。
        /// </summary>
        NIF_STATE = 0x08,

        /// <summary>
        /// 气球工具提示已设置。指定该标志时 szInfo、szInfoTitle、dwInfoFlags 和 uTimeout 成员有效。
        /// 请注意， uTimeout 仅在 Windows 2000 和 Windows XP 中有效。
        /// 若要显示气球通知，请指定 <see cref="NIF_INFO"> 并在 szInfo 中提供文本。
        /// 若要删除气球通知，请指定 <see cref="NIF_INFO"> 并通过 szInfo 提供空字符串。
        /// 若要在不显示通知的情况下添加通知区域图标，请不要设置 <see cref="NIF_INFO"> 标志。
        /// </summary>
        NIF_INFO = 0x10,

        // 设置内部标识符。
        // Windows 7 及更高版本： guidItem 有效。
        // Windows Vista 及更早版本：保留。
        NIF_GUID = 0x20,

        /// <summary>
        /// Windows Vista 及更高版本。
        /// 如果无法立即显示气球通知，请将其丢弃。将此标志用于表示实时信息的通知，如果稍后显示这些信息，这些信息将毫无意义或具有误导性。
        /// 例如，一条消息，指出“您的电话正在响铃”。
        /// 仅当与 <see cref="NIF_INFO"> 标志结合使用时，<see cref="NIF_REALTIME"> 才有意义。
        /// </summary>
        NIF_REALTIME = 0x40,

        /// <summary>
        /// Windows Vista 及更高版本。
        /// 使用标准工具提示。通常，当 uVersion 设置为 NOTIFYICON_VERSION_4 时，标准工具提示将被禁止显示，并且可以由应用程序绘制的弹出 UI 替换。
        /// 如果应用程序想要使用NOTIFYICON_VERSION_4显示标准工具提示，它可以指定 <see cref="NIF_SHOWTIP"> 以指示仍应显示标准工具提示。
        /// </summary>
        NIF_SHOWTIP = 0x80
    }
}

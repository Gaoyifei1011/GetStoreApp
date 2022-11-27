using System;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 指示设置了<see cref="NotifyIconData"/> 结构的哪些成员，从而包含有效数据或向工具提示提供有关其应如何显示的其他信息。
    /// </summary>
    [Flags]
    public enum IconDataMembers
    {
        /// <summary>
        /// 消息 ID 已设置。
        /// </summary>
        Message = 0x01,

        /// <summary>
        /// 通知图标已设置。
        /// </summary>
        Icon = 0x02,

        /// <summary>
        /// 工具提示已设置。
        /// </summary>
        Tip = 0x04,

        /// <summary>
        /// 状态信息（<see cref="IconState"/>）。这适用于 <see cref="NotifyIconData.IconState"/> 和 <see cref="NotifyIconData.StateMask"/>。
        /// </summary>
        State = 0x08,

        /// <summary>
        /// 气球工具提示已设置。因此，设置了以下成员：<see cref="NotifyIconData.BalloonText"/>, <see cref="NotifyIconData.BalloonTitle"/>, <see cref="NotifyIconData.BalloonFlags"/>, and <see cref="NotifyIconData.VersionOrTimeout"/>.
        /// </summary>
        Info = 0x10,

        // 设置内部标识符。保留，因此注释掉了。
        //Guid = 0x20,

        /// <summary>
        /// Windows Vista（Shell32.dll版本 6.0.6）及更高版本。如果无法立即显示工具提示，请将其丢弃。
        /// 将此标志用于表示实时信息的工具提示，如果稍后显示这些信息，这些信息将毫无意义或具有误导性。
        /// 例如，一条消息，指出“您的电话正在响铃”。
        /// 这会修改并且必须与<see cref="Info"/> 标志结合使用。
        /// </summary>
        Realtime = 0x40,

        /// <summary>
        /// Windows Vista（Shell32.dll版本 6.0.6）及更高版本。
        /// 使用标准工具提示。通常，当 uVersion 设置为 NOTIFYICON_VERSION_4 时，标准工具提示将替换为应用程序绘制的弹出式用户界面 （UI）。
        /// 如果应用程序想要在这种情况下显示标准工具提示，则无论悬停 UI 是否显示，它都可以指定NIF_SHOWTIP以指示仍应显示标准工具提示。
        /// 请注意，NIF_SHOWTIP标志在下次调用Shell_NotifyIcon之前有效。
        /// </summary>
        UseLegacyToolTips = 0x80
    }
}

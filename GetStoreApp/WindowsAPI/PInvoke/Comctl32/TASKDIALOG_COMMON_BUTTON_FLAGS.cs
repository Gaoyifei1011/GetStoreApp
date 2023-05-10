using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.WindowsAPI.PInvoke.Comctl32
{
    /// <summary>
    /// 指定对话框中显示的推送按钮。 此参数可以是以下组中的标志的组合。
    /// </summary>
    [Flags]
    public enum TASKDIALOG_COMMON_BUTTON_FLAGS
    {
        /// <summary>
        /// 任务对话框包含按下按钮： 确定。
        /// </summary>
        TDCBF_OK_BUTTON = 0x0001,

        /// <summary>
        /// 任务对话框包含按钮： 是。
        /// </summary>
        TDCBF_YES_BUTTON = 0x0002,

        /// <summary>
        /// 任务对话框包含按钮： 否。
        /// </summary>
        TDCBF_NO_BUTTON = 0x0004,

        /// <summary>
        /// 任务对话框包含按钮： 取消。 必须为此对话框指定此按钮，才能响应 alt-F4 和转义) 的典型取消 (操作。
        /// </summary>
        TDCBF_CANCEL_BUTTON = 0x0008,

        /// <summary>
        /// 任务对话框包含推送按钮： 重试。
        /// </summary>
        TDCBF_RETRY_BUTTON = 0x0010,

        /// <summary>
        /// 任务对话框包含按钮： 关闭。
        /// </summary>
        TDCBF_CLOSE_BUTTON = 0x0020,
    }
}

using System;

namespace GetStoreApp.WindowsAPI.Controls.Taskbar
{
    /// <summary>
    /// <see cref="THUMBBUTTON"> 用于控制按钮的特定状态和行为。
    /// </summary>
    [Flags]
    internal enum THUMBBUTTONFLAGS
    {
        /// <summary>
        /// 按钮处于活动状态，可供用户使用。
        /// </summary>
        THBF_ENABLED = 0x00000000,

        /// <summary>
        /// 该按钮已被禁用。 它存在，但具有指示它不会响应用户操作的视觉状态。
        /// </summary>
        THBF_DISABLED = 0x00000001,

        /// <summary>
        /// 单击按钮时，任务栏按钮的浮出控件会立即关闭。
        /// </summary>
        THBF_DISMISSONCLICK = 0x00000002,

        /// <summary>
        /// 不要绘制按钮边框，仅使用图像。
        /// </summary>
        THBF_NOBACKGROUND = 0x00000004,

        /// <summary>
        /// 该按钮未向用户显示。
        /// </summary>
        THBF_HIDDEN = 0x00000008,

        /// <summary>
        /// 该按钮已启用，但不是交互式按钮;未绘制按下的按钮状态。 此值适用于在通知中使用按钮的实例。
        /// </summary>
        THBF_NONINTERACTIVE = 0x00000010
    }
}

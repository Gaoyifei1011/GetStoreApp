namespace GetStoreApp.WindowsAPI.PInvoke.Comctl32
{
    public enum TASKDIALOGICON
    {
        /// <summary>
        /// 任务对话框不显示图标
        /// </summary>
        None = 0,

        /// <summary>
        /// 任务对话框包含一个由用户帐户控制 (UAC) 盾牌组成的图标，并在图标周围显示一个灰色条。
        /// </summary>
        TD_SHIELD_GRAY_BAR = 65527,

        /// <summary>
        /// 任务对话框包含一个由绿色盾牌中的白勾组成的图标，并在图标周围显示一个绿色条。
        /// </summary>
        TD_SHIELD_SUCCESS_GREEN_BAR = 65528,

        /// <summary>
        /// 任务对话框包含一个由红色盾牌中的白色 X 组成的图标，并在图标周围显示一个红色条。
        /// </summary>
        TD_SHIELD_ERROR_RED_BAR = 65529,

        /// <summary>
        /// 任务对话框包含一个由黄色盾牌中的惊叹号组成的图标，并在图标周围显示一个黄色条。
        /// </summary>
        TD_SHIELD_WARNING_YELLOW_BAR = 65530,

        /// <summary>
        /// 任务对话框包含一个由用户帐户控制 (UAC) 盾牌组成的图标，并在图标周围显示一个蓝色条。
        /// </summary>
        TD_SHIELD_BLUE_BAR = 65531,

        /// <summary>
        /// 任务对话框包含一个由用户帐户控制 (UAC) 盾牌组成的图标
        /// </summary>
        TD_SHIELD_ICON = 65532,

        /// <summary>
        /// 任务对话框包含一个由在圆圈中的小写字母 i 组成的符号。
        /// </summary>
        TD_INFORMATION_ICON = 65533,

        /// <summary>
        /// 任务对话框包含一个由在红色背景圆圈中的白色 X 组成的图标。
        /// </summary>
        TD_ERROR_ICON = 65534,

        /// <summary>
        /// 任务对话框中会显示感叹号图标。
        /// </summary>
        TD_WARNING_ICON = 65535,
    }
}

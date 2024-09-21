using System;

namespace GetStoreAppShellMenuExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// EXPCMDSTATE 值表示 Shell 项的命令状态。
    /// </summary>
    [Flags]
    public enum EXPCMDSTATE : int
    {
        /// <summary>
        /// 该项已启用。
        /// </summary>
        ECS_ENABLED = 0x0,

        /// <summary>
        /// 该项不可用。 它可能显示为用户灰显且不可访问的项。
        /// </summary>
        ECS_DISABLED = 0x1,

        /// <summary>
        /// 该项处于隐藏状态。
        /// </summary>
        ECS_HIDDEN = 0x2,

        /// <summary>
        /// 该项带有检查框显示，并且未选中该检查框。
        /// </summary>
        ECS_CHECKBOX = 0x4,

        /// <summary>
        /// 显示带有检查框的项，并选中该检查框。 始终使用 ECS_CHECKBOX 返回ECS_CHECKED。
        /// </summary>
        ECS_CHECKED = 0x8,

        /// <summary>
        /// Windows 7 及更高版本。 项是通过单选按钮选择的一组互斥选项之一。 ECS_RADIOCHECK并不意味着该项是所选选项，尽管它可能是。
        /// </summary>
        ECS_RADIOCHECK = 0x10
    }
}

using System;

namespace GetStoreAppShellExtension.WindowsAPI.ComTypes
{
    /// <summary>
    /// 指向当前命令标志
    /// </summary>
    [Flags]
    public enum EXPCMDFLAGS : int
    {
        /// <summary>
        /// Windows 7 及更高版本。 未设置命令标志。
        /// </summary>
        ECF_DEFAULT = 0,

        /// <summary>
        /// 命令具有子命令。
        /// </summary>
        ECF_HASSUBCOMMANDS = 0x1,

        /// <summary>
        /// 将显示拆分按钮。
        /// </summary>
        ECF_HASSPLITBUTTON = 0x2,

        /// <summary>
        /// 标签处于隐藏状态。
        /// </summary>
        ECF_HIDELABEL = 0x4,

        /// <summary>
        /// 命令是分隔符。
        /// </summary>
        ECF_ISSEPARATOR = 0x8,

        /// <summary>
        /// 显示 UAC 防护板。
        /// </summary>
        ECF_HASLUASHIELD = 0x10,

        /// <summary>
        /// 在 Windows 7 中引入。 命令位于紧邻分隔符下方的菜单中。
        /// </summary>
        ECF_SEPARATORBEFORE = 0x20,

        /// <summary>
        /// 在 Windows 7 中引入。 命令位于紧靠分隔符上方的菜单中。
        /// </summary>
        ECF_SEPARATORAFTER = 0x40,

        /// <summary>
        /// 在 Windows 7 中引入。 选择命令会打开一个下拉子菜单， (例如“ 包含在库) 中”。
        /// </summary>
        ECF_ISDROPDOWN = 0x80,

        /// <summary>
        /// 在 Windows 8 中引入。
        /// </summary>
        ECF_TOGGLEABLE = 0x100,

        /// <summary>
        /// 在 Windows 8 中引入。
        /// </summary>
        ECF_AUTOMENUICONS = 0x200
    }
}

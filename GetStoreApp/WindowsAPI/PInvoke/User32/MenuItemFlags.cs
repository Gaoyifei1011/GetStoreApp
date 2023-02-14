using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 控制菜单项的外观和行为。
    /// </summary>
    [Flags]
    public enum MenuItemFlags
    {
        /// <summary>
        /// 使用位图作为菜单项。lpNewItem 参数包含位图的句柄。
        /// </summary>
        MF_BITMAP = 0x00000004,

        /// <summary>
        /// 在菜单项旁边放置一个复选标记。如果应用程序提供复选标记位图（参考 SetMenuItemBitmap），则此标志会在菜单项旁边显示复选标记位图。
        /// </summary>
        MF_CHECKED = 0x00000008,

        /// <summary>
        /// 禁用菜单项，使其无法选择，但标志不会使其灰显。
        /// </summary>
        MF_DISABLED = 0x00000002,

        /// <summary>
        /// 启用菜单项以便可以选择它，并将其从灰色状态还原。
        /// </summary>
        MF_ENABLED = 0x00000000,

        /// <summary>
        /// 禁用菜单项并将其灰显，使其无法选择。
        /// </summary>
        MF_GRAYED = 0x00000001,

        /// <summary>
        /// 功能与菜单栏的MF_MENUBREAK标志相同。对于下拉菜单、子菜单或快捷菜单，新列与旧列之间用竖线分隔。
        /// </summary>
        MF_MENUBARBREAK = 0x00000020,

        /// <summary>
        /// 将项放在新行（对于菜单栏）或新列（对于下拉菜单、子菜单或快捷菜单）中，而不分隔列。
        /// </summary>
        MF_MENUBREAK = 0x00000040,

        /// <summary>
        /// 指定该项是所有者描述的项。在首次显示菜单之前，拥有菜单的窗口会收到一条 <see cref="WindowMessage.WM_MEASUREITEM"> 消息，以检索菜单项的宽度和高度。 然后，每当必须更新菜单项的外观时，<see cref="WindowMessage.WM_DRAWITEM"> 消息都会发送到所有者窗口的窗口过程。
        /// </summary>
        MF_OWNERDRAW = 0x00000100,

        /// <summary>
        /// 指定菜单项打开下拉菜单或子菜单。uIDNewItem 参数指定下拉菜单或子菜单的句柄。
        /// 此标志用于将菜单名称添加到菜单栏，或将子菜单打开为下拉菜单、子菜单或快捷菜单的菜单项。
        /// </summary>
        MF_POPUP = 0x00000010,

        /// <summary>
        /// 绘制水平分界线。此标志仅在下拉菜单、子菜单或快捷菜单中使用。该行不能灰显、禁用或突出显示。lpNewItem 和 uIDNewItem 参数将被忽略。
        /// </summary>
        MF_SEPARATOR = 0x00000800,

        /// <summary>
        /// 指定菜单项为文本字符串;lpNewItem 参数是指向字符串的指针。
        /// </summary>
        MF_STRING = 0x00000000,

        /// <summary>
        /// 不在项目旁边放置复选标记（默认）。如果应用程序提供复选标记位图（参考 SetMenuItemBitmap），则此标志在菜单项旁边显示清除位图。
        /// </summary>
        MF_UNCHECKED = 0x00000000,

        /// <summary>
        /// 指示菜单项由其命令标识。
        /// </summary>
        MF_BYCOMMAND = 0x00000000,

        /// <summary>
        /// 指示菜单项由其从零开始的相对位置标识。
        /// </summary>
        MF_BYPOSITION = 0x00000400,

        /// <summary>
        /// 从菜单项中删除突出显示。
        /// </summary>
        MF_UNHILITE = 0x00000000,

        /// <summary>
        /// 突出显示菜单项
        /// </summary>
        MF_HILITE = 0x00000080,

        /// <summary>
        /// 已过时 -- 仅由旧的 RES 文件使用
        /// </summary>
        MF_END = 0x00000080,

        MF_USECHECKBITMAPS = 0x00000200,
        MF_INSERT = 0x00000000,
        MF_CHANGE = 0x00000080,
        MF_APPEND = 0x00000100,
        MF_DELETE = 0x00000200,
        MF_REMOVE = 0x00001000,
        MF_DEFAULT = 0x00001000,
        MF_SYSMENU = 0x00002000,
        MF_HELP = 0x00004000,
        MF_RIGHTJUSTIFY = 0x00004000,
        MF_MOUSESELECT = 0x00008000,
    }
}

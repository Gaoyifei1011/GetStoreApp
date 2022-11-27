using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum MenuItemType
    {
        /// <summary>
        /// 使用文本字符串显示菜单项。 dwTypeData 成员是指向 null 终止字符串的指针，cch 成员是字符串的长度。
        /// MFT_STRING 替换为 MIIM_STRING。
        /// </summary>
        MFT_STRING = 0x00000000,

        /// <summary>
        /// 使用位图显示菜单项。 dwTypeData 成员的低顺序字是位图句柄，cch 成员将被忽略。
        /// MFT_BITMAP 由 MIIM_BITMAP 和 hbmpItem 替换。
        /// </summary>
        MFT_BITMAP = 0x00000004,

        /// <summary>
        /// 将菜单项放在菜单栏的新行 () 或下拉菜单、子菜单或快捷菜单) 的新列 (中。 对于下拉菜单、子菜单或快捷菜单，垂直线将新列与旧列分开。
        /// </summary>
        MFT_MENUBARBREAK = 0x00000020,

        /// <summary>
        /// 将菜单项放在菜单栏的新行 () 或下拉菜单、子菜单或快捷菜单) 的新列 (中。 对于下拉菜单、子菜单或快捷菜单，列不会用垂直线分隔。
        /// </summary>
        MFT_MENUBREAK = 0x00000040,

        /// <summary>
        /// 分配将菜单项绘制到拥有菜单的窗口的责任。 窗口在首次显示菜单前接收 WM_MEASUREITEM 消息，每当必须更新菜单项的外观时，都会收到 WM_DRAWITEM 消息。 如果指定了此值， dwTypeData 成员将包含应用程序定义的值。
        /// </summary>
        MFT_OWNERDRAW = 0x00000100,

        /// <summary>
        /// 如果 hbmpChecked 成员为 NULL，则使用单选按钮标记而不是复选标记显示所选菜单项。
        /// </summary>
        MFT_RADIOCHECK = 0x00000200,

        /// <summary>
        /// 指定菜单项是分隔符。 菜单项分隔符显示为水平分隔线。 忽略 dwTypeData 和 cch 成员。 此值仅在下拉菜单、子菜单或快捷菜单中有效。
        /// </summary>
        MFT_SEPARATOR = 0x00000800,

        /// <summary>
        /// 指定菜单从右到左级联 (默认值为从左到右) 。 这用于支持从右到左的语言，如阿拉伯语和希伯来语。
        /// </summary>
        MFT_RIGHTORDER = 0x00002000,

        /// <summary>
        /// 右对齐菜单项和任何后续项。 仅当菜单项位于菜单栏中时，此值才有效。
        /// </summary>
        MFT_RIGHTJUSTIFY = 0x00004000,
    }
}

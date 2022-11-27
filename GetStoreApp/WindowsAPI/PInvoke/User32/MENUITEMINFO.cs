using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含有关菜单项的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MENUITEMINFO
    {
        /// <summary>
        /// 结构大小（以字节为单位）。 调用方必须将此成员设置为 sizeof(MENUITEMINFO)。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 指示要检索或设置的成员。
        /// </summary>
        public MenuMembersMask fMask;

        /// <summary>
        /// 菜单项类型。
        /// </summary>
        public MenuItemType fType;

        /// <summary>
        /// 菜单项状态。
        /// </summary>
        public MenuItemState fState;

        /// <summary>
        /// 用于标识菜单项的应用程序定义值。 将 fMask 设置为 MIIM_ID 以使用 wID。
        /// </summary>
        public int wID;

        /// <summary>
        /// 与菜单项关联的下拉菜单或子菜单的句柄。 如果菜单项不是打开下拉菜单或子菜单的项，则此成员为 NULL。 将 fMask 设置为 MIIM_SUBMENU 以使用 hSubMenu。
        /// </summary>
        public IntPtr hSubMenu;

        /// <summary>
        /// 选中该项时要显示的位图的句柄。 如果此成员为 NULL，则使用默认位图。 如果指定 了MFT_RADIOCHECK 类型值，则默认位图为项目符号。
        /// 否则，它是复选标记。 将 fMask 设置为 MIIM_CHECKMARKS 以使用 hbmpChecked。
        /// </summary>
        public IntPtr hbmpChecked;

        /// <summary>
        /// 位图的句柄，如果未选中该项，则显示在项旁边。 如果此成员为 NULL，则不使用位图。 将 fMask 设置为 MIIM_CHECKMARKS 以使用 hbmpUnchecked。
        /// </summary>
        public IntPtr hbmpUnchecked;

        /// <summary>
        /// 与菜单项关联的应用程序定义值。 将 fMask 设置为 MIIM_DATA 以使用 dwItemData。
        /// </summary>
        public IntPtr dwItemData;

        /// <summary>
        /// 菜单项的内容。 此成员的含义取决于 fType 的值，仅在 fMask 成员中设置MIIM_TYPE标志时才使用。
        /// </summary>
        public string dwTypeData;

        /// <summary>
        /// 当收到有关 MFT_STRING 类型的菜单项的信息时，菜单项文本的长度（以字符为单位）。 但是，仅当在 fMask 成员中设置MIIM_TYPE标志并且为零时，才使用 cch。
        /// 此外，通过调用 SetMenuItemInfo 设置菜单项的内容时，将忽略 cch。在 fMask 成员中设置MIIM_STRING标志时，将使用 cch 成员。
        /// </summary>
        public uint cch;

        /// <summary>
        /// 要显示的位图的句柄.在 fMask 成员中设置MIIM_BITMAP标志时，将使用它。
        /// </summary>
        public IntPtr hbmpItem;
    }
}

using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>The menu item state in <see cref="MENUITEMINFO" />.</summary>
    [Flags]
    public enum MenuItemState
    {
        /// <summary>
        /// 检查菜单项。 有关所选菜单项的详细信息，请参阅 <see cref="MENUITEMINFO.hbmpChecked" /> 成员。
        /// </summary>
        MFS_CHECKED = MenuItemFlags.MF_CHECKED,

        /// <summary>
        /// 指定菜单项为默认值。 菜单只能包含一个默认菜单项，以粗体显示。
        /// </summary>
        MFS_DEFAULT = MenuItemFlags.MF_DEFAULT,

        /// <summary>
        /// 禁用菜单项并将其灰显，以便无法选择它。这相当于 <see cref="MFS_GRAYED" />。
        /// </summary>
        MFS_DISABLED = MFS_GRAYED,

        /// <summary>启用菜单项，以便可以选择它。 这是默认状态。</summary>
        MFS_ENABLED = MenuItemFlags.MF_ENABLED,

        /// <summary>
        /// 禁用菜单项并将其灰显，以便无法选择它。 这相当于 <see cref="MFS_DISABLED" />.
        /// </summary>
        MFS_GRAYED = 0x00000003,

        /// <summary>
        /// 突出显示菜单项。
        /// </summary>
        MFS_HILITE = MenuItemFlags.MF_HILITE,

        /// <summary>
        /// 取消选中菜单项。 有关清除菜单项的详细信息，请参阅 <see cref="MENUITEMINFO.hbmpChecked" /> 成员。
        /// </summary>
        MFS_UNCHECKED = MenuItemFlags.MF_UNCHECKED,

        /// <summary>
        /// 从菜单项中删除突出显示。 这是默认状态。
        /// </summary>
        MFS_UNHILITE = MenuItemFlags.MF_UNHILITE,
    }
}

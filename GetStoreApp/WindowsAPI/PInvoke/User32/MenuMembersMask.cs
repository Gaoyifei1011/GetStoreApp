using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum MenuMembersMask
    {
        /// <summary>
        /// 检索或设置 fState 成员。
        /// </summary>
        MIIM_STATE = 0x00000001,

        /// <summary>
        /// 检索或设置 wID 成员。
        /// </summary>
        MIIM_ID = 0x00000002,

        /// <summary>
        /// 检索或设置 hSubMenu 成员。
        /// </summary>
        MIIM_SUBMENU = 0x00000004,

        /// <summary>
        /// 检索或设置 hbmpChecked 和 hbmpUnchecked 成员。
        /// </summary>
        MIIM_CHECKMARKS = 0x00000008,

        /// <summary>
        /// 检索或设置 fType 和 dwTypeData 成员。MIIM_TYPE 替换为 MIIM_BITMAP、 MIIM_FTYPE和 MIIM_STRING。
        /// </summary>
        MIIM_TYPE = 0x00000010,

        /// <summary>
        /// 检索或设置 dwItemData 成员。
        /// </summary>
        MIIM_DATA = 0x00000020,

        /// <summary>
        /// 检索或设置 dwTypeData 成员。
        /// </summary>
        MIIM_STRING = 0x00000040,

        /// <summary>
        /// 检索或设置 hbmpItem 成员。
        /// </summary>
        MIIM_BITMAP = 0x00000080,

        /// <summary>
        /// 检索或设置 fType 成员。
        /// </summary>
        MIIM_FTYPE = 0x00000100,
    }
}

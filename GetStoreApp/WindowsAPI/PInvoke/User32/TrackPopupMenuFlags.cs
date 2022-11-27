using System;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    [Flags]
    public enum TrackPopupMenuFlags : uint
    {
        // Group 1 指定函数水平定位快捷菜单的方式。

        /// <summary>
        /// 指定函数水平定位快捷菜单的方式。
        /// </summary>
        TPM_LEFTALIGN = 0x0000,

        /// <summary>
        /// 将快捷菜单水平相对于 x 参数指定的坐标居中。
        /// </summary>
        TPM_CENTERALIGN = 0x0004,

        /// <summary>
        /// 定位快捷菜单，使其右侧与 x 参数指定的坐标对齐。
        /// </summary>
        TPM_RIGHTALIGN = 0x0008,

        // Group 2 指定函数如何垂直定位快捷菜单

        /// <summary>
        /// 定位快捷菜单，使其顶部与 y 参数指定的坐标对齐。
        /// </summary>
        TPM_TOPALIGN = 0x0000,

        /// <summary>
        /// 将快捷菜单垂直相对于 y 参数指定的坐标居中。
        /// </summary>
        TPM_VCENTERALIGN = 0x0010,

        /// <summary>
        /// 定位快捷菜单，使其底部与 y 参数指定的坐标对齐。
        /// </summary>
        TPM_BOTTOMALIGN = 0x0020,

        // Group 3 控制用户选择的发现，而无需为菜单设置父窗口。

        /// <summary>
        /// 当用户单击菜单项时，该函数不会发送通知消息。
        /// </summary>
        TPM_NONOTIFY = 0x0080,

        /// <summary>
        /// 该函数在返回值中返回用户选择的菜单项标识符。
        /// </summary>
        TPM_RETURNCMD = 0x0100,

        // Group 4 指定快捷菜单跟踪的鼠标按钮。

        /// <summary>
        /// 用户可以仅选择鼠标左键的菜单项。
        /// </summary>
        TPM_LEFTBUTTON = 0x0000,

        /// <summary>
        /// 用户可以仅选择鼠标左键的菜单项。
        /// </summary>
        TPM_RIGHTBUTTON = 0x0002,

        // Group 5 使用以下标志的任何合理组合来修改菜单的动画。

        /// <summary>
        /// 从左到右对菜单进行动画处理。
        /// </summary>
        TPM_HORPOSANIMATION = 0x0400,

        /// <summary>
        /// 显示没有动画的菜单。
        /// </summary>
        TPM_NOANIMATION = 0x4000,

        /// <summary>
        /// 从右到左对菜单进行动画处理。
        /// </summary>
        TPM_HORNEGANIMATION = 0x0800,

        /// <summary>
        /// 从上到下对菜单进行动画处理。
        /// </summary>
        TPM_VERPOSANIMATION = 0x1000,

        /// <summary>
        /// 从下到上对菜单进行动画处理。
        /// </summary>
        TPM_VERNEGANIMATION = 0x2000
    }
}

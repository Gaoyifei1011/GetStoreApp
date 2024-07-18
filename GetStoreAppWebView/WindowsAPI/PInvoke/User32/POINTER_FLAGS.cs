using System;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 可以显示在 POINTER_INFO 结构的 pointerFlags 字段中的值。
    /// </summary>
    [Flags]
    public enum POINTER_FLAGS : uint
    {
        /// <summary>
        /// 没有指针标志，这是默认设置。
        /// </summary>
        POINTER_FLAG_NONE = 0x00000000,

        /// <summary>
        /// 指示新指针的到来。
        /// </summary>
        POINTER_FLAG_NEW = 0x00000001,

        /// <summary>
        /// 指示此指针继续存在。如果未设置此标志，则表示指针已离开检测范围。
        /// 通常，仅当悬停指针离开检测范围（设置POINTER_FLAG_UPDATE）或与窗口表面接触的指针离开检测范围（设置POINTER_FLAG_UP）时，才设置此标志。
        /// </summary>
        POINTER_FLAG_INRANGE = 0x00000002,

        /// <summary>
        /// 指示此指针与数字化仪表面接触。如果未设置此标志，则表示悬停指针。
        /// </summary>
        POINTER_FLAG_INCONTACT = 0x00000004,

        /// <summary>
        /// 指示主要操作，类似于按下鼠标左键。
        /// 触摸指针在与数字化仪表面接触时设置了此标志。
        /// 笔指针在未按下按钮的情况下与数字化仪表面接触时会设置此标志。
        /// 鼠标指针在鼠标左键按下时设置了此标志。
        /// </summary>
        POINTER_FLAG_FIRSTBUTTON = 0x00000010,

        /// <summary>
        /// 指示辅助操作，类似于按下鼠标右键。
        /// 触摸指针不使用此标志。
        /// 当笔指针在按下笔筒按钮的情况下与数字化仪表面接触时，会设置此标志。
        /// 鼠标指针在鼠标右键按下时设置了此标志。
        /// </summary>
        POINTER_FLAG_SECONDBUTTON = 0x00000020,

        /// <summary>
        /// 类似于鼠标滚轮按钮向下。
        /// 触摸指针不使用此标志。
        /// 笔指针不使用此标志。
        /// 鼠标指针在鼠标滚轮按钮按下时设置了此标志。
        /// </summary>
        POINTER_FLAG_THIRDBUTTON = 0x00000040,

        /// <summary>
        /// 类似于第一个伸出的鼠标 （XButton1） 按钮向下。
        /// 触摸指针不使用此标志。
        /// 笔指针不使用此标志。
        /// 当第一个扩展的鼠标 （XBUTTON1） 按钮按下时，鼠标指针会设置此标志。
        /// </summary>
        POINTER_FLAG_FOURTHBUTTON = 0x00000080,

        /// <summary>
        /// 类似于按下第二个扩展鼠标 （XButton2） 按钮。
        /// 触摸指针不使用此标志。
        /// 笔指针不使用此标志。
        /// 当第二个扩展的鼠标 （XBUTTON2） 按钮按下时，鼠标指针会设置此标志。
        /// </summary>
        POINTER_FLAG_FIFTHBUTTON = 0x00000100,

        /// <summary>
        /// 指示此指针已被指定为主指针。主指针是单个指针，可以执行非主指针可用的操作之外的操作。例如，当主指针与窗口的表面接触时，它可能会通过向窗口发送WM_POINTERACTIVATE消息来为窗口提供激活的机会。
        /// 主指针是从系统上的所有当前用户交互（鼠标、触摸、笔等）中标识的。因此，主指针可能与你的应用没有关联。多点触控交互中的第一个联系人设置为主指针。识别主指针后，必须先解除所有联系人，然后才能将新联系人标识为主指针。对于不处理指针输入的应用，只有主指针的事件才会提升为鼠标事件。
        /// </summary>
        POINTER_FLAG_PRIMARY = 0x00002000,

        /// <summary>
        /// 置信度是源设备关于指针是表示预期交互还是意外交互的建议，这对于意外交互（例如与手掌）可能触发输入的PT_TOUCH指针尤其相关。此标志的存在表示源设备具有高度的置信度，即此输入是预期交互的一部分。
        /// </summary>
        POINTER_FLAG_CONFIDENCE = 0x00004000,

        /// <summary>
        /// 指示指针以异常方式离开，例如当系统收到指针的无效输入时，或者当具有活动指针的设备突然离开时。如果接收输入的应用程序能够这样做，则应将交互视为未完成，并撤消相关指针的任何影响。
        /// </summary>
        POINTER_FLAG_CANCELED = 0x00008000,

        /// <summary>
        /// 指示此指针已转换为关闭状态；也就是说，它与数字化仪表面接触。
        /// </summary>
        POINTER_FLAG_DOWN = 0x00010000,

        /// <summary>
        /// 指示这是一个不包括指针状态更改的简单更新。
        /// </summary>
        POINTER_FLAG_UPDATE = 0x00020000,

        /// <summary>
        /// 指示此指针已转换为向上状态;也就是说，与数字化仪表面的接触结束。
        /// </summary>
        POINTER_FLAG_UP = 0x00040000,

        /// <summary>
        /// 指示与指针轮关联的输入。对于鼠标指针，这相当于鼠标滚轮 （WM_MOUSEHWHEEL） 的动作。
        /// </summary>
        POINTER_FLAG_WHEEL = 0x00080000,

        /// <summary>
        /// 指示与指针 h 轮关联的输入。对于鼠标指针，这相当于鼠标水平滚轮 （WM_MOUSEHWHEEL） 的动作。
        /// </summary>
        POINTER_FLAG_HWHEEL = 0x00100000,

        /// <summary>
        /// 指示此指针已由另一个元素捕获（与另一个元素关联），并且原始元素已丢失捕获（请参考 WM_POINTERCAPTURECHANGED）。
        /// </summary>
        POINTER_FLAG_CAPTURECHANGED = 0x00200000,

        /// <summary>
        /// 指示此指针具有关联的转换。
        /// </summary>
        POINTER_FLAG_HASTRANSFORM = 0x00400000
    }
}

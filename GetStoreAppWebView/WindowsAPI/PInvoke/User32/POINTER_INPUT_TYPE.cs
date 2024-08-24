namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 标识指针输入类型。
    /// </summary>
    public enum POINTER_INPUT_TYPE
    {
        /// <summary>
        /// 泛型指针类型。 此类型永远不会出现在指针消息或指针数据中。 某些数据查询函数允许调用方将查询限制为特定的指针类型。 可在这些函数中使用 PT_POINTER 类型来指定查询将包含所有类型的指针
        /// </summary>
        PT_POINTER = 1,

        /// <summary>
        /// 触摸指针类型。
        /// </summary>
        PT_TOUCH = 2,

        /// <summary>
        /// 笔指针类型。
        /// </summary>
        PT_PEN = 3,

        /// <summary>
        /// 鼠标指针类型。
        /// </summary>
        PT_MOUSE = 4,

        /// <summary>
        /// 触摸板指针类型 (Windows 8.1 及更高版本) 。
        /// </summary>
        PT_TOUCHPAD = 5
    }
}

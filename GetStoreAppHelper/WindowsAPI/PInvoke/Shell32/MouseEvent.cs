namespace GetStoreAppHelper.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 已点击事件的事件标志。
    /// </summary>
    public enum MouseEvent
    {
        /// <summary>
        /// 鼠标在任务栏图标的区域内移动。
        /// </summary>
        MouseMove,

        /// <summary>
        /// 单击鼠标右键。
        /// </summary>
        IconRightMouseDown,

        /// <summary>
        /// 单击鼠标左键。
        /// </summary>
        IconLeftMouseDown,

        /// <summary>
        /// 释放鼠标右键。
        /// </summary>
        IconRightMouseUp,

        /// <summary>
        /// 释放鼠标左键。
        /// </summary>
        IconLeftMouseUp,

        /// <summary>
        /// 单击鼠标中键。
        /// </summary>
        IconMiddleMouseDown,

        /// <summary>
        /// 释放鼠标中键。
        /// </summary>
        IconMiddleMouseUp,

        /// <summary>
        /// 任务栏图标已双击。
        /// </summary>
        IconDoubleClick,

        /// <summary>
        /// 气泡提示被点击。
        /// </summary>
        BalloonToolTipClicked
    }
}

using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含通过调用 ChangeWindowMessageFilterEx 函数获取的扩展结果信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CHANGEFILTERSTRUCT
    {
        /// <summary>
        /// 结构大小（以字节为单位）。 必须设置为 sizeof(CHANGEFILTERSTRUCT)，否则函数将失败 并ERROR_INVALID_PARAMETER
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 如果函数成功，此字段包含 ChangeFilterStatus 枚举值之一
        /// </summary>
        public ChangeFilterStatus ExtStatus;
    }
}

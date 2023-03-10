using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 包含一个 64 位值，该值表示自 1601 年 1 月 1 日以来的 100 纳秒间隔数 (UTC) 。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FILETIME
    {
        /// <summary>
        /// 文件时间的低序部分。
        /// </summary>
        public uint dwLowDateTime;

        /// <summary>
        /// 文件时间的高阶部分。
        /// </summary>
        public uint dwHighDateTime;
    }
}

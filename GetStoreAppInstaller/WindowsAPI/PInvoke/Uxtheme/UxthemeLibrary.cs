using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.Uxtheme
{
    /// <summary>
    /// Uxtheme.dll 函数库
    /// </summary>
    public static partial class UxthemeLibrary
    {
        private const string Uxtheme = "uxtheme.dll";

        /// <summary>
        /// 设置 win32 右键菜单的样式
        /// </summary>
        /// <param name="preferredAppMode">菜单样式</param>
        [LibraryImport(Uxtheme, EntryPoint = "#135", SetLastError = false), PreserveSig]
        public static partial void SetPreferredAppMode(PreferredAppMode preferredAppMode);

        /// <summary>
        /// 刷新右键菜单样式
        /// </summary>
        [LibraryImport(Uxtheme, EntryPoint = "#136", SetLastError = false), PreserveSig]
        public static partial void FlushMenuThemes();
    }
}

using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.UxTheme
{
    /// <summary>
    /// 从 UxTheme.dll 函数库。
    /// </summary>
    public static partial class UxThemeLibrary
    {
        private const string UxTheme = "uxtheme.dll";

        // 未公开的API，修改Win32传统右键菜单主题色
        // 适用于Windows 10 1809（19041）系统
        [LibraryImport(UxTheme, EntryPoint = "#135")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool AllowDarkModeForApp([MarshalAs(UnmanagedType.Bool)] bool allow);

        // 适用于Windows 10 1903（18362）和更高版本的系统
        [LibraryImport(UxTheme, EntryPoint = "#135")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetPreferredAppMode(PreferredAppMode mode);
    }
}

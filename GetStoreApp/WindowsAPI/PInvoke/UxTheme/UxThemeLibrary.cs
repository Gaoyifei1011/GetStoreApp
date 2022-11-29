using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.UxTheme
{
    /// <summary>
    /// 从 UxTheme.dll Windows库中导出的函数。
    /// </summary>
    public static class UxThemeLibrary
    {
        private const string UxTheme = "uxtheme.dll";

        // 未公开的API，修改Win32传统右键菜单主题色
        // 适用于Windows 10 1809（17763）系统
        [DllImport(UxTheme, EntryPoint = "#135")]
        public static extern bool AllowDarkModeForApp(bool allow);

        // 适用于Windows 10 1903（18362）和更高版本的系统
        [DllImport(UxTheme, EntryPoint = "#135")]
        public static extern bool SetPreferredAppMode(PreferredAppMode mode);
    }
}

using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Uxtheme
{
    /// <summary>
    /// uxtheme.dll 函数库
    /// </summary>
    public static partial class UxthemeLibrary
    {
        private const string Uxtheme = "uxtheme.dll";

        [LibraryImport(Uxtheme, EntryPoint = "#135", SetLastError = false)]
        public static partial IntPtr SetPreferredAppMode(PreferredAppMode preferredAppMode);

        [LibraryImport(Uxtheme, EntryPoint = "#136", SetLastError = false)]
        public static partial IntPtr FlushMenuThemes();
    }
}

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

        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#135")]
        public static extern IntPtr SetPreferredAppMode(PreferredAppMode preferredAppMode);

        [DllImport(Uxtheme, CharSet = CharSet.Unicode, EntryPoint = "#136")]
        public static extern IntPtr FlushMenuThemes();
    }
}

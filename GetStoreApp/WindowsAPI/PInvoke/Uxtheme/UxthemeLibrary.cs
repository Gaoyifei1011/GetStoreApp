using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Uxtheme
{
    public static partial class UxthemeLibrary
    {
        [LibraryImport("uxtheme.dll", EntryPoint = "#135")]
        public static partial IntPtr SetPreferredAppMode(PreferredAppMode preferredAppMode);

        [LibraryImport("uxtheme.dll", EntryPoint = "#136")]
        public static partial IntPtr FlushMenuThemes();
    }
}

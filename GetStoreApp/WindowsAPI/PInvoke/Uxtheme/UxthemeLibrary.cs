using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Uxtheme
{
    public static partial class UxthemeLibrary
    {
        private const string Uxtheme = "uxtheme.dll";

        [LibraryImport(Uxtheme, EntryPoint = "#135")]
        public static partial IntPtr SetPreferredAppMode(PreferredAppMode preferredAppMode);

        [LibraryImport(Uxtheme, EntryPoint = "#136")]
        public static partial IntPtr FlushMenuThemes();
    }
}

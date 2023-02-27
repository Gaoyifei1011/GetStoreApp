using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.WinUI
{
    /// <summary>
    /// Microsoft.ui.xaml.dll 函数库
    /// </summary>
    public static partial class WinUILibrary
    {
        private const string WinUI = "Microsoft.ui.xaml.dll";

        [LibraryImport(WinUI)]
        public static partial void XamlCheckProcessRequirements();
    }
}

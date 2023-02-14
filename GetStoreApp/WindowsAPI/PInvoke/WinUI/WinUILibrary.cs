using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.WinUI
{
    /// <summary>
    /// Microsoft.ui.xaml.dll 函数库
    /// </summary>
    public static class WinUILibrary
    {
        private const string WinUI = "Microsoft.ui.xaml.dll";

        [DllImport(WinUI)]
        public static extern void XamlCheckProcessRequirements();
    }
}

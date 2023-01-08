using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.WinUI
{
    public static class WinUILibrary
    {
        private const string WinUI = "Microsoft.ui.xaml.dll";

        [DllImport(WinUI)]
        public static extern void XamlCheckProcessRequirements();
    }
}
